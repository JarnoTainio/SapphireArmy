using System.IO;
using System;
using System.Diagnostics;
using UnityEngine;

public class SaveManager
{
    public static string filePath = "save";

    public static bool SaveExists(int index)
    {
        return File.Exists(filePath + index.ToString() + ".save");
    }

    public static bool Save(GameData gameData)
    {
        try 
        { 
            File.WriteAllText(filePath + gameData.saveIndex + ".save", gameData.data.ToString());
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return false;
        }
        return true;
    }

    public static bool Load(int index, GameData gameData)
    {
        try
        {
            string str = File.ReadAllText(filePath + index + ".save");
            gameData.data.Load(str);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return false;
        }
        return true;
    }

    public static Data Load(int index)
    {
        try
        {
            string str = File.ReadAllText(filePath + index + ".save");
            Data data = new Data();
            data.Load(str);
            return data;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return null;
        }
    }

    public static bool Delete(int index)
    {
        try 
        { 
            if (SaveExists(index))
            {
                File.Delete(filePath + index + ".save");
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return false;
        }
        return true;
    }
}
