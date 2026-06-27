using System;

public enum RoutingMode { Dijkstra, ShortestHops, CongestionAware }
public enum AuctionMode { SecondPrice, FirstPrice }

// Global, runtime-switchable simulation modes.
public static class GameModes
{
    public static RoutingMode Routing { get; private set; } = RoutingMode.Dijkstra;
    public static AuctionMode Auction { get; private set; } = AuctionMode.SecondPrice;

    // Fired when routing changes so agents can recalculate paths.
    public static event Action OnRoutingChanged;

    public static void SetRouting(RoutingMode mode)
    {
        Routing = mode;
        OnRoutingChanged?.Invoke();
    }

    public static void SetAuction(AuctionMode mode) => Auction = mode;
}
