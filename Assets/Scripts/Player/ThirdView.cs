using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdView : MonoBehaviour
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
        cam = GameObject.Find("Third Mode Camera").GetComponent<Camera>();
        camTrans = cam.transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Confined;
        //selectedBlockText.text = "Null selected";
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {

            world.inUI = !world.inUI;

        }

        if (!world.inUI)
        {
            GetPlayerInputs();
            placeCursorBlocks();
        }


    }

    private void placeCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();
        while (step < reach)
        {
            Vector3 dir = cam.ScreenPointToRay(Input.mousePosition).direction;
            Vector3 pos = cam.transform.position + (dir * step);
            Vector3 placepos = pos / VoxelData.BlockSize;

            if (world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(placepos.x), Mathf.FloorToInt(placepos.y), Mathf.FloorToInt(placepos.z)) * VoxelData.BlockSize;
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;

            }

            lastPos = new Vector3(Mathf.FloorToInt(placepos.x), Mathf.FloorToInt(placepos.y), Mathf.FloorToInt(placepos.z)) * VoxelData.BlockSize;

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
                if (selectedBlockIndex > world.blocktype.BlockTypes.Length - 1)
                {
                    selectedBlockIndex = 1;
                }
            }
            else
            {
                selectedBlockIndex--;
                if (selectedBlockIndex < 1)
                {
                    selectedBlockIndex = (byte)(world.blocktype.BlockTypes.Length - 1);
                }
            }
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
