public static class M__Managers
{
    public static M_Feedbacks _feedbacks => M_Feedbacks.instance;
    public static M_PlayerInputs _inputs => M_PlayerInputs.instance;
    public static M_Pathfinding _pathfinding => M_Pathfinding.instance;
    public static M_GameRules _rules => M_GameRules.instance;
    public static M_Terrain _terrain => M_Terrain.instance;
    public static M_Characters _characters => M_Characters.instance;
    public static M_UI _ui => M_UI.instance;
    public static GameCamera _camera => GameCamera.instance;
}