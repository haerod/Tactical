# IMPORTANT
## FEATURE WITH 1h or less
### OTHERS

---

---

# PROJECT
**IMPORTANT 
- 1h -> Test to build (#if UNITY_EDITOR errors to prevent)

- 1d -> Transform the project in Package (usable with the Package Manager)
    - I checked and there are two options :
        - Create a package but everything goes inside (nothing in the project)
        - Push the project on the asset store (not ready yet)
    - 1d -> Dependencies with Text Mesh Pro (and Cinemachine when camera will be imported)
- 0.5d -> Namespaces
- 1d -> Documentation
- 1w -> Online API like Unity (ask Simon)
- 0.5d -> Notes in Inspector to make it easier to understand
- 1h -> In Tactical Course project, export all the comments //BUTT to this project

## CAMERA
- 1h -> Clamp camera in board bounds
- 1h -> Camera screen shake (Cinemachine based)
- 0.5d -> Smooth rotation

## FOG OF WAR
- 0.5d -> Fog of war as a Manager (not in Rules)
- 1h -> Fog of war view lines as Module (of Fog of war)
- 1h -> Test : Be sure, if Fog of war is disabled, unit can see everything
- 1w -> Fog of war behavior : mask things undiscovered (difference with discovered and not in line of sight)
- 1h -> Fog mask on FoW, not on tile

## ACTION TILE PREVIEW
- 1h -> Set FM_TilesActionPreview abstract
- 1h -> Zone with limits
- 1h -> Use pulling to not instantiate always area prefabs

## TEAM
- 1h -> Make a distinction between Ally and Teammate
- 0.5d -> Team alliances

## ANIMATION
- 1d -> Use animation layers for crouch (crouch dodge, crouch hit reaction)
- 1h -> Anim class work on events (easy features already done, check the other ones)

## UNIT UI
- 0.5d -> Clean the prefab UI Infos (maybe ask for help -> Henri)
- 1h -> Orient to camera is implemented twice on the prefab
- 0.5d -> Module Health bar : Damage preview

## NOMENCLATURE
- 1h -> Description of the classes
- 1h -> Class name nomenclature (Manager, Holder)

## UNITS MANAGER
- 1h -> units.EndCurrentUnitTurn as an event (not called in classes) -> use it in a lot of Modules
- 0.5d -> Don't need a LateStart in M_Units
- 0.5d -> Find tasks to make it more simple

Note
- We need a difference between TurnStart (give actions) and TakeControl (dispaly UI and controls)

**Note**
- A lot of complexity come from the turn based system. 
But turn based is a feature can be added separately.
"Current unit" is a complex logic, but "controlled unit" is maybe easier to understand.

## DEATH
- 1h -> Stability test : Try to kill a character during its own turn and out of its turn
    - OnDeath, need to end its turn

## PREFAB TACTICAL TOOL
- 1h -> Multiple presets of the tactical tool, depending on the rules

## RIDER
- 1/4h -> Create a scriptable object template in Rider

## LOOK UNIT
- 1h -> Optim : Look.VisibleUnits/VisibleEnemies calculated once
- 1d -> Line of sight algorithm check progressively tiles around

## COVER SYSTEM
- 1h -> Find the tasks and write it in TODO (this system is messy)
- 0.5d -> Cover as an action (removable)
- 1d -> Can shoot from a step (lean)

## ENEMY BEHAVIOR
- 0.5d -> Use action system for the turns
- 1h -> Bug : Turn of enemy ends at MovementStart

## AUTO CHANGE WEAPONS
- 1h -> Remove this script to OnValidate method on U_WeaponHolder

### RAGDOLL
- .5d -> Ragdoll is dirty and creates exceptions : fix it
  - Can be better if it gives to the current unit a ragdoll, not create a new one

### OBSTACLES
- 0.5d -> Obstacle transparency
- 1d -> Shoot across obstacles (maybe clean the cover system first)
- 1w -> Edges / vertex covers (working with line of sight)
- 1w -> Destructible obstacles
- 1w -> Large elements (multiple tiles)

### HEALTH BAR
- 0.5d -> Display "Undamaged, Barely injured, etc." to replace the health bar

### ACTIONS
- 1d -> Can add and remove Actions without bugs
- 0.5d -> Make an action toolbar
- 1w -> Actions systems: possibility between multiple designs
    - possibility to have a maximum of the same action by turn (ex: 2 movements allowed, but one attack)
    - action points
    - play again on condition

**NOTE**
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

### PATHFINDING SYSTEM
- 1d -> Bug : Pathfinding is strange, not the more direct as possible
- 1w -> Vault over little covers (pathfinding and animation)
- 1w -> Movements with special costs or obligation to stop

### TURN BASED SYSTEM
- 0.5d -> Initiative system (or priority)
  - 1d -> UI Module turn by turn initiative

### PROJECTILES
- 1d -> Projectile logic
  - 1h -> Bow weapon
  - 1h -> Fireball

### MODULAR CHARACTER LOGIC
- 1w -> Can make a drone unit (without animations)

### MOVE UNIT
- 0.5d -> Separate Move in multiple parts (moving, orient to, move tiles, etc.), fully encapsuled, with a manager
- 0.5d -> Movement in fog of war

### UNIT VISUALS
- 1d -> Make it easy to change model (humanoid) -> /!\ Ragdoll Module is linked

### ERGONOMIC
- 1w -> Ergonomic of units (custom editor)

### UNITY EVENTS
- 1d -> Add events for FX, sound, etc.

### RULES MANAGER
- 1d -> Multiple victory conditions

### ATTACK
- 1w -> Missing shots can touch another tile

### OVERWATCH
- 1w -> Overwatch action (when turn etc. are cleaned)

### SPAWNERS
- 1d -> Unit spawners

### 3D GRID
- 1w -> 3D grid

### ARMOR
- 1d -> Armor logic (damage reduction)
- 1d -> Shield logic (external HP)

### CRITICAL HIT
- 0.5d -> Critical hit chance (+ feedback)

### PROGRESSION SYSTEM
- 1w -> Units can easily progress (ex: unlock skills)

### ITEM RESISTANCE
- 0.5d -> items have a resistance, reducing on each use, and are destroyed in the end

### RIPOST
- 1w -> when you attack an unit, it riposts automatically
