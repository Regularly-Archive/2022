using Google.Protobuf;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;
using SharedEntities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrpcStreamServer
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileService.FileServiceClient _fileServiceClient;
        public FilesController(FileService.FileServiceClient fileServiceClient)
        {
            _fileServiceClient = fileServiceClient;
        }

        [HttpGet("Download")]
        public async Task<ActionResult> Download(string filePath)
        {
            var downloadRequest = new DownloadFileRequest() { FilePath = filePath };
            var downloadResult = _fileServiceClient.DownloadFile(downloadRequest);

            using (var fileStream = new MemoryStream())
            {
                var received = 0L;
                while (await downloadResult.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var current = downloadResult.ResponseStream.Current;
                    var buffer = current.Content.ToByteArray();

                    fileStream.Seek(received, SeekOrigin.Begin);
                    await fileStream.WriteAsync(buffer);

                    received += buffer.Length;
                    received = Math.Min(received, current.TotalSize);
                }

                if (received > 0)
                    return File(fileStream, "application/octet-stream", Path.GetFileName(filePath));
                else
                    throw new FileNotFoundException(filePath);
            }
        }

        // POST api/files/
        [HttpPost("Upload")]
        public async Task<ActionResult> Upload()
        {
            var form = Request.Form;
            if (form.Files.Count == 0)
                return BadRequest();

            var uploadResult = _fileServiceClient.UploadFile();
            var fileToUpload = form.Files[0];

            using (var fileStream = fileToUpload.OpenReadStream())
            {
                var sended = 0L;
                var totalLength = fileStream.Length;
                var buffer = new byte[1024 * 1024];
                while (sended < totalLength)
                {
                    var length = await fileStream.ReadAsync(buffer);
                    sended += length;

                    var request = new UploadFileRequest() { Content = ByteString.CopyFrom(buffer), FileName = fileToUpload.FileName };
                    await uploadResult.RequestStream.WriteAsync(request);
                }
            }

            await uploadResult.RequestStream.CompleteAsync();
            var reply = await uploadResult.ResponseAsync;
            return Ok(reply.FilePath);
        }
    }
}
