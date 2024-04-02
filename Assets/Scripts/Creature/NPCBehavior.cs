using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehavior : MonoBehaviour
{

    public GameObject spawn;
    public GameObject canteen;
    public GameObject target;
    public bool isEating=true;
    public float satiety = 100f;
    NavMeshAgent NavMeshAgent;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (satiety<=0) 
        {
            target = canteen;
            NavMeshAgent.destination = target.transform.position;
        }
        if (satiety >=100&&isEating==true)
        {
            isEating = false;
            target = spawn;
            NavMeshAgent.destination = target.transform.position;
        }
        if (NavMeshAgent.pathPending && (target == canteen) && NavMeshAgent.remainingDistance < 0.5f&& isEating == false&& satiety <100)
        {
            isEating = true;
        }
    }
    private void FixedUpdate()
    {

        if (isEating == false&&satiety >=10&&(target==spawn)&& NavMeshAgent.remainingDistance < 1f)
        {
            satiety -= 10f;
            Debug.Log(satiety);
        }

       if (isEating == true && satiety <= 90&& (target == canteen) && NavMeshAgent.remainingDistance < 1f)
        {
            satiety += 10f;
            Debug.Log(satiety);
        }

    }

}
