using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class WorldDataManager : MonoBehaviour
{
    private string worldDBConnectionString = Application.streamingAssetsPath;
    public SQLiteConnection Connection;
    // Start is called before the first frame update
    void Start()
    {
        CreateSceneTable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateSceneTable()
    {
        //if (File.Exists(worldDBConnectionString))
        //{
            Connection = new SQLiteConnection(Application.streamingAssetsPath + "/WorldDataBase.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Connection.CreateTable<sceneData>();
        //}
        //else
        //{
        //    File.Create(worldDBConnectionString);
        //    Connection = new SQLiteConnection(worldDBConnectionString + "/WorldDataBase.db", SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        //    Connection.CreateTable<sceneData>("World");
        //}
        
        foreach (var a in sceneData.BasicScenes)
        {
            Connection.Insert(a);
        }
    }

    public void CreateChunkTable()
    {
        SQLiteConnection chunkConnection; 
    }
}

