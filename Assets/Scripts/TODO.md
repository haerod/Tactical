# DEBUG

## Camera
- recenter camera doesn't work (function is called but does nothing)

## Restart scene
- bug on F_Covers, easy repro

## Behavior
- If NPC target character out of view, it waits indefinively

# REFACTOR

## M_Rules
- Fog of war vision rules not in M_Rules -> fog of war as a Manager ?
    - Fog of war view lines as Module

## Actions
- Replace EndTurn by EndAction (if an unit can do multiple actions)

## Melee weapons
- Melee weapons don't have to care about covers
- When unit hover a enemy and itself is covered, stand up animation

## Unit UI
- Make any unit UI independent of the main script

## Tiles
- Tiles material work with an enum, use a Scriptable object instead
- Tile class does a lot of things: need to fraction it (in Coordinates and a new class like Tile renderer)

## Others
- Rename F_... and UI_... modules in Module_... (code and prefab)
- Rename Character in Unit
- Can add and remove Actions without bugs
- Maybe use raycasts for Autosnap
- Try to kill a character during its own turn
    - OnDeath, need to end its turn
- Create a class/scriptable object template in Rider
- Input.SetActivePlayInput in events and not in the code

# CONTENT

- Magical spells
- Drone
- Different turn based system (ex: initiative)

# OPTIMISATION

- Create a variable "actionHasBeenDone" to calculate only once by turn actions with a big cost (visible enemies, etc.)
- Algorithm of line of sight checking progressively the tiles around

# FEATURES

- Display weapons / characters infos on UI

- Team alliances (difference between Allie and Teammate)
- Coordinates of the covers / logic of tiles hold covers -> create a new grid for elements on edges / vertex
- Character can throw spells (like fireball)
- Separate folders between Toy box and Technical box
- Namespaces
- Ergonomic of characters (custom editor)
- Actions systems: possibility between multiple designs
- Online API like Unity (ask Simon)
- Add events (FX, sound, etc.) for everything
- Multiple victory conditions
- Obstacle transparency
- Destructible obstacles
- Better enemy behaviors
- Large elements (multiple tiles)
- Missing shots can touch another tile
- Border cover can block the line of sight
- Overwatch action
- In Tactical Course project, export all the comments //BUTT to this project
- Camera rotation
- Camera actions
- Enemy spawners
- Vault over little covers
- Movements with special costs or obligation to stop
- 3D grid
- Tiles of different types
- Can shoot from a step (lean)
- Armor and shield
- Notes on elements
- Ragdoll on death
- Animator of the characters more usable