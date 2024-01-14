using System.Collections.Generic;
using UnityEngine;

[HideInInspector]
[System.Serializable]
public class WorldData
{

    public string worldName = "Prototype"; // Will be set by player eventually.
    public int seed;

    [System.NonSerialized]
    public Dictionary<int, ChunkData> Chunks = new Dictionary<int, ChunkData>();

    

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();

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

    public ChunkData RequestChunk(Vector2Int coord, bool create)
    {
        int coordID =Chunk.GetChunkIntID(coord);
        Debug.Log("RequestChunk");
        ChunkData c;



        if (Chunks.ContainsKey(coordID)) // If chunk is there, return it.
        { c = Chunks[coordID];
            Debug.Log("LoadChunk");

        }

        else if (!create) // If it's not and we haven't asked it to be created, return null.
        { c = null; 
        
        }

        else
        { // If it's not and we asked it to be created, create the chunk then return it.
            Debug.Log("LoadChunk");
            LoadChunk(coordID);
            c = Chunks[coordID];

            Debug.Log(Chunks[coordID].ChunkID);
        }



        return c;
    }

    public void LoadChunk(int coord)
    {

        // If the chunk is already loaded we don't need to do anything.
        if (Chunks.ContainsKey(coord))
            return;

        // If not, we check if it is saved and if yes, get the data from there.
        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if (chunk != null)
        {
            Chunks.Add(coord, chunk);
            return;
        }

        // If not, add it to the list and populate it's voxels.
        Chunks.Add(coord, new ChunkData(coord));
        Chunks[coord].Populate();
    }

    public bool IsVoxelInWorld(Vector3 pos)
    {

        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;

    }

    public void AddToModifiedChunkList(ChunkData chunk)
    {

        // Only add to list if ChunkData is not already in the list.
        if (!modifiedChunks.Contains(chunk))
            modifiedChunks.Add(chunk);

    }

    public void SetVoxel(Vector3 pos, byte value)
    {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (!IsVoxelInWorld(pos))
            return;

        // Find out the ChunkCoord value of our voxel's chunk.
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        // Then reverse that to get the position of the chunk.
        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        // Check if the chunk exists. If not, create it.
        int ID = x + (z * VoxelData.WorldChunksSize);
        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        // Then create a Vector3Int with the position of our voxel *within* the chunk.
        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));
        //Debug.Log(string.Format("{0}, {1}, {2}", voxel.x, voxel.y, voxel.z));
        // Then set the voxel in our chunk.

        Chunks[ID].BlockList[Chunk.GetBlockIntID(voxel)].SetBlockType(value);

        AddToModifiedChunkList(chunk);

    }

    public byte GetVoxel(Vector3Int pos)
    {

        int yPos = Mathf.FloorToInt(pos.y);

        if (yPos == 0)
        { return 1; }

        else if (yPos <= 70 && yPos > 0)
        { return 6; }

        else
        { return 0; }


    }

}
