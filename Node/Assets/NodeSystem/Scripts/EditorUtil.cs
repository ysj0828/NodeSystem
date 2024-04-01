using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorUtil
{
    public static void LogListV2(List<Vector2> list)
    {
        string s = "";
        foreach (object obj in list)
        {
            s += obj.ToString();
            s += "\n";
        }

        Debug.Log(s);
    }
}
