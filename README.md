# Unity 6 Workshop Series 2026

Welcome to the **Games Workshop KU 2026** repository. This project covers the transition from a blank scene to a simple 3D game using Unity 6 (LTS).

## 🚀 Session Overview

### 🛠 Session 1: The Foundations
**Goal:** Navigate the Unity 6 interface and master 3D world-building.
* **Editor Mastery:** Understanding the "Big Five" (Scene, Game, Hierarchy, Inspector, Project).
* **Navigation:** Professional shortcuts for orbiting, panning, and focusing (`F` key focus).
* **GameObjects & Components:** The "Empty Box" mental model—adding Mesh Renderers, Colliders, and Rigidbodies.
* **Transform Logic:** Managing Position, Rotation, and Scale, including Parent-Child hierarchies.

### ⌨️ Session 2: Interaction & Scripting
**Goal:** Moving from static art to interactive gameplay using C#.
* **Input Systems:** Mapping player movement via `Input.GetAxis` and `Input.GetKeyDown`.
* **Variables & Inspector:** Using `[SerializeField]` to tune movement speed and jump force without touching code.
* **The Game Loop:** The difference between `Start()` (initialization) and `Update()` (per-frame logic).
* **Physics Logic:** Controlling `Rigidbody` velocity through scripts for smooth movement.

### 🎮 Session 3: Game Logic & UI
**Goal:** Creating rules, win/loss conditions, and player feedback.
* **Triggers & Collisions:** Using `OnTriggerEnter` for pickups and checkpoints.
* **Script Communication:** Using `GetComponent` to allow objects to interact (e.g., Player hitting a Hazard).
* **The UI Canvas:** Creating HUDs (Heads-Up Displays) that scale across different screen resolutions.
* **Game Management:** Building a "Game Loop" with Start screens, Score tracking, and "Game Over" states.

### ✨ Session 4: Polish & Production
**Goal:** Optimization through Prefabs and final export.
* **The Prefab System:** Creating reusable templates for enemies and pickups to enable global updates.
* **Dynamic Spawning:** Using `Instantiate` and `Destroy` to manage objects at runtime.
* **Audio & Animation:** Implementing `AudioSource` for sound effects and the `Animator Controller` to sync movement with character animations.
* **The Build:** Configuring **Build Settings** and exporting the project as a standalone executable.

---

## 🛠 Setup & Requirements
* **Engine:** Unity 6 (LTS)
* **Template:** 3D (URP) - Universal Render Pipeline
* **Assets:** [Kenney.nl Free 3D Asset Packs](https://kenney.nl/assets)
* **Code Sharing:** [CodeShare.io](https://codeshare.io/)

## 📜 Scripting Best Practices
In this workshop, we follow these coding standards:
1.  **Encapsulation:** Use `[SerializeField]` instead of `public` for inspector variables.
2.  **Performance:** Cache component references in `Awake()` or `Start()`, never in `Update()`.
3.  **Readability:** One responsibility per method; descriptive naming (PascalCase for methods, camelCase for variables).
4.  **Safety:** Always null-check external references before accessing them.
