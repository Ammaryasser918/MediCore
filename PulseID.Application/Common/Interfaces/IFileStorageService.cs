using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveToUploadsFolder(IFormFile file, CancellationToken cancellationToken = default);
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    }
}
