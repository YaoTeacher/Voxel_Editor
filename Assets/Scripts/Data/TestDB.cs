using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDB : MonoBehaviour
{
    private static SQLiteHelper worldDB;
    private static string WorldDBPath; 
    // Start is called before the first frame update
    void Start()
    {
        WorldDBPath = Application.streamingAssetsPath + "/World/";
        Debug.Log(WorldDBPath);
        worldDB = new SQLiteHelper("data source=" + WorldDBPath + "World_Data.db");

        worldDB.CreateTable("worldlist", new string[] { "ID", "Name", "Type", "Seed", "IsActive", "SceneWidth" }, new string[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT", "INTEGER", "INTEGER", "INTEGER", "INTEGER" });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
