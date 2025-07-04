namespace MetaExchange.Core.Domain.Exchange.Model;

/// <summary>
/// The available funds on an exchange, i.e. which amounts of cryptocurrency
/// and Euro are stored on the crypto exchange account.
/// </summary>
public class AvailableFunds
{
    /// <summary>
    /// The available amount of cryptocurrency on the account (e.g. 10.8503 BTC).
    /// </summary>
    public required decimal Crypto { get; init; }
    
    /// <summary>
    /// The available amount of Euro on that account (e.g. 117520.12 EUR).
    /// </summary>
    public required decimal Euro { get; init; }
}