using System.Text.Json;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Core.ExchangeDataProvider.File.Mapping;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Core.ExchangeDataProvider.File;

public class FileExchangeDataProvider : IExchangeDataProvider
{
    /// <summary>
    /// The search pattern "*.json" for JSON files.
    /// </summary>
    private const string JsonFileSearchPattern = "*.json";
    
    private readonly string _rootFolderPath;
    private readonly ILogger<FileExchangeDataProvider> _logger;

    public FileExchangeDataProvider(string rootFolderPath, ILogger<FileExchangeDataProvider> logger)
    {
        _rootFolderPath = rootFolderPath;
        _logger = logger;
    }
    
    public IEnumerable<Exchange> GetExchanges()
    {
        foreach (var jsonFilePath in Directory.EnumerateFiles(_rootFolderPath, JsonFileSearchPattern))
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
            _logger.LogInformation("Error parsing {JsonFilePath}: {Exception}", jsonFilePath, e);
            return null;
        }
    }
}