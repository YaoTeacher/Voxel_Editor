using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData:BaseData
{
    public string name;
    public int type;
    public bool isIndoor;
    public Vector3Int LessBorderPoint;
    public Vector3Int BiggerBorderPoint;

    

    public AreaData() { }

    public AreaData(Vector3Int firstpoint, Vector3Int lastpoint) 
    {
        if (firstpoint.x < lastpoint.x)
        {
            LessBorderPoint.x = firstpoint.x;
            BiggerBorderPoint.x = lastpoint.x;
        }
        else
        {
            LessBorderPoint.x = lastpoint.x;
            BiggerBorderPoint.x = firstpoint.x;
        }
        if (firstpoint.y < lastpoint.y)
        {
            LessBorderPoint.y = firstpoint.y;
            BiggerBorderPoint.y = lastpoint.y;
        }
        else
        {
            LessBorderPoint.y = lastpoint.y;
            BiggerBorderPoint.y = firstpoint.y;
        }
        if (firstpoint.z < lastpoint.z)
        {
            LessBorderPoint.z = firstpoint.z;
            BiggerBorderPoint.z = lastpoint.z;
        }
        else
        {
            LessBorderPoint.z = lastpoint.z;
            BiggerBorderPoint.z = firstpoint.z;
        }
    }

}

public class FlowFieldCellData : BaseData
{
    public Vector3Int WorldIndex;
    public float cost;
    public float finalcost;
    public Vector3 direction;

    public FlowFieldCellData(){}
    public FlowFieldCellData(Vector3Int worldIndex, float blockrough)
    {
        WorldIndex = worldIndex;
        cost = blockrough;
    }

    public void SetDirection()
    {

    }
}

public class EnterPoint : BaseData 
{
    public Vector3Int WorldIndex;
    public byte type;
    public bool IsAccessable;
}

