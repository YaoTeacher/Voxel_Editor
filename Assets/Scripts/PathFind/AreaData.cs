using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData:BaseData
{
    public int type;
    public bool IsIndoor;
    public Vector3Int LessBorderPoint;
    public Vector3Int BiggerBorderPoint;
    public Vector3Int Target;

    public Dictionary<Vector3Int,PathBlockData> GroundData = new Dictionary<Vector3Int,PathBlockData>();

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

    public List<Vector3Int> GetGroundNeibor(PathBlockData path)
    {
        List<Vector3Int> neibor = new List<Vector3Int>();

        for (int z = -1; z <= 1;z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && z == 0&& y == 0)
                    {
                        continue;
                    }
                    if (x == 1 && z == 1)
                    {
                        if (GroundData.ContainsKey(path.WorldIndex + new Vector3Int(1, y, 0))&& GroundData.ContainsKey(path.WorldIndex + new Vector3Int(0, y, 1)))
                        {
                            neibor.Add(path.WorldIndex + new Vector3Int(x, y, z));
                            continue;
                        }
                        else
                            continue;
                    }
                    if (x == -1 && z == 1)
                    {
                        if (GroundData.ContainsKey(path.WorldIndex + new Vector3Int(-1, y, 0)) && GroundData.ContainsKey(path.WorldIndex + new Vector3Int(0, y, 1)))
                        {
                            neibor.Add(path.WorldIndex + new Vector3Int(x, y, z));
                            continue;
                        }
                        else
                            continue;
                    }
                    if (x == 1 && z == -1)
                    {
                        if (GroundData.ContainsKey(path.WorldIndex + new Vector3Int(1, y, 0)) && GroundData.ContainsKey(path.WorldIndex + new Vector3Int(0, y, -1)))
                        {
                            neibor.Add(path.WorldIndex + new Vector3Int(x, y, z));
                            continue;
                        }
                        else
                            continue;
                    }
                    if (x == -1 && z == -1)
                    {
                        if (GroundData.ContainsKey(path.WorldIndex + new Vector3Int(-1, y, 0)) && GroundData.ContainsKey(path.WorldIndex + new Vector3Int(0, y, -1)))
                        {
                            neibor.Add(path.WorldIndex + new Vector3Int(x, y, z));
                            continue;
                        }
                        else
                            continue;
                    }
                    if (GroundData[path.WorldIndex + new Vector3Int(x, y, z)] != null)
                    {

                        neibor.Add(path.WorldIndex + new Vector3Int(x, y, z));
                    }
                    else
                        continue;
                }
            }
        }

        return neibor;
    }

    public void AddAreaPoint(Vector3Int point)
    {
        if (GroundData.ContainsKey(point))
        {

        }
    }

    public Dictionary<Vector3Int, PathBlockData> GenerateFlowField(PathBlockData taget)
    {
        Dictionary < Vector3Int, PathBlockData > flowfielddic = new Dictionary<Vector3Int, PathBlockData >();
        return flowfielddic;
    }

}

public class PathBlockData : BaseData
{
    public Vector3Int WorldIndex;
    public float cost;
    public float finalcost;
    public Vector3 direction;

    public PathBlockData(){}
    public PathBlockData(Vector3Int worldIndex, Vector3 direction)
    {
        WorldIndex = worldIndex;
        this.direction = direction;
    }
}

public class EnterPoint : BaseData 
{
    public Vector3Int WorldIndex;
    public byte type;
    public bool IsAccessable;
}

