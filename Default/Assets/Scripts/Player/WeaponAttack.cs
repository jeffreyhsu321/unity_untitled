using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour {

    CircleCollider2D weaponCollider;

    [HideInInspector]
    public bool isAttacking = false;

    void Start() {
        weaponCollider = GetComponent<CircleCollider2D>();
        weaponCollider.enabled = false;

        transform.name = transform.name.Replace("(Clone)", "").Trim();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && (isAttacking == false))
        {
            StartCoroutine(Attacking());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("Hit", 10f);
        }
    }

    IEnumerator Attacking() {
        yield return new WaitForSeconds(0.05f);
        weaponCollider.enabled = true;
        isAttacking = true;
        yield return new WaitForSeconds(0.4f);
        weaponCollider.enabled = false;
        isAttacking = false;
    }
}
