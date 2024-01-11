using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Block
{

    Vector3Int index;
    byte type;

    public Block( Vector3Int index, byte type)
    {
        this.index = index;
        this.type = type;
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
