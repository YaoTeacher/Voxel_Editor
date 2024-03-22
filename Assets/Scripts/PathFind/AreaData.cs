using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class AreaData:BaseData
{

    public string name;
    public int type;
    public bool isInDoor;
    public Vector3Int LessBorderPoint;
    public Vector3Int BiggerBorderPoint;

    public Dictionary<Vector3Int, FlowFieldCellData> onGroundCell=new Dictionary<Vector3Int, FlowFieldCellData>();

    public AreaData() { }

    public AreaData(Vector3Int firstpoint, Vector3Int lastpoint,int id) 
    {
        Id = id;
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

    public List<FlowFieldCellData> GetGroundNeibor(FlowFieldCellData path)
    {
        List<FlowFieldCellData> neibor = new List<FlowFieldCellData>();

        for (int z = -1; z <= 1; z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }
                for (int y = -1; y <= 1; y++)
                {
                    if (onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(x, y, z)))
                    {
                        if (x == 1 && z == 1)
                        {
                            if (onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(1, y, 0)) && onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(0, y, 1)))
                            {
                                neibor.Add(onGroundCell[path.WorldIndex + new Vector3Int(x, y, z)]);
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == -1 && z == 1)
                        {
                            if (onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(-1, y, 0)) && onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(0, y, 1)))
                            {
                                neibor.Add(onGroundCell[path.WorldIndex + new Vector3Int(x, y, z)]);
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == 1 && z == -1)
                        {
                            if (onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(1, y, 0)) && onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(0, y, -1)))
                            {
                                neibor.Add(onGroundCell[path.WorldIndex + new Vector3Int(x, y, z)]);
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == -1 && z == -1)
                        {
                            if (onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(-1, y, 0)) && onGroundCell.ContainsKey(path.WorldIndex + new Vector3Int(0, y, -1)))
                            {
                                neibor.Add(onGroundCell[path.WorldIndex + new Vector3Int(x, y, z)]);
                                continue;
                            }
                            else
                                continue;
                        }
                        neibor.Add(onGroundCell[path.WorldIndex + new Vector3Int(x, y, z)]);

                    }
                    else continue;

                    
                }
            }
        }

        return neibor;
    }
}

public class FlowFieldCellData : BaseData
{
    public int areaID=-1;
    public Vector3Int WorldIndex;
    public float cost=255;
    public float finalcost=9999;
    public Vector3 direction=new Vector3Int(0,0,0);

    public FlowFieldCellData(){}
    public FlowFieldCellData(Vector3Int worldIndex, float blockrough)
    {
        WorldIndex = worldIndex;
        cost = blockrough;
    }

    public void SetDirection()
    {

    }

    public void ResetCell()
    {
        finalcost = 9999;
    }
}

public class EnterPoint : BaseData 
{
    public Vector3Int WorldIndex;
    public byte type;
    public bool IsAccessable;
}

