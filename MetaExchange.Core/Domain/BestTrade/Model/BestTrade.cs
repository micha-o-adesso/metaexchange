namespace MetaExchange.Core.Domain.BestTrade.Model;

/// <summary>
/// The best trade contains recommended orders to buy/sell a specified
/// amount of cryptocurrency at the lowest/highest possible price.
/// Additionally, it provides information about traded and remaining
/// amounts, prices, and whether the full amount has been traded.
/// </summary>
public class BestTrade
{
    /// <summary>
    /// The recommended orders to execute for achieving the best trade.
    /// </summary>
    public List<OrderRecommendation> RecommendedOrders { get; init; } = [];

    /// <summary>
    /// The total amount of traded cryptocurrency (in BTC).
    /// </summary>
    public decimal TotalAmountTraded { get; init; }
    
    /// <summary>
    /// The total price of traded cryptocurrency (in EUR).
    /// </summary>
    public decimal TotalPriceTraded { get; init; }
    
    /// <summary>
    /// The remaining amount of cryptocurrency (in BTC) that could not be traded due to insufficient orders.
    /// </summary>
    public decimal RemainingAmountToTrade { get; init; }
    
    /// <summary>
    /// The average price per unit (in EUR/BTC) of the traded cryptocurrency.
    /// Calculated as TotalPriceTraded divided by TotalAmountTraded.
    /// Is <c>null</c> if TotalAmountTraded is 0.
    /// </summary>
    public decimal? AveragePricePerUnit => TotalAmountTraded == 0m
        ? null
        : TotalPriceTraded / TotalAmountTraded;

    /// <summary>
    /// A flag indicating whether the full amount of cryptocurrency has been traded.
    /// </summary>
    public bool IsFullAmountTraded => RemainingAmountToTrade <= 0m;
}