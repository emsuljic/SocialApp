using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            //smth to store our resulting that we're gettting back from Cloudinary

            var uploadResult = new ImageUploadResult();

            //chek to see if we have smth in file parameter
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParamas = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                //upload file to Cloudinary
                uploadResult = await _cloudinary.UploadAsync(uploadParamas);

            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            
            return result;
        }
        
    }
}