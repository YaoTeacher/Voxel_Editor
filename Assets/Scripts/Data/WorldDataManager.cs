using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public class WorldDataManager 
{
    private static  SQLiteHelper worldDB;
    private static string WorldDBPath = Application.persistentDataPath + "/World/";
    public static void GenerateWorldDataDB()
    {
        worldDB = new SQLiteHelper("data source="+WorldDBPath+"World_Data.db");
        worldDB.CreateTable("worldlist", new string[] { "ID", "Name", "Type","Seed", "IsActive", "SceneWidth" }, new string[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT", "INTEGER", "INTEGER", "INTEGER", "INTEGER" });
    }
}

