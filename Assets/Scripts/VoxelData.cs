using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class VoxelData
{

    public static readonly int ChunkWidth = 8;
    public static readonly int ChunkHeight = 40;
    public static readonly int WorldChunksSize = 8;
    public static readonly float BlockSize= 0.5F;

    public static int WorldSizeInVoxels
    {

        get { return WorldChunksSize * ChunkWidth; }

    }

    public static int WorldCentre
    {

        get { return (WorldChunksSize * ChunkWidth) / 2; }

    }

    public static readonly int ViewDistanceInChunks = 3;

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

    public static Dictionary<Vector2Int, int> transP = new Dictionary<Vector2Int, int>
   {
        { new Vector2Int(2,0),0 },
        { new Vector2Int(2,1),1 },
        { new Vector2Int(2,2),2 },
        { new Vector2Int(2,3),3 },
        { new Vector2Int(2,4),4 },
        { new Vector2Int(2,5),5 },

        { new Vector2Int(3,0),0 },
        { new Vector2Int(3,1),1 },
        { new Vector2Int(3,2),3 },
        { new Vector2Int(3,3),2 },
        { new Vector2Int(3,4),5 },
        { new Vector2Int(3,5),4 },

        { new Vector2Int(4,0),0 },
        { new Vector2Int(4,1),1 },
        { new Vector2Int(4,2),5 },
        { new Vector2Int(4,3),4 },
        { new Vector2Int(4,4),2 },
        { new Vector2Int(4,5),3 },

        { new Vector2Int(5,0),0 },
        { new Vector2Int(5,1),1 },
        { new Vector2Int(5,2),4 },
        { new Vector2Int(5,3),5 },
        { new Vector2Int(5,4),3 },
        { new Vector2Int(5,5),2 },

        { new Vector2Int(12,0),3 },
        { new Vector2Int(12,1),2 },
        { new Vector2Int(12,2),0 },
        { new Vector2Int(12,3),1 },
        { new Vector2Int(12,4),4 },
        { new Vector2Int(12,5),5 },

        { new Vector2Int(13,0),2 },
        { new Vector2Int(13,1),3 },
        { new Vector2Int(13,2),1 },
        { new Vector2Int(13,3),0 },
        { new Vector2Int(13,4),4 },
        { new Vector2Int(13,5),5 },

        { new Vector2Int(14,0),5 },
        { new Vector2Int(14,1),4 },
        { new Vector2Int(14,2),2 },
        { new Vector2Int(14,3),3 },
        { new Vector2Int(14,4),0 },
        { new Vector2Int(14,5),1 },

        { new Vector2Int(15,0),4 },
        { new Vector2Int(15,1),5 },
        { new Vector2Int(15,2),2 },
        { new Vector2Int(15,3),3 },
        { new Vector2Int(15,4),1 },
        { new Vector2Int(15,5),0 },

   };





//// 顶面朝右
//else if (block.orientation == 14)
//{
//    if (p == 0) translatedP = 5; // 底-左
//    if (p == 1) translatedP = 4; // 顶-右
//    if (p == 4) translatedP = 0; // 左-顶
//    if (p == 5) translatedP = 1; // 右-底
//}
//            }
}

