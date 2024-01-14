using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Block
{
    byte type;
    byte levelOfDamege;
    public readonly int blockID;


    public Block(  byte type,int x,int y, int z)
    {

        this.type = type;
        blockID = (z * 16 + x) * 128 + y;

    }

    public Block(byte type, Vector3Int BlockInWorld)
    {

        this.type = type;
        blockID = (BlockInWorld.z * VoxelData.ChunkWidth + BlockInWorld.x) * VoxelData.ChunkHeight + BlockInWorld.y;

    }

    public void SetBlockType(byte type)
    {
        this.type = type;
    }

    public byte GetBlockType()
    {
        return type;
    }

    public Vector3Int GetVector3Index()
    {
        Vector3Int ID =new Vector3Int();
        ID.z=(int) MathF.Floor(blockID / (VoxelData.ChunkHeight * VoxelData.ChunkWidth));
        ID.x= (int)MathF.Floor((blockID-(ID.z* VoxelData.ChunkHeight * VoxelData.ChunkWidth))/ VoxelData.ChunkHeight);
        ID.y = blockID - (ID.x * VoxelData.ChunkHeight * VoxelData.ChunkWidth) - (ID.x * VoxelData.ChunkHeight);
        return ID;

    }
}
