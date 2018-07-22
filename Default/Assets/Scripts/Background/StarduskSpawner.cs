using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarduskSpawner : MonoBehaviour {

    public Celestials cel;

    public List<GameObject> starduskList;
    public int[] starduskCountList;

    //public GameObject starduskMPF;  //prefab
    //public GameObject starduskFPF;  //prefab

    //public int starduskMCount;
    //public int starduskFCount;

    public int xBoundMax;
    public int xBoundMin;

    public int yBoundMax;
    public int yBoundMin;

    void Start() {
        for (int i = 0; i < starduskList.Count; i++)
        {
            for (int j = 0; j < starduskCountList[i]; j++)
            {
                GameObject stardusk = (GameObject)Instantiate(starduskList[i]);
                stardusk.transform.parent = transform;
            }
        }
    }
    
}
