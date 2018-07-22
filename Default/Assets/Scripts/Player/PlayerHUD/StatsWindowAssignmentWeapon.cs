using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatsWindowAssignmentWeapon : MonoBehaviour {

    public GameObject Player;
    PlayerStats stats;
    EquipmentStats equipStats;
    Text statValue;

    string gameObjectName;

    int currentWeapon;

    void Start()
    {
        stats = Player.GetComponent<PlayerStats>();
        currentWeapon = Player.GetComponent<Player>().currentWeapon;
        equipStats = Player.GetComponent<Player>().weaponsQuickSlotted[currentWeapon].GetComponent<EquipmentStats>();

        statValue = transform.Find("Panel").Find("stat_values").Find("Power").GetComponent<Text>();
        statValue.text = equipStats.equipmentStats.Power.BaseValue.ToString();
        statValue = transform.Find("Panel").Find("stat_values").Find("Knockback").GetComponent<Text>();
        statValue.text = equipStats.equipmentStats.Knockback.BaseValue.ToString();

        statValue = transform.Find("WindowDesc").GetComponent<Text>();
        statValue.text = Player.GetComponent<Player>().weaponsQuickSlotted[currentWeapon].GetComponent<EquipmentStats>().equipmentStats.Name;
    }

    public void updateStatWindow(string statName, int currentWeapon)
    {
        currentWeapon = Player.GetComponent<Player>().currentWeapon;
        equipStats = Player.GetComponent<Player>().listEquippedWeapons[currentWeapon].GetComponent<EquipmentStats>();

        statValue = transform.Find("Panel").Find("stat_values").Find(statName).GetComponent<Text>();

        statValue.text = (statName == "Name") ? equipStats.equipmentStats.Name :
            (statName == "Power") ? (equipStats.equipmentStats.Power.BaseValue).ToString() :
            (statName == "Vitality") ? (equipStats.equipmentStats.Vitality.BaseValue).ToString() :
            (statName == "Knockback") ? (equipStats.equipmentStats.Knockback.BaseValue).ToString() :
            "error";

        statValue = transform.Find("WindowDesc").GetComponent<Text>();
        statValue.text = Player.GetComponent<Player>().listEquippedWeapons[currentWeapon].GetComponent<EquipmentStats>().equipmentStats.Name;
    }
}

