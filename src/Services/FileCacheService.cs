using System.Text.Json;

namespace SkSharp;

internal class FileCacheService
{
    internal async Task<LoginTokens?> ReadCacheFile(string cacheFilePath) {
        if (!File.Exists(cacheFilePath)) {
            return null;
        }

        var fileContent = await File.ReadAllTextAsync(cacheFilePath);
        var cacheFile = JsonSerializer.Deserialize<LoginTokens>(fileContent);
        return cacheFile;
    }

    internal async Task WriteCacheFile(string cacheFilePath, LoginTokens loginTokens) {
        await File.WriteAllTextAsync(cacheFilePath, JsonSerializer.Serialize(loginTokens));
    }
}