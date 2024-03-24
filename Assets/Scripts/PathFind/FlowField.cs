using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{

    public Dictionary<int, AreaData> Areas = new Dictionary<int, AreaData>(); 

    public Dictionary<Vector3Int, FlowFieldCellData> GroundData = new Dictionary<Vector3Int, FlowFieldCellData>();
    public Dictionary<Vector3Int, EnterPoint> EnterPointData = new Dictionary<Vector3Int, EnterPoint>();
    public Dictionary<Vector3Int, FlowFieldCellData> SpawnPointData = new Dictionary<Vector3Int, FlowFieldCellData>();
    public int changeTime = 0;
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
                   if (GroundData.ContainsKey(worldindex)&& GroundData[worldindex].areaID==-1)
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


                foreach (FlowFieldCellData n in Areas[GroundData[target].areaID].onGroundCell.Values)
                {

                    if (n.finalcost != 9999)
                    {
                        n.finalcost = 9999;
                    }

                }

                if (Areas[f.areaID].EnterPoints.Keys.Count+1> Areas[f.areaID].allowedNumberForEnterPoint)
                {
                    Debug.Log("Out Of Range! Clear!");
                    Debug.Log($"{EnterPointData.Count}");
                    foreach (EnterPoint e in Areas[f.areaID].EnterPoints.Values)
                    {
                        EnterPointData.Remove(e.WorldIndex);
                    };
                    Debug.Log($"{EnterPointData.Count}");
                    Areas[f.areaID].EnterPoints.Clear();
                }
                changeTime++;
                EnterPointData[f.WorldIndex] = new EnterPoint(f);
                Areas[f.areaID].EnterPoints[f.WorldIndex] = new EnterPoint(f);

                f.finalcost = 0;
                cellsToCheck.Enqueue(f);

                    while (cellsToCheck.Count > 0)
                    {
                        FlowFieldCellData curCell = cellsToCheck.Dequeue();

                        List<FlowFieldCellData> curNeibors = Areas[f.areaID].GetGroundNeibor(curCell);


                        foreach (FlowFieldCellData n in curNeibors)
                        {
                            if (n.cost == -1|| n.cost == 0) { continue; }


                            if (n.cost + curCell.finalcost < n.finalcost)
                            {
                                n.finalcost = curCell.finalcost + n.cost * CalculateCost(curCell, n);
                                n.direction = curCell.WorldIndex - n.WorldIndex;
                                n.direction = n.direction.normalized;

                                cellsToCheck.Enqueue(n);
                            }
                        }
                    }
   
               
                
                
            }
        }
    }

    public void SetSpawnPos(Vector3Int target)
    {
        if (GroundData.ContainsKey(target))
        {
            if (GroundData[target].areaID != -1)
            {
                if (!SpawnPointData.ContainsKey(target))
                {
                    SpawnPointData[target] = GroundData[target];
                }
                else
                {
                    SpawnPointData.Remove(target);
                }
            }
        }
    }

    public Vector3 CheckVector(Vector3 pos)
    {
        if (GroundData.ContainsKey(World.GetWorldIndexFromPos(pos)))
        {
            return GroundData[World.GetWorldIndexFromPos(pos)].direction;
        }
        else if (GroundData.ContainsKey(World.GetWorldIndexFromPos(pos)- new Vector3Int(0, 1, 0)))
        {
            return GroundData[World.GetWorldIndexFromPos(pos) - new Vector3Int(0, 1, 0)].direction;
        }
        else if (GroundData.ContainsKey(World.GetWorldIndexFromPos(pos) + new Vector3Int(0, 1, 0)))
        {
            return GroundData[World.GetWorldIndexFromPos(pos) + new Vector3Int(0, 1, 0)].direction;
        }
        else
        {
            Debug.Log("Stop");
            return new Vector3Int(0, 0, 0);
        }
    }

    public void GenerateFlowField(Vector3Int target)
    {
        if (GroundData.ContainsKey(target))
        {

            if (GroundData[target].areaID != -1)
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
                        if (n.cost + curCell.finalcost < n.finalcost)
                        {
                            n.finalcost = curCell.finalcost + n.cost;
                            cellsToCheck.Enqueue(n);
                        }
                    }
                }


            }
        }
    }

    private float CalculateCost(FlowFieldCellData node1, FlowFieldCellData node2)
    {
        //ȡ����ֵ
        int deltaX = node1.WorldIndex.x - node2.WorldIndex.x;
        if (deltaX < 0) deltaX = -deltaX;
        int deltaY = node1.WorldIndex.y - node2.WorldIndex.y;
        if (deltaY < 0) deltaY = -deltaY;
        int deltaZ = node1.WorldIndex.z - node2.WorldIndex.z;
        if (deltaZ < 0) deltaZ = -deltaZ;
        int delta = deltaX + deltaY +deltaZ;

        if (delta == 1)
        {
            return 1;
        }
        else if (delta == 2)
        {
            return 1.414f;
        }
        else
        {
            return 1.732f;
        }
    }

}
