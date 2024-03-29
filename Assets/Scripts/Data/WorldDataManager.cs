using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Linq;
using Unity.Entities;

public class WorldDataManager 
{
    
    private static string WorldDBPath = Application.streamingAssetsPath + "/World/World_Data.db";
    private static string WorldListName = "worldlist";
    private static string AreaListName = "Groundlist";

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

            foreach (var s in sceneData.Scenes.Values)
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

        SaveChunks(scene);

    }

    public static void GenerateWorldList()
    {
        worldDB.CreateTable<sceneData>(WorldListName);
        worldDB.Insert<sceneData>(WorldListName, sceneData.Scenes.Values.ToList());
    }
    public static sceneData LoadScene(string ScenceName)
    {
        if (!worldDB.IsTableCreate<sceneData>(WorldListName))
        {
            GenerateWorldList();
        }

        if (worldDB.SelectBySql<sceneData>(WorldListName, $"Name='{ScenceName}'")!=null)
        {

            Debug.Log("Scene found. Loading from save.");
            Debug.Log(WorldDBPath + ScenceName);

            sceneData scene = worldDB.SelectBySql<sceneData>(WorldListName, $"Name='{ScenceName}'").First();
            worldDB.CreateTable<chunkData>(ScenceName);



            SaveChunks(scene);

            return scene;

        }
        else
        {

            Debug.Log("World not found. Creating new scene.");

            sceneData scene = new sceneData (ScenceName, 2, true);
            worldDB.Insert<sceneData>(WorldListName,scene);
            worldDB.CreateTable<chunkData>(ScenceName);
            SaveChunks(scene);

            return scene;

        }


    }
    //chunkdata
    #region 
    public static void SaveChunks(sceneData scene)
    {

        // Copy modified chunks into a new list and clear the old one to prevent
        // chunks being added to list while it is saving.
        Debug.Log(scene.Chunks.Values.Count.ToString());

        scene.modifiedChunks.Clear();

        // Loop through each chunk and save it.
        int count = 0;
        foreach (chunkData chunk in scene.Chunks.Values)
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


        if (worldDB.SelectById<chunkData>(sceneName, chunk.Id)==null)
        {
            worldDB.Insert<chunkData>(sceneName, chunk);
            string chunkName = sceneName + "_" + position.x + "_" + position.y;
            chunk.Name = chunkName;
        }

        if (!worldDB.IsTableCreate<blockData>(chunk.Name))
        {
            worldDB.CreateTable<blockData>(chunk.Name);
            chunk.Populate();
        }

        Debug.Log("Saving " + chunk.Name);

        if (chunk.BlockstoUpdate.Values.Count == 0)
            return;

   
        worldDB.Update<blockData>(chunk.Name, chunk.BlockstoUpdate.Values.ToList());

    }
    public static chunkData LoadChunk(string sceneName, int ID)
    {
        Vector2Int position = Chunk.GetChunkVector2Index(ID);

        string chunkName = sceneName + "_" + position.x + "_" + position.y;

        chunkData chunk = new chunkData();

        if (worldDB.SelectById<chunkData>(sceneName, ID)!=null)
        {
            chunk = worldDB.SelectById<chunkData>(sceneName, ID);
           
        }
        else
        {
            chunk = new chunkData(ID,chunkName);
            worldDB.Insert<chunkData>(sceneName, chunk);
        }

        if (!worldDB.IsTableCreate<blockData>(chunkName))
        {
            worldDB.CreateTable<blockData>(chunkName);
            chunk.Populate();
            worldDB.Insert<blockData>(chunkName, chunk.Blocks.Values.ToList());

        }
        else
        {
            chunk.Blocks = worldDB.SelectBySqlDic<blockData>(chunk.Name);
        }
        
        return chunk;

        // If we didn't find the chunk in our folder, return null and our WorldData script
        // will make a new one.


    }
    #endregion

    //WayPointsdata
    #region
    public static void SaveScenceAreas()
    {

        // If not, create it.
        if (!worldDB.IsTableCreate<scenceGroundData>($"worldGroundList"))
        {
            worldDB.CreateTable<scenceGroundData>($"worldGroundList");
        }
        else
        {
            Debug.Log("Saving " + "worldGroundList");

            foreach (var s in scenceGroundData.Grounds.Values)
            {
                if (worldDB.SelectById<scenceGroundData>("worldGroundList", s.Id) == null)
                {
                    worldDB.Insert<scenceGroundData>("worldGroundList", s);
                }
                else
                {
                    worldDB.Update<scenceGroundData>("worldGroundList", s);
                }
                SaveAreas(s);

            }
        }

    }
    public static void SaveAreas(scenceGroundData scenceGround)
    {
        // Copy modified chunks into a new list and clear the old one to prevent
        // chunks being added to list while it is saving.
        Debug.Log(scenceGround.Areas.Values.Count.ToString());


        // Loop through each chunk and save it.
        int count = 0;
        foreach (AreaData a in scenceGround.Areas.Values)
        {

            //SaveArea(a, );
            count++;

        }

        Debug.Log(count + " Area saved.");
    }
    #endregion
}

