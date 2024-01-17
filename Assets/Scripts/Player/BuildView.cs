using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

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

    Vector3 placepos;
    Vector3 LastPlacePos;

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
            placepos = World.GetWorldIndexFromPos(pos);

            if (World.Instance.CheckForVoxel(pos))
            {
                print("highlight:" + placepos.y);
                print("highlight:" + placepos.y * VoxelData.BlockSize);
                if (placepos.y > 0 || placepos.y < 21){
                    highlightBlock.position = placepos * VoxelData.BlockSize+new Vector3 (0.000001f, 0.000001f, 0.000001f);
                }
                else
                {
                    highlightBlock.position = placepos * VoxelData.BlockSize;
                }

                print("highlight:" + highlightBlock.position.y);
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;

            }

            if (placepos.y > 0 || placepos.y < 21)
            {
                lastPos = placepos * VoxelData.BlockSize + new Vector3(0.000001f, 0.000001f, 0.000001f); ;
            }
            else
            {
                lastPos = placepos * VoxelData.BlockSize;
            }

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
            SaveSystem.SaveWorld(world.worldData);
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

        // ����������Ƿ���
        if (Input.GetMouseButtonDown(0))
        {
            isMouseLeftClicked = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseLeftClicked = false;
        }

        // �ж��Ƿ�ͬʱ���㳤�� Ctrl ���͵��������������
        if (isCtrlPressed && isMouseLeftClicked)
        {
            print("highlight:" + highlightBlock.position.y);
            world.GetChunkFromPos(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
        }
        else if (isMouseLeftClicked)
        {
            print("place:" + placeBlock.position);
            world.GetChunkFromPos(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);
        }
    }


}
