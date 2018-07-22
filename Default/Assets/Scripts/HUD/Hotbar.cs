using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    
    public GameObject hotbar;

    Sprite itemSlot;

	void Start () {
	}

    public void updateHotbar(int slotNumber, Sprite itemSprite) {
        hotbar.transform.Find("slot" + slotNumber.ToString()).Find("item").GetComponent<Image>().sprite = itemSprite;
        hotbar.transform.Find("slot" + slotNumber.ToString()).Find("item").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }
}
