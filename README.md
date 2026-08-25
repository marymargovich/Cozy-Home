# Cozy Home

A Unity project for the Cozy Home experience, centered on a warm interactive room scene, time-of-day ambience, animation-driven props, and round-based sound interaction.

## Project overview

This repository contains the Unity project files for a cozy indoor scene with:

- unified room-item interaction logic
- round-based audio activation for sound-capable objects
- animated props, toggles, and visual state transitions
- environmental lighting updates tied to lamps and garlands
- floating-note feedback for successful interactions
- local Git-based versioning without an external remote repository

## Current architecture

The interaction system has been refactored around a single unified component:

- `RoomItem` handles click interaction, animation state, audio playback, and note spawning
- `MusicPuzzleManager` manages round activation for sound-capable objects
- `TimeOfDayController` updates the room tint based on lamp and garland state

## Room item system

`RoomItem` is the active interaction component and replaces the older fragmented prop/toggle/slot system.

Features:

- `IPointerClickHandler`-based click handling
- automatic dependency attachment through Unity `RequireComponent`
- optional animation/toggle logic when `hasAnimation` is enabled
- per-item sound variation support through `AudioClip[]`
- audio playback only when the item is active for the current round
- floating-note effect spawning gated by round activity
- direct sync with `TimeOfDayController` lamp states on click toggle

## Round & sound logic

`MusicPuzzleManager` manages which objects are active for sound in the current round.

Rules:

- `isGuaranteedSound == true` items stay active for every round
- non-guaranteed sound items are shuffled and a limited active subset is selected
- inactive items remain silent and do not spawn floating notes
- active items can still animate visually even when the music round gate is not triggered for audio

## Visual environment integration

The room lighting system is tied to click-driven item state changes.

- floor lamp state is synced through `SetFloorLampState(bool)`
- ceiling lamp state is synced through `SetCeilingLampState(bool)`
- garland states are synced through `SetGarland1State(bool)` and `SetGarland2State(bool)`
- the tint refresh happens immediately on click so the visual environment responds right away

## Floating note effect system

The project includes a lightweight note-feedback effect.

- `EffectManager` handles note spawning from a shared instance
- note positions are derived from pointer or screen-space coordinates when available
- notes are only spawned when the clicked item is active for the current round

## Unity setup

1. Open the project root in Unity.
2. Load the main development scene.
3. Work from the custom `_Project` assets and scripts.
4. Validate interactions in editor play mode before final milestone commits.

## Key folders

- `Assets/_Project` — scripts, art, audio, scenes, and prefabs
- `Assets/Scenes` — Unity scene files
- `ProjectSettings` — Unity project configuration
- `Packages` — Unity package data

## Local Git note

This repository is configured for local Git usage only. There is no remote GitHub connection configured, so all changes are kept local to the machine and committed to the local `main` branch.

## Development notes

- prefer the unified `RoomItem` model for new interactive props
- keep legacy interaction scripts marked deprecated or remove them after prefab migration
- keep comments and project docs in English for consistency
- use concise, meaningful commit messages when stabilizing a feature milestone
