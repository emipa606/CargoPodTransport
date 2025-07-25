# .github/copilot-instructions.md

## Mod Overview and Purpose

The RimWorld Helicopter Mod introduces an advanced transportation system to the game, allowing players to build and utilize helicopters for transporting goods and colonists across the map. The primary purpose of the mod is to enhance the gameplay experience by providing a faster and more versatile means of transportation compared to traditional caravans.

## Key Features and Systems

- **Helicopter Deployments**: Deploy helicopters to transport colonists, goods, and even animals quickly across long distances.
- **Drop Pod Functionality**: Utilize drop pods for precise landings and rapid deployment in tactical scenarios.
- **Fuel Management**: Manage the refueling of helicopters to ensure operational readiness and efficiency.
- **User Interface Enhancements**: New gizmos and UI interactions for selecting helicopter destinations and managing cargo.
- **Dynamic Landing and Takeoff**: Control where helicopters land and take off, providing strategic advantage in the placement and quick deployment of resources.

## Coding Patterns and Conventions

- **Static Classes for Utilities**: Utilitarian functions related to helicopters are organized in static classes such as `HelicoptersArrivalActionUtility`.
- **Inheritance from Core Classes**: Use of inheritance to extend RimWorld core functionality, e.g., `TravelingHelicopters` inherits from game classes to utilize core game logic.
- **Encapsulation**: Private methods are used for internal logic processing, with public methods exposing necessary interfaces, as seen in `CompLaunchableHelicopter`.

## XML Integration

The mod uses XML for defining mod-specific configurations and integrating with RimWorld's data-driven systems. Although the XML specifics are not provided in the summary, they typically involve:

- **Helicopter Definitions**: XML files contain definitions for helicopter types, cargo capacity, fuel requirements, and other attributes.
- **Integration with RimWorld Objects**: Link XML definitions to C# classes and methods to ensure that newly introduced entities interact seamlessly within RimWorld's ecosystem.

## Harmony Patching

- **Purpose of Harmony Patching**: Modify existing RimWorld code safely without altering the base game files.
- **Method Patching**: Use Harmony to intercept and modify methods, allowing additional functionality like custom drop pod actions or helicopter-specific UI elements.
- **Static Class Implementation**: Patches are often organized in static classes for better organization and performance.

## Suggestions for Copilot

When writing or extending code for this mod, consider the following suggestions for better integration and functionality:

1. **Conform to Existing Code Patterns**: Use static classes for utility methods and ensure appropriate use of public and private access modifiers.
2. **Adopt Consistent Naming Conventions**: Follow the established naming conventions seen in existing classes such as `CompLaunchableHelicopter` and `Gizmo_MapRefuelableFuelStatus`.
3. **XML Conformance**: Ensure that XML entries match exactly with their corresponding C# class properties and expected data structures.
4. **Consider Modularity**: Keep new features modular to allow for easier expansions or modifications in the future.
5. **Utilize Harmony Thoughtfully**: Harmony patches should enhance or fix specific issues without impacting overall game balance or performance. Always ensure patches are reversible and documented.

By following these guidelines and utilizing the key features and systems of the RimWorld Helicopter Mod, developers and contributors can create a polished and seamless gameplay experience.
