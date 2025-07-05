using MetaExchange.Domain.Modules.BestTrade.Model;
using MetaExchange.Domain.Modules.Exchange;
using MetaExchange.Domain.Modules.Exchange.Model;
using Microsoft.Extensions.Logging;

namespace MetaExchange.Domain.Modules.BestTrade;

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
    public Modules.BestTrade.Model.BestTrade TradeCryptoAtBestPrice(OrderType tradeType, decimal cryptoAmount)
    {
        if (cryptoAmount < 0m)
        {
            throw new ArgumentException(
                "The crypto amount to trade must be greater than or equal to 0.",
                nameof(cryptoAmount));
        }
        
        Modules.BestTrade.Model.BestTrade bestTrade = new Modules.BestTrade.Model.BestTrade
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
        var exchangeOrders = _exchangesById
            .Values
            .SelectMany(exchange => (tradeType == OrderType.Buy
                    ? exchange.OrderBook.Asks  // buy -> we want to buy at the lowest price, so we look at the asks
                    : exchange.OrderBook.Bids) // sell -> we want to sell at the highest price, so we look at the bids
                .Select(order => new ExchangeOrder(exchange.Id, order)));
        
        // sort all orders from all exchanges by price per crypto unit (EUR/BTC)
        var exchangeOrdersBestFirst = tradeType == OrderType.Buy
            ? exchangeOrders // buy -> we want to buy at the lowest price, so we sort by ascending price
                    .OrderBy(exchangeOrder => exchangeOrder.Order.PricePerCryptoUnit)
                    .ToList()
            : exchangeOrders // sell -> we want to sell at the highest price, so we sort by descending price
                .OrderByDescending(exchangeOrder => exchangeOrder.Order.PricePerCryptoUnit)
                .ToList();
        
        // loop through the sorted orders and trade crypto until the specified amount is reached
        foreach (var exchangeOrder in exchangeOrdersBestFirst)
        {
            if (bestTrade.RemainingAmountToTrade <= 0)
                break;

            // what amount can we trade from this order?
            var amountToTrade = Math.Min(bestTrade.RemainingAmountToTrade, exchangeOrder.Order.CryptoAmount);
            
            // how much will this reduce our available funds on this exchange?
            var fundReduction= tradeType == OrderType.Buy
                ? amountToTrade * exchangeOrder.Order.PricePerCryptoUnit // buy -> reduces our available amount of Euro
                : amountToTrade;                                         // sell -> reduces our available amount of cryptocurrency

            // do we have enough available funds on this exchange?
            var availableFundsOnExchange= availableFundsByExchangeId[exchangeOrder.ExchangeId];
            if (fundReduction > availableFundsOnExchange)
            {
                // not enough funds on this exchange, so we can only trade as much as we have
                _logger.LogInformation(
                    "Not enough funds on exchange '{ExchangeOrderExchangeId}' to trade {AmountToTrade} crypto at {ExchangeOrderOrderPricePerCryptoUnit} EUR/BTC.",
                    exchangeOrder.ExchangeId, amountToTrade, exchangeOrder.Order.PricePerCryptoUnit);
                
                fundReduction = availableFundsOnExchange;
                amountToTrade = tradeType == OrderType.Buy
                    ? fundReduction / exchangeOrder.Order.PricePerCryptoUnit // buy -> buy as much as we can afford
                    : fundReduction;                                         // sell -> sell as much as we have
            }

            if (amountToTrade > 0)
            {
                _logger.LogInformation(
                    "Trading {AmountToBuy} crypto at {ExchangeOrderOrderPricePerCryptoUnit} on exchange '{ExchangeOrderExchangeId}'.",
                    amountToTrade, exchangeOrder.Order.PricePerCryptoUnit, exchangeOrder.ExchangeId);
                
                // reduce the available funds on this exchange
                availableFundsByExchangeId[exchangeOrder.ExchangeId] -= fundReduction;

                // add a new order recommendation and update the best trade
                bestTrade = new Modules.BestTrade.Model.BestTrade
                {
                    RecommendedOrders = bestTrade.RecommendedOrders
                        .Append(new OrderRecommendation
                        {
                            Type = tradeType,
                            PricePerCryptoUnit = exchangeOrder.Order.PricePerCryptoUnit,
                            CryptoAmount = amountToTrade,
                            ExchangeId = exchangeOrder.ExchangeId
                        })
                        .ToList(),
                    RemainingAmountToTrade = bestTrade.RemainingAmountToTrade - amountToTrade,
                    TotalAmountTraded = bestTrade.TotalAmountTraded + amountToTrade,
                    TotalPriceTraded = bestTrade.TotalPriceTraded + amountToTrade * exchangeOrder.Order.PricePerCryptoUnit
                };
            }
        }

        return bestTrade;
    }
}