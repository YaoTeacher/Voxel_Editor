using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{

    // The global position of the chunk. ie, (16, 16) NOT (1, 1). We want to be able to
    // access it as a Vector2Int, but Vector2Int's are not serialized so we won't be able
    // to save them. So we'll store them as ints.
    int X;
    int Z;
    Vector3Int[,,] ChunkIndex = new Vector3Int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public Vector2Int position
    {

        get { return new Vector2Int(X, Z); }
        set
        {
            X = value.x;
            Z = value.y;
        }
    }

    [HideInInspector] // Displaying lots of data in the inspector slows it down even more so hide this one.

    // Constructors take in position data to ensure we never have ChunkData without a position.
    public ChunkData(Vector2Int pos) { position = pos; }
    public ChunkData(int x, int z) { position = new Vector2Int(x, z); }

    public void Populate(WorldData world)
    {

        
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    Vector3Int index = new Vector3Int(x + X * VoxelData.ChunkWidth, y, z + Z * VoxelData.ChunkWidth);

                    ChunkIndex[x, y, z] = index;
                }
            }
        }
        World.Instance.worldData.AddToModifiedChunkList(this);
    }

}
