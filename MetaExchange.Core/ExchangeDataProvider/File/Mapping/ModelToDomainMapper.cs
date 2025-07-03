using MetaExchange.Core.Domain;

namespace MetaExchange.Core.ExchangeDataProvider.File.Mapping;

public static class ModelToDomainMapper
{
     public static Exchange ToDomain(this Model.Exchange model)
     {
         return new Exchange
         {
             Id = model.Id,
             AvailableFunds = model.AvailableFunds.ToDomain(),
             OrderBook = model.OrderBook.ToDomain()
         };
     }

     private static AvailableFunds ToDomain(this Model.AvailableFunds model)
     {
         return new AvailableFunds
         {
             Crypto = model.Crypto,
             Euro = model.Euro
         };
     }

     private static OrderBook ToDomain(this Model.OrderBook model)
     {
         return new OrderBook
         {
             Bids = model.Bids
                 .Select(o => o.Order.ToDomain())
                 .ToList(),
             Asks = model.Asks
                 .Select(o => o.Order.ToDomain())
                 .ToList()
         };
     }

     private static Order ToDomain(this Model.Order model)
     {
         return new Order
         {
             Id = model.Id,
             Time = model.Time,
             Type = model.Type.ToDomain(),
             Kind = model.Kind.ToDomain(),
             Amount = model.Amount,
             Price = model.Price
         };
     }

     private static OrderType ToDomain(this Model.OrderType model)
     {
         return model switch
         {
             Model.OrderType.Buy => OrderType.Buy,
             Model.OrderType.Sell => OrderType.Sell,
             _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
         };
     }

     private static OrderKind ToDomain(this Model.OrderKind model)
     {
         return model switch
         {
             Model.OrderKind.Limit => OrderKind.Limit,
             _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
         };
     }
}