using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatsWindowAssignment : MonoBehaviour {

    public GameObject Player;
    PlayerStats stats;
    Text statValue;
    public Slider expSlider;
    public Text Lv;

    string gameObjectName;

    void Start() {
        stats = Player.GetComponent<PlayerStats>();

        statValue = transform.Find("Panel").Find("stat_values").Find("Name").GetComponent<Text>();
        statValue.text = stats.playerStats.Name;

        statValue = transform.Find("Panel").Find("stat_values").Find("Power").GetComponent<Text>();
        statValue.text = stats.playerStats.Power.BaseValue.ToString();

        statValue = transform.Find("Panel").Find("stat_values").Find("Vitality").GetComponent<Text>();
        statValue.text = stats.playerStats.Vitality.BaseValue.ToString();

        expSlider.value = stats.playerStats.Experience.BaseValue;

        Lv.text = "Lv. " + stats.playerStats.Level.BaseValue.ToString();
    }

    public void updateStatWindow(bool isSpecifyStat, string statName) {
        try
        {
            if (isSpecifyStat)
        {
                statValue = transform.Find("Panel").Find("stat_values").Find(statName).GetComponent<Text>();
                statValue.text = (statName == "Name") ? stats.playerStats.Name :
                (statName == "Power") ? (stats.playerStats.Power.BaseValue).ToString() :
                (statName == "Vitality") ? (stats.playerStats.Vitality.BaseValue).ToString() :
                (statName == "Knockback") ? (stats.playerStats.Knockback.BaseValue).ToString() :
                "error";
        }

        expSlider.value = stats.playerStats.Experience.BaseValue;
        Lv.text = "Lv. " + stats.playerStats.Level.BaseValue.ToString();
        }
        catch (Exception e) { }
    }
}
