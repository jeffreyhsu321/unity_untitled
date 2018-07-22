using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public PlayerInventoryController inventoryController;

    static bool isSetSlotOriginal = false;

    bool isHovering = false;

    // Use this for initialization
    void Start () {
		
	}
	
	/// <summary>
    /// move selectedItemImg into tmp transit slot
    /// </summary>
	void Update () {
        //get current slot as original slot
        if (!isSetSlotOriginal && isHovering && Input.GetMouseButtonDown(0)) {
            //get reference to slot object
            inventoryController.slotOriginal = this.gameObject;

            //get index of the item in inventoryList
            if(Int32.TryParse(this.gameObject.name.Substring(6, 2), out inventoryController.slotOriginalNum))
            {
                inventoryController.slotOriginalNum = Int32.Parse(this.gameObject.name.Substring(6, 2));
            }
            else if (Int32.TryParse(this.gameObject.name.Substring(6, 1), out inventoryController.slotOriginalNum))
            {
                inventoryController.slotOriginalNum = Int32.Parse(this.gameObject.name.Substring(6, 1));
            }

            //move selectedItemImg to ImageInTransit slot
            inventoryController.itemSelectedImg.transform.SetParent(inventoryController.slotTransit.transform);
            
            isSetSlotOriginal = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            isSetSlotOriginal = false;
            //inventoryController.slotOriginal = null;
        }
	}

    /// <summary>
    /// select current slot the mouse is over
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //is hovering
        isHovering = true;

        //if dragging an item over this slot
        if (inventoryController.isGrabbing && Input.GetMouseButton(0))
        {
            //set slotSelected to the slot the cursor is hovering over
            inventoryController.slotSelected = this.gameObject;

            //get index of the object in inventoryList
            if (Int32.TryParse(inventoryController.slotSelected.name.Substring(6, 2), out inventoryController.slotSelectedNum))
            {
                inventoryController.slotSelectedNum = Int32.Parse(inventoryController.slotSelected.name.Substring(6, 2));
            }
            else if (Int32.TryParse(inventoryController.slotSelected.name.Substring(6, 1), out inventoryController.slotSelectedNum))
            {
                inventoryController.slotSelectedNum = Int32.Parse(inventoryController.slotSelected.name.Substring(6, 1));
            }
            
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;

        //set is grabbing to false
        if (inventoryController.isGrabbing && !Input.GetMouseButton(0)) {
            inventoryController.isGrabbing = false;
            inventoryController.itemSelectedImg = null;
        }

        //null selected slot when out of slot
        inventoryController.slotSelected = null;
    }
}
