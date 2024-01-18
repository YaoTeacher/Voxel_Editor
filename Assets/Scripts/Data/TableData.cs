using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System;

public class sceneData
{


    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public int Seed { get; set; }
    public bool IsActive { get; set; }
    public int SceneWidth { get; set; }

    [NonSerialized]
    public static sceneData[] BasicScenes = new sceneData[]
{
       new sceneData{Id=0,Name="test",Type=0,Seed=0,IsActive=true,SceneWidth=4},
       new sceneData{Id=1,Name="mainMap",Type=1,Seed=0,IsActive=true,SceneWidth=8}
};

}

public class chunkData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public bool IsActive { get; set; }
    public int SceneId { get; set; }

}

public class blockData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int Type { get; set; }
    public int State { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

}

