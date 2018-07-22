using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackEffect : MonoBehaviour
{
    void attack()
    {
        GetComponent<Animation>().Play("sword_slash_1");
    }
}
