namespace MetaExchange.Core.Domain.BestTrade.Model;

public class BestTrade
{
    /// <summary>
    /// The orders to execute for the best trade.
    /// </summary>
    public List<OrderRecommendation> RecommendedOrders { get; init; } = [];

    /// <summary>
    /// The total amount of the trade, which is the sum of all order amounts in the trade.
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// The total price of the trade, which is the sum of all order prices in the trade.
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// The average price per unit of the trade, calculated as TotalPrice divided by TotalAmount.
    /// </summary>
    public decimal? AveragePricePerUnit => TotalAmount == 0
        ? null
        : TotalPrice / TotalAmount;
}