using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Implementations
{
    public class FileHandler : IFileHandler
    {
        private readonly IOptions<SystemSettings> _options;

        public FileHandler(IOptions<SystemSettings> options)
        {

            _options = options;
        }

        public Task ArchiveFile(string fileName, string fileType)
        {
            string sourceFilePath = $"{_options.Value.AttachmentsServerPath}{fileName}.{fileType}";
            string destinationFilePath = $"{_options.Value.ArchiveServerPath}{fileName}.{fileType}";

            if (File.Exists(sourceFilePath))
            {
                File.Move(sourceFilePath, destinationFilePath);
            }

            return Task.CompletedTask;

        }

        public async Task DeleteFile(string fileName, string fileType)
        {
            string ReportURL = $"{_options.Value.AttachmentsServerPath}{fileName}.{fileType}";

            if (File.Exists(ReportURL))
            {
                File.Delete(ReportURL);
            }
        }

        public Task<byte[]> GetFile(string fileName, string fileType)
        {
            string ReportURL = $"{_options.Value.AttachmentsServerPath}{fileName}.{fileType}";
            byte[] fileBytes = File.ReadAllBytes(ReportURL);
            return Task.FromResult(fileBytes);
        }

        public async Task UploadFile(IFormFile formFile, string fileName, string fileType)
        {
            var filePath = $"{_options.Value.AttachmentsServerPath}{fileName}.{fileType}";
            if (formFile.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }

        }
    }
}
