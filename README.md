# Cozy Home

A Unity project for the Cozy Home experience, focused on a warm interactive home scene, UI-driven animations, and environment behavior.

## Project overview

This repository contains the Unity project files for a small interactive room scene with:

- UI and canvas-based interaction logic
- environment lighting and time-of-day transitions
- animation-driven props and state changes
- room interaction and behavior scripts
- local project versioning without a remote GitHub connection

## Current implemented systems

- Mouse runner animation for UI scene interaction
- Seasonal and time-of-day room tinting
- UI state management and interface flow
- interaction logic under the custom project structure

## Unity setup

1. Open the project root in Unity.
2. Load the main development scene.
3. Edit project content under Assets/_Project.
4. Test the scene in the editor before saving milestones.

## Key folders

- Assets/_Project — project-specific assets, scripts, and scene content
- Assets/Scenes — Unity scenes
- ProjectSettings — project configuration and editor settings
- Packages — Unity package configuration

## Local Git note

This repository is configured for local Git usage only. The remote repository is not connected, so all commits remain local and are not pushed to any external service.

## Development notes

- Keep Unity-generated artifacts such as Library, Temp, Logs, and UserSettings out of version control where possible.
- Prefer editing gameplay and UI logic inside the custom _Project structure.
- Use meaningful commit messages when completing milestones or feature updates.

## Interaction example

A UI interaction component named MouseRunner has been added to move a mouse object from a start point behind the fireplace to an end point under the table using a smooth coroutine-based motion.
