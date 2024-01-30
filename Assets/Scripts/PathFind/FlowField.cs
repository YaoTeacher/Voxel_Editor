using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class FlowField
{

    public Dictionary<int, AreaData> Areas = new Dictionary<int, AreaData>(); 

    public Dictionary<Vector3Int, FlowFieldCellData> GroundData = new Dictionary<Vector3Int, FlowFieldCellData>();

    public AreaData SetNewArea(Vector3Int firstpoint, Vector3Int lastpoint)
    {
        AreaData area = new AreaData(firstpoint, lastpoint,Areas.Count);
        Areas[area.Id]= area;
        Debug.Log($"{area.Id},{Areas.Values.Count}");
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
        Debug.Log("generateground!");
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
        Debug.Log("generatearea!");
        for (int z = areaData.LessBorderPoint.z; z <= areaData.BiggerBorderPoint.z; z++)
        {

            for (int x = areaData.LessBorderPoint.x; x <= areaData.BiggerBorderPoint.x; x++)
            {

                for (int y = areaData.LessBorderPoint.y; y <= areaData.BiggerBorderPoint.y; y++)
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
        Areas[areaData.Id] = areaData;
        Debug.Log($"{areaData.Id}");
        Debug.Log($"{areaData.onGroundCell.Values.Count}");
        Debug.Log($"{Areas.Values.Count}");
        Debug.Log($"{Areas[areaData.Id].onGroundCell.Values.Count}");
    }

    public void GenerateCostMap(Vector3Int target)
    {
        if (GroundData.ContainsKey(target))
        {
            
            if (GroundData[target].areaID!=-1)
            {
                Queue<FlowFieldCellData> cellsToCheck = new Queue<FlowFieldCellData>();
                FlowFieldCellData f = GroundData[target];
                f.finalcost = 0;
                cellsToCheck.Enqueue(f);

                while (cellsToCheck.Count > 0)
                {
                    FlowFieldCellData curCell = cellsToCheck.Dequeue();
                    List<FlowFieldCellData> curNeibors = Areas[f.areaID].GetGroundNeibor(curCell);
                    foreach (FlowFieldCellData n in curNeibors)
                    {
                        if (n.cost == -1) { continue; }
                        if (n.cost+curCell.finalcost<n.finalcost) 
                        {
                            n.finalcost = curCell.finalcost+n.cost;
                            cellsToCheck.Enqueue(n);
                        }
                    }
                }
                
                
            }
        }
    }

    
}
