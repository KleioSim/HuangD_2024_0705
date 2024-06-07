using Godot;

namespace HuangD.Godot.Utilties;

class LOG
{
    public static void INFO(string log)
    {
        GD.Print($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [INFO] {log}");
    }
}
