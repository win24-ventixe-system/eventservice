using Microsoft.AspNetCore.Http;

namespace Data.Handlers;

public interface IFileHandler
{
    Task<string> UploadFileAsync(IFormFile file);
}
