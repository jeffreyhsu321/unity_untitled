using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStats : MonoBehaviour
{
    public EquipmentBaseStats equipmentStats;

    public string itemName;
    public string type;
    public int power;
    public int vitality;
    public int knockback;
    public int rarity;
    public int slot;
    public Sprite icon;

    public BaseStat powerStat;
    public BaseStat vitalityStat;
    public BaseStat knockbackStat;
    public BaseStat rarityStat;

    void Awake()
    {
        powerStat = new BaseStat(power, "Power");
        vitalityStat = new BaseStat(vitality, "Vitality");
        knockbackStat = new BaseStat(knockback, "Knockback");
        rarityStat = new BaseStat(rarity, "Rarity");

        equipmentStats = new EquipmentBaseStats(itemName, type, powerStat, vitalityStat, knockbackStat, rarityStat, slot, icon);
    }
}
