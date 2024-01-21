using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SQLite4Unity3d;
using System;
using Unity.Mathematics;

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

    [NonSerialized]
    public static List<sceneData> BasicScenes =new List<sceneData>()
    {
       new sceneData(0,"test",0,0,true),
       new sceneData(1,"mainMap",1,0,true)
    };
    [NonSerialized]
    public static List<sceneData> GenerateScenes = new List<sceneData>();

    [System.NonSerialized]
    public List<chunkData> modifiedChunks = new List<chunkData>();

    [System.NonSerialized]
    public Dictionary<int, chunkData> Chunks = new Dictionary<int, chunkData>();

    private sceneData(int id,string name,int type,int seed, bool _ac)
    {
        Id = id;
        Name = name;
        Type = type;
        Seed = seed;
        IsActive = _ac;
    }
    public sceneData(string _name,int _type,bool _ac)
    {
        Id = BasicScenes.Count + GenerateScenes.Count - 1;
        Name = _name;
        IsActive = _ac;
        Type = _type;

        if (_type == 2)
        {
            Seed = 2;
        }

        GenerateScenes.Add(this);
    }

    public sceneData(string _name, int _seed, bool _ac, int _type = 2)
    {
        Id = BasicScenes.Count + GenerateScenes.Count - 1;
        Name = _name;
        Seed = _seed;
        IsActive = _ac;
        Type = _type;
    }

    public sceneData(sceneData wD)
    {
        Id = BasicScenes.Count + GenerateScenes.Count - 1;
        Name = wD.Name;
        Seed = wD.Seed;
        Type = wD.Type;
        IsActive = wD.IsActive;

    }

    public chunkData RequestChunk(Vector2Int coord, bool create)
    {
        int coordID = Chunk.GetChunkIntID(coord);
        //Debug.Log("RequestChunk"+coord);
        chunkData c;

        lock (World.Instance.ChunkListThreadLock)
        {

            if (Chunks.ContainsKey(coordID)) // If chunk is there, return it.
            {
                c = Chunks[coordID];
                //Debug.Log("LoadChunk" + coordID);

            }

            else if (!create) // If it's not and we haven't asked it to be created, return null.
            {
                c = null;

            }

            else
            { // If it's not and we asked it to be created, create the chunk then return it.
                //Debug.Log("LoadChunk");
                LoadChunk(coordID);
                c = Chunks[coordID];

                Debug.Log(Chunks[coordID].Id);
            }

        }

        return c;
    }

    public void LoadChunk(int coord)
    {

        // If the chunk is already loaded we don't need to do anything.
        if (Chunks.ContainsKey(coord))
            return;

        // If not, we check if it is saved and if yes, get the data from there.
        chunkData chunk = WorldDataManager.LoadChunk(Name, coord);
        if (chunk != null)
        {
            Chunks.Add(coord, chunk);
            return;
        }

        // If not, add it to the list and populate it's voxels.
        Chunks.Add(coord, new chunkData(coord));
        Chunks[coord].Populate();
    }

    public static bool IsVoxelInScene(Vector3Int worldindex)
    {
        if (worldindex.x >= 0 && worldindex.x < VoxelData.WorldSizeInVoxels && worldindex.y >= 0 && worldindex.y < VoxelData.ChunkHeight && worldindex.z >= 0 && worldindex.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }

    public void AddToModifiedChunkList(chunkData chunk)
    {

        // Only add to list if ChunkData is not already in the list.
        if (!modifiedChunks.Contains(chunk))
            modifiedChunks.Add(chunk);
    }

    public void SetVoxel(Vector3 pos, byte value)
    {
        Vector3Int worldindex = World.GetWorldIndexFromPos(pos);
        Vector2Int chunkindex = World.GetChunkIndexFromPos(pos);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        // If the voxel is outside of the world we don't need to do anything with it.

        if (!IsVoxelInScene(worldindex))
            return;

        // Check if the chunk exists. If not, create it.
        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);

        chunk.Blocks[ID].SetBlockType(value);

        AddToModifiedChunkList(chunk);

    }
    public void SetVoxel(Vector3Int worldindex, byte value)
    {

        Vector2Int chunkindex = World.GetChunkIndexFromWorldIndex(worldindex);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        // If the voxel is outside of the world we don't need to do anything with it.
        if (!IsVoxelInScene(worldindex))
            return;

        // Check if the chunk exists. If not, create it.
        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);

        chunk.Blocks[ID].SetBlockType(value);

        AddToModifiedChunkList(chunk);

    }

    public blockData GetVoxel(Vector3Int worldindex)
    {
        Vector2Int chunkindex = World.GetChunkIndexFromWorldIndex(worldindex);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        // If the voxel is outside of the world we don't need to do anything with it.
        if (!IsVoxelInScene(worldindex))
            return null;

        // Check if the chunk exists. If not, create it.
        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);

        // Then set the voxel in our chunk.
        return chunk.GetVoxel(ID);

    }
}

public class chunkData : BaseData
{
    

    [ModelHelp(true, "Name", "string", false, false)]
    public string Name { get; set; }
    [ModelHelp(true, "index_x", "int", false, true)]
    public int index_x { get; set; }
    [ModelHelp(true, "index_z", "int", false, true)]
    public int index_z { get; set; }
    [ModelHelp(true, "isActive", "bool", false, false)]
    public bool IsActive { get; set; }

    public Dictionary<int, blockData> Blocks = new Dictionary<int, blockData>();

    public chunkData(Vector2Int pos) {  Id = Chunk.GetChunkIntID(pos); }
    public chunkData(int x, int z) { Id = Chunk.GetChunkIntID(new Vector2Int(x, z)); }

    public chunkData(int ID)
    {
        Id = ID;
        Vector2Int index = Chunk.GetChunkVector2Index(Id);
        index_x = index.x;
        index_x = index.y;
        IsActive = true;
    }

    public void Populate()
    {
        for (int z = 0; z < VoxelData.ChunkWidth; z++)
        {

            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {

                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    Vector3Int index = new Vector3Int(x + index_x * VoxelData.ChunkWidth, y, z + index_z * VoxelData.ChunkWidth);
                    int ChunkIdx = z * VoxelData.ChunkHeight * VoxelData.ChunkWidth + x * VoxelData.ChunkHeight + y;
                    blockData block = new blockData(World.GetVoxel(index),index);
                    Blocks[ChunkIdx] = block;
                }
            }
        }
        
        World.Instance.senceData.AddToModifiedChunkList(this);
    }

    public blockData GetVoxel(int ID)
    {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (ID < 0 || ID > VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1)
            return null;
        // Check if the chunk exists. If not, create it.

        return Blocks[ID];
        // Then set the voxel in our chunk.


    }

}

public class blockData : BaseData
{
    [ModelHelp(true, "Type", "byte", false, false)]
    public byte Type { get; set; }
    [ModelHelp(true, "State", "int", false, false)]
    public int State { get; set; }
    [ModelHelp(true, "index_x", "int", false, true)]
    public int index_x { get; set; }
    [ModelHelp(true, "index_y", "int", false, true)]
    public int index_y { get; set; }
    [ModelHelp(true, "index_z", "int", false, true)]
    public int index_z { get; set; }

    public blockData(byte type,Vector3Int index)
    {
        this.Type = type;
        index_x = index.x;
        index_y = index.y;
        index_z = index.z;
    }
    public void SetBlockType(byte type)
    {
        this.Type = type;
    }

    public byte GetBlockType()
    {
        return Type;
    }
}

