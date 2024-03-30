using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class AreaData:BaseData
{
    [ModelHelp(true, "name", "string", false, false)]
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

    public Dictionary<Vector3Int, GroundCellData> onGroundCell=new Dictionary<Vector3Int, GroundCellData>();
    public Dictionary<Vector3Int, EnterPoint> EnterPoints = new Dictionary<Vector3Int, EnterPoint>();
    public List<int>neiborAreas = new List<int>();

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

    public List<GroundCellData> GetGroundNeibor(GroundCellData path)
    {
        List<GroundCellData> neibor = new List<GroundCellData>();

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

public class GroundCellData : BaseData
{
    public int areaID=-1;
    public Vector3Int WorldIndex;
    public float cost=255;
    public float finalcost=9999;
    public Vector3 direction=new Vector3Int(0,0,0);

    public GroundCellData(){}
    public GroundCellData(Vector3Int worldIndex, float blockrough)
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

public class AreaLink : BaseData
{
    public int StartArea { set; get; }
    public int EndArea { set; get; }

    public float Direction_X{ get; set; }
    public float Direction_Y{ get; set; }
    public float Direction_Z { get; set; }

    public AreaLink(){}

    public AreaLink(int startArea, int endArea,Vector3 direction)
    {
        StartArea = startArea;
        EndArea = endArea;
        Direction_X =direction.x;
        Direction_Y =direction.y;
        Direction_Z =direction.z;
    }
    public Vector3 GetLinkDirection()
    {
        return new Vector3(Direction_X, Direction_Y, Direction_Z);
    }
}

public class EnterPoint : BaseData 
{
    public int areaID ;
    public Vector3Int WorldIndex;
    public byte type;
    public bool IsAccessable;

    public EnterPoint(GroundCellData f) 
    { 
        areaID = f.areaID;
        WorldIndex = f.WorldIndex;
        type = 1;
        IsAccessable = true;

    }
}

