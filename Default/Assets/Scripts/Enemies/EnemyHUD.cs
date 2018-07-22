using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour {

    EnemyStats stats;
    
    public RectTransform HUDCanvasRectT;
    Vector3 canvasRectTSize;
    public Slider healthSlider;
    public Text nameUI;

    void Start() {
        stats = GetComponent<EnemyStats>();
        nameUI.GetComponent<Text>().text = stats.enemyName;

        canvasRectTSize = HUDCanvasRectT.localScale;
    }

    void Update() {
    }

    public void showHealth(float health) {
        healthSlider.value = health;
    }

    public void flipHUD(int direction) {
        HUDCanvasRectT.localScale = new Vector3(direction * canvasRectTSize.x, canvasRectTSize.y, canvasRectTSize.z);
    }
}
