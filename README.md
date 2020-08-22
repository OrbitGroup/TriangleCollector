# TriangleCollector

TriangleCollector is a containerized .NET Core app which determines all possible triangles in an exchange, subscribes to the relevant orderbooks, and calculates triangle profitability based on orderbook updates.

This is very much a work-in-progress and there is still a lot to be done.

## Development

Development should be fully supported on Linux/Windows/Mac as the project does not take a dependency on anything other than native .NET Core libraries. 

All you need to be able to run this project is .NET Core 3.1 and an IDE. You do not need to have Docker installed. Just load the solution and try to run it, your IDE (Visual Studio, Rider, etc) should automatically restore the nuget packages like magic.

## Models

The models consist of Orderbook, Triangle, and OrderbookConverter. The only real things of note here are:

- The orderbook model contains the logic for merging orderbook updates.

- The triangle model contains the logic for determining profitability. 

- OrderbookConverter is used for json deserialization of orderbook snapshots and updates. Although the code isn't perfect, it's very fast and typically deserialization takes somewhere in the range of 1-5ms.

## Services

Services are the construct for the BackgroundService abstraction in .NET Core. There are 6 services that are running mostly simultaneously in TriangleCollector, with the exception being OrderbookSubscriber which right now is not running continuously.

### Orderbook Subscriber

This service gets all the symbols from the exchange, then determines all possible triangles, then sends the subscription message to the exchange API for each triangle. The subscriber creates a new ClientWebSocket and OrderbookListener per group of symbols as determined in the subscriber service (currently 50 symbols per ClientWebSocket).

### Orderbook Listener

Orderbook Listeners are created by the OrderbookSubscriber. OrderbookSubscriber is what sets the number symbols that a given ClientWebSocket can have. ClientWebSockets and Orderbook Listeners are 1:1. If the max symbols in the subscriber is set to 50, and you have 200 pairs, you will end up with 4 ClientWebSockets and 4 Orderbook Listeners with 50 symbols each.

This listens for orderbook updates and merges them into the corresponding "official" orderbook for that symbol. Official in this codebase simply refers to the one that the TriangleCalculator treats as the real orderbook.

Once an update comes in and has been merged into the official orderbook, the symbol is added to the UpdateSymbols queue.

### SymbolMonitor

The SymbolMonitor is watching the UpdatedSymbols queue and getting the impacted triangles given each symbol update (which was precalculated) and pushing those triangles to the TrianglesToRecalculate queue. I think eventually this service will be combined with another.

### TriangleCalculator

TriangleCalculator is listening to the TrianglesToRecalculate queue and then grabbing all 3 orderbooks, updating the Triangle objects' ask/bid prices for each coin, and then recalculating profitability. Triangles in the Triangles dictionary are updated and refresh times are updated in the TriangleRefreshTimes dictionary.