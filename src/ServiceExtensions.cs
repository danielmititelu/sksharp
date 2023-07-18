using Microsoft.Extensions.DependencyInjection;
using SkSharp.PublicHosted;
using SkSharp.Services;

namespace SkSharp
{
    public static class ServiceExtensions
    {
        public static void AddSkSharpServices(this IServiceCollection services,
            string username,
            string password,
            string cacheFilePath)
        {
            services.AddSingleton(new SkSharpOptions
            {
                Username = username,
                Password = password,
                CacheFilePath = cacheFilePath
            });
            services.AddSingleton<SkypePipe>();
            services.AddSingleton<SkypeService>();
            services.AddSingleton<FileCacheService>();
            services.AddSingleton<SkSharpChat>();
        }
    }
}
