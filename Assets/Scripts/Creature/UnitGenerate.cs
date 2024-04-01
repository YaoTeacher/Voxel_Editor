using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitGenerate : MonoBehaviour
{
    
    public World world;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;
    public Vector3 nowMoveDirection;

    private List<GameObject> unitsInGame;

    private void Awake()
    {
        unitsInGame = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnUnits();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            DestroyUnits();
        }
    }

    private void FixedUpdate()
    {
        //if (world.curFlowField == null) { return; }
        //foreach (GameObject unit in unitsInGame)
        //{
        //    nowMoveDirection = world.curFlowField.CheckVector(unit.transform.position);
        //    unit.transform.position += nowMoveDirection * moveSpeed;
        //}
    }

    private void SpawnUnits()
    {
        //if (world.curFlowField.SpawnPointData.Count > 0)
        //{
        //    print("Start!");
        //    int colMask = LayerMask.GetMask("Impassible", "Units");
        //    GroundCellData newPos;
        //    for (int i = 0; i < numUnitsPerSpawn; i++)
        //    {

        //        do
        //        {
        //            newPos = world.curFlowField.SpawnPointData.Values.ToList<GroundCellData>()[Random.Range(0, world.curFlowField.SpawnPointData.Values.Count)];

        //            if (newPos.areaID != -1)
        //            {
        //                Vector3 position = new Vector3(newPos.WorldIndex.x * VoxelData.BlockSize + 0.25f, newPos.WorldIndex.y * VoxelData.BlockSize+0.5f, newPos.WorldIndex.z * VoxelData.BlockSize + 0.25f);
        //                GameObject newUnit = Instantiate(unitPrefab, position, Quaternion.LookRotation(newPos.direction));
        //                newUnit.transform.parent = transform;
                        
        //                unitsInGame.Add(newUnit);
        //                //newUnit.target = world.curFlowField.Areas[newPos.areaID].EnterPoints.Keys.ToList<Vector3Int>()[0];
        //                print("Spawn");
        //            }


        //        }
        //        while (Physics.OverlapSphere(new Vector3(newPos.WorldIndex.x * VoxelData.BlockSize, newPos.WorldIndex.y * VoxelData.BlockSize, newPos.WorldIndex.z * VoxelData.BlockSize), 0.25f, colMask).Length > 0);
        //    }

        //}
        
    }

    private void DestroyUnits()
    {
        foreach (GameObject g in unitsInGame)
        {
            Destroy(g);
        }
        unitsInGame.Clear();
    }

    public void GetAreasList(Vector3 target,GameObject newUnit)
    {
        //Vector3Int targetIndex = World.GetWorldIndexFromPos(target);
        //Vector3Int position = World.GetWorldIndexFromPos(newUnit.transform.position);

        //int areaID = world.curFlowField.GroundData[position].areaID;
        //int targetAreaID = world.curFlowField.GroundData[targetIndex].areaID;

        //Queue<AreaData> areasToCheck = new Queue<AreaData>();
        //AreaData area = world.curFlowField.Areas[areaID];
        //areasToCheck.Enqueue(area);
        //while (areasToCheck.Count > 0)
        //{
        //    AreaData curArea = areasToCheck.Dequeue();

        //    List<int> curNeibors = curArea.neiborAreas.Keys.ToList();


        //    foreach (int n in curNeibors)
        //    {
        //        AreaData neibor = world.curFlowField.Areas[n];
        //        if (neibor==null) { continue; }

        //    }
        //}

    }
}

public class CreatureStateData:BaseData
{
    public int scenceId;
    public int areaId;
    public Vector3Int targetBlock;
    public Vector3 position;
    public Vector3 nowMoveDirection;

}
