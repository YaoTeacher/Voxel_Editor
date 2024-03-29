using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData:BaseData
{
    [ModelHelp(true, "parentScenceID", "int", false, false)]
    public string name{ get; set; }
    public int type { get; set; }
    //public bool isInDoor { get; set; }
    // 0 blank 1 street 2 indoorroom

    public int LessBorderPointX { get; set; }
    public int LessBorderPointY { get; set; }
    public int LessBorderPointZ { get; set; }

    public int BiggerBorderPointX { get; set; }
    public int BiggerBorderPointY{ get; set; }
    public int BiggerBorderPointZ { get; set; }
    public Vector3Int LessBorderPoint { get; set; }
    public Vector3Int BiggerBorderPoint { get; set; }
    public int ParentWorldID { get; set; }
    //public bool isAllowCross { get; set; }

    public Dictionary<Vector3Int, FlowFieldCellData> onGroundCell=new Dictionary<Vector3Int, FlowFieldCellData>();
    public Dictionary<Vector3Int, EnterPoint> EnterPoints = new Dictionary<Vector3Int, EnterPoint>();

    public int allowedNumberForEnterPoint = 1;
    public AreaData() { }

    public AreaData(Vector3Int firstpoint, Vector3Int lastpoint,int id) 
    {
        Id = id;
        Vector3Int less=new Vector3Int(0,0,0);
        Vector3Int bigger=new Vector3Int(0, 0, 0);
        if (firstpoint.x < lastpoint.x)
        {
            less.x = firstpoint.x;
            bigger.x = lastpoint.x;
        }
        else
        {
            less.x = lastpoint.x;
            bigger.x = firstpoint.x;
        }
        if (firstpoint.y < lastpoint.y)
        {
            less.y = firstpoint.y;
            bigger.y = lastpoint.y;
        }
        else
        {
            less.y = lastpoint.y;
            bigger.y = firstpoint.y;
        }
        if (firstpoint.z < lastpoint.z)
        {
            less.z = firstpoint.z;
            bigger.z = lastpoint.z;
        }
        else
        {
            less.z = lastpoint.z;
            bigger.z = firstpoint.z;
        }
        LessBorderPoint = less;
        BiggerBorderPoint   = bigger;
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
    public int areaID ;
    public Vector3Int WorldIndex;
    public byte type;
    public bool IsAccessable;

    public EnterPoint(FlowFieldCellData f) 
    { 
        areaID = f.areaID;
        WorldIndex = f.WorldIndex;
        type = 1;
        IsAccessable = true;

    }
}

