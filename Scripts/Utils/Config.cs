using UnityEngine;

public class Config
{
    public static string Units = "ScriptableObjects/Units/";
    public static string Events = "ScriptableObjects/Events/";
    public static string Actions = "ScriptableObjects/Actions/";
    public static string Passives = "ScriptableObjects/Passives/";

    public static Unit GetUnit(string name)
    {
        return (Unit) Resources.Load(Units + name);
    }

    public static Action GetAction(string name)
    {
        return (Action) Resources.Load(Actions + name);
    }

    public static Passive GetPassive(string name)
    {
        return (Passive)Resources.Load(Passives + name);
    }

    public static Event GetEvent(string name)
    {
        return (Event) Resources.Load(Events + name);
    }
}
