using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

[Serializable]
public class BaseData
{
    [ModelHelp(true, "Id", "int", true, false)]
    public int Id { get; set; }
}
public class sceneData : BaseData
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
    public static Dictionary<int, sceneData> Scenes =new  Dictionary<int, sceneData>

    {
       {0,new sceneData(0,"test",0,0,true)},
       {1,new sceneData(1,"mainMap",1,0,true)}
    };

    [NonSerialized]
    public List<chunkData> modifiedChunks = new List<chunkData>();

    [NonSerialized]
    public Dictionary<int, chunkData> Chunks = new Dictionary<int, chunkData>();



    public sceneData(int id,string name,int type,int seed, bool _ac)
    {
        Id = id;
        Name = name;
        Type = type;
        Seed = seed;
        IsActive = _ac;

    }

    public sceneData()
    {
    }

    public sceneData(string _name,int _type,bool _ac)
    {
        Id = Scenes.Count;
        Name = _name;
        IsActive = _ac;
        Type = _type;

        if (_type == 2)
        {
            Seed = 2;
        }

    }

    public sceneData(string _name, int _seed, bool _ac, int _type = 2)
    {
        Id = Scenes.Count;
        Name = _name;
        Seed = _seed;
        IsActive = _ac;
        Type = _type;

    }

    public sceneData(sceneData wD)
    {
        Id = Scenes.Count;
        Name = wD.Name;
        Seed = wD.Seed;
        Type = wD.Type;
        IsActive = wD.IsActive;


    }

    public static void GenerateNewScence(int seed)
    {
        Scenes[Scenes.Keys.Count]= new sceneData(Scenes.Keys.Count,$"randomMap+{Scenes.Keys.Count}",1,seed,true);
    }
    public static void UpdateList(sceneData scene)
    {
        Scenes[scene.Id] = scene;
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

        Chunks.Add(coord, new chunkData(coord,"",this.Name));
        Chunks[coord].Populate();
        WorldDataManager.SaveChunk(Chunks[coord], this.Name);
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

    public byte GetVoxelType(Vector3Int worldindex)
    {
        Vector2Int chunkindex = World.GetChunkIndexFromWorldIndex(worldindex);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        if (!IsVoxelInScene(worldindex))
            return 0;

        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);

        return chunk.GetVoxelType(ID);

    }

    public byte[] IsIndexGround(Vector3Int worldindex,Vector3Int upworldindex)
    {
        byte[] t = new byte[2]; 
        Vector2Int chunkindex = World.GetChunkIndexFromWorldIndex(worldindex);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        Vector3Int upindex = new Vector3Int(upworldindex.x - (chunkindex.x * VoxelData.ChunkWidth), upworldindex.y, upworldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        if (!IsVoxelInScene(worldindex))
        {
            t[0] = 0;
            t[1] = 0;
            return t;
        }

        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);
        t[0]= chunk.GetVoxelType(ID);
        if (!IsVoxelInScene(upworldindex))
        {
            t[1] = 0;
        }
        else
        {
            int upID = Chunk.GetBlockIntID(upindex);
            t[1] = chunk.GetVoxelType(upID);
        }

        return t;

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
    [System.NonSerialized]
    public Dictionary<int, blockData> Blocks = new Dictionary<int, blockData>();

    public chunkData(Vector2Int pos) {  Id = Chunk.GetChunkIntID(pos); }
    public chunkData(int x, int z) { Id = Chunk.GetChunkIntID(new Vector2Int(x, z)); }

    [NonSerialized]
    public Dictionary<int, blockData> BlockstoUpdate = new Dictionary<int, blockData>();
    public chunkData()
    {
    }
    public chunkData(int id, string name, int x,int z,bool isac)
    {
        Id = id;
        Name = name;
        index_x = x;
        index_z = z;
        IsActive = isac;
    }
    public chunkData(int ID,string name="",string scenename="")
    {
        Id = ID;
        Vector2Int index = Chunk.GetChunkVector2Index(Id);
        index_x = index.x;
        index_z = index.y;
        if(name != "")
        {
            Name = name;
        }
        else
        {
            Name = scenename+"_"+index_x+"_"+index_z;
        }

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
                    blockData block = new blockData(ChunkIdx,World.GetVoxel(index),index);
                    Blocks[ChunkIdx] = block;
                }
            }
        }
        
        World.Instance.scenedata.AddToModifiedChunkList(this);
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

    public blockData GetBlockDataFromWorldIndex(Vector3Int worldindex)
    {
        Vector3Int chunkindex = worldindex - new Vector3Int(index_x * VoxelData.ChunkWidth, 0, index_z * VoxelData.ChunkWidth);
        return Blocks[Chunk.GetBlockIntID(chunkindex)];
    }

    public byte GetVoxelType(int ID)
    {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (ID < 0 || ID > VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1)
            return 0;

        return Blocks[ID].Type;
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
    [ModelHelp(true, "orientation", "int", false, true)]
    public int orientation{ get; set; }
    public blockData(int id, byte type,int state, int x, int y, int z, int orientation)
    {
        Id = id;
        Type = type;
        State = state;
        index_x = x;
        index_y = y;
        index_z = z;
        this.orientation = orientation;
    }

    public blockData()
    {

    }
    public blockData(int id,byte type,Vector3Int index, int orientation = 3)
    {
        Id = id;
        this.Type = type;
        State = 4;
        index_x = index.x;
        index_y = index.y;
        index_z = index.z;
        this.orientation = orientation;
    }
    public void SetBlockState(byte type,int ori)
    {
        this.Type = type;
        this.orientation = ori;
    }

    public void SetBlockType(byte type)
    {
        this.Type = type;
    }

    public byte GetBlockType()
    {
        return Type;
    }
    [ModelHelp(false, "properties", "string", false, false)]
    public BlockType properties
    {

        get { return World.Instance.blocktype.BlockTypes[Type]; }

    }
}

