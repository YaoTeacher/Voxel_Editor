using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{

    public static readonly int ChunkWidth = 8;
    public static readonly int ChunkHeight = 4;
    public static readonly int WorldChunksSize = 3;
    public static readonly float BlockSize= 0.5F;

    public static int WorldSizeInVoxels
    {

        get { return WorldChunksSize * ChunkWidth; }

    }

    public static int WorldCentre
    {

        get { return (WorldChunksSize * ChunkWidth) / 2; }

    }

    public static readonly int ViewDistanceInChunks = 1;

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {

        get { return 1f / (float)TextureAtlasSizeInBlocks; }

    }

    public static readonly Vector3[] voxelVerts = new Vector3[8] {

        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(BlockSize, 0.0f, 0.0f),
        new Vector3(BlockSize, BlockSize, 0.0f),
        new Vector3(0.0f, BlockSize, 0.0f),
        new Vector3(0.0f, 0.0f, BlockSize),
        new Vector3(BlockSize, 0.0f, BlockSize),
        new Vector3(BlockSize, BlockSize, BlockSize),
        new Vector3(0.0f, BlockSize, BlockSize),

    };

    public static readonly int[] faceChecks = new int[6] {

        -1, //down
        1,  //up
        -ChunkWidth*ChunkHeight, //back
        ChunkWidth*ChunkHeight,  //front
        -ChunkHeight, //left
        ChunkHeight  //right



    };

    public static readonly int[,] voxelTris = new int[6, 4] {

        // Back, Front, Top, Bottom, Left, Right

		// 0 1 2 2 1 3
        {1, 5, 0, 4}, // Bottom Face
        {3, 7, 2, 6}, // Top Face
        {0, 3, 1, 2},// Back Face
        {5, 6, 4, 7},// Front Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

    public static readonly Vector2[] voxelUvs = new Vector2[4] {

        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 1.0f)

    };

   
}

