using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {

    public Camera cam;
    private float camHeight;
    private float camWidth;

    ParticleSystem starsPS;

    public Celestials cel;

    void Start() {
        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        starsPS = GetComponent<ParticleSystem>();
        var shape = starsPS.shape;
        shape.scale = new Vector3(camWidth, camHeight, 1);

        var main = starsPS.main;
        main.startLifetime = cel.nightLength / 2;
    }

    void Update() {
        if (cel.isDay)
        {
            if (starsPS.isPlaying)
            {
                starsPS.Stop();
            }
        }
        else if (!cel.isDay)
        {
            transform.position = cam.transform.position;
            if (!starsPS.isPlaying)
            {
                starsPS.Play();
            }
        }
    }

}
