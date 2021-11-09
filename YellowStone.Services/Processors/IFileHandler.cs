using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YellowStone.Models.DTO;
using YellowStone.Services.FBNService.DTOs;

namespace YellowStone.Services.Processors
{
   public interface IFileHandler
    {
        Task UploadFile(IFormFile formFile, string fileName, string fileType);
        Task<byte[]> GetFile(string fileName, string fileType);
        Task DeleteFile(string fileName, string fileType);
        Task ArchiveFile(string fileName, string fileType);
    }
}
