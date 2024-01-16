using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class ChunkData
{

    // The global position of the chunk. ie, (16, 16) NOT (1, 1). We want to be able to
    // access it as a Vector2Int, but Vector2Int's are not serialized so we won't be able
    // to save them. So we'll store them as ints.
    public readonly int X;
    public readonly int Z;
    public readonly int ChunkID;


    [HideInInspector] // Displaying lots of data in the inspector slows it down even more so hide this one.
    public Dictionary<int, Block> BlockList = new Dictionary<int, Block>();
    // Constructors take in position data to ensure we never have ChunkData without a position.
    public ChunkData(Vector2Int pos) { ChunkID = Chunk.GetChunkIntID(pos); }
    public ChunkData(int x, int z) { ChunkID = Chunk.GetChunkIntID(new Vector2Int(x, z)); }

    public ChunkData(int ID) { ChunkID = ID;
        Vector2Int index = Chunk.GetChunkVector2Index(ChunkID);
        X = index.x;
        Z = index.y;
    }

    public void Populate()
    {

        
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Vector3Int index = new Vector3Int(x + X * VoxelData.ChunkWidth, y, z + Z * VoxelData.ChunkWidth);
                    int ChunkIdx = z * VoxelData.ChunkHeight * VoxelData.ChunkWidth + x * VoxelData.ChunkHeight + y;
                    Block block = new Block(World.GetVoxel(index));
                    BlockList[ChunkIdx] = block;

                }
            }
        }
        World.Instance.worldData.AddToModifiedChunkList(this);
    }

    public Block GetVoxel(int ID)
    {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (ID<0||ID> VoxelData.ChunkWidth* VoxelData.ChunkWidth* VoxelData.ChunkHeight-1)
            return null;
        // Check if the chunk exists. If not, create it.

        return BlockList[ID];
        // Then set the voxel in our chunk.


    }

}
