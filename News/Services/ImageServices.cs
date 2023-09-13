using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NewsAPI.Helpers;

namespace NewsAPI.Services
{
    public class ImageServices : IImageServices
    {
        public IConfiguration Configuration { get; }
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly Cloudinary _cloudinary;
        private readonly List<string> _extensionsAllowed = new() { ".jpg", ".png", ".jpeg" };
        private readonly long _maxAllowedPosterSize = 2097152; // 2MB

        public ImageServices(IConfiguration configuration)
        {
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var account = new Account
                (
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret
                );

            _cloudinary = new Cloudinary(account);

        }

        public async Task<string> UploadImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!IsExtensionAllowed(extension))
            {
                throw new Exception("Invalid image type, only (jpg, png, and jpeg) are allowed.");
            }
            if (!IsSizeAllowed(file.Length))
            {
                throw new Exception("Max allowed size for image is 2MB.");
            }
            var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UploadPreset = "ml_default"
            });

            return uploadResult.SecureUrl.AbsoluteUri;
        }

        private bool IsExtensionAllowed(string extension)
        {
            return _extensionsAllowed.Contains(extension);
        }

        private bool IsSizeAllowed(long size)
        {
            return size < _maxAllowedPosterSize;
        }


    }
}
