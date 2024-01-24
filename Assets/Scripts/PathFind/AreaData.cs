using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData:BaseData
{
    public int type;
    public bool IsIndoor;
    public Vector3Int firstBorderPoint;
    public Vector3Int lastBorderPoint;

    public Dictionary<Vector3Int,PathData> GroundData = new Dictionary<Vector3Int,PathData>();

    public List<Vector3Int> GetNeibor(PathData path)
    {
        List<Vector3Int> neibor = new List<Vector3Int>();

        for (int z = -1; z <= 1;z++)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
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
}

public class PathData: BaseData 
{
    public int type;
    public bool IsGround;
    public bool IsAccessable;
    public Vector3Int WorldIndex;
}

public class FlowMap : BaseData 
{
    
}

