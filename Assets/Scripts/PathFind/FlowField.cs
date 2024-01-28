using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{

    public Dictionary<Vector3Int, FlowFieldCellData> grid { get; private set; }

    public Dictionary<Vector3Int, FlowFieldCellData> GroundData = new Dictionary<Vector3Int, FlowFieldCellData>();

    public AreaData SetNewArea(Vector3Int firstpoint, Vector3Int lastpoint)
    {
        AreaData area = new AreaData(firstpoint, lastpoint);
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

    public void GenerateArea(World scene, AreaData areaData, int creature = 4)
    {
        for (int z = areaData.LessBorderPoint.z; z <= areaData.BiggerBorderPoint.z; z++)
        {

            for (int x = areaData.LessBorderPoint.x; x < areaData.BiggerBorderPoint.x; x++)
            {

                for (int y = areaData.LessBorderPoint.y; y < areaData.BiggerBorderPoint.y; y++)
                {
                    Vector3Int point = new Vector3Int(x, y, z);
                    Vector3Int uppoint = point;

                    for (int c = 0; c <= creature; c++)
                    {
                        uppoint += new Vector3Int(0, 1, 0);
                        if (scene.IsGround(point, uppoint))
                        {
                            GroundData[point] = new FlowFieldCellData(point, 1f);
                            if (y + c <= areaData.BiggerBorderPoint.y)
                            {
                                y += c - 1;
                            }
                            else y = areaData.BiggerBorderPoint.y - 1;
                        }

                    }
                }
            }
        }
    }

    public List<Vector3Int> GetGroundNeibor(FlowFieldCellData path)
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

                    if (x == 1 && z == 1)
                    {
                        if (GroundData.ContainsKey(path.WorldIndex + new Vector3Int(1, y, 0)) && GroundData.ContainsKey(path.WorldIndex + new Vector3Int(0, y, 1)))
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
