using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    PlayerStats stats;

    public RectTransform HUDCanvasRectT;
    Vector3 canvasRectTSize;
    public Slider healthSlider;
    public Text nameUI;

    public GameObject winStat;
    public GameObject winStatWeapon;
    public GameObject winInventory;
    public GameObject winEquipment;

    public PlayerInventoryController playerInventoryController;
    public PlayerEquipmentController playerEquipmentController;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        nameUI.GetComponent<Text>().text = stats.playerName;

        canvasRectTSize = HUDCanvasRectT.localScale;
    }

    void Update()
    {
    }

    /// <summary>
    /// update healthbar
    /// </summary>
    /// <param name="health"></param>
    public void showHealth(float health)
    {
        healthSlider.value = health;
    }

    /// <summary>
    /// flip player HUD based on facing direction
    /// </summary>
    /// <param name="direction"></param>
    public void flipHUD(float direction)
    {
        HUDCanvasRectT.localScale = new Vector3(-1 * direction * canvasRectTSize.x, canvasRectTSize.y, canvasRectTSize.z);
    }

    /// <summary>
    /// toggle stat windows (player and weapon if weapon is equipped)
    /// </summary>
    /// <param name="isEquip"></param>
    /// <param name="charWindow"></param>
    public void toggleStatsWindow(bool isEquip, bool charWindow) {
        winStat.active = (charWindow)? !winStat.active : winStat.active;
        winStatWeapon.active = (isEquip) ? winStat.active : false;
    }

    /// <summary>
    /// toggle inventory window
    /// </summary>
    public void toggleInventoryWindow() {
        winInventory.active = !winInventory.active;

        //update inventory when active
        if (winInventory.active) { playerInventoryController.updateInventory(); }
    }

    /// <summary>
    /// toggle equipment window
    /// </summary>
    public void toggleEquipmentWindow()
    {
        winEquipment.active = !winEquipment.active;

        //update inventory when active
        if (winEquipment.active) { playerEquipmentController.updateEquipment(); }
    }
}
