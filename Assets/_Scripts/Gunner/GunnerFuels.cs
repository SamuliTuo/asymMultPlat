using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunnerFuels : MonoBehaviour {

    [Header("Public:")]
    public float flyFuel = 1;
    public float shieldFuel = 1;

    [Header("Exposed privates:")]
    [SerializeField] private float flyRegenRate = 1;
    [SerializeField] private float shieldRegenRate = 1;

    private GunnerController control;
    private GunnerEnergyShield shield;
    private Image flyFuelMeter;
    private Image shieldFuelMeter;
    private Image shieldBrokenMeter;


    private void Start() {
        control = GetComponent<GunnerController>();
        shield = GetComponent<GunnerEnergyShield>();
        flyFuelMeter = transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Image>();
        shieldFuelMeter = transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Image>();
        shieldBrokenMeter = transform.GetChild(2).GetChild(1).GetChild(2).GetComponent<Image>();
    }

    private void Update() {
        RegenFlyFuel();
        RegenShield();
    }

    public void FillUpFuels() {
        flyFuel = 1;
        shieldFuel = 1;
    }

    public void ConsumeFlyFuel(float rate) {
        flyFuel -= rate * Time.deltaTime;
    }

    public void ConsumeShieldFuel(float rate) {
        flyFuel -= rate * Time.deltaTime;
    }

    void RegenFlyFuel() {
        var dist = (transform.position - (control.pilot.position + control.followPos)).magnitude;
        if (flyFuel < 1 && dist < control.followMaxOffset * 1.3f) {
            flyFuel += flyRegenRate * Time.deltaTime;
            if (flyFuel > 1) {
                flyFuel = 1;
            }
            
        }
        // Fuel meter
        flyFuelMeter.fillAmount = flyFuel;
        if (flyFuel == 1) {
            flyFuelMeter.transform.parent.gameObject.SetActive(false);
        }
        else {
            flyFuelMeter.transform.parent.gameObject.SetActive(true);
        }
    }

    void RegenShield() {
        if (shieldFuel < 1 && !shield.shielding) {
            shieldFuel += shieldRegenRate * Time.deltaTime;
            if (shieldFuel > 1) {
                shieldFuel = 1;
            }
            else if (shieldFuel < 0) {
                shieldFuel = 0;
            }
        }
        // Fuel meter
        shieldFuelMeter.fillAmount = shieldFuel;
        if (shieldFuel == 1) {
            shieldFuelMeter.transform.parent.gameObject.SetActive(false);
        }
        else {
            shieldFuelMeter.transform.parent.gameObject.SetActive(true);
        }
        // Shield broken
        if (shield.shieldBroken && shieldFuel < 0.5f) {
            shieldBrokenMeter.fillAmount = shieldFuel;
        }
        else {
            shieldBrokenMeter.fillAmount = 0;
        }
    }
}
