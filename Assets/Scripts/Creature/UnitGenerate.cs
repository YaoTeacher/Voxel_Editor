using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitGenerate : MonoBehaviour
{
    public GameObject spawn;
    public GameObject canteen;
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
    }

    private void SpawnUnits()
    {
            print("Start!");
            int colMask = LayerMask.GetMask("Impassible", "Units");
            for (int i = 0; i < numUnitsPerSpawn; i++)
            {
            Transform position = spawn.transform;

            do
            {

                        
                  GameObject newUnit = Instantiate(unitPrefab, position);
                  newUnit.transform.parent = transform;
                  newUnit.GetComponent<NPCBehavior>().canteen =canteen;
                  newUnit.GetComponent<NPCBehavior>().spawn =spawn;
                  unitsInGame.Add(newUnit);
                        //newUnit.target = world.curFlowField.Areas[newPos.areaID].EnterPoints.Keys.ToList<Vector3Int>()[0];
                   print("Spawn");


            }
                while (Physics.OverlapSphere(position.position, 0.25f, colMask).Length > 0);
            }


    }

    private void DestroyUnits()
    {
        foreach (GameObject g in unitsInGame)
        {
            Destroy(g);
        }
        unitsInGame.Clear();
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
