using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBaseStats
{

    public string Name;
    public string Type;
    public BaseStat Power;
    public BaseStat Vitality;
    public BaseStat Knockback;
    public BaseStat Rarity;
    public int Slot;
    public Sprite Icon;

    public EquipmentBaseStats(string name, string type, BaseStat power, BaseStat vitality, BaseStat knockback, BaseStat rarity, int slot, Sprite icon)
    {
        this.Name = name;
        this.Type = type;
        this.Power = power;
        this.Vitality = vitality;
        this.Knockback = knockback;
        this.Rarity = rarity;
        this.Slot = slot;
        this.Icon = icon;
    }
}
