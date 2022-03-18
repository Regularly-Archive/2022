using Google.Protobuf;
using Grpc.Core;
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
        private const int ChunkSize = 1024 * 512;
        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.png";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            using(var fileStream = File.Open(filePath, FileMode.Append, FileAccess.Write))
            {
                var writeSize = 0;
                while (await requestStream.MoveNext())
                {
                    var current = requestStream.Current;
                    var bytes = current.ToByteArray();

                    fileStream.Seek(writeSize, SeekOrigin.Begin);
                    await fileStream.WriteAsync(bytes, 0, bytes.Length);
                    writeSize += bytes.Length;
                }

                return new UploadFileResponse() { FilePath = fileName };
            }
        }

        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), request.FilePath);
            if (File.Exists(filePath))
            {
                using (var fileStream = File.OpenRead(filePath))
                {

                    var readSize = 0;
                    var fileSize = fileStream.Length;

                    var buffer = new byte[ChunkSize];
                    while (readSize < fileSize)
                    {
                        fileStream.Seek(readSize, SeekOrigin.Begin);
                        fileStream.Read(buffer, 0, buffer.Length);
                        await responseStream.WriteAsync(new DownloadFileResponse() { Content = ByteString.CopyFrom(buffer), TotalSize = fileSize });
                        readSize += buffer.Length;
                    }
                }
            }
            
        }
    }
}
