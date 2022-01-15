public static class M__Managers
{
    public static M_Feedback _feedbacks => M_Feedback.instance;
    public static M_PlayerInput _inputs => M_PlayerInput.instance;
    public static M_Pathfinding _pathfinding => M_Pathfinding.instance;
    public static M_GameRules _rules => M_GameRules.instance;
    public static M_Terrain _terrain => M_Terrain.instance;
    public static M_Characters _characters => M_Characters.instance;
    public static M_UI _ui => M_UI.instance;
    public static GameCamera _camera => GameCamera.instance;
}