using System.Text.Json;
using MetaExchange.Core.Domain.Exchange;
using MetaExchange.Core.Domain.Exchange.Model;
using MetaExchange.Core.Infrastructure.FileExchangeDataProvider.Mapping;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Core.Infrastructure.FileExchangeDataProvider;

/// <summary>
/// An exchange data provider that reads exchange data from JSON files in a specified directory.
/// </summary>
public class FileExchangeDataProvider : IExchangeDataProvider
{
    /// <summary>
    /// The search pattern "*.json" for JSON files.
    /// </summary>
    private const string JsonFileSearchPattern = "*.json";
    
    private readonly string _rootFolderPath;
    private readonly ILogger<FileExchangeDataProvider> _logger;

    public FileExchangeDataProvider(string rootFolderPath, ILoggerFactory loggerFactory)
    {
        _rootFolderPath = rootFolderPath;
        _logger = loggerFactory.CreateLogger<FileExchangeDataProvider>();
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
    
    private Infrastructure.FileExchangeDataProvider.Model.Exchange? DeserializeExchange(string jsonFilePath)
    {
        try
        {
            using var fileStream = File.OpenRead(jsonFilePath);
            var exchange = JsonSerializer.Deserialize<Infrastructure.FileExchangeDataProvider.Model.Exchange>(fileStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });
            return exchange;
        }
        catch (Exception e)
        {
            _logger.LogError("Could not parse {JsonFilePath}: {Exception}", jsonFilePath, e);
            return null;
        }
    }
}