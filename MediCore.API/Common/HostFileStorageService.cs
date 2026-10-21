using Microsoft.AspNetCore.Http.Metadata;
using MediCore.Application.Common.Interfaces;
using MediCore.Domain.Exceptions;

namespace MediCore.API.Common
{
    public class HostFileStorageService : IFileStorageService
    {
        const long MaxImageBytes = 10 * 1024 * 1024;      // 10 MB
        const long MaxMedicalFileBytes = 25 * 1024 * 1024; // 25 MB

        static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png",  ".webp"
        };

        static readonly HashSet<string> MedicalExtensions = new(StringComparer.OrdinalIgnoreCase)
            { ".pdf", ".jpg", ".jpeg", ".png", ".webp", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".dcm" };


        readonly string _root;
        readonly IHttpContextAccessor _httpContextAccessor;

        public HostFileStorageService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _root = StoragePaths.GetRoot(configuration, environment);
            _httpContextAccessor = httpContextAccessor;
        }

        static string FolderName(UploadFolder folder) => folder switch
        {
            UploadFolder.MedicalFiles => "medical-files",
            UploadFolder.PersonalImages => "personal-images",
            _ => throw new ArgumentOutOfRangeException(nameof(folder))
        };

        public async Task<string> SaveAsync(IFormFile file, UploadFolder folder, CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0)
                throw new BusinessRuleViolationException("No file uploaded");

            var extension = Path.GetExtension(file.FileName);
            var isImageFolder = folder == UploadFolder.PersonalImages;
            var allowed = isImageFolder ? ImageExtensions : MedicalExtensions;
            var maxBytes = isImageFolder ? MaxImageBytes : MaxMedicalFileBytes;

            if (string.IsNullOrEmpty(extension) || !allowed.Contains(extension))
                throw new BusinessRuleViolationException(
                    $"File type '{extension}' is not allowed. Allowed: {string.Join(", ", allowed)}.");

            if (file.Length > maxBytes)
                throw new BusinessRuleViolationException($"File is too large. Maximum size is {maxBytes / (1024 * 1024)} MB.");

            var folderName = FolderName(folder);
            var directory = Path.Combine(_root, folderName);
            Directory.CreateDirectory(directory);

            var fileName = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
            var fullPath = Path.Combine(directory, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"{folderName}/{fileName}";

        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = ResolveSafe(path);
            if (fullPath is null || !File.Exists(fullPath)) 
            {
                throw new FileNotFoundException("The stored file could not be found.", path);
            }
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(stream);
        }


        public Task DeleteAsync(string? path, CancellationToken cancellationToken = default)
        {
            var fullPath = ResolveSafe(path);
            if (fullPath is not null && File.Exists(fullPath))
                File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public string? GetUrl(string? path)
        {
            var fullPath = ResolveSafe(path);
            if (fullPath is null || !File.Exists(fullPath))
                return null;

            var relative = path!.Replace('\\', '/').TrimStart('/');
            var requestPath = StoragePaths.RequestPath.Trim('/');

            var fullRelativePath = string.IsNullOrEmpty(requestPath)
                ? relative
                : $"{requestPath}/{relative}";

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is null)
                return $"/{fullRelativePath}";

            var pathBase = request.PathBase.Value?.TrimEnd('/') ?? "";
            return $"{request.Scheme}://{request.Host}{pathBase}/{fullRelativePath}";
        }


        string? ResolveSafe(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path))
                return null;

            var fullPath = Path.GetFullPath(Path.Combine(_root, path.Replace('/', Path.DirectorySeparatorChar)));
            var rootWithSeparator = _root.EndsWith(Path.DirectorySeparatorChar)
                ? _root
                : _root + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase) ? fullPath : null;
        }


    }
}
