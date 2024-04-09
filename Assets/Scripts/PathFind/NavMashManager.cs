using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteAlways]
public class NavMashManager : MonoBehaviour
{
    public World World;
    public Dictionary<int, RegionData> Regions = new Dictionary<int, RegionData>()
    { {0,new RegionData(new Vector3Int(0,0,0),new Vector3Int(VoxelData.ChunkWidth*VoxelData.WorldChunksSize-1,0,VoxelData.ChunkWidth*VoxelData.WorldChunksSize-1),0) }
    };
    public Dictionary<int, AreaData> Areas = new Dictionary<int, AreaData>()
    {{0,new AreaData("EditArea",new Vector3Int(0,0,0),new Vector3Int(VoxelData.ChunkWidth*VoxelData.WorldChunksSize-1,0,VoxelData.ChunkWidth*VoxelData.WorldChunksSize-1),0)
    } };
    public List<CellData> TransparentBlock = new List<CellData>();
    public Dictionary<int, GameObject> RegionsManager = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> AreasManager = new Dictionary<int, GameObject>();
    List<GameObject> regionsToUpdate = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorldAreas();
        GenerateWorldRegions();
        updateNavmeshModifier();

    }

    // Update is called once per frame
    void Update()
    {
        if (regionsToUpdate.Count > 0 && World.Instance.isGenerateFinished == true)
        {
            foreach (GameObject g in regionsToUpdate)
            {
                g.GetComponent<NavMeshSurface>().UpdateNavMesh(g.GetComponent<NavMeshSurface>().navMeshData);
            }
            World.Instance.isGenerateFinished = false;
        }
    }

    void updateNavmeshModifier()
    {
        if (gameObject.GetComponent<NavMeshModifier>() == null)
        {
            gameObject.AddComponent<NavMeshModifier>();
        }
        gameObject.GetComponent<NavMeshModifier>().applyToChildren = true;
        gameObject.GetComponent<NavMeshModifier>().overrideGenerateLinks = true;
        gameObject.GetComponent<NavMeshModifier>().generateLinks = true;
    }
    void GenerateWorldRegions()
    {
        foreach (RegionData r in Regions.Values)
        {
            GameObject g;

            if (!System.Object.ReferenceEquals(gameObject.transform.Find("Regions/" + $"region_{r.Id}"), null))
            {
                g = gameObject.transform.Find("Regions/" + $"region_{r.Id}").gameObject;

            }
            else
            {
                g = new GameObject();
                g.name = $"region_{r.Id}";
                g.transform.SetParent(gameObject.transform.GetChild(0));

            }
            g.AddComponent<NavMeshSurface>();
            g.GetComponent<NavMeshSurface>().buildHeightMesh = true;
            g.GetComponent<NavMeshSurface>().overrideTileSize = true;
            g.GetComponent<NavMeshSurface>().overrideVoxelSize = true;
            g.GetComponent<NavMeshSurface>().defaultArea = 0;
            g.GetComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;
            g.GetComponent<NavMeshSurface>().center = new Vector3(r.centerIndexPointX, VoxelData.ChunkHeight * VoxelData.BlockSize / 2, r.centerIndexPointZ);
            g.GetComponent<NavMeshSurface>().size = new Vector3(r.VoxelLengthX, VoxelData.ChunkHeight * VoxelData.BlockSize / 2, r.VoxelLengthZ);
            g.GetComponent<NavMeshSurface>().voxelSize = 0.02f;
            g.GetComponent<NavMeshSurface>().tileSize = 128;
            g.GetComponent<NavMeshSurface>().agentTypeID = 0;

            g.GetComponent<NavMeshSurface>().BuildNavMesh();
            RegionsManager[r.Id] = g;


        }
    }



    void GenerateWorldAreas()
    {
        foreach (AreaData a in Areas.Values)
        {
            GameObject g = new GameObject();
            if (gameObject.transform.Find("Areas/" +
                $"Area_{a.Id}"))

            {
                g = gameObject.transform.Find("Areas/" +
                $"Area_{a.Id}").gameObject;

            }
            else
            {
                g.name = $"Area_{a.Id}";
                g.transform.SetParent(gameObject.transform.GetChild(1));

            }

            if (a.type >= 1)
            {
                g.GetOrAddComponent<NavMeshModifierVolume>();
                g.GetComponent<NavMeshModifierVolume>().center = new Vector3(a.centerIndexPointX, a.centerIndexPointY, a.centerIndexPointZ);
                g.GetComponent<NavMeshModifierVolume>().size = new Vector3(a.VoxelLengthX, a.VoxelLengthY, a.VoxelLengthZ);
                g.GetComponent<NavMeshModifierVolume>().area = 4;
                g.GetComponent<NavMeshModifierVolume>().enabled = true;
                AreasManager[a.Id] = g;
            }
            else
            {
                continue;
            }


        }
    }

    void indoorAreaCheck()
    {

            int areaIndex = 1;
            Vector3Int lessPoint = new Vector3Int(0, 0, 0);
            Vector3Int greaterPoint = new Vector3Int(0, 0, 0);
            AreaData area;  
            if (Areas[0].name != "EditArea")
            {
            Areas[0] = new AreaData("EditArea", new Vector3Int(0, 0, 0), new Vector3Int(VoxelData.ChunkWidth * VoxelData.WorldChunksSize - 1, 0, VoxelData.ChunkWidth * VoxelData.WorldChunksSize - 1), 0);
            }
            

            Queue<CellData> cellsToCheck = new Queue<CellData>();
            CellData f = TransparentBlock[0];


            foreach (CellData n in TransparentBlock)
            {

                    if (n.areaID!= 0)
                    {
                        n. areaID= 0;
                    }

            }

        //if (Areas[f.areaID].innerEnterPoints.Keys.Count + 1 > Areas[f.areaID].allowedNumberForEnterPoint)
        //{
        //    Debug.Log("Out Of Range! Clear!");
        //    Debug.Log($"{EnterPointData.Count}");
        //    foreach (TargetPoint e in Areas[f.areaID].innerEnterPoints.Values)
        //    {
        //        EnterPointData.Remove(e.WorldIndex);
        //    };
        //    Debug.Log($"{EnterPointData.Count}");
        //    Areas[f.areaID].innerEnterPoints.Clear();
        //}
        //changeTime++;
        lessPoint = f.WorldIndex;
        greaterPoint = f.WorldIndex;
        cellsToCheck.Enqueue(f);

                while (cellsToCheck.Count > 0)
                {
                    CellData curCell = cellsToCheck.Dequeue();

                    List<CellData> curNeibors = Areas[f.areaID].GetTransparentNeibor(curCell.WorldIndex);


                    foreach (CellData n in curNeibors)
                    {
                        if (n.areaID!=0) { continue; }

                        n.
                        //if (n.cost + curCell.finalcost < n.finalcost)
                        //{
                        //    n.finalcost = curCell.finalcost + n.cost * CalculateCost(curCell, n);
                        //    n.direction = curCell.WorldIndex - n.WorldIndex;
                        //    n.direction = n.direction.normalized;

                            cellsToCheck.Enqueue(n);
                        }
                    }
                }
            
    }
}
