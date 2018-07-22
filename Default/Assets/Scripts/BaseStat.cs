using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat {

    //public List<StatBonus> StatBonuses { get; set; }
    public float BaseValue { get; set; }
    public string StatName { get; set; }
    //public string StatDescription { get; set; }
    //public int FinalValue { get; set; }

    public BaseStat(float baseValue, string statName) {
        //this.StatBonuses = new List<StatBonus>();

        this.BaseValue = baseValue;
        this.StatName = statName;
        //this.StatDescription = statDescription;
    }

    /*public void AddStatBonus(StatBonus statBonus) {
        this.StatBonuses.Add(statBonus);
    }

    public void RemoveStatBonus(StatBonus statBonus) {
        this.StatBonuses.Remove(StatBonuses.Find(x => x.BonusValue == statBonus.BonusValue));
    }

    public int GetFinalStatValue() {
        this.FinalValue = 0;
        this.StatBonuses.ForEach(x => this.FinalValue += x.BonusValue);
        FinalValue += BaseValue;
        return FinalValue;
    }*/

}
