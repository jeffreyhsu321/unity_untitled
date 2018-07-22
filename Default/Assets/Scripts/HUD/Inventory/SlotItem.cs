using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public PlayerInventoryController inventoryController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }


    /// <summary>
    /// reference the selected object and activate item window
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!inventoryController.isGrabbing)
        {
            inventoryController.isGrabbing = true;

            //set itemSelectedImg to the Image object that the pointer isa hovering above
            inventoryController.itemSelectedImg = this.gameObject;
        }
        
        //activate item window
        inventoryController.itemWindow.SetActive(true);

        //update and control the item window
        inventoryController.updateItemWindow(this.gameObject);
    }

    /// <summary>
    /// used to hide item window
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        //deactivate item window
        inventoryController.itemWindow.SetActive(false);
    }
}
