using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SQLite4Unity3d;
using System;



public class BaseData
{
    [ModelHelp(true, "Id", "int", true, false)]
    public int Id { get; set; }
}
public class sceneData:BaseData
{

    [ModelHelp(true, "Name", "string", false, false)]
    public string Name { get; set; }
    [ModelHelp(true, "Type", "int", false, false)]
    public int Type { get; set; }
    [ModelHelp(true, "Seed", "int", false, true)]
    public int Seed { get; set; }
    [ModelHelp(true, "isActive", "bool", false, false)]
    public bool IsActive { get; set; }
    [ModelHelp(true, "SceneWidth", "int", false, false)]
    public int SceneWidth { get; set; }

    [NonSerialized]
    public static sceneData[] BasicScenes = new sceneData[]
    {
       new sceneData{Id=0,Name="test",Type=0,Seed=0,IsActive=true,SceneWidth=4},
       new sceneData{Id=1,Name="mainMap",Type=1,Seed=0,IsActive=true,SceneWidth=8}
    };
    [NonSerialized]
    public static sceneData[] GenerateScenes = new sceneData[] { };
}

public class chunkData : BaseData
{
    public string Name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public bool IsActive { get; set; }
    public int SceneId { get; set; }

}

public class blockData : BaseData
{
    public int Type { get; set; }
    public int State { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }

}

