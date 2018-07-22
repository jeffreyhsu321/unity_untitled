using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stardusk : MonoBehaviour {

    Celestials cel;

    private int xBoundMax;
    private int xBoundMin;

    private int yBoundMax;
    private int yBoundMin;

    private ParticleSystem starduskPS;

    void Start() {
        xBoundMax = gameObject.GetComponentInParent<StarduskSpawner>().xBoundMax;
        xBoundMin = gameObject.GetComponentInParent<StarduskSpawner>().xBoundMin;
        yBoundMax = gameObject.GetComponentInParent<StarduskSpawner>().yBoundMax;
        yBoundMin = gameObject.GetComponentInParent<StarduskSpawner>().yBoundMin;

        starduskPS = GetComponent<ParticleSystem>();
        cel = GetComponentInParent<StarduskSpawner>().cel;

        transform.position = new Vector3(Random.Range(xBoundMin, xBoundMax), Random.Range(yBoundMin, yBoundMax), transform.position.z);
    }

    void Update() {
        if (!cel.isDay)
        {

            if (starduskPS.isStopped)
            {
                transform.position = new Vector3(Random.Range(xBoundMin, xBoundMax), Random.Range(yBoundMin, yBoundMax), transform.position.z);
                starduskPS.Play();
            }
        }
        else {
            starduskPS.Stop();
        }
    }

}
