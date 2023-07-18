using System.Text.Json;

namespace SkSharp;

public class FileCacheService
{
    internal async Task<LoginTokens> ReadCacheFile(string cacheFilePath) {
        if (!File.Exists(cacheFilePath)) {
            return new LoginTokens();
        }

        var fileContent = await File.ReadAllTextAsync(cacheFilePath);
        var cacheFile = JsonSerializer.Deserialize<LoginTokens>(fileContent);
        if (cacheFile == null)
        {
            throw new Exception("Could not read cache File");
        }
        return cacheFile;
    }

    internal async Task WriteCacheFile(string cacheFilePath, LoginTokens loginTokens) {
        await File.WriteAllTextAsync(cacheFilePath, JsonSerializer.Serialize(loginTokens));
    }
}