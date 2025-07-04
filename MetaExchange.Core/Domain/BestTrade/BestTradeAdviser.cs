using MetaExchange.Core.Domain.BestTrade.Model;
using MetaExchange.Core.Domain.Exchange;
using MetaExchange.Core.Domain.Exchange.Model;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Core.Domain.BestTrade;

/// <summary>
/// The best trade adviser is the main component of MetaExchange's core functionality.
/// It analyzes the order books of all exchanges and outputs a set of orders to
/// execute against these order books in order to buy/sell the specified
/// amount of cryptocurrency at the lowest/highest possible price.
/// </summary>
public class BestTradeAdviser
{
    private readonly Dictionary<string, Exchange.Model.Exchange> _exchangesById = new();
    private readonly ILogger<BestTradeAdviser> _logger;

    public BestTradeAdviser(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BestTradeAdviser>();
    }
    
    /// <summary>
    /// Loads the data of all exchanges from the specified exchange data provider.
    /// </summary>
    /// <param name="exchangeDataProvider">The exchange data provider.</param>
    public void LoadExchanges(IExchangeDataProvider exchangeDataProvider)
    {
        _exchangesById.Clear();
        foreach (var exchange in exchangeDataProvider.GetExchanges())
        {
            _exchangesById.Add(exchange.Id, exchange);
        }
    }

    /// <summary>
    /// Outputs a set of orders to execute against these order books in order to buy/sell
    /// the specified amount of cryptocurrency at the lowest/highest possible price.
    /// </summary>
    /// <param name="tradeType">The type of the trade (i.e. Buy or Sell).</param>
    /// <param name="cryptoAmount">The amount of cryptocurrency to trade.</param>
    public Model.BestTrade TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        if (cryptoAmount < 0m)
        {
            throw new ArgumentException(
                "The crypto amount to trade must be greater than or equal to 0.",
                nameof(cryptoAmount));
        }
        
        Model.BestTrade bestTrade = new Model.BestTrade
        {
            RemainingAmountToTrade = cryptoAmount
        };
        
        // for each exchange, we need to keep track of how much funds we have available for trading
        var availableFundsByExchangeId = _exchangesById
            .Values
            .ToDictionary(
                exchange => exchange.Id,
                exchange => tradeType == OrderType.Buy
                    ? exchange.AvailableFunds.Euro     // buy -> we have to consider our available amount of Euro
                    : exchange.AvailableFunds.Crypto); // sell -> we have to consider our available amount of cryptocurrency
        
        // depending on the trade type, we need to get the order book's asks (for buying) or bids (for selling)
        var orderDetails = _exchangesById
            .Values
            .SelectMany(exchange => (tradeType == OrderType.Buy
                    ? exchange.OrderBook.Asks  // buy -> we want to buy at the lowest price, so we look at the asks
                    : exchange.OrderBook.Bids) // sell -> we want to sell at the highest price, so we look at the bids
                .Select(order => new ExchangeOrder(exchange.Id, order)));
        
        // sort all orders from all exchanges by price per crypto unit (EUR/BTC)
        var orderDetailsBestFirst = tradeType == OrderType.Buy
            ? orderDetails // buy -> we want to buy at the lowest price, so we sort by ascending price
                    .OrderBy(orderDetail => orderDetail.Order.PricePerCryptoUnit)
                    .ToList()
            : orderDetails // sell -> we want to sell at the highest price, so we sort by descending price
                .OrderByDescending(orderDetail => orderDetail.Order.PricePerCryptoUnit)
                .ToList();
        
        // loop through the sorted orders and trade crypto until the specified amount is reached
        foreach (var orderDetail in orderDetailsBestFirst)
        {
            if (bestTrade.RemainingAmountToTrade <= 0)
                break;

            // what amount can we trade from this order?
            var amountToTrade = Math.Min(bestTrade.RemainingAmountToTrade, orderDetail.Order.CryptoAmount);
            
            // how much will this reduce our available funds on this exchange?
            var fundReduction= tradeType == OrderType.Buy
                ? amountToTrade * orderDetail.Order.PricePerCryptoUnit // buy -> reduces our available amount of Euro
                : amountToTrade;                                       // sell -> reduces our available amount of cryptocurrency

            // do we have enough available funds on this exchange?
            var availableFundsOnExchange= availableFundsByExchangeId[orderDetail.ExchangeId];
            if (fundReduction > availableFundsOnExchange)
            {
                // not enough funds on this exchange, so we can only trade as much as we have
                _logger.LogInformation(
                    "Not enough funds on exchange '{OrderDetailExchangeId}' to trade {AmountToTrade} crypto at {OrderDetailOrderPricePerCryptoUnit} EUR/BTC.",
                    orderDetail.ExchangeId, amountToTrade, orderDetail.Order.PricePerCryptoUnit);
                
                fundReduction = availableFundsOnExchange;
                amountToTrade = tradeType == OrderType.Buy
                    ? fundReduction / orderDetail.Order.PricePerCryptoUnit // buy -> buy as much as we can afford
                    : fundReduction;                                       // sell -> sell as much as we have
            }

            if (amountToTrade > 0)
            {
                _logger.LogInformation(
                    "Trading {AmountToBuy} crypto at {OrderDetailOrderPricePerCryptoUnit} on exchange '{OrderDetailExchangeId}'.",
                    amountToTrade, orderDetail.Order.PricePerCryptoUnit, orderDetail.ExchangeId);
                
                // reduce the available funds on this exchange
                availableFundsByExchangeId[orderDetail.ExchangeId] -= fundReduction;

                // add a new order recommendation and update the best trade
                bestTrade = new Model.BestTrade
                {
                    RecommendedOrders = bestTrade.RecommendedOrders
                        .Append(new OrderRecommendation
                        {
                            Type = tradeType,
                            PricePerCryptoUnit = orderDetail.Order.PricePerCryptoUnit,
                            CryptoAmount = amountToTrade,
                            ExchangeId = orderDetail.ExchangeId
                        })
                        .ToList(),
                    TotalAmountTraded = bestTrade.TotalAmountTraded + amountToTrade,
                    TotalPriceTraded = bestTrade.TotalPriceTraded + amountToTrade * orderDetail.Order.PricePerCryptoUnit,
                    RemainingAmountToTrade = bestTrade.RemainingAmountToTrade - amountToTrade
                };
            }
        }

        return bestTrade;
    }
}