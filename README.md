<div align="center">

# 🌐 Spectrum — The Congestion Auction

**A VR sandbox that turns game theory into something you can grab with your hands.**

Manipulate a live data network in virtual reality and watch algorithmic mechanism design unfold in real time — shortest-path routing, congestion, and second-price auctions, all driven by your own hands.

[![Unity](https://img.shields.io/badge/Unity-6_LTS-000000?logo=unity&logoColor=white)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Meta_Quest_|_PCVR-1C1C1C)](https://www.meta.com/quest/)
[![XR](https://img.shields.io/badge/XR-OpenXR-5586EC)](https://www.khronos.org/openxr/)
[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Educational-blue)](#-license)

</div>

---

## 📖 Overview

**Spectrum** is an educational VR experience that teaches **Algorithmic Mechanism Design** and **Network Routing** through direct, physical interaction.

You stand inside a virtual command center surrounded by a floating network graph. Grab nodes with your controllers, drag them through space, and watch autonomous agents reroute their packets in real time. As traffic concentrates and nodes saturate, the simulation triggers a **Vickrey (second-price) auction** — letting you *see* why truthful bidding is the dominant strategy, rather than just reading it in a textbook.

> Abstract concepts like incentive compatibility and the price of anarchy become tangible when you can reshape the network with your hands and immediately feel the consequences.

---

## ✨ Features

- 🕹️ **Hands-on graph manipulation** — grab and move nodes; edge costs update live with distance.
- 📦 **Autonomous routing** — agents continuously find cheapest paths via Dijkstra's algorithm.
- 🚦 **Visible congestion** — nodes shift green → yellow → red; sprite swarms fracture and spatial audio rises with load.
- 🔨 **Vickrey auctions** — at capacity, agents bid their true valuation; the winner pays the second-highest price, losers reroute.
- 📊 **Social welfare scoring** — a live HUD tracks system-wide efficiency for you to optimize.
- 🎓 **Guided tutorial** — a built-in onboarding sequence introduces each mechanic in context.

---

## 🎮 How It Works

```
You move a node
      │
      ▼
Edge costs recalculate (Euclidean distance)
      │
      ▼
Agents reroute along new shortest paths  ──►  Node loads change
      │                                              │
      ▼                                              ▼
Congestion crosses 80% capacity            Color · audio · sprite feedback
      │
      ▼
Vickrey auction fires  ──►  Winner keeps route, pays 2nd price
      │
      ▼
Social welfare recalculated  ──►  HUD updates
```

---

## 🛠️ Tech Stack

| Component | Technology |
|-----------|-----------|
| Engine | Unity 6 LTS (Universal Render Pipeline) |
| VR Runtime | OpenXR |
| Interaction | XR Interaction Toolkit 2.5+ |
| Input | Unity Input System |
| Language | C# / .NET Standard 2.1 |
| Rendering | GPU instancing (`Graphics.DrawMeshInstanced`) |
| Targets | Meta Quest 2/3 (standalone) · PCVR (SteamVR / Quest Link) |

---

## 📁 Project Structure

```
Spectrum/
├── Assets/
│   ├── Scripts/
│   │   ├── Graph/        # NetworkGraph, NetworkNode, NetworkEdge, GraphSpawner, GraphSaveLoad
│   │   ├── Routing/      # DijkstraSolver, RoutingAgent, AgentSpawner
│   │   ├── Auction/      # VickreyAuction, AuctionManager
│   │   ├── Welfare/      # WelfareTracker
│   │   ├── Rendering/    # SpriteSwarmRenderer, CongestionVisuals
│   │   ├── Audio/        # CongestionAudio
│   │   └── UI/           # TutorialManager, AuctionPanelUI, WelfareHUD
│   ├── Prefabs/          # Node, Edge, Packet
│   ├── Materials/        # Node, Edge, Packet, Sprite, Panel, Ground
│   ├── Sprites/          # node_particle_16x16.png
│   ├── Audio/            # congestion_hum.wav
│   ├── UI/               # AuctionPanel, WelfareHUD
│   └── Scenes/           # CommandCenter.unity
├── README.md
├── SETUP.md              # Installation & configuration
├── BUILD_GUIDE.md        # Step-by-step build walkthrough (Parts A–O)
├── PREFABS.md            # Prefab construction details
├── OBJECTS.md            # Complete object manifest
└── ARCHITECTURE.md       # System design & data flow
```

---

## 🚀 Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download) with **Unity 6 LTS**
- A VR headset (Meta Quest 2/3 or any OpenXR-compatible device)
- Windows 10/11 with a VR-capable GPU

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/ZeonArc/IIIIMetaverse2026.git

# 2. Open the project in Unity Hub (Unity 6 LTS)

# 3. Open the scene
#    Assets/Scenes/CommandCenter.unity

# 4. Connect your headset and press Play
```

For full installation, prefab assembly, and scene wiring, follow the **[Build Guide](BUILD_GUIDE.md)** from Part A. It walks you through every step with verification checkpoints.

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| **[BUILD_GUIDE.md](BUILD_GUIDE.md)** | Complete A–O walkthrough from install to deploy |
| **[SETUP.md](SETUP.md)** | Software prerequisites, configuration, troubleshooting |
| **[PREFABS.md](PREFABS.md)** | Step-by-step prefab construction |
| **[OBJECTS.md](OBJECTS.md)** | Full manifest of every object to create |
| **[ARCHITECTURE.md](ARCHITECTURE.md)** | System layers, data flow, performance notes |

---

## 🎯 Educational Goals

- Understand why **truthful bidding** is optimal in second-price (Vickrey) auctions.
- Observe how selfish routing produces congestion — the **price of anarchy**.
- Experience how **mechanism design** can steer a system toward better social welfare.
- Build spatial intuition for **graph algorithms** through direct manipulation.

---

## 🗺️ Roadmap

- [ ] Alternative auction types (first-price, combinatorial)
- [ ] Scenario mode with preset topologies and target welfare scores
- [ ] Larger networks with priority-queue Dijkstra
- [ ] Multiplayer comparison mode
- [ ] Hand-tracking support

---

## 🤝 Contributing

Contributions are welcome. Please open an issue to discuss substantial changes before submitting a pull request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes
4. Push the branch and open a pull request

---

## 📄 License

This project is released for **educational purposes**. See the repository for details.

---

<div align="center">

*Built with Unity · Designed to make game theory something you can hold.*

</div>
