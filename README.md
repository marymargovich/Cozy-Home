# Cozy Home

Cozy Home is a Unity project built as a small interactive cozy-room experience with environmental atmosphere, click-based interaction, sound-driven gameplay loops, and a localized secret hint system.

## Overview

The project focuses on a warm indoor scene where the player interacts with room objects to trigger ambience, animations, lighting changes, and round-based sound activity. The interaction logic is centered around a unified `RoomItem` system, while the environment responds dynamically to the current active state of objects.

## Core gameplay loop

- interactive props and furniture items use `RoomItem`
- active objects can play sounds, animate, and influence the room state
- `MusicPuzzleManager` picks the current round's sound-active items
- lighting and decorative environment elements react to state changes in real time
- floating note feedback confirms successful valid interactions
- a secret hint appears after a defined number of valid interactions

## Main systems

### Room interaction

`RoomItem` is the primary interactive component used throughout the project.

Features include:

- click handling through `IPointerClickHandler`
- animation support with sprite/animator state switching
- configurable sound variation arrays
- optional guaranteed active sound behavior
- safe audio playback checks to avoid invalid or missing source calls
- lamp and room-state synchronization with the environment controller
- note spawning tied to valid active items

### Round logic and sound gating

`MusicPuzzleManager` controls which items are currently active in the round.

Rules implemented in the project:

- guaranteed items remain active every round
- non-guaranteed items are randomized into the active subset
- inactive items do not produce sound or floating note feedback
- the system prevents invalid audio calls when items are missing a source component

### Environment feedback

The room responds visually to item interaction:

- floor lamp state updates through `TimeOfDayController`
- ceiling lamp state updates when relevant items are toggled
- garland states update as objects are activated or deactivated
- the lighting scheme changes immediately to match the current room state

### Secret hint system

`SecretHintController` adds a contextual hidden clue after repeated valid interactions.

Included functionality:

- interaction counter before the hint appears
- localized sprites for English, Russian, and Hebrew
- smooth fade-in / fade-out hint animation
- language-aware sprite switching through the UI language manager

## Project structure

- `Assets/_Project` — main gameplay scripts, art, scenes, and prefabs
- `Assets/Scenes` — Unity scene files
- `ProjectSettings` — Unity editor settings
- `Packages` — Unity package configuration
- `README.md` — project summary and setup notes

## Current status

The project is in the final stable milestone for this build:

- the `RoomItem` interaction model is the active architecture
- the round-based sound logic is integrated and working
- environment lighting and animation state follow object state changes
- secret hint flow is implemented and language-aware
- the repository is committed locally on the `main` branch

## Local Git note

This project is configured for local Git usage only. There is no remote Git repository configured at the moment, so all progress is kept in the local repository history.

## Unity usage

1. Open the project in Unity.
2. Load the main scene used for the Cozy Home experience.
3. Test interaction flow in Play Mode.
4. Keep all gameplay logic within the `_Project` folder structure.

## Notes

- the project uses a unified interaction model rather than multiple fragmented prop systems
- the codebase is organized around a clear environment + interaction + UI flow
- documentation is maintained in English for project consistency
