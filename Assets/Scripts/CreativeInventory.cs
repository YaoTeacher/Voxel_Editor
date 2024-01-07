using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public BlockInfo ItemList;

    List<ItemSlot> slots = new List<ItemSlot>();
    int FirstInList = 1;



    private void Start() {

        world = GameObject.Find("World").GetComponent<World>();
        UpdateList();



        

    }
    public void UpdateList()
    {
        
        if (slots.Count <= 0)
        {
            for (int i = FirstInList; i < world.blocktype.BlockTypes.Length; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, transform);
                if (i < FirstInList + 9)
                {

                    ItemStack stack = new ItemStack((byte)i, 64);
                    ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);
                    slot.isCreative = true;
                    slots.Add(slot);
                }
                else
                {
                    FirstInList = i;
                    return;
                }


            }

        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < 9)
                {
                    if (slots[i].stack.id >= (world.blocktype.BlockTypes.Length - 1))
                    {

                        slots[i].stack.id = 0;
                        slots[i].UpdateList();
                        print("Update1!");
                    }
                    else
                    {
                        slots[i].stack.id= (byte)(i +FirstInList);
                        slots[i].UpdateList();
                        print("Update2!");
                    }
                }
                else
                {
                    FirstInList = i;
                    return;
                }


            }
        }

    }
    public void CheckFrontPage()
    {
        if (FirstInList - 9 <= 0)
        {
            FirstInList = Mathf.FloorToInt((float)(world.blocktype.BlockTypes.Length - 1) / 9)*9+1;
            print("renew1!");
        }
        UpdateList();
    }
    public void CheckNextPage()
    {
        if (FirstInList >= (world.blocktype.BlockTypes.Length-1))
        {
            FirstInList = 1;
            print("renew2!");
        }
        UpdateList();
    }

}

