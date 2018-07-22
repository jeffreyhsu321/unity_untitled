using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PlayerEquipmentController : MonoBehaviour {

    public GameObject equipmentWindow;
    public GameObject itemWindow;
    public List<List<GameObject>> equipmentList = new List<List<GameObject>>();
    public List<GameObject> equipmentListWeapon;
    public List<GameObject> equipmentListArmor;
    public PlayerInventoryController inventoryController;

    public IDictionary<string, bool> equippedSlots;

    public int numOfSlotsWeapon;
    public int numOfSlotsArmor;

    // Use this for initialization
    void Awake () {
        for (int i = 0; i < numOfSlotsWeapon; i++)
        {
            equipmentListWeapon.Add(inventoryController.pu_blank);
        }
        for (int i = 0; i < numOfSlotsArmor; i++) {
            equipmentListArmor.Add(inventoryController.pu_blank);
        }
        equipmentList.Add(equipmentListWeapon);
        equipmentList.Add(equipmentListArmor);

        //this dictionary is used to prevent overlapping armor slots items
        equippedSlots = new Dictionary<string, bool>()
        {
            {"torso", false},
            {"rightArm", false},
            {"leftArm", false},
            {"rightLeg", false},
            {"leftLeg", false},
            {"rightHand", false},
            {"leftHand", false},
            {"rightThigh", false},
            {"leftThigh", false},
            {"head", false}
        };
    }
	
	// Update is called once per frame
	void Update () {
    }

    /// <summary>
    /// update inventory window to reflect inventory list
    /// </summary>
    public void updateEquipment()
    {
        //weapon
        for (int slotNumber = 0; slotNumber <= equipmentListWeapon.Count - 1; slotNumber++)   //for each slot in equipment window
        {
            //get reference to slot
            Transform slot = equipmentWindow.transform.Find("Weapon").transform.Find("Grid").Find("slot " + "(" + slotNumber.ToString() + ")");

            //get active state of item
            bool prevActive = equipmentListWeapon[slotNumber].active;

            //get and set name of item in equipment window
            //activate item
            equipmentListWeapon[slotNumber].SetActive(true);

            //update name and image
            if (equipmentListWeapon[slotNumber].name != inventoryController.pu_blank.name)
            {
                slot.Find("Image").GetComponent<Image>().sprite = equipmentListWeapon[slotNumber].GetComponent<SpriteRenderer>().sprite;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                string itemName = equipmentListWeapon[slotNumber].GetComponent<EquipmentStats>().equipmentStats.Name;
                slot.Find("Text").GetComponent<Text>().text = itemName;
            }
            else
            {
                slot.Find("Image").GetComponent<Image>().sprite = equipmentListWeapon[slotNumber].GetComponent<SpriteRenderer>().sprite;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                slot.Find("Text").GetComponent<Text>().text = "";
            }

            //return item state to previous state
            equipmentListWeapon[slotNumber].SetActive(prevActive);
        }

        //armor
        for (int slotNumber = 0; slotNumber <= equipmentListArmor.Count - 1; slotNumber++)   //for each slot in equipment window
        {
            //get reference to slot
            Transform slot = equipmentWindow.transform.Find("Armor").transform.Find("Grid").Find("slot " + "(" + slotNumber.ToString() + ")");

            //get active state of item
            bool prevActive = equipmentListArmor[slotNumber].active;

            //get and set name of item in equipment window
            //activate item
            equipmentListArmor[slotNumber].SetActive(true);

            //update name and image
            if (equipmentListArmor[slotNumber].name != inventoryController.pu_blank.name)
            {
                slot.Find("Image").GetComponent<Image>().sprite = equipmentListArmor[slotNumber].GetComponent<EquipmentStats>().equipmentStats.Icon;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                string itemName = equipmentListArmor[slotNumber].GetComponent<EquipmentStats>().equipmentStats.Name;
                slot.Find("Text").GetComponent<Text>().text = itemName;
            }
            else
            {
                slot.Find("Image").GetComponent<Image>().sprite = equipmentListArmor[slotNumber].GetComponent<SpriteRenderer>().sprite;
                slot.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                slot.Find("Text").GetComponent<Text>().text = "";
            }

            //return item state to previous state
            equipmentListArmor[slotNumber].SetActive(prevActive);
        }
    }
}
