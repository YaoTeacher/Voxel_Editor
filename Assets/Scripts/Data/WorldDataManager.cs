using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Linq;
using UnityEngine.Analytics;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

public class WorldDataManager 
{
    
    private static string WorldDBPath = Application.persistentDataPath + "/World/World_Data.db";
    private static string WorldListName = "worldlist";

    private static SqlDbCommand worldDB = new SqlDbCommand(WorldDBPath);
    public static void SaveWorld()
    {

        // If not, create it.
        if (!worldDB.IsTableCreate<sceneData>(WorldListName))
        {
            worldDB.CreateTable<sceneData>(WorldListName);
        }
        else
        {
            Debug.Log("Saving " + WorldListName);

            worldDB.Update<sceneData>(WorldListName, sceneData.BasicScenes.ToList());

            foreach (var s in sceneData.BasicScenes)
            {
                if (worldDB.SelectById<sceneData>(WorldListName, s.Id) == null)
                {
                    worldDB.Insert<sceneData>(WorldListName, s);
                }
                else
                {
                    worldDB.Update<sceneData>(WorldListName, s);
                }
                SaveScene(s);

            }
            foreach (var s in sceneData.BasicScenes)
            {
                if (worldDB.SelectById<sceneData>(WorldListName, s.Id) == null)
                {
                    worldDB.Insert<sceneData>(WorldListName, s);
                }
                else
                {
                    worldDB.Update<sceneData>(WorldListName, s);
                }
                SaveScene(s);
            }
        }
        


    }

    public static void LoadWorld()
    {

    }

    public static void SaveScene(sceneData scene)
    {
        if (worldDB.IsTableCreate<chunkData>(scene.Name))
        {

            Debug.Log($"{scene.Name} found. Loading from save.");
            Debug.Log(WorldDBPath + scene.Name);

        }
        else
        {
            Debug.Log("World not found. Creating new world.");
            worldDB.CreateTable<chunkData>(scene.Name);

        }


        Debug.Log("Saving " + scene.Name);

        Thread thread = new Thread(() => SaveChunks(scene));
        thread.Start();

    }

    public static void GenerateWorldList()
    {
        worldDB.CreateTable<sceneData>(WorldListName);
        worldDB.Insert<sceneData>(WorldListName, sceneData.BasicScenes.ToList());
    }
    public static sceneData LoadScene(string ScenceName, int seed = 0)
    {
        if (!worldDB.IsTableCreate<sceneData>(WorldListName))
        {
            GenerateWorldList();
        }
        //// Get the path to our world saves.
        //string loadPath = WorldDBPath;

        // Check if a save exists for the name we were passed.
        if (worldDB.SelectBySql<sceneData>(WorldListName, $"Name='{ScenceName}'")!=null)
        {

            Debug.Log("Scene found. Loading from save.");
            Debug.Log(WorldDBPath + ScenceName);


            // And then return the world.

            sceneData scene = worldDB.SelectBySql<sceneData>(WorldListName, $"Name='{ScenceName}'").First();
            SaveChunks(scene);

            return scene;

            // Else, if it doesn't exist, we need to create it and save it.
        }
        else
        {

            Debug.Log("World not found. Creating new scene.");

            sceneData scene = new sceneData (ScenceName, 2, true);
            worldDB.Insert<sceneData>(WorldListName,scene);
            SaveChunks(scene);

            return scene;

        }


    }

    public static void SaveChunks(sceneData scene)
    {
        if (!worldDB.IsTableCreate<chunkData>(scene.Name))
        {
            worldDB.CreateTable<chunkData>(scene.Name);
        }

        // Copy modified chunks into a new list and clear the old one to prevent
        // chunks being added to list while it is saving.
        List<chunkData> chunks = new List<chunkData>(scene.modifiedChunks);
        scene.modifiedChunks.Clear();

        // Loop through each chunk and save it.
        int count = 0;
        foreach (chunkData chunk in chunks)
        {

            SaveChunk(chunk, scene.Name);
            count++;

        }

        Debug.Log(count + " Chunks saved.");

    }
    public static void SaveChunk(chunkData chunk,string sceneName)
    {

        //// Set our save location and make sure we have a saves folder ready to go.
        //string savePath = WorldDBPath;

        Vector2Int position = Chunk.GetChunkVector2Index(chunk.Id);

        string chunkName = sceneName+"_" + position.x + "_" + position.y;
        chunk.Name = chunkName;
        if (worldDB.SelectById<chunkData>(sceneName, chunk.Id)==null)
        {
            worldDB.Insert<chunkData>(sceneName, chunk);
        }
        
        Debug.Log("Saving " + chunkName);
        if (worldDB.IsTableCreate<blockData>(chunkName))
        {
            if (worldDB.SelectById<blockData>(chunkName, chunk.Id) == null)
            {
                worldDB.Insert<blockData>(chunkName, chunk.Blocks.Values.ToList());
            }
            else
            {
                worldDB.Update<blockData>(chunkName, chunk.Blocks.Values.ToList());
            }
        }
        else
        {
            worldDB.CreateTable<blockData>(chunkName);
            worldDB.Insert<blockData>(chunkName, chunk.Blocks.Values.ToList());

        }



    }
    public static chunkData LoadChunk(string sceneName, int ID)
    {
        Vector2Int position = Chunk.GetChunkVector2Index(ID);

        string chunkName = sceneName + "_" + position.x + "_" + position.y;

        if (worldDB.IsTableCreate<chunkData>(sceneName))
        {
            return worldDB.SelectById<chunkData>(sceneName, ID);
        }
        else
        {
            return null;
        }

        // If we didn't find the chunk in our folder, return null and our WorldData script
        // will make a new one.
        

    }

}

