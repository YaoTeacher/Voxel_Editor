using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public World world;

    public FlowField curFlowField;
    public GridDebug gridDebug;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

            if (isPress())
            {
                //Debug.Log("Ctrl + Click");
                InitializeFlowField();
            }

        
    }

    private void InitializeFlowField()
    {
        UnityEngine.Debug.Log($"{world.scenedata.Name}");
        curFlowField = new FlowField();
        curFlowField.GenerateGround(world,4);
        gridDebug.SetFlowField(curFlowField);
    }

    public bool isPress()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            return true;
        }
        else if( Input.GetKeyUp(KeyCode.V))
        {
            return false;
        }
        else { return false; }
    }
}
