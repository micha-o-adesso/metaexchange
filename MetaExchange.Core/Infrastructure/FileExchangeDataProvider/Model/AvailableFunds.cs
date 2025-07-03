namespace MetaExchange.Core.Infrastructure.FileExchangeDataProvider.Model;

/// <summary>
/// The available funds on an exchange.
/// </summary>
public class AvailableFunds
{
    /// <summary>
    /// The available funds in the cryptocurrency BTC (e.g. 10.8503).
    /// </summary>
    public required decimal Crypto { get; set; }
    
    /// <summary>
    /// The available funds in the fiat currency EUR (e.g. 117520.12).
    /// </summary>
    public required decimal Euro { get; set; }
}