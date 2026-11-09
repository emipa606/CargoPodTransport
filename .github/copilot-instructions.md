# GitHub Copilot Instructions for Cargo Pod Transport (Continued) Mod

## Mod Overview and Purpose

Cargo Pod Transport (Continued) is a RimWorld mod that adds a new method of transporting items, pawns, and resources through the game world. This mod is a continuation of the original Cargo Pod Transport mod created by AKreedz, and has been updated and expanded by JellyPowered.

The primary purpose of this mod is to provide players with a more flexible and efficient method of transporting goods and pawns across the world map. Unlike traditional transport pods, the Cargo Pod Transport can carry a larger payload and can land anywhere on the map without disappearing.

## Key Features and Systems

### Key Features

- **Increased Capacity**: The Cargo Pod Transport has a larger carrying capacity than traditional transport pods.
- **Airborne Transport**: The mod allows for airborne transportation, allowing players to move items and pawns quickly and efficiently across the map.
- **Edge Drop Compatibility**: The mod includes a feature that moves the drop pod to a suitable location if it is dropped on the edge of the map.
- **Compatibility**: The mod is compatible with other popular RimWorld mods such as RimSort and Rimefeller.

### Key Systems

- **Helicopter System**: The core system of the mod, which manages the creation, movement, and arrival of Cargo Pod Transports.
- **ActiveDropPod_PodOpen**: A class that handles the opening and closing of Cargo Pod Transports.
- **HelicopterLeaving**: A class that manages the departure of Cargo Pod Transports from the map.
- **HelicoptersArrivalActionUtility**: A utility class that provides various actions that can be taken when a Cargo Pod Transport arrives at its destination.
- **HelicopterStatic**: A static class that provides utility functions for the helicopter system.
- **Dialog_Trade_SetupPlayerCaravanVariables**: A class that sets up the player caravan variables for trade operations involving Cargo Pod Transports.

## Coding Patterns and Conventions

### General Conventions

- **Class Naming**: Classes are named using PascalCase, with the first letter of each word capitalized.
- **Method Naming**: Methods are also named using PascalCase.
- **Variable Naming**: Variables are named using camelCase, with the first letter of the first word lowercase and subsequent words capitalized.
- **Constants**: Constants are named using all uppercase letters, with underscores separating words.

### XML Conventions

- **Def Naming**: Defs are named using PascalCase, with the first letter of each word capitalized.
- **Def File Structure**: Def files are organized into folders based on the type of def they contain (e.g., ResearchProjectDefs, ThingDefs_Buildings).

### Harmony Patching

- **Patch Naming**: Harmony patches are named using the format `MethodName_Prefix`, `MethodName_Postfix`, or `MethodName_Transpiler`, depending on the type of patch.
- **Patch Location**: Patches are located within the appropriate classes and methods in the mod's source code.

## XML Integration

XML files play a crucial role in defining the various aspects of the mod, such as research projects, thing definitions, and more. The mod uses XML files to define the following:

- **ResearchProjectDefs**: Defines the research projects that players can undertake to unlock new technologies and abilities.
- **ResearchTabDefs**: Defines the tabs that organize the research projects into categories.
- **ThingDefs_Buildings**: Defines the building structures that players can construct.
- **ThingDefs_Misc**: Defines various miscellaneous items and entities within the game world.
- **Things_Special**: Defines special items and entities that have unique properties.

## Harmony Patching

Harmony is a popular modding framework for RimWorld that allows modders to modify the behavior of the game's code without directly editing the game files. The Cargo Pod Transport mod uses Harmony patches to modify the following aspects of the game:

- **ActiveDropPod_PodOpen**: Patches the `PodOpen` method to handle the opening and closing of Cargo Pod Transports.
- **HelicopterLeaving**: Patches the `LeaveMap` method to manage the departure of Cargo Pod Transports from the map.
- **HelicoptersArrivalActionUtility**: Patches various methods to provide additional actions when a Cargo Pod Transport arrives at its destination.
- **HelicopterStatic**: Patches various utility functions to support the helicopter system.
- **Dialog_Trade_SetupPlayerCaravanVariables**: Patches the `Postfix` method to set up the player caravan variables for trade operations involving Cargo Pod Transports.

## Suggestions for Copilot

### General Suggestions

- **Consistent Naming Conventions**: Use consistent naming conventions for classes, methods, variables, and constants to improve code readability and maintainability.
- **Documentation**: Document code as needed to explain complex logic or important functionality.
- **Error Handling**: Implement error handling to ensure the mod runs smoothly and provides meaningful error messages when issues arise.

### XML Integration Suggestions

- **Modular XML Files**: Organize XML files into modular and reusable components to make it easier to maintain and update the mod.
- **Version Control**: Use version control systems (e.g., Git) to track changes to XML files and ensure compatibility with different versions of the game.

### Harmony Patching Suggestions

- **Test Patches**: Thoroughly test Harmony patches to ensure they work as intended and do not interfere with other mods or the game's core functionality.
- **Documentation**: Document Harmony patches to explain their purpose and the methods they modify, which can be helpful for other modders.

By following these guidelines, you can effectively use GitHub Copilot to enhance the development of the Cargo Pod Transport (Continued) mod, ensuring that the mod remains flexible, efficient, and compatible with the latest version of RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

