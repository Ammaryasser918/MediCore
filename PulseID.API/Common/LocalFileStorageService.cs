using PulseID.Application.Common.Interfaces;

namespace PulseID.API.Common
{
    public class LocalFileStorageService : IFileStorageService
    {
        public async Task<string> SaveToUploadsFolder(IFormFile file, CancellationToken cancellationToken = default)
        {
            var uploadDirectory = @"D:\VS_projects\PulseID\PulseID.API\FileUploads";

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("The stored medical file could not be found.", path);
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return Task.CompletedTask;
            }
            throw new FileNotFoundException("This File Doesn't Exist.");
        }


    }
}
