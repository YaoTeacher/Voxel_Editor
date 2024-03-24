using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicBehavior : MonoBehaviour
{
    
    public World world;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;

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
        if (world.curFlowField == null) { return; }
        foreach (GameObject unit in unitsInGame)
        {
            Vector3 moveDirection = world.curFlowField.CheckVector(unit.transform.position);
            unit.transform.position += moveDirection * moveSpeed;
        }
    }

    private void SpawnUnits()
    {
        if (world.curFlowField.SpawnPointData.Count > 0)
        {
            print("Start!");
            int colMask = LayerMask.GetMask("Impassible", "Units");
            FlowFieldCellData newPos;
            for (int i = 0; i < numUnitsPerSpawn; i++)
            {

                do
                {
                    newPos = world.curFlowField.SpawnPointData.Values.ToList<FlowFieldCellData>()[Random.Range(0, world.curFlowField.SpawnPointData.Values.Count)];

                    if (newPos.areaID != -1)
                    {
                        Vector3 position = new Vector3(newPos.WorldIndex.x * VoxelData.BlockSize + 0.25f, newPos.WorldIndex.y * VoxelData.BlockSize+0.5f, newPos.WorldIndex.z * VoxelData.BlockSize + 0.25f);
                        GameObject newUnit = Instantiate(unitPrefab, position, Quaternion.LookRotation(newPos.direction));
                        newUnit.transform.parent = transform;
                        unitsInGame.Add(newUnit);
                        //newUnit.target = world.curFlowField.Areas[newPos.areaID].EnterPoints.Keys.ToList<Vector3Int>()[0];
                        print("Spawn");
                    }


                }
                while (Physics.OverlapSphere(new Vector3(newPos.WorldIndex.x * VoxelData.BlockSize, newPos.WorldIndex.y * VoxelData.BlockSize, newPos.WorldIndex.z * VoxelData.BlockSize), 0.25f, colMask).Length > 0);
            }

        }
        
    }

    private void DestroyUnits()
    {
        foreach (GameObject go in unitsInGame)
        {
            Destroy(go);
        }
        unitsInGame.Clear();
    }
}
