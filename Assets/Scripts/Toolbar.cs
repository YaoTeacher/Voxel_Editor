using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    World world ;
    public BuildView player;

    public RectTransform highlight;
    public ItemSlot[] Itemslots;

    int slotIndex = 0;
    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        foreach (var s in Itemslots)
        {
            s.icon.sprite = world.BlockTypes[s.itemID].icon;
            s.icon.enabled = true;
        }
    }
    private void Update()
    {

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {

            if (scroll > 0)
                slotIndex--;
            else
                slotIndex++;

            if (slotIndex > Itemslots.Length - 1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = Itemslots.Length - 1;

            highlight.position = Itemslots[slotIndex].icon.transform.position;
            player.selectedBlockIndex = Itemslots[slotIndex].itemID;

        }


    }


}

[System.Serializable]
public class ItemSlot
{
    public byte itemID;
    public Image icon;
}
