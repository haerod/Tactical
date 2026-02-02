public static class M__Managers
{
    // For any new singleton, add a new
    // public static SingletonClass nameOfTheVariable => SingletonClass.nameOfTheInstance
    // Now, you can simply use nameOfTheVariable to call the singleton

    public static M_Input _input => M_Input.instance;
    public static M_Level _level => M_Level.instance;
    public static M_Board _board => M_Board.instance;
    public static M_Units _units => M_Units.instance;
    public static M_Camera _camera => M_Camera.instance;
}