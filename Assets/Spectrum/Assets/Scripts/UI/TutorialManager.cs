using UnityEngine;
using TMPro;


public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro instructionText;
    [SerializeField] private NetworkGraph graph;
    [SerializeField] private AuctionManager auctionManager;

    private int _step;
    private bool _grabbed;

    private readonly string[] _steps = {
        "Welcome to Spectrum!\nGrab a node with your controller grip button.",
        "Great! Move it around.\nWatch the edge costs change in real-time.",
        "Agents route packets along cheapest paths.\nNodes turn red when congested.",
        "When congestion hits 80%, a Vickrey Auction triggers.\nHighest bidder wins, pays second-highest price.",
        "Your goal: arrange the network to maximize Social Welfare.\nExperiment freely!",
    };

    void Start()
    {
        ShowStep(0);
        foreach (var node in graph.Nodes)
            node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectEntered.AddListener(_ => OnGrab());

        auctionManager.OnAuctionResolved += OnFirstAuction;
    }

    private void OnGrab()
    {
        if (!_grabbed) { _grabbed = true; Advance(); }
    }

    private void OnFirstAuction(NetworkNode n, AuctionResult r)
    {
        if (_step == 3) Advance();
        auctionManager.OnAuctionResolved -= OnFirstAuction;
    }

    public void Advance()
    {
        _step++;
        if (_step < _steps.Length) ShowStep(_step);
        else instructionText.gameObject.SetActive(false);
    }

    private void ShowStep(int i) => instructionText.text = _steps[i];
}
