using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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

