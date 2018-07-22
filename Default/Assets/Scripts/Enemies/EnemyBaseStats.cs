using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseStats{

    public string Name;
    public BaseStat Power;
    public BaseStat Vitality;
    public BaseStat Experience;
    public float Size;

    public EnemyBaseStats(string name, BaseStat power, BaseStat vitality, BaseStat experience, float size)
    {
        this.Name = name;
        this.Power = power;
        this.Vitality = vitality;
        this.Experience = experience;
        this.Size = size;
    }
}
