using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData:BaseData
{
    public int type;
    public bool IsIndoor;
    public Vector3Int firstBorderPoint;
    public Vector3Int lastBorderPoint;
    public PathData[,,] points;

}

public class PathData: BaseData 
{
    public int type;
    public bool IsGround;
    public bool IsAccessable;
}

public class FlowMap : BaseData 
{
    
}

