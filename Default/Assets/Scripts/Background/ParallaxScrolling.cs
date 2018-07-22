using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour {

    public Transform[] backgrounds;
    public float[] parallaxScales; //the amount of movement for each layment
    public float smoothing;
    private Vector3 previousPlayerPos;

    public Player player;
    
	void Start () {
        previousPlayerPos = transform.position;
        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < parallaxScales.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z;
        }
	}

    void LateUpdate () {
        for (int i = 0; i < backgrounds.Length; i++) {
            Vector3 parallax = (previousPlayerPos - transform.position) * parallaxScales[i] / smoothing;
            backgrounds[i].position = new Vector3(backgrounds[i].position.x - parallax.x, backgrounds[i].position.y, backgrounds[i].position.z);
        }
        previousPlayerPos = transform.position;
    }

}
