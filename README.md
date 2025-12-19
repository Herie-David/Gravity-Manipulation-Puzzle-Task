# Gravity Manipulation Technical Test

A third-person gravity manipulation prototype developed in Unity.  
This project demonstrates a robust, decoupled architecture for dynamic 3D physics, custom grounding systems, and relative input mapping.

---

## Key Features

### Dynamic Gravity System
A centralized `GravityManager` that utilizes the Observer Pattern to notify game entities (Player, Camera, Hologram) of physics shifts without tight coupling.

### Local-Relative Controls
Gravity manipulation is mapped to the character’s local orientation, ensuring that directional selections such as *Up* or *Right* remain intuitive regardless of the player’s current surface.

### Custom Grounding System
Replaces `CharacterController.isGrounded`, which is hardcoded to world-down, with a custom Raycast-based grounding solution to support full 360-degree navigation across walls and ceilings.

### Collision-Aware Camera
A third-person camera system featuring SphereCast-based occlusion detection to prevent environmental clipping, along with smooth rotational slerping to maintain visual stability during gravity transitions.

### Game State Management
A Singleton-based `GameSessionManager` responsible for:
- A 2-minute countdown timer  
- Collectible tracking  
- Win and loss conditions  

---

## Technical Architecture

### 1. Decoupled Logic
The project adheres to the Single Responsibility Principle (SRP):

- **InputReader**  
  Captures hardware input and translates it into player intent.

- **GravityManager**  
  Acts as the single source of truth for world gravity and physics orientation.

- **PlayerController**  
  Handles player movement, physics interaction, and animation state.

- **HologramController**  
  Manages visual gravity previews without affecting gameplay state.

---

### 2. Physics and Locomotion
To prevent jitter and tunneling issues common with character controllers, all movement and gravity forces are consolidated into a single `CharacterController.Move()` call per frame. Player velocity is reset during gravity transitions to avoid unintended impulse or slingshot effects.

---

## Controls

- **Move:** W, A, S, D  
- **Jump:** Space  
- **Select Gravity:** Arrow Keys (relative to player orientation)  
- **Commit Gravity Change:** Enter (Return)  
- **Restart (Game Over):** UI Button  

---

## Submission Details

- **Unity Version:** 2022.3.62f3 LTS  
- **Platforms:** Standalone builds for Windows and macOS  
- **Build Location:** `/Builds` directory
