using System.Collections.Generic;
using System.Linq;

public struct AuctionResult
{
    public RoutingAgent Winner;
    public float WinningBid;
    public float PricePaid;
    public List<RoutingAgent> Losers;
    public AuctionMode Mode;
}

public static class AuctionResolver
{
    public static AuctionResult Resolve(List<RoutingAgent> bidders)
    {
        return GameModes.Auction == AuctionMode.FirstPrice
            ? FirstPrice(bidders)
            : SecondPrice(bidders);
    }

    // Vickrey: truthful bidding is dominant. Winner pays the second-highest bid.
    private static AuctionResult SecondPrice(List<RoutingAgent> bidders)
    {
        var sorted = bidders.OrderByDescending(a => a.Valuation).ToList();
        return new AuctionResult
        {
            Winner = sorted[0],
            WinningBid = sorted[0].Valuation,
            PricePaid = sorted.Count > 1 ? sorted[1].Valuation : 0f,
            Losers = sorted.Skip(1).ToList(),
            Mode = AuctionMode.SecondPrice
        };
    }

    // First-price: agents shade bids below true value (Nash bid = v*(n-1)/n).
    // Winner pays their own (shaded) bid — so truthful bidding is NOT optimal.
    private static AuctionResult FirstPrice(List<RoutingAgent> bidders)
    {
        int n = bidders.Count;
        float shade = (n - 1f) / n;
        var sorted = bidders.OrderByDescending(a => a.Valuation * shade).ToList();
        float winBid = sorted[0].Valuation * shade;
        return new AuctionResult
        {
            Winner = sorted[0],
            WinningBid = winBid,
            PricePaid = winBid,
            Losers = sorted.Skip(1).ToList(),
            Mode = AuctionMode.FirstPrice
        };
    }
}
