using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildView : MonoBehaviour
{
    private Camera cam;


    private World world;

    public float walkSpeed = 3f;

    public float boundsTolerance = 0.1f;


    public Transform highlightBlock;
    public Transform placeBlock;

    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1;

    Vector3Int DestroyIndex =new Vector3Int();
    Vector3Int BuildIndex=new Vector3Int();

    private void Start()
    {
        cam = GameObject.Find("Build Mode Camera").GetComponent<Camera>();
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Confined;
        //selectedBlockText.text = "Null selected";
    }
    private void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GetPlayerInputs();
            placeCursorBlocks();

            

        }
        else
        {
            highlightBlock.gameObject.SetActive(false);
            placeBlock.gameObject.SetActive(false);
        }

    }


    private void placeCursorBlocks()
    {
        float step = checkIncrement* VoxelData.BlockSize;
        Vector3 lastPos = new Vector3();
        
        while (step < reach)
        {
            Vector3 dir = cam.ScreenPointToRay(Input.mousePosition).direction;
            Vector3 pos = cam.transform.position + (dir * step);
            Vector3Int PlaceIndex = World.GetWorldIndexFromPos(pos);

            if (World.Instance.CheckForVoxel(PlaceIndex))
            {

                highlightBlock.position = new Vector3(PlaceIndex.x * VoxelData.BlockSize, PlaceIndex.y * VoxelData.BlockSize, PlaceIndex.z * VoxelData.BlockSize);
                DestroyIndex = PlaceIndex;
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;

            }


            lastPos = new Vector3(PlaceIndex.x * VoxelData.BlockSize, PlaceIndex.y * VoxelData.BlockSize, PlaceIndex.z * VoxelData.BlockSize);
            BuildIndex = PlaceIndex;
            step += checkIncrement;

        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    private void GetPlayerInputs()
    {


        if (highlightBlock.gameObject.activeSelf)
        {
            BuildBlock(); 
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            WorldDataManager.SaveWorld();
        }
    }

    void BuildBlock()
    {
        bool isCtrlPressed = false;
        bool isMouseLeftClicked = false;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            isCtrlPressed = true;
        }
        else
        {
            isCtrlPressed = false;
        }

        // 检测鼠标左键是否点击
        if (Input.GetMouseButtonDown(0))
        {
            isMouseLeftClicked = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseLeftClicked = false;
        }

        // 判断是否同时满足长按 Ctrl 键和点击鼠标左键的条件
        if (isCtrlPressed && isMouseLeftClicked)
        {
            print("highlight:" + highlightBlock.position.y);
            world.GetChunkFromPos(highlightBlock.position).EditVoxel(DestroyIndex, 0);
        }
        else if (isMouseLeftClicked)
        {
            print("place:" + placeBlock.position);
            world.GetChunkFromPos(placeBlock.position).EditVoxel(BuildIndex, selectedBlockIndex);
        }
    }


}
