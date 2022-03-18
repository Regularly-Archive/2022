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
        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var current = requestStream.Current;
                var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(current.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                await File.WriteAllBytesAsync(filePath, current.Content.ToByteArray());
                return new UploadFileResponse() { FilePath = fileName };
            }

            return null;
        }

        public override async Task DownloadFile(DownloadFileRequest request, IServerStreamWriter<DownloadFileResponse> responseStream, ServerCallContext context)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), request.FilePath);
            if (File.Exists(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                var content = Google.Protobuf.ByteString.CopyFrom(fileBytes);
                await responseStream.WriteAsync(new DownloadFileResponse() { Content = content });
            }
        }
    }
}
