using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class TestDB : MonoBehaviour
{
    private static string WorldDBPath; 
    // Start is called before the first frame update
    void Start()
    {
        WorldDBPath = Application.streamingAssetsPath + "/World/World_Data.db";
        SqlDbCommand worldDB = new SqlDbCommand(WorldDBPath);
        Debug.Log(worldDB.IsTableCreate<sceneData>("worldlist"));
        //worldDB.DeleteTable("worldlist");
        //worldDB.CreateTable<sceneData>("worldlist");
        //sceneData[] New = new sceneData []
        //{        
        //    new sceneData{Id=0,Name="tast",Type=0,Seed=0,IsActive=true,SceneWidth=4},
        //    new sceneData{Id=1,Name="mainMa1",Type=2,Seed=0,IsActive=true,SceneWidth=8}
        //};

        //worldDB.Insert("worldlist",sceneData.BasicScenes.ToList());
        //worldDB.Update("worldlist", New.ToList());
        //worldDB.DeleteBySql("worldlist","Name='test'");
        //List<sceneData> newS = new List<sceneData>();
        //newS = worldDB.SelectBySql<sceneData>("worldlist","Id = 1");
        //foreach (sceneData s in newS)
        //{
        //    Debug.Log(s.Name);
        //}
        




        //worldDB.CreateTable("worldlist", new string[] { "ID", "Name", "Type", "Seed", "IsActive", "SceneWidth" }, new string[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT", "INTEGER", "INTEGER", "INTEGER", "INTEGER" });
    }

    // Update is called once per frame


}
