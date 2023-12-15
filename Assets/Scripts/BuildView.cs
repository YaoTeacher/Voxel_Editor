using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class BuildView : MonoBehaviour
{
    private Transform camTrans;
    private Camera cam;

    private World world;

    public float walkSpeed = 3f;

    public float boundsTolerance = 0.1f;

    private float horizontal;
    private float vertical;
    private float Height;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;

    public Transform highlightBlock;
    public Transform placeBlock;

    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1;

    private void Start()
    {
        cam = GameObject.Find("Build Mode Camera").GetComponent<Camera>();
        camTrans = cam.transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Confined;
        //selectedBlockText.text = "Null selected";
    }
    private void Update()
    {
        GetPlayerInputs();
        placeCursorBlocks();
    }


    private void placeCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();
        while (step < reach)
        {
            Vector3 dir = cam.ScreenPointToRay(Input.mousePosition).direction;
            Vector3 pos = cam.transform.position + (dir * step);

            if (world.CheckVoxelSolid(pos))
            {

                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;

            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            step += checkIncrement;

        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");


        velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * 3f;

        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Vector3.up * mouseHorizontal);
            camTrans.Rotate(Vector3.right * -mouseVertical);

        }
        transform.Translate(velocity, Space.World);


        Trans_Screen_Y();

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
            {
                selectedBlockIndex++;
                if (selectedBlockIndex > world.BlockTypes.Length - 1)
                {
                    selectedBlockIndex = 1;
                }
            }
            else
            {
                selectedBlockIndex--;
                if (selectedBlockIndex < 1)
                {
                    selectedBlockIndex = (byte)(world.BlockTypes.Length - 1);
                }
            }
        }

        if (highlightBlock.gameObject.activeSelf)
        {
            BuildBlock(); 
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
            print("highlight:" + highlightBlock.position);
            world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
        }
        else if (isMouseLeftClicked)
        {
            print("place:" + placeBlock.position);
            world.GetChunkFromVector3(highlightBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);
        }
    }

    void Trans_Screen_Y()
    {
        if (Input.GetKeyDown(KeyCode.Z))//  && transform.position.y <= 30
        {
            Debug.Log("rise");
            transform.Translate(new Vector3(0, 5, 0));

        }

        if (Input.GetKeyDown(KeyCode.X))//&& transform.position.y >= -30
        {
            Debug.Log("decline");
            transform.Translate(new Vector3(0, -5, 0));
        }
    }


}
