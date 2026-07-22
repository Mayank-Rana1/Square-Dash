# 🎮 Square Dash

A fast-paced mobile arcade game built with **Unity** and **C#**, featuring cloud-powered global leaderboards, rewarded advertisements, and performance optimizations for smooth gameplay on low-end Android devices.

---

## ✨ Features

- 🏆 Global Leaderboards using Unity Cloud Services
- 🌍 REST API integration for online score synchronization
- 📱 Optimized for Android devices
- 🎥 Rewarded Video Ads using AdMob & Unity Ads
- ⚡ Object Pooling system for efficient memory management
- 🚀 Stable 60 FPS on low-end devices
- 🎮 Smooth gameplay with optimized physics and rendering

---

## 🛠 Tech Stack

- Unity
- C#
- Unity Cloud Services
- Unity Leaderboards
- REST APIs
- Google AdMob
- Unity Ads
- Android SDK

---

## 🏗 Architecture

```
Player
   │
   ▼
Game Manager
   │
   ├── Score System
   ├── Object Pool
   ├── Spawn Manager
   ├── UI Manager
   └── Ads Manager
          │
          ▼
   AdMob / Unity Ads

Leaderboard Manager
        │
        ▼
Unity Cloud Services
```

---

## 🚀 Performance Optimizations

- Implemented Object Pooling to eliminate frequent Instantiate/Destroy calls.
- Reduced Garbage Collection spikes.
- Optimized update loops and collision handling.
- Maintained a consistent 60 FPS across low-end Android devices.

---

## ☁ Cloud Features

- Global Online Leaderboards
- REST API Integration
- Secure Cloud Score Storage
- Cross-device Score Synchronization

---

## 💰 Monetization

- Google AdMob Integration
- Unity Ads SDK
- Rewarded Video Advertisements
- User-friendly ad placement strategy

---

## 📱 Platform

- Android

---

## Screenshots

> Add gameplay screenshots here.

| Gameplay | Leaderboard |
|----------|-------------|
| ![](images/gameplay.png) | ![](images/leaderboard.png) |

---

## Demo

🎮 **Live Demo:** *(Add your link)*

---

## Installation

```bash
git clone https://github.com/yourusername/Square-Dash-Mobile-Game.git
```

Open the project in Unity and build for Android.

---

## Future Improvements

- Multiplayer mode
- Daily Challenges
- Cloud Save
- Achievements
- Seasonal Events
- More Power-ups

---

## Author

Mayank Rana

---

⭐ If you found this project interesting, consider giving it a star.
