using UnityEngine;
using TMPro;

public class WelfareHUD : MonoBehaviour
{
    [SerializeField] private WelfareTracker tracker;
    [SerializeField] private TextMeshProUGUI welfareText;
    [SerializeField] private TextMeshProUGUI latencyText;
    [SerializeField] private TextMeshProUGUI auctionCountText;

    void Update()
    {
        welfareText.text = $"Social Welfare: {tracker.SocialWelfare:F1}";
        latencyText.text = $"Avg Latency: {tracker.AverageLatency:F2}";
        auctionCountText.text = $"Auctions: {tracker.TotalAuctions}";
    }
}
