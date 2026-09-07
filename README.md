# Cozy Home

Cozy Home is a Unity project built as a small interactive cozy-room experience with a warm indoor scene, touch-friendly interaction, environment-driven ambience, and localized UI feedback.

▶️ **[Запустить игру онлайн](https://marymargovich.github.io/Cozy-Home/)**

## Overview

This project focuses on a living-room style interactive space where the player taps or clicks objects to trigger visual changes, ambient effects, sound cues, and layered UI states. The final build is centered on a unified interaction model, a reactive weather system, and a status bar that reflects the actual current state of the room.

## Current gameplay loop

- interactive objects are driven by `RoomItem`
- valid interactions can animate the object and update the room state
- `MusicPuzzleManager` controls which sound-capable objects are active in the current round
- weather state affects the visual environment and the weather sound channel
- the status bar updates time-of-day, season, and current weather icons based on the active state
- the secret hint appears after several valid interactions and supports localization

## Main systems

### Room interaction

`RoomItem` is the primary interaction component used by the playable objects.

Included logic:

- pointer/tap click handling through `IPointerClickHandler`
- sprite and animator-based states for interactive props
- optional sound playback and clip selection
- round gating for valid sound interactions
- lamp and room-state synchronization with the environment controller
- floating note feedback for valid action results

### Weather system

The weather system is built around `WeatherController` and `WeatherAudioManager`.

Current behavior:

- `WeatherType.Clear` produces no background weather audio and leaves the room silent
- all active non-clear weather states trigger their matching weather ambience
- the controller updates the active weather state and notifies the UI layer
- weather changes are reflected in the status bar and tooltip text

### Status bar and UI state

`StatusBarController` updates the icon group for time of day, season, and weather.

Important fixes in the final version:

- sprite selection is explicit and enum-based instead of raw array indexing
- `WeatherType.Clear` icon is matched to the clear sprite instead of fog or another weather state
- weather icon and tooltip are synchronized with the active weather value rather than stale startup state
- old serialized array values are still read as a compatibility fallback when needed

### Secret hint system

`SecretHintController` adds localized contextual hints after repeated valid interactions.

Features include:

- localized English, Russian, and Hebrew sprites
- interaction counter threshold before showing the hint
- fade-in and fade-out animation
- dynamic sprite update when the language changes

## Project structure

- `Assets/_Project` — main gameplay logic, art, UI, and environment systems
- `Assets/Scenes` — Unity scene files
- `ProjectSettings` — Unity editor configuration
- `Packages` — Unity package definitions
- `README.md` — project summary and current status

## Current status

The project is in the final stable milestone for this version:

- touch/click interactivity is the active input model
- room logic is unified around `RoomItem`
- weather state and audio are synchronized to real active weather values
- status bar icons are explicit and no longer depend on fragile array index order
- clear weather remains silent without background weather ambience
- the project is ready to be used as a local, final build snapshot

## Unity usage

1. Open the project in Unity.
2. Load the main room scene.
3. Run the scene in Play Mode.
4. Validate interaction, weather changes, status icons, and UI text in editor play mode.

## Notes

- interaction logic is designed for mouse and touch input through Unity pointer events
- the weather audio system intentionally keeps clear weather silent
- the status bar now reflects the real weather state instead of a stale startup value
