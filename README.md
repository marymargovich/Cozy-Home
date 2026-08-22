# Cozy Home

A Unity project for the Cozy Home experience, focused on an interactive home scene, warm visual atmosphere, and UI-driven player interactions.

## Project overview

This repository contains the Unity project files for a small interactive room scene with:

- canvas-based and world-based interaction logic
- environment lighting, time-of-day, and seasonal styling
- animated props and toggled game states
- UI feedback effects such as floating notes
- local project versioning without a remote GitHub connection

## Current implemented systems

- mouse and touch-friendly interaction flow using the Unity Input System
- prop click and toggle logic for interactive room objects
- floating note visual effects for UI feedback
- environment tinting and seasonal visual changes
- animation-driven interaction states for layered props and controls

## Floating note effect system

The project includes a lightweight floating note effect for UI feedback.

- `EffectManager` handles note spawning from a single shared instance.
- `FloatingNoteEffect` controls note lifetime and self-destruction.
- spawned notes are placed under the canvas container and positioned at the pointer or world-space interaction point.
- the effect supports both mouse clicks and touch/tap input through the newer Pointer input API.

## Unity setup

1. Open the project root in Unity.
2. Load the main development scene.
3. Edit project content under `Assets/_Project`.
4. Test the scene in the editor before saving milestones.

## Key folders

- `Assets/_Project` — project-specific assets, scripts, scenes, and prefabs
- `Assets/Scenes` — Unity scene files
- `ProjectSettings` — editor and project configuration
- `Packages` — Unity package configuration

## Input System notes

The project uses the Unity New Input System instead of the legacy `Input.mousePosition` API.

- pointer position is read from `Pointer.current.position.ReadValue()` when available
- UI event data is used when available from `PointerEventData.position`
- world-to-screen conversion is used as a safe fallback for object interactions
- the code avoids runtime exceptions when the pointer or mouse is unavailable

## Local Git note

This repository is configured for local Git usage only. The remote repository is not connected, so all commits remain local and are not pushed to any external service.

## Development notes

- keep Unity-generated artifacts such as `Library`, `Temp`, `Logs`, and `UserSettings` out of version control where possible
- prefer editing gameplay and UI logic inside the custom `_Project` structure
- use meaningful commit messages when completing milestones or feature updates
- keep comments and documentation in English for consistency with the project codebase

## Interaction example

UI and prop interactions are handled through component-level click logic and effect spawning. A floating note can be triggered from pointer input, screen-space coordinates, or a world-space object position to give immediate visual feedback for user actions.
