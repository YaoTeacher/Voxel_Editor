using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class NavMashManager : MonoBehaviour
{
    public Dictionary<int,RegionData>Reigions = new Dictionary<int,RegionData>();
    public Dictionary<int,AreaData>Areas = new Dictionary<int,AreaData>();
    public Dictionary<int,GameObject>RegionsManage = new Dictionary<int,GameObject>();
    public Dictionary<int, GameObject> AreasManage = new Dictionary<int, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        GenerateWorldAreas();
        GenerateWorldRegions();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateWorldRegions()
    {
        foreach (RegionData r in Reigions.Values)
        {
            GameObject g = new GameObject();
            if (gameObject.transform.Find("/Regions/" + $"{r.Id}"))
            {
                g = gameObject.transform.Find($"{r.Id}").gameObject;

            }
            else
            {
                g.transform.SetParent(gameObject.transform.GetChild(0));

            }
            g.GetOrAddComponent<NavMeshSurface>();
            g.GetComponent<NavMeshSurface>().buildHeightMesh = true;
            g.GetComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;
            g.GetComponent<NavMeshSurface>().center = new Vector3(r.centerIndexPointX, VoxelData.ChunkHeight * VoxelData.BlockSize / 2, r.centerIndexPointZ);
            g.GetComponent<NavMeshSurface>().size = new Vector3(r.VoxelLengthX, VoxelData.ChunkHeight * VoxelData.BlockSize / 2, r.VoxelLengthZ);
            g.GetComponent<NavMeshSurface>().voxelSize = 0.02f;
            g.GetComponent<NavMeshSurface>().tileSize = 128;
            g.GetComponent<NavMeshSurface>().agentTypeID = 0;
            g.GetComponent<NavMeshSurface>().BuildNavMesh();


        }
    }



    void GenerateWorldAreas()
    {
        foreach (AreaData a in Areas.Values)
        {
            GameObject g = new GameObject();
            if (gameObject.transform.Find("/Areas/"+
                $"{a.Id}"))
                
            {
                g = gameObject.transform.Find("/Areas/" +
                $"{a.Id}").gameObject;

            }
            else
            {
                g.transform.SetParent(gameObject.transform.GetChild(1));

            }
            g.GetOrAddComponent<NavMeshModifierVolume>();
            g.GetComponent<NavMeshModifierVolume>().center = new Vector3(a.centerIndexPointX, a.centerIndexPointY, a.centerIndexPointZ);
            g.GetComponent<NavMeshModifierVolume>().size = new Vector3(a.VoxelLengthX, a.VoxelLengthY, a.VoxelLengthZ);
            g.GetComponent<NavMeshModifierVolume>().area= 4;


        }
    }
}
