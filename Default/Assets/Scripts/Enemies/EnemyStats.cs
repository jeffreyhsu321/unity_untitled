using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public EnemyBaseStats enemyStats;

    public string enemyName;
    public float power;
    public float vitality;
    public float experience;
    public float size;
    
    BaseStat powerStat;
    BaseStat vitalityStat;
    BaseStat experienceStat;

    void Awake()
    {
        powerStat = new BaseStat(power, "Power");
        vitalityStat = new BaseStat(vitality, "Vitality");
        experienceStat = new BaseStat(experience, "Experience");

        enemyStats = new EnemyBaseStats(enemyName, powerStat, vitalityStat, experienceStat, size);
    }
}
