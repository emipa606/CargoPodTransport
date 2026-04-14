# GitHub Copilot Instructions for Cargo Pod Transport (Continued) Mod

## Mod Overview and Purpose
The Cargo Pod Transport (Continued) mod, originally known as the Helicopter MOD, enhances RimWorld by adding a new transportation system called the Cargo Pod Transport. This system offers a more robust and versatile alternative to the existing transport pods. The mod was originally created by AKreedzs, with updates by Jellypowered, and is now being continued and maintained for newer versions of RimWorld.

## Key Features and Systems
- **Enhanced Transport Capacity**: The Cargo Pod Transport comes with a larger capacity, allowing players to carry more items and pawns across distances.
- **World Map Accessibility**: Unlike traditional transport pods, Cargo Pod Transports can fly anywhere on the world map without disappearing upon landing.
- **Edge Drop Compatibility**: The mod includes functionality to move the drop pod to a suitable location if it is dropped at the edge of the map, thanks to contributions from SmashPhil and Neceros from SRTS Expanded.
- **Integrations**: Supports both RimWorld version 1.2 and the Rimefeller mod.
- **Research and Build**: Players can build the Cargo Pod Transport once they unlock the transport pod technology, found in the Misc category.
- **Additional Features**: 
  - Ability to carry downed pawns into cargo pods.
  - Static landing sites to prevent unexpected pod relocations post-landing.
  
## Coding Patterns and Conventions
- **Namespace Usage**: All classes are organized within relevant namespaces (though not explicitly mentioned in the provided data, organization should follow best practices).
- **Static Class and Methods**: Used extensively in utility and singleton-like tasks (e.g., `ActiveDropPod_PodOpen`, `HelicoptersArrivalActionUtility`, and `HelicopterStatic`).
- **Inheritance and Interfaces**: Implementation of interfaces like `IActiveTransporter` in `HelicopterLeaving` and custom component classes derived from `ThingComp` for modular additions (e.g., `CompLaunchableHelicopter`).
- **Code Documentation**: Ensure all code is well-documented, following XML documentation standards for methods and classes.

## XML Integration
- RimWorld modding extensively uses XML for defining objects, items, and attributes. Ensure to define all new entities like the Cargo Pod Transport and integrate well with existing XML definitions.
- Use XML patching to modify existing game definitions where necessary, ensuring compatibility and minimizing conflicts.

## Harmony Patching
- Utilize the Harmony library for runtime modifications of base game behavior, particularly for integrating with game updates and other mods without direct alteration of core files.
- Ensure all patches are appropriately encapsulated and targeted to avoid unintended game behavior changes.
- Recommended Harmony patterns include postfix and prefix for methods that need to be extended or intercepted.

## Suggestions for Copilot
- **Code Suggestions**: Set up Copilot to suggest completion for complex method structures, particularly those involving multiple parameters such as `TryLaunch` in `CompLaunchableHelicopter`.
- **XML Assistance**: Use Copilot to expedite writing XML definitions and patch operations based on existing patterns in the mod.
- **Harmony Patch Examples**: Configure Copilot to suggest example Harmony patches based on existing methods and game mechanics that may require modification.

## Additional Notes
- For support and troubleshooting, it is encouraged to use the mod's dedicated Discord channel. Always ensure to post logs using the Log Uploader tool.
- For mod sorting and compatibility checking, using tools like RimSort is recommended.

### Conclusion
By following this guidance, modifications and enhancements to the Cargo Pod Transport (Continued) mod can be efficiently developed and maintained. Leveraging the power of tools like GitHub Copilot will facilitate streamlined code development and integration with existing game systems.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
