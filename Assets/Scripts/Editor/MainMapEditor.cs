using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.PackageManager.UI;
using System;

public class MainMapEditor : EditorWindow
{

    public WorldData worldData;

    [MenuItem("EditorTool/MapEditor", priority = 0)]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MainMapEditor));
    }

    void OnGUI()
    {
        
        GUILayout.Button("��ȡ��ͼ");
        GUILayout.Button("�����ͼ");
        
    }

    void LoadMap()
    {
        string loadPath = World.Instance.appPath + "/saves/" + worldData.worldName + "/";
    }


}
