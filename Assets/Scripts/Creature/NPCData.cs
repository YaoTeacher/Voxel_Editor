using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NPCData : MonoBehaviour
{

    public Vector3 target;
    public int ArriveTime = 0;

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
}
