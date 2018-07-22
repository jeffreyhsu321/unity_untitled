using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ReserveEquipment : MonoBehaviour {

    public List<List<GameObject>> listReserveEquipment = new List<List<GameObject>>();
    public List<GameObject> listReserveWeapon;
    public List<GameObject> listReserveArmor;

	// Use this for initialization
	void Start () {
        listReserveEquipment.Add(listReserveWeapon);
        listReserveEquipment.Add(listReserveArmor);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
