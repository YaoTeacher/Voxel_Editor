using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.LightAnchor;

public class Player : MonoBehaviour
{
    GameObject Mode1;
    GameObject Mode2;
    GameObject UI1;
    GameObject UI2;
    byte modeID=0;

    private Transform camTrans;
    private Camera cam1;
    private Camera cam2;

    public int orientation;
    public float walkSpeed = 3f;

    public float boundsTolerance = 0.1f;

    private float horizontal;
    private float vertical;
    private float Height;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;

    private void Awake()
    {
        Mode1 = GameObject.Find("Build Mode");
        Mode2 = GameObject.Find("Third Mode");
        UI1 = GameObject.Find("BuildSystem");
        UI2 = GameObject.Find("PlayerSystem");
        cam1 = GameObject.Find("Build Mode Camera").GetComponent<Camera>();
        cam2 = GameObject.Find("Third Mode Camera").GetComponent<Camera>();
        camTrans = cam1.transform;

    }
    private void Start()
    {
        modeID = 0;
        ChangeGameMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeMode();
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GetPlayerInputs();

        }
        Vector3 topDirection = Vector3.up; // 定义顶部方向为(0, 1, 0)
        Vector3 cameraDirection = Camera.main.transform.forward; // 获取摄像机的视线方向
        Vector3 XZDirection = transform.forward;
            XZDirection.y = 0;
        // 判断摄像机视线是否在方块顶部45度视锥内
        if (Vector3.Angle(cameraDirection, topDirection) > 45f)
        {
            // 在45度视锥外,使用12~15特殊角度值
            if (Vector3.Angle(XZDirection, Vector3.back) <= 45)
                orientation = 13;
            else if (Vector3.Angle(XZDirection, Vector3.forward) <= 45)
                orientation = 12;
            else if (Vector3.Angle(XZDirection, Vector3.left) <= 45)
                orientation = 14;
            else if (Vector3.Angle(XZDirection, Vector3.right) <= 45)
                orientation = 15;
        }
        else
        {
            // 在45度视锥内,使用原有的2~5转向判断
            if (Vector3.Angle(XZDirection, Vector3.forward) <= 45)
                orientation = 3;
            else if (Vector3.Angle(XZDirection, Vector3.right) <= 45)
                orientation = 5;
            else if (Vector3.Angle(XZDirection, Vector3.back) <= 45)
                orientation = 2;
            else
                orientation = 4;
        }

    }

    void ChangeGameMode()
    {
        if (modeID == 0)
        {
            SetGameMode1(true);
            SetGameMode2(false);

        }
        if (modeID == 1)
        {
            SetGameMode1(false);
            SetGameMode2(true);

        }
    }

    void SetGameMode1(bool setmode)
    {
        Mode1.SetActive(setmode);
        UI1.SetActive(setmode);
        cam1.gameObject.SetActive(setmode);
        camTrans = cam1.transform;
        if (setmode == true)
        {
            transform.position = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.Euler(new Vector3(0,0, 0));
        
        }
    }

    void SetGameMode2(bool setmode)
    {
        Mode2.SetActive(setmode);
        UI2.SetActive(setmode);
        cam2.gameObject.SetActive(setmode);
        camTrans = cam2.transform;
        if (setmode == true)
        {
            transform.position = new Vector3(0,0,0);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    void ChangeMode()
    {
            Debug.Log("Change mode!");
            modeID++;
            if (modeID >= 2)
            {
                modeID = 0;
            }
            ChangeGameMode();
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

