using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Block
{

    Vector3 pos;
    Vector3Int index;
    byte type;

    public Block(Vector3 pos, Vector3Int index, byte type)
    {
        this.pos = pos;
        this.index = index;
        this.type = type;
    }
    
    public void SetPos(Vector3 pos)
    {
        this.pos = pos;
    }

    public Vector3 GetPos()
    {
        return this.pos;
    }
    public Vector3Int GetIndex()
    {
        return this.index;
    }


    public void SetBlockType(byte type)
    {
        this.type = type;
    }

    public byte GetBlockType()
    {
        return type;
    }
}
