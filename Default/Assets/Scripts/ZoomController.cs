using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour {

    public Collider2D collider;
    public Camera cam;
    public bool isZooming = true;
    public bool isZoomed = true;
    public float zoomOutSize;
    public float zoomInSize;

    void Start() {
        cam.orthographicSize = zoomInSize;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Q)) {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomInSize -1 , 3f * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            cam.orthographicSize = zoomOutSize;
            if (cam.orthographicSize > zoomOutSize) { isZooming = false; isZoomed = false; }
        }

        if (isZooming)
        {
            if (isZoomed)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomOutSize + 1, 3f * Time.deltaTime);
                if (cam.orthographicSize > zoomOutSize) { isZooming = false; isZoomed = false; }
            }
            else
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomInSize - 1, 3f * Time.deltaTime);
                if (cam.orthographicSize < zoomInSize) { isZooming = false; isZoomed = true; }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isZooming && !isZoomed) {
            isZooming = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy") && !isZooming && isZoomed)
        {
            isZooming = true;   
        }
    }
}
