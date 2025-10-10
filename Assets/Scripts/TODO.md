# DEBUG

## Camera
- recenter camera doesn't work (function is called but does nothing)

# REFACTOR

## M_Rules
- Fog of war vision rules not in M_Rules -> fog of war as a Manager ?
    - Fog of war view lines as Module

## Tiles
- Tiles material work with an enum, use a Scriptable object instead
- Tile class does a lot of things: need to fraction it (in Coordinates and a new class like Tile renderer)

## Team
- Make a distinction between Ally and Teammate

## Animation
- Crouch dodge
- Crouch hit reaction

## Others
- Can add and remove Actions without bugs
- Maybe use raycasts for Autosnap
- Try to kill a character during its own turn
    - OnDeath, need to end its turn
- Create a scriptable object template in Rider
- Auto change team materials as a module
- Cover as an action (removable)
- Description of the classes
- Class name nomenclature
  - Manager
  - Holder
  - UFM/FM
- units.EndCurrentUnitTurn as an event (not called in classes)
- set FM_TilesActionPreview abstract
- be sure, if Fog of war is disabled, unit can see everything

# CONTENT

- Weapons
  - bow
- Magical spells
  - fireball
- Drone
- Different turn based system 
  - initiative

# OPTIMISATION

- Look.VisibleUnits/VisibleEnemies calculated once
- Algorithm of line of sight checking progressively the tiles around

# FEATURES

- Actions systems: possibility between multiple designs
  - possibility to have a maximum of the same action by turn (ex: 2 movements allowed, but one attack)
  - action points
  - play again on condition
- Can shoot from a step (lean)

- Transform the tool in package
- Obstacle transparency
- Team alliances (difference between Allie and Teammate)
- Coordinates of the covers / logic of tiles hold covers -> create a new grid for elements on edges / vertex
- Character can throw spells (like fireball)
- Separate folders between Toy box and Technical box
- Namespaces
- Ergonomic of characters (custom editor)
- Online API like Unity (ask Simon)
- Add events (FX, sound, etc.) for everything
- Multiple victory conditions
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
- Armor and shield
- Notes on elements
- Animator of the characters more usable
- Ammo system
- UFM Damage preview
- Critical hit chance
