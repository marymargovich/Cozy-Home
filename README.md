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

The project is currently stabilized around the unified `RoomItem` interaction flow:

- `RoomItem` handles click-driven state changes, animation toggling, audio playback, and note spawning
- `MusicPuzzleManager` tracks the active objects for the current round and gates sound/note activation
- `TimeOfDayController` updates the room lighting and lamp tint states in response to object toggles
- `EffectManager` provides the floating note feedback for valid active interactions

## Room item system

`RoomItem` is the active interaction component and replaces the older fragmented prop/toggle/slot architecture.

Current features:

- `IPointerClickHandler` click handling for interactive props and toggles
- optional animation state support with sprite and animator swap logic
- support for per-item sound variations through `AudioClip[]`
- guarded audio execution so inactive or non-audio items do not trigger missing-source errors
- round-aware note spawning only for active sound items
- immediate lamp/environment sync on toggle changes

## Round & sound logic

`MusicPuzzleManager` manages which objects are active for sound in the current round.

Rules:

- `isGuaranteedSound == true` items remain active for every round
- regular sound items are randomized into the current active subset
- inactive items stay silent and do not spawn floating notes
- visual animation can still respond to interaction even when the music round gate blocks sound
- the system is designed to prevent invalid sound calls when an object has no `AudioSource`

## Visual environment integration

The room lighting system is tied directly to the item state transitions.

- floor lamp state is synced through `SetFloorLampState(bool)`
- ceiling lamp state is synced through `SetCeilingLampState(bool)`
- garland states are synced through `SetGarland1State(bool)` and `SetGarland2State(bool)`
- color/tint refresh happens immediately so the room environment responds in the same interaction cycle

## Floating note effect system

The project includes a lightweight note-feedback effect.

- `EffectManager` handles note spawning from a shared instance
- note positions are derived from pointer or screen-space coordinates when available
- notes are only created for items that are active in the current round

## Current status

The project is in a stable working state for the unified `RoomItem` flow:

- no script errors were reported for the active interaction and round manager scripts
- the round logic and audio gating are aligned with the current architecture
- the codebase is consistent with the RoomItem-driven interaction model

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
