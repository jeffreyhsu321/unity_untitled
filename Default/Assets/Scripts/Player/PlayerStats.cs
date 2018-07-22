using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public PlayerBaseStats playerStats;

    public string playerName;
    public float power;
    public float vitality;
    public float knockback;
    public float experience;
    public float level;

    public BaseStat powerStat;
    public BaseStat vitalityStat;
    public BaseStat knockbackStat;
    public BaseStat experienceStat;
    public BaseStat levelStat;

    public StatsWindowAssignment statsWindowAssignment;
    
    public Slider playerExpSlider;

    void Awake()
    {
        powerStat = new BaseStat(power, "Power");
        vitalityStat = new BaseStat(vitality, "Vitality");
        knockbackStat = new BaseStat(knockback, "Knockback");
        experienceStat = new BaseStat(experience, "Experience");
        levelStat = new BaseStat(level, "Level");

        playerStats = new PlayerBaseStats(playerName, powerStat, vitalityStat, knockbackStat, experienceStat, levelStat);
    }

    public void updateStatWindow(bool isSpecifyStat, string statName) {
        statsWindowAssignment.updateStatWindow(isSpecifyStat, statName);
    }

    public void levelUp(float overflowExp) {
        playerStats.Level.BaseValue += 1;
        playerExpSlider.maxValue *= 1.2f;
        playerStats.Experience.BaseValue = 0 + overflowExp;
        updateStatWindow(false, "Level");
    }
}
