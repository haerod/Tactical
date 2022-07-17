public static class M__Managers
{
    // For any new singleton, add a new
    // public static SingletonClass nameOfTheVariable => SingetonClass.nameOfTheInstance
    // Now, you can simply use nameOfTheVariable to call the singleton

    public static M_Feedback _feedback => M_Feedback.instance;
    public static M_Input _input => M_Input.instance;
    public static M_Pathfinding _pathfinding => M_Pathfinding.instance;
    public static M_Rules _rules => M_Rules.instance;
    public static M_Board _board => M_Board.instance;
    public static M_Characters _characters => M_Characters.instance;
    public static M_UI _ui => M_UI.instance;
    public static M_Camera _camera => M_Camera.instance;
    public static M_Creator _creator => M_Creator.instance;
    public static M_Turns _turns => M_Turns.instance;
}