using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{

    public Vector3 target;
    public int nowArea;
    public int ArriveTime = 0;

    public Queue<AreaData> AreatoMove = new Queue<AreaData>();


    void Update()
    {
        if (transform.position == target)
        {
            ArriveTime++;
        }
        if (ArriveTime >= 60)
        {
            Destroy(gameObject);
        }
    }

    public void GetAreasList()
    {
        Vector3Int targetIndex = World.GetWorldIndexFromPos(target);
        Vector3Int position = World.GetWorldIndexFromPos(transform.position);

    }

}
