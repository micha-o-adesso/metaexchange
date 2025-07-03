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
             Type = model.Type,
             Kind = model.Kind,
             Amount = model.Amount,
             Price = model.Price
         };
     }
}