# DEBUG

- 0.5d -> Recenter camera doesn't work (function is called but does nothing)
- 1d -> Pathfinding is strange, not the more direct as possible

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

## Unit UI
- Clean the prefab UI Infos
- Orient to camera is implemented twice on the prefab

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

- Separate Move in multiple parts (moving, orient to, move tiles, etc.), fully encapsuled, with a manager
- Separate Action Move from Behavior Move
- Don't need a LateStart in M_Units

- Actions systems: possibility between multiple designs
  - possibility to have a maximum of the same action by turn (ex: 2 movements allowed, but one attack)
  - action points
  - play again on condition
- Can shoot from a step (lean)

- Enemy behavior
  - Use actions system

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
- Ammo system -> import for XDD project
- UFM Damage preview
- Critical hit chance
- Projectiles

# NOTES

## Actions behaviors
- An action is defined by **do something**
- To know if you can do an action, you have to **fulfill the conditions**
  - an action class needs conditions (cost, triggers, anything)
  - an action manager haves to manage the costs
- I see **different implementations**
  - 1st
    - the manager is a compilator of the actions, but do not in particular : it asks the Action if its conditions are fulfilled
    - the Action manages its data (costs), its conditions and its execution script (do something)
  - 2nd
    - the manager is a resources manager, it says to the action if it can play or not
    - the action is only an interface with its execution script (do something)
  - in the two cases, execution script can be separated from this logic
