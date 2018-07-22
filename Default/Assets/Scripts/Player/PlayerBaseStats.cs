using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseStats
{

    public string Name;
    public BaseStat Power;
    public BaseStat Vitality;
    public BaseStat Knockback;
    public BaseStat Experience;
    public BaseStat Level;

    public PlayerBaseStats(string name, BaseStat power, BaseStat vitality, BaseStat knockback, BaseStat experience, BaseStat level)
    {
        this.Name = name;
        this.Power = power;
        this.Vitality = vitality;
        this.Knockback = knockback;
        this.Experience = experience;
        this.Level = level;
    }
}
