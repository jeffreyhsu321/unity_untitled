using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour {

	public float driftSpeed;

    public int xBoundMax;
    public int xBoundMin;

    int driftDirection;

    void Start()
    {
        transform.position = new Vector3((Random.value < 0.5) ? xBoundMax : xBoundMin, transform.position.y, transform.position.z);
        driftDirection = (Random.value < 0.5) ? 1 : -1;
    }

    void Update() {
        transform.position =
            (transform.position.x > xBoundMax) ? new Vector3(xBoundMin, transform.position.y, transform.position.z)
            : (transform.position.x < xBoundMin) ? new Vector3(xBoundMax, transform.position.y, transform.position.z)
            : new Vector3(transform.position.x + (driftSpeed * driftDirection), transform.position.y, transform.position.z);
    }
}
