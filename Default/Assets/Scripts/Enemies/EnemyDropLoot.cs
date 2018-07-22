using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropLoot : MonoBehaviour {

    public List<GameObject> lootTable;
    public GameObject Equipments;

    int chosenItemNum;
    public List<float> lootTableRarities;

    int dice;

    GameObject loot;

    EquipmentStats equipmentStats;

    void Start() {
    }

    public void dropLoot(Vector3 enemyPosition)
    {
        dice = Random.Range(0, 100); Debug.Log("dice" + dice.ToString());

        for (int i = 0; i < lootTableRarities.Count; i++)
        {
            if (dice <= lootTableRarities[i])
            {
                Instantiate(lootTable[i], enemyPosition, Quaternion.identity, Equipments.transform);
                break;
            }
            else
            {
            }
        }
    }
	
}
