# Cozy Home

A Unity project for the Cozy Home experience, including splash flow, environment systems, UI, and interaction logic.

## Project overview

This repository contains the Unity project files for a small interactive home scene with:

- splash screen flow
- environment and weather control
- audio integration
- UI state management
- interaction and spawn systems

## Unity setup

Open the project in Unity and use the project settings already included in the repository.

Recommended workflow:

1. Open the Unity project root folder.
2. Load the main scene used for development.
3. Make changes in the Assets/_Project folder.
4. Test in the editor before committing project updates.

## Key folders

- `Assets/_Project` — project-specific content and scripts
- `Assets/Scenes` — Unity scenes
- `ProjectSettings` — project configuration
- `Packages` — Unity package configuration

## Local git note

This repository is configured for local Git usage only. GitHub remote has been removed, so commits stay local and are not pushed anywhere.

## Development notes

- Keep Unity-generated artifacts such as `Library`, `Temp`, `Logs`, and `UserSettings` out of version control through the project `.gitignore`.
- Prefer editing project logic under the custom `_Project` structure.
- Use meaningful commit messages when saving milestones.
