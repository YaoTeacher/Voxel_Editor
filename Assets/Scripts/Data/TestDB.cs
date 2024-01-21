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
    void Update()
    {
        
    }

    //public static WorldData LoadScene(string ScenceName, int seed = 0)
    //{

    //    // Get the path to our world saves.
    //    string loadPath = WorldDBPath;

    //    // Check if a save exists for the name we were passed.
    //    if (File.Exists(loadPath))
    //    {

    //        Debug.Log("World found. Loading from save.");
    //        Debug.Log(loadPath + ScenceName);


    //        // And then return the world.
    //        WorldData world =;

    //        return new WorldData(world);

    //        // Else, if it doesn't exist, we need to create it and save it.
    //    }
    //    else
    //    {

    //        Debug.Log("World not found. Creating new world.");

    //        WorldData world = new WorldData(ScenceName, seed);
    //        SaveScene(world);

    //        return world;

    //    }


    //}
    public static void SaveScene(WorldData world)
    {

        // Set our save location and make sure we have a saves folder ready to go.
        string savePath = WorldDBPath;
        // If not, create it.
        //if (!Directory.Exists(savePath))
        //{
        //    CreateWorldDB(WorldDBPath);

        //};

        Debug.Log("Saving " + world.worldName);



        Thread thread = new Thread(() => SaveChunks(world));
        thread.Start();

    }

    public static void SaveChunks(WorldData world)
    {

        // Copy modified chunks into a new list and clear the old one to prevent
        // chunks being added to list while it is saving.
        List<ChunkData> chunks = new List<ChunkData>(world.modifiedChunks);
        world.modifiedChunks.Clear();

        // Loop through each chunk and save it.
        int count = 0;
        foreach (ChunkData chunk in chunks)
        {

            SaveSystem.SaveChunk(chunk, world.worldName);
            count++;

        }

        Debug.Log(count + " Chunks saved.");

    }

    //public static void CreateWorldDB(string WorldDBPath)
    //{
        
    //    Debug.Log(WorldDBPath);
    //    worldDB = new SQLiteHelper("data source=" + WorldDBPath);
    //}

}
