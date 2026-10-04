namespace PulseID.API.Common
{
    public static class StoragePaths
    {
        public const string RequestPath = "/uploads";

        public static string GetRoot(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var configured = configuration["FileStorage:RootPath"];
            var root = !string.IsNullOrWhiteSpace(configured)
                ? configured
                : Path.Combine(environment.ContentRootPath, "wwwroot", "uploads");
            root = Path.GetFullPath(root);
            Directory.CreateDirectory(root);
            return root;
        }
    }
}
