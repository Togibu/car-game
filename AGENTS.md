# AGENTS.md

## Project Context
- **Project**: Unity 3D car game prototype in a low‑poly city/nature environment.
- **Primary Scene**: `Assets/Scenes/SampleScene.unity` (appears to be main scene).
- **Core Gameplay**: Player drives a car with physics-based movement, can reset position, view speed, and control lights. Camera supports orbiting and first‑person view.
- **Assets**: Large collection of vehicle and city/nature models (POLYGON city pack and other low‑poly assets).

## Key Scripts (Gameplay Systems)
- `Assets/movement.cs`  
  Physics-based car control using `Rigidbody` (acceleration, braking, turn smoothing, speed cap, reverse cap, drift damping). Displays speed via TextMeshPro. Reset with `R`.
- `Assets/cam_movement.cs`  
  Orbit camera with mouse rotation + zoom. Switches to first‑person when zoomed in; auto‑rotates after inactivity.
- `Assets/tire.cs`  
  Visual steering for front tires based on `A/D` input.
- `Assets/policelight.cs`  
  Toggle police blink lights with `F`; cycle headlight modes with `E` (off/low/high).
- `Assets/sun.cs`  
  Rotating sun (Directional Light) for day/night cycle.
- `Assets/StraßenLampen.cs`  
  Streetlights turn on/off based on sun angle.
- `Assets/LightCollidet.cs`  
  Street lamp becomes non‑kinematic when hit by the player.
- `Assets/driving.cs`  
  Simple AI path following between waypoint targets (not confirmed in scene).

## Recent Fixes
- Removed duplicate `movement` class file that caused CS0101/CS0111.
- Updated `movement.cs` to use `Rigidbody.linearDamping`, `angularDamping`, and `linearVelocity` to resolve obsolete API warnings.

## Known Notes / Assumptions
- Code uses legacy Input Manager (`Input.GetAxis`, `Input.GetKey`).
- Many assets are included; no explicit gameplay loop beyond free driving is visible in code.

## Next Steps (Requested)
Discuss desired features, current problems, and prioritize tasks to work on.
