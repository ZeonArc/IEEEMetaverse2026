using UnityEngine;
using TMPro;

public class AuctionPanelUI : MonoBehaviour
{
    [SerializeField] private AuctionManager auctionManager;
    [SerializeField] private GameObject panelPrefab;

    void OnEnable() => auctionManager.OnAuctionResolved += ShowResult;
    void OnDisable() => auctionManager.OnAuctionResolved -= ShowResult;

    private void ShowResult(NetworkNode node, AuctionResult result)
    {
        var panel = Instantiate(panelPrefab, node.transform.position + Vector3.up * 0.3f, Quaternion.identity);
        
        // Ensure the panel faces the player
        if (panel.GetComponent<PTBillboard>() == null)
        {
            panel.AddComponent<PTBillboard>();
        }

        var tmp = panel.GetComponentInChildren<TextMeshPro>();
        tmp.text = $"{result.Mode} AUCTION @ Node {node.Id}\n" +
                   $"Winner: Agent {result.Winner.GetInstanceID() % 1000}\n" +
                   $"Bid: {result.WinningBid:F1}\n" +
                   $"Paid: {result.PricePaid:F1}\n" +
                   $"Rerouted: {result.Losers.Count} agents";

        Destroy(panel, 4f);
    }
}
