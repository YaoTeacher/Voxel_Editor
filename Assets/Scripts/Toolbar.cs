using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    World world ;
    public BuildView player;
    public UIItemSlot[] slots;
    public RectTransform highlight;
    public BarList[] Itemslots;

    int slotIndex = 0;
    private void Start()
    {

        byte index = 1;
        foreach (UIItemSlot s in slots)
        {

            ItemStack stack = new ItemStack(index, Random.Range(2, 65));
            ItemSlot slot = new ItemSlot(s, stack);
            index++;

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
public class BarList
{
    public byte itemID;
    public Image icon;
}
