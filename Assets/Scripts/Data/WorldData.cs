using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

[HideInInspector]
[System.Serializable]
public class WorldData
{

    public string worldName = "Prototype"; // Will be set by player eventually.
    public int seed;

    [System.NonSerialized]
    public Dictionary<int, chunkData> Chunks = new Dictionary<int, chunkData>();

    

    [System.NonSerialized]
    public List<chunkData> modifiedChunks = new List<chunkData>();

    public WorldData(string _worldName, int _seed)
    {

        worldName = _worldName;
        seed = _seed;

    }

    public WorldData(WorldData wD)
    {

        worldName = wD.worldName;
        seed = wD.seed;

    }

    public chunkData RequestChunk(Vector2Int coord, bool create)
    {
        int coordID =Chunk.GetChunkIntID(coord);
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
        chunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if (chunk != null)
        {
            Chunks.Add(coord, chunk);
            return;
        }

        // If not, add it to the list and populate it's voxels.
        Chunks.Add(coord, new chunkData(coord));
        Chunks[coord].Populate();
    }

    public static bool IsVoxelInWorld(Vector3Int worldindex)
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

        if (!IsVoxelInWorld(worldindex))
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
        if (!IsVoxelInWorld(worldindex))
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
        if (!IsVoxelInWorld(worldindex))
            return null;

        // Check if the chunk exists. If not, create it.
        chunkData chunk = RequestChunk(chunkindex, true);

        int ID = Chunk.GetBlockIntID(index);

        // Then set the voxel in our chunk.
        return chunk.GetVoxel(ID);

    }

}
