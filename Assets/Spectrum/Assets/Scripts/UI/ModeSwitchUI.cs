using UnityEngine;
using UnityEngine.UI;
using TMPro;

// World-space panel to switch routing/auction modes at runtime.
// Wire two buttons + two labels in the inspector.
public class ModeSwitchUI : MonoBehaviour
{
    [SerializeField] private Button routingButton;
    [SerializeField] private TextMeshProUGUI routingLabel;
    [SerializeField] private Button auctionButton;
    [SerializeField] private TextMeshProUGUI auctionLabel;

    void Start()
    {
        routingButton.onClick.AddListener(CycleRouting);
        auctionButton.onClick.AddListener(CycleAuction);
        Refresh();
    }

    private void CycleRouting()
    {
        int next = ((int)GameModes.Routing + 1) % System.Enum.GetValues(typeof(RoutingMode)).Length;
        GameModes.SetRouting((RoutingMode)next);
        Refresh();
    }

    private void CycleAuction()
    {
        int next = ((int)GameModes.Auction + 1) % System.Enum.GetValues(typeof(AuctionMode)).Length;
        GameModes.SetAuction((AuctionMode)next);
        Refresh();
    }

    private void Refresh()
    {
        routingLabel.text = $"Routing: {GameModes.Routing}";
        auctionLabel.text = $"Auction: {GameModes.Auction}";
    }
}
