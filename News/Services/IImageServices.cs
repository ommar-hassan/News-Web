using CloudinaryDotNet.Actions;

namespace NewsAPI.Services
{
    public interface IImageServices
    {
        Task<string> UploadImage(IFormFile file);
    }
}
