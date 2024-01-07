using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    GameObject Mode1;
    GameObject Mode2;
    GameObject UI1;
    GameObject UI2;
    byte modeID=0;

    private void Awake()
    {
        Mode1 = GameObject.Find("Build Mode");
        Mode2 = GameObject.Find("Third Mode");
        UI1 = GameObject.Find("BuildSystem");
        UI2 = GameObject.Find("PlayerSystem");

    }
    private void Start()
    {
        SetGameMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeMode();
        }
    }

    void SetGameMode()
    {
        if (modeID == 0)
        {
            Mode1.SetActive(true);
            Mode2.SetActive(false);
            UI1.SetActive(true);
            UI2.SetActive(false);

        }
        if (modeID == 1)
        {
            Mode1.SetActive(false);
            Mode2.SetActive(true);
            UI1.SetActive(false);
            UI2.SetActive(true);
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
            SetGameMode();
        
    }
}
