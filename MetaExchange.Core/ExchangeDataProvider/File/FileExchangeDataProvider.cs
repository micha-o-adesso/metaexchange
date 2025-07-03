using System.Text.Json;
using MetaExchange.Core.Domain;
using MetaExchange.Core.ExchangeDataProvider.File.Mapping;

namespace MetaExchange.Core.ExchangeDataProvider.File;

public class FileExchangeDataProvider(string rootFolderPath) : IExchangeDataProvider
{
    public IEnumerable<Exchange> GetExchanges()
    {
        foreach (var jsonFilePath in Directory.EnumerateFiles(rootFolderPath, "*.json"))
        {
            var exchange = DeserializeExchange(jsonFilePath);
            if (exchange != null)
                yield return exchange.ToDomain();
        }
    }
    
    private File.Model.Exchange? DeserializeExchange(string jsonFilePath)
    {
        try
        {
            using var fileStream = System.IO.File.OpenRead(jsonFilePath);
            var exchange = JsonSerializer.Deserialize<ExchangeDataProvider.File.Model.Exchange>(fileStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });
            return exchange;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error parsing {jsonFilePath}: {e}");
            return null;
        }
    }
}