# VRChat Drone Controller

This project contains scripts implementing a physics-based, player-piloted drone in VRChat. It uses a base-class inheritance structure to provide two different flight models while maximizing code re-use.

The system is designed to work for both VR and Desktop players, with a focus on customizable "game feel" through exposed UI Sliders for real-time physics tuning.

## Project Structure

This project uses an inheritance model to reduce duplicate code (following the DRY principle).

* **`BaseDroneController.cs`** (Parent Class)
    * This script is **not** meant to be attached to any GameObject.
    * It contains all the **shared logic** for both drone types.
    * **Handles:**
        * All VR and Desktop input reading in `Update()`.
        * Reading all UI Slider values in `Update()` and applying them to the `Rigidbody` (mass, angular drag).
        * Storing the drone's start position/rotation for use in the `ResetPosition()` function.
        * The core `VRControls()` and `DesktopControls()` functions that apply forces and torque to move the drone itself.

* **`new_drone.cs`** (Child Class - "Advanced Drone")
    * Inherits from `BaseDroneController`.
    * This is the script you attach to your drone for the "advanced" flight model.
    * **Adds:**
        * An **auto-stabilizer** that levels the drone when no input is given.
        * `isPiloting` logic to detect when the player is actively piloting the drone.
        * A `FixedUpdate()` with custom gravity and a simple braking force (`dragValue`) that fights upward momentum.

* **`droneMove.cs`** (Child Class - "Simple Drone")
    * Inherits from `BaseDroneController`.
    * This is the script you attach for the "simple" flight model.
    * **Adds:**
        * Logic to disable the seat's `BoxCollider` when seated.
        * A `FixedUpdate()` with custom gravity and a simple braking force (`dragValue`) that fights upward momentum.

## Key Features

* **Physics-Based Flight**: All movement is handled by a `Rigidbody` for realistic motion.
* **Dual-Input Support**: Works seamlessly for both Desktop (WASD + Arrow Keys) and VR (Oculus Touch) players.
* **Real-Time Tuning**: Uses Unity UI Sliders to control:
    * `Mass`
    * `Angular Drag`
    * `Pitch` Multiplier
    * `Roll` Multiplier
    * `Yaw` Multiplier
    * `Thrust` Multiplier
* **VRChat Ready**: Includes all necessary logic for a `VRCStation` to pilot the drone.
* **Auto-Stabilizer**: The `new_drone` script includes an optional stabilizer to make flight easier.
* **Clean, Reusable Code**: Inheritance model makes it easy to maintain or create new drone types.

## Logic

* **Input Reading (`BaseDroneController.Update`)**: All player inputs (VR and Desktop) are read every frame in the base class and stored in `protected` variables (like `vrThrottle`, `desktopYaw`, etc.). This keeps all input logic in one place.
 
* **Physics (`new_drone.FixedUpdate` / `droneMove.FixedUpdate`)**:
    * All physics operations (`AddForce`, `AddTorque`) are correctly placed in `FixedUpdate()`.
    * Each child script implements its own `FixedUpdate()` to create a unique feel.
    * They check the `seated` boolean (from the base class).
    * They call the `VRControls()` or `DesktopControls()` functions (from the base class) to apply the player's steering.
    
* **Slider Logic (`BaseDroneController.Update`)**:
    * The `Update()` loop reads the `.value` from all sliders every frame.
    * Input-based sliders (pitch, roll, etc.) are used as multipliers.
    * Physics-based sliders (mass, angular drag) are applied directly to the `Rigidbody`.
