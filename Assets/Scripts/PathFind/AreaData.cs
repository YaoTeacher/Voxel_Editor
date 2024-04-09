using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

public class AreaData:BaseData
{
    [ModelHelp(true, "name", "string", false, false)]
    public string name{ get; set; }
    [ModelHelp(true, "type", "int", false, false)]
    public int type { get; set; }
    //public bool isInDoor { get; set; }
    // 0 blank 1 street 2 room

    public float centerIndexPointX { get; set; }
    public float centerIndexPointY { get; set; }
    public float centerIndexPointZ { get; set; }

    public float VoxelLengthX { get; set; }
    public float VoxelLengthY { get; set; }
    public float VoxelLengthZ { get; set; }

    public Vector3Int LessBorderPoint { get; set; }
    public Vector3Int BiggerBorderPoint { get; set; }
    public int ParentWorldID { get; set; }
    //public bool isAllowCross { get; set; }


    //public int allowedNumberForEnterPoint = 1;
    public AreaData() { }

    public AreaData(string name, Vector3Int firstpoint, Vector3Int lastpoint, int id)
    {
        this.name = name; 
        Id = id;
        Vector3Int less = new Vector3Int(0, 0, 0);
        Vector3Int bigger = new Vector3Int(0, 0, 0);
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
        BiggerBorderPoint = bigger;

        centerIndexPointX= (bigger.x+less.x+1)/2*VoxelData.BlockSize;
        centerIndexPointY = (bigger.y + less.y + 1)/2 * VoxelData.BlockSize;
        centerIndexPointZ = (bigger.z + less.z + 1)/2 * VoxelData.BlockSize;

        VoxelLengthX = (bigger.x - less.x + 1) * VoxelData.BlockSize;
        VoxelLengthY = (bigger.y - less.y + 1) * VoxelData.BlockSize;
        VoxelLengthZ = (bigger.z - less.z + 1) * VoxelData.BlockSize;

    }


    //public AreaData(Vector3Int firstpoint, Vector3Int lastpoint,int id) 
    //{
    //    Id = id;
    //    Vector3Int less=new Vector3Int(0,0,0);
    //    Vector3Int bigger=new Vector3Int(0, 0, 0);
    //    if (firstpoint.x < lastpoint.x)
    //    {
    //        less.x = firstpoint.x;
    //        bigger.x = lastpoint.x;
    //    }
    //    else
    //    {
    //        less.x = lastpoint.x;
    //        bigger.x = firstpoint.x;
    //    }
    //    if (firstpoint.y < lastpoint.y)
    //    {
    //        less.y = firstpoint.y;
    //        bigger.y = lastpoint.y;
    //    }
    //    else
    //    {
    //        less.y = lastpoint.y;
    //        bigger.y = firstpoint.y;
    //    }
    //    if (firstpoint.z < lastpoint.z)
    //    {
    //        less.z = firstpoint.z;
    //        bigger.z = lastpoint.z;
    //    }
    //    else
    //    {
    //        less.z = lastpoint.z;
    //        bigger.z = firstpoint.z;
    //    }
    //    LessBorderPoint = less;
    //    BiggerBorderPoint   = bigger;
    //}

    public List<Vector3Int> GetTransparentNeibor(Vector3Int path)
    {
        List<Vector3Int> neibor = new List<Vector3Int>();

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
                    if (World.Instance.scenedata.GetVoxelType(path + new Vector3Int(x, y, z))==0)
                    {
                        if (x == 1 && z == 1)
                        {
                            if (World.Instance.scenedata.GetVoxelType(path + new Vector3Int(1, y, 0)) ==0&& World.Instance.scenedata.GetVoxelType(path + new Vector3Int(0, y, 1))==0)
                            {
                                neibor.Add(path + new Vector3Int(x, y, z));
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == -1 && z == 1)
                        {
                            if (World.Instance.scenedata.GetVoxelType(path + new Vector3Int(-1, y, 0))==0 && World.Instance.scenedata.GetVoxelType(path + new Vector3Int(0, y, 1))==0)
                            {
                                neibor.Add(path + new Vector3Int(x, y, z));
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == 1 && z == -1)
                        {
                            if (World.Instance.scenedata.GetVoxelType(path+ new Vector3Int(1, y, 0))==0 && World.Instance.scenedata.GetVoxelType(path+ new Vector3Int(0, y, -1))==0)
                            {
                                neibor.Add(path + new Vector3Int(x, y, z));
                                continue;
                            }
                            else
                                continue;
                        }
                        if (x == -1 && z == -1)
                        {
                            if (World.Instance.scenedata.GetVoxelType(path  + new Vector3Int(-1, y, 0))==0 && World.Instance.scenedata.GetVoxelType(path + new Vector3Int(0, y, -1))==0)
                            {
                                neibor.Add(path + new Vector3Int(x, y, z));
                                continue;
                            }
                            else
                                continue;
                        }
                        neibor.Add(path + new Vector3Int(x, y, z));

                    }
                    else continue;


                }
            }
        }

        return neibor;
    }
}

public class RegionData :BaseData
{

    public float centerIndexPointX { get; set; }
    public float centerIndexPointZ { get; set; }

    public float VoxelLengthX { get; set; }
    public float VoxelLengthZ { get; set; }

    public RegionData() { }
    public RegionData(Vector3Int firstpoint, Vector3Int lastpoint, int id)
    {
        Id = id;
        Vector3Int less = new Vector3Int(0, 0, 0);
        Vector3Int bigger = new Vector3Int(0, 0, 0);
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
        centerIndexPointX = (bigger.x + less.x +1)/2 * VoxelData.BlockSize;
        centerIndexPointZ = (bigger.z + less.z +1) /2* VoxelData.BlockSize;

        VoxelLengthX = (bigger.x - less.x +1) * VoxelData.BlockSize;
        VoxelLengthZ = (bigger.z - less.z +1) * VoxelData.BlockSize;

    }

    public bool isInRegion(Vector3 position)
    {
        if (position.x >= centerIndexPointX - VoxelLengthX && position.x <= centerIndexPointX + VoxelLengthX &&position.z>=centerIndexPointZ-VoxelLengthZ&& position.z <= centerIndexPointZ + VoxelLengthZ)
        {
            return true;
        }
        else { return false; }
    }
}



public class CellData : BaseData
{
    [ModelHelp(true, "areaID", "int", false, true)]
    public int areaID = 0;

    [ModelHelp(true, "index_x", "int", false, true)]
    public int index_x { get; set; }
    [ModelHelp(true, "index_y", "int", false, true)]
    public int index_y { get; set; }
    [ModelHelp(true, "index_z", "int", false, true)]
    public int index_z { get; set; }

    public Vector3Int WorldIndex;
    public CellData() { }
    public CellData(Vector3Int worldIndex, int Area)
    {
        WorldIndex = worldIndex;
        index_x = worldIndex.x; 
        index_y = worldIndex.y; 
        index_z = worldIndex.z;
        areaID = Area;
    }

    public void SetArea(int areaID)
    {
        this.areaID = areaID;     
    }

    public void ResetCell()
    {
        areaID = 0;
    }
}

//public class CellData : BaseData
//{
//    public int areaID = -1;
//    public Vector3Int WorldIndex;
//    public float cost = 255;
//    public float finalcost = 9999;
//    public Vector3 direction = new Vector3Int(0, 0, 0);

//    public CellData() { }
//    public CellData(Vector3Int worldIndex, float blockrough)
//    {
//        WorldIndex = worldIndex;
//        cost = blockrough;
//    }

//    public void SetDirection()
//    {

//    }

//    public void ResetCell()
//    {
//        finalcost = 9999;
//    }
//}

//public class AreaLink : BaseData
//{
//    public int startAreaID { set; get; }
//    public int endAreaID { set; get; }
//    public float cost { set; get; }

//    public List<TargetPoint>outAreaPoints = new List<TargetPoint>();
//    public AreaLink(){}

//    public AreaLink(int startArea, int endArea,float cost)
//    {
//        startAreaID = startArea;
//        endAreaID = endArea;
//        this.cost = cost;
//    }

//}

//public class TargetPoint : BaseData 
//{
//    public int areaID ;
//    public Vector3Int WorldIndex;
//    public byte type;
//    public bool IsAccessable;

//    public TargetPoint(GroundCellData f) 
//    { 
//        areaID = f.areaID;
//        WorldIndex = f.WorldIndex;
//        type = 1;
//        IsAccessable = true;

//    }
//}

