using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcStreamServer.Services
{
    public class FileService : SharedEntities.FileService.FileServiceBase
    {
        private const int ChunkSize = 1024 * 1024;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            var requests = new Queue<UploadFileRequest>();
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                requests.Enqueue(request);
            }

            var first = requests.Peek();
            var fileExt = Path.GetExtension(first.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}{fileExt}";
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload", fileName);

            var fileFolder = Directory.GetParent(filePath);
            if (fileFolder != null && !fileFolder.Exists)
                fileFolder.Create();
            
            using (var fileStream = File.Open(filePath, FileMode.Append, FileAccess.Write))
            {
                var received = 0L;
                while (requests.Count() > 0)
                {
                    var current = requests.Dequeue();
                    var buffer = current.Content.ToByteArray();
                    fileStream.Seek(received, SeekOrigin.Begin);
                    await fileStream.WriteAsync(buffer);
                    received += buffer.Length;
                }

                return new UploadFileResponse() { FilePath = $"/Upload/{fileName}" };
            }
        }

        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), request.FilePath);
            if (File.Exists(filePath))
            {
                using (var fileStream = File.OpenRead(filePath))
                {

                    var received = 0L;
                    var totalLength = fileStream.Length;

                    var buffer = new byte[ChunkSize];
                    while (received < totalLength)
                    {
                        var length = await fileStream.ReadAsync(buffer);
                        received += length;
                        var response = new DownloadFileResponse()
                        {
                            Content = ByteString.CopyFrom(buffer),
                            TotalSize = totalLength
                        };
                        await responseStream.WriteAsync(response);
                    }
                }
            }

        }
    }
}
