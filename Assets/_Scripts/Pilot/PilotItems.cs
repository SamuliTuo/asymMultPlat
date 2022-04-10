using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PilotItems : MonoBehaviour {

    public GameObject currentItem;

    [SerializeField] private Image timerImage = null;
    [SerializeField] private Text timerText = null;

    private PilotHP hp;
    [SerializeField] private float currentItemCooldown = 0;
    [SerializeField] private float remainingCooldown = 10;


    void Start() {
        hp = GetComponent<PilotHP>();
        RefreshUI();
    }

    public void UpdateItems() {
        if (remainingCooldown > 0) {
            remainingCooldown -= Time.deltaTime;
            RefreshUI();
        }
        else if (Gamepad.current.leftTrigger.wasPressedThisFrame) {
            UseItem();
        }
    }

    void UseItem() {
        if (currentItem != null) {
            var clone = Instantiate(currentItem, transform.position, Quaternion.identity);
            clone.GetComponent<Item_HealBox>().InitItem(hp);
            remainingCooldown = currentItemCooldown;
            RefreshUI();
        }
    }

    void RefreshUI() {
        if (remainingCooldown <= 0) {
            timerText.gameObject.SetActive(false);
        }
        else {
            timerText.text = Mathf.Round(remainingCooldown).ToString();
            if (!timerText.gameObject.activeSelf) {
                timerText.gameObject.SetActive(true);
            }
        }
        timerImage.fillAmount = 1 - remainingCooldown / currentItemCooldown;
    }
}
