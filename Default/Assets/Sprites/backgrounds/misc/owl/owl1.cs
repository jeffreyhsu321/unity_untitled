using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class owl1 : MonoBehaviour {

    Animator anim;

    Coroutine co1;
    Coroutine co2;
    Coroutine co3;

    bool isRapid;

    void Start() {
        anim = GetComponent<Animator>();
        co1 = StartCoroutine(randAnim());
        co2 = StartCoroutine(randAnim());
        co3 = StartCoroutine(opacityChange());
        isRapid = false;
    }

    IEnumerator randAnim() {

        while (true) {
            isRapid = (Random.value < 0.9);
            
            int choice = Random.Range(1, 5);
            yield return new WaitForSeconds((isRapid) ? 0 : Random.Range(8, 12));
            anim.SetTrigger(choice.ToString());
        }
    }

    IEnumerator opacityChange() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0,30));
            anim.SetBool("fadeIn", false);
            anim.SetBool("fadeOut", true);
            yield return new WaitForSeconds(Random.Range(0, 30));
            anim.SetBool("fadeOut", false);
            anim.SetBool("fadeIn", true);
        }
    }
}