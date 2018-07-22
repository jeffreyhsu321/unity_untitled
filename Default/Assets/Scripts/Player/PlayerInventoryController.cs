using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
/// <summary>
/// Controls inventory list and inventory window
/// </summary>
public class PlayerInventoryController : MonoBehaviour 
{

    //inventory window
    public List<GameObject> inventoryList;
    public GameObject inventoryWindow;
    public GameObject itemWindow;

    bool prevActive;    //previous active state of item

    public int maxNumOfSlots;
    int numOfSlots;
    int numOfSlotsFree;

    string itemName;

    Transform slot; //the slot of each item in the inventory window

    Transform attr; //the attribute portion of item window

    public GameObject slotTransit;
    public GameObject pu_blank;

    //equipment window
    public PlayerEquipmentController equipmentController;

    [HideInInspector]

    //references to item to enlarge (note, itemSelected is actual object with attr)
    public int itemShowNum;
    public GameObject itemShow;

    //references to the Image representation of itemSelected in inventory window
    public GameObject itemSelectedImg;

    //references to the Slot in Grid of Inventory Window
    public int slotSelectedNum;
    public GameObject slotSelected;

    //references to the original Slot the itemSelectedImg resides in
    public int slotOriginalNum;
    public GameObject slotOriginal;
    
    //if false then allow grab
    public bool isGrabbing = false;

    public bool equipMode = false;



    void Start()
    {
        //numOfSlots = checkNumOfSlots();
        numOfSlots = maxNumOfSlots;
        for (int i = 0; i < numOfSlots; i++) {
            inventoryList.Add(pu_blank);
            numOfSlotsFree++;
        }
    }


    /// <summary>
    /// drag and drop detection and control
    /// </summary>
    public void Update()
    {
        //only check for pointer actions if InventoryWindow is active
        if (inventoryWindow.active)
        {

            //on mouse down: drag
            if (Input.GetMouseButton(0) && itemSelectedImg != null)
            {
                float x, y;

                //deactivate item window
                itemWindow.SetActive(false);

                //reference to selected object (itemSelectedImg) is done in SlotItem.OnPointerEnter(PointerEventData);

                //convert mousePosition from Vector2 to Vector3
                Vector3 pointerPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

                //retrive new x y (offset/not centered)
                x = Camera.main.ScreenToWorldPoint(pointerPos).x;
                y = Camera.main.ScreenToWorldPoint(pointerPos).y;

                //transform the position of Image after centering it
                itemSelectedImg.transform.position = new Vector2(x, y);
            }

            //on mouse up: return to slot or switch slots
            if (Input.GetMouseButtonUp(0) && isGrabbing)
            {
                //is not grabbing anymore
                isGrabbing = false;

                //return back to slot

                //retrive width and height of Image
                float width, height;
                width = itemSelectedImg.GetComponent<RectTransform>().rect.width;    // * itemSelectedImg.GetComponent<RectTransform>().lossyScale.x;
                height = itemSelectedImg.GetComponent<RectTransform>().rect.height;  // * itemSelectedImg.GetComponent<RectTransform>().lossyScale.y;

                if (slotSelected == null || slotSelected == slotOriginal)
                {
                    Debug.Log(0);
                    //return the Image back to the original slot
                    itemSelectedImg.transform.SetParent(slotOriginal.transform);
                    itemSelectedImg.transform.SetSiblingIndex(0);

                    //return the Image back to the Slot
                    itemSelectedImg.transform.localPosition = new Vector3(width / 2, -height / 2, 0);
                }
                else
                {
                    //switching slots
                    //visual
                    slotSelected.transform.Find("Image").transform.SetParent(slotOriginal.transform);
                    slotOriginal.transform.Find("Image").transform.SetSiblingIndex(0);

                    itemSelectedImg.transform.SetParent(slotSelected.transform);
                    slotSelected.transform.Find("Image").transform.SetSiblingIndex(0);

                    slotOriginal.transform.Find("Image").transform.localPosition = new Vector3(width / 2, -height / 2, 0);

                    //return the Image back to the Slot
                    itemSelectedImg.transform.localPosition = new Vector3(width / 2, -height / 2, 0);

                    //interal
                    if (!equipMode)
                    {
                        Debug.Log(1);
                        //manipulate inventoryList to reflect change
                        GameObject tmp = inventoryList[slotSelectedNum];
                        inventoryList[slotSelectedNum] = inventoryList[slotOriginalNum];
                        inventoryList[slotOriginalNum] = tmp;
                    }
                    else if (equipMode && slotSelected.transform.parent.parent.name[0] == 'I' && slotOriginal.transform.parent.parent.name[0] == 'I')
                    {
                        Debug.Log(2);
                        //moving from inventory to inventory
                        GameObject tmp = inventoryList[slotSelectedNum];
                        inventoryList[slotSelectedNum] = inventoryList[slotOriginalNum];
                        inventoryList[slotOriginalNum] = tmp;
                    }
                    else if (equipMode && slotSelected.transform.parent.parent.parent.name[0] == 'E' && slotOriginal.transform.parent.parent.parent.name[0] == 'E')
                    {
                        Debug.Log(3);
                        //moving from equip to equip

                        //weapon
                        if (slotSelected.transform.parent.parent.name == equipmentController.equipmentList[0][slotOriginalNum].gameObject.tag)
                        {
                            bool itemCurrentActiveState = equipmentController.equipmentList[0][slotOriginalNum].active;
                            equipmentController.equipmentList[0][slotOriginalNum].active = true;
                            if (slotSelected.transform.parent.parent.name == equipmentController.equipmentList[0][slotOriginalNum].gameObject.tag)
                            {
                                GameObject tmp = equipmentController.equipmentList[0][slotSelectedNum];
                                equipmentController.equipmentList[0][slotSelectedNum] = equipmentController.equipmentList[0][slotOriginalNum];
                                equipmentController.equipmentList[0][slotOriginalNum] = tmp;
                                equipmentController.equipmentList[0][slotSelectedNum].active = itemCurrentActiveState;
                            }
                        }
                        //armor
                        else if (slotSelected.transform.parent.parent.name == equipmentController.equipmentList[1][slotOriginalNum].gameObject.tag)
                        {
                            bool itemCurrentActiveState = equipmentController.equipmentList[1][slotOriginalNum].active;
                            equipmentController.equipmentList[1][slotOriginalNum].active = true;
                            if (slotSelected.transform.parent.parent.name == equipmentController.equipmentList[1][slotOriginalNum].gameObject.tag)
                            {
                                GameObject tmp = equipmentController.equipmentList[1][slotSelectedNum];
                                equipmentController.equipmentList[1][slotSelectedNum] = equipmentController.equipmentList[1][slotOriginalNum];
                                equipmentController.equipmentList[1][slotOriginalNum] = tmp;
                                equipmentController.equipmentList[1][slotSelectedNum].active = itemCurrentActiveState;
                            }
                        }
                    }
                    else if (equipMode && slotSelected.transform.parent.parent.parent.name[0] == 'E' && slotOriginal.transform.parent.parent.name[0] == 'I')
                    {
                        Debug.Log(4);
                        //equip

                        //weapon
                        if (slotSelected.transform.parent.parent.name == "Weapon" && inventoryList[slotOriginalNum].gameObject.tag == "Weapon")
                        {
                            bool itemCurrentActiveState = inventoryList[slotOriginalNum].active;
                            inventoryList[slotOriginalNum].active = true;
                            GameObject tmp = equipmentController.equipmentList[0][slotSelectedNum];
                            equipmentController.equipmentList[0][slotSelectedNum] = inventoryList[slotOriginalNum];
                            inventoryList[slotOriginalNum] = tmp;
                            equipmentController.equipmentList[0][slotSelectedNum].active = itemCurrentActiveState;
                        }
                        //armor
                        else if(slotSelected.transform.parent.parent.name == "Armor" && inventoryList[slotOriginalNum].gameObject.tag == "Armor")
                        {
                            bool allowEquip = false;
                            switch (inventoryList[slotOriginalNum].gameObject.GetComponent<EquipmentStats>().equipmentStats.Slot)
                            {
                                case 0:
                                    allowEquip = (!equipmentController.equippedSlots["torso"]);
                                    break;
                                case 1:
                                    allowEquip = (!equipmentController.equippedSlots["torso"]
                                        && !equipmentController.equippedSlots["rightArm"]
                                        && !equipmentController.equippedSlots["leftArm"]);
                                    break;
                                case 2:
                                    allowEquip = (!equipmentController.equippedSlots["rightHand"]
                                        && !equipmentController.equippedSlots["leftHand"]);
                                    break;
                                case 3:
                                    allowEquip = (!equipmentController.equippedSlots["rightThigh"]
                                        && !equipmentController.equippedSlots["leftThigh"]);
                                    break;
                                case 4:
                                    allowEquip = (!equipmentController.equippedSlots["rightThigh"]
                                        && !equipmentController.equippedSlots["leftThigh"]
                                        && !equipmentController.equippedSlots["rightLeg"]
                                        && !equipmentController.equippedSlots["leftLeg"]);
                                    break;
                                default:
                                    break;
                            }

                            if (allowEquip)
                            {
                                bool itemCurrentActiveState = inventoryList[slotOriginalNum].active;
                                inventoryList[slotOriginalNum].active = true;
                                GameObject tmp = equipmentController.equipmentList[1][slotSelectedNum];
                                equipmentController.equipmentList[1][slotSelectedNum] = inventoryList[slotOriginalNum];
                                inventoryList[slotOriginalNum] = tmp;
                                equipmentController.equipmentList[1][slotSelectedNum].active = itemCurrentActiveState;

                                this.GetComponent<Player>().equipArmor(equipmentController.equipmentList[1][slotSelectedNum].gameObject);
                            }
                        }
                    }
                    else if (equipMode && slotSelected.transform.parent.parent.name[0] == 'I' && slotOriginal.transform.parent.parent.parent.name[0] == 'E')
                    {
                        Debug.Log(5);
                        //dequip

                        //weapon
                        if (slotOriginal.transform.parent.parent.name == "Weapon" && (inventoryList[slotSelectedNum].gameObject.tag == "Weapon" || inventoryList[slotSelectedNum] == pu_blank))
                        {
                            GameObject tmp = inventoryList[slotSelectedNum];
                            inventoryList[slotSelectedNum] = equipmentController.equipmentList[0][slotOriginalNum];
                            equipmentController.equipmentList[0][slotOriginalNum] = tmp;
                        }
                        //armor
                        else if (slotOriginal.transform.parent.parent.name == "Armor" && (inventoryList[slotSelectedNum].gameObject.tag == "Armor" || inventoryList[slotSelectedNum] == pu_blank))
                        {
                            GameObject tmp = inventoryList[slotSelectedNum];
                            inventoryList[slotSelectedNum] = equipmentController.equipmentList[1][slotOriginalNum];
                            equipmentController.equipmentList[1][slotOriginalNum] = tmp;

                            this.GetComponent<Player>().dequipArmor(inventoryList[slotSelectedNum].gameObject);
                            if (equipmentController.equipmentList[1][slotOriginalNum] != pu_blank)
                            {
                                this.GetComponent<Player>().equipArmor(equipmentController.equipmentList[1][slotOriginalNum].gameObject);
                            }
                        }
                        
                    }
                }
                updateInventory();
                equipmentController.updateEquipment();

                //clear selection
                itemSelectedImg = null;
                slotOriginal = null;
            }
        }
    }


    /// <summary>
    /// append item to inventory list
    /// </summary>
    /// <param name="itemObject"></param>
    public void addToInventory(GameObject itemObject)
    {
        if (numOfSlotsFree != 0)
        {

            for(int i = 0; i < inventoryList.Count; i++)
            {
                if (inventoryList[i].name == pu_blank.name)
                {
                    inventoryList[i] = itemObject;
                    numOfSlotsFree--;
                    break;
                }
            }
        }
        else {
            Debug.Log("Your inventory is full!");
        }
    }


    /// <summary>
    /// update inventory window to reflect inventory list
    /// </summary>
    public void updateInventory()
    {
        for (int slotNumber = 0; slotNumber < inventoryList.Count; slotNumber++)   //for each slot in inventory window
        {
            //get reference to slot
            slot = inventoryWindow.transform.Find("Grid").Find("slot " + "(" + slotNumber.ToString() + ")");

            //get active state of item
            prevActive = inventoryList[slotNumber].active;

            //get and set name of item in inventory window
            //activate item
            inventoryList[slotNumber].SetActive(true);

            //update name and image
            if (inventoryList[slotNumber].name != pu_blank.name)
            {
                slot.Find("Image").GetComponent<Image>().sprite = inventoryList[slotNumber].GetComponent<EquipmentStats>().equipmentStats.Icon;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                itemName = inventoryList[slotNumber].GetComponent<EquipmentStats>().equipmentStats.Name;
                slot.Find("Text").GetComponent<Text>().text = itemName;
            }
            else {
                slot.Find("Image").GetComponent<Image>().sprite = pu_blank.GetComponent<SpriteRenderer>().sprite;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                slot.Find("Text").GetComponent<Text>().text = "";
            }

            //return item state to previous state
            inventoryList[slotNumber].SetActive(prevActive);
        }
    }


    /// <summary>
    /// update the item window (detailed description window next to inventory window)
    /// </summary>
    /// <param name="clickedObject"></param>
    public void updateItemWindow(GameObject itemShowHovered) {

        attr = itemWindow.transform.Find("Attr");

        //get reference to object by using slot (slotNumber) and inventoryList[slotNumber]
        if (Int32.TryParse(itemShowHovered.transform.parent.gameObject.name.Substring(6,2), out itemShowNum))
        {
            //2 digit slot number
            itemShowNum = Int32.Parse(itemShowHovered.transform.parent.gameObject.name.Substring(6, 2));
        }
        else if (Int32.TryParse(itemShowHovered.transform.parent.gameObject.name.Substring(6, 1), out itemShowNum))
        {
            //1 digit slot number
            itemShowNum = Int32.Parse(itemShowHovered.transform.parent.gameObject.name.Substring(6, 1));
        }

        //get reference to item gameobject
        if (itemShowNum < inventoryList.Count)
        {
            itemShow = inventoryList[itemShowNum];
        }

        //update name and image of slot if it's not an empty slot
        if (itemShow != pu_blank)
        {
            attr.Find("Name").GetComponent<Text>().text = itemShow.GetComponent<EquipmentStats>().equipmentStats.Name;
            itemWindow.transform.Find("Image").Find("Image").GetComponent<Image>().sprite = itemShow.GetComponent<EquipmentStats>().equipmentStats.Icon;
        }
        else
        {
            attr.Find("Name").GetComponent<Text>().text = pu_blank.name;
            itemWindow.transform.Find("Image").Find("Image").GetComponent<Image>().sprite = pu_blank.GetComponent<SpriteRenderer>().sprite;
        }
    }


    /// <summary>
    /// check the number of slots in inventory window created in Unity (by going through hierarchy)
    /// </summary>
    /// <returns></returns>
    /*int checkNumOfSlots()
    {
        try
        {
            for (int i = 0; i <= maxNumOfSlots; i++)
            {
                inventoryWindow.transform.Find("Grid").Find("slot " + "(" + i.ToString() + ")");
                numOfSlots++;
            }
        }
        catch (Exception e) { }
        return numOfSlots;
    }*/
}
