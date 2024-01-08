using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : MonoBehaviour {

    public GameObject slotPrefab;
    World world;
    public BlockInfo ItemList;

    List<ItemSlot> slots = new List<ItemSlot>();
    int LastInList = 1;



    private void Start() {

        world = GameObject.Find("World").GetComponent<World>();
        UpdateList();



        

    }
    public void UpdateList()
    {
        
        if (slots.Count <= 0)
        {
            for (int i = LastInList; i < world.blocktype.BlockTypes.Length-1; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, transform);
                if (i < LastInList + 9)
                {

                    ItemStack stack = new ItemStack((byte)i, 64);
                    ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);
                    slot.isCreative = true;
                    slots.Add(slot);
                }
                else
                {

                    LastInList = LastInList + 8;
                    return;
                }


            }


        }
        else
        {
            for (int i = 0; i < 9; i++)
            {

                       LastInList = LastInList + 1;
                       if (LastInList > (world.blocktype.BlockTypes.Length - 1))
                       {
                        slots[i].stack.id = 0;
                        slots[i].stack.amount = 64;
                        slots[i].UpdateList();
                        
                       }
                       else
                       {
                        slots[i].stack.id = (byte)(LastInList);
                        slots[i].stack.amount = 64;
                        slots[i].UpdateList();
                       }


            }


        }

    }
    public void CheckFrontPage()
    {
        print(LastInList);
        if ((LastInList - 17) <1)
        {
            LastInList =(int)((Mathf.FloorToInt((float)(world.blocktype.BlockTypes.Length - 1) / 9)*9));
            print(LastInList);

        }
        else
        {
            LastInList = LastInList - 18;
            print(LastInList);
        }

        UpdateList();
    }
    public void CheckNextPage()
    {
        if (LastInList > (world.blocktype.BlockTypes.Length-1))
        {
            LastInList = 0;
            print(LastInList);

        }
        else
        {
            print(LastInList);
        }

        UpdateList();
    }

}

