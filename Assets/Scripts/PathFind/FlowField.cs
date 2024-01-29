using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{

    public Dictionary<int, AreaData> Areas { get; private set; }

    public Dictionary<Vector3Int, FlowFieldCellData> GroundData = new Dictionary<Vector3Int, FlowFieldCellData>();

    public AreaData SetNewArea(Vector3Int firstpoint, Vector3Int lastpoint)
    {
        AreaData area = new AreaData(firstpoint, lastpoint,Areas.Count);
        Areas[Areas.Count]= area;
        return area;
    }

    //public bool IsGround(Vector3Int point,int height)
    //{
    //    for (int x = 0;x<=height; x++)
    //    {
    //        point = point + new Vector3Int(0, 1, 0);
    //        if (CheckForVoxel(point))
    //            //if (scenedata.RequestChunk(ChunkIndex, false).GetBlockDataFromWorldIndex(point + new Vector3Int(0, 1, 0)) != null||!CheckForVoxel(point + new Vector3Int(0, 1, 0)))
    //            return false;
    //    }


    //    return true;
    //}

    public void GenerateGround(World scene,int creature = 4)
    {
        for (int z = 0; z <VoxelData.WorldSizeInVoxels; z++)
        {

            for (int x = 0; x < VoxelData.WorldSizeInVoxels; x++)
            {

                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    Vector3Int point = new Vector3Int(x, y, z);
                    Vector3Int uppoint = point;

                    for (int c = 0; c <= creature; c++)
                    { 
                        uppoint += new Vector3Int(0, 1, 0);
                        if (!scene.IsGround(point,uppoint))
                        {
                            y += c;
                            break;
                        }
                        else 
                        {
                            
                            if (c == creature)
                            {
                                GroundData[point] = new FlowFieldCellData(point, World.Instance.blocktype.BlockTypes[1].rough);
                                y += c;
                                break;
                            }

                            continue; 
                        }
                    }
                    
                }
            }
        }
    }

    public void GenerateArea(AreaData areaData)
    {
        for (int z = areaData.LessBorderPoint.z; z <= areaData.BiggerBorderPoint.z; z++)
        {

            for (int x = areaData.LessBorderPoint.x; x < areaData.BiggerBorderPoint.x; x++)
            {

                for (int y = areaData.LessBorderPoint.y; y < areaData.BiggerBorderPoint.y; y++)
                {
                    Vector3Int worldindex = new Vector3Int(x, y, z);
                   if (GroundData.ContainsKey(worldindex))
                    {
                        GroundData[worldindex].areaID = areaData.Id;
                        areaData.onGroundCell[worldindex]= GroundData[worldindex];
                    }
                }
            }
        }
    }

    public void GenerateCostMap(Vector3Int target)
    {
        if (GroundData.ContainsKey(target))
        {
            FlowFieldCellData f = GroundData[target];
            if (GroundData[target].areaID!=-1)
            {
                foreach (FlowFieldCellData f2 in Areas[f.areaID].onGroundCell.Values)
                {
                    Areas[f.areaID].GetGroundNeibor(f2);
                }
                
            }
        }
    }

    
}
