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
    public bool isNeedToEat=true;
    public float satiety = 100f;
    public NavMeshAgent NavMeshAgent;

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
        if (satiety >=100&&isNeedToEat==true)
        {
            isNeedToEat = false;
            target = spawn;
            NavMeshAgent.destination = target.transform.position;
        }
        if (!NavMeshAgent.pathPending && NavMeshAgent.remainingDistance < 0.5f&& isNeedToEat == false&& satiety >= 100)
        {
            isNeedToEat = true;
        }
    }
    private void FixedUpdate()
    {

        if (isNeedToEat == true&&satiety >=10&&(target==spawn||target==null))
        {
            satiety -= 10f;
            Debug.Log(satiety);
        }

       if (isNeedToEat == true && satiety <= 90&& !NavMeshAgent.pathPending&& target == canteen && NavMeshAgent.remainingDistance < 0.5f)
        {
            satiety += 10f;
            Debug.Log(satiety);
        }

    }

}
