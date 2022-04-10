using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechaFuel : MonoBehaviour {

    [SerializeField] private float fuelRegenRate = 1;
    [SerializeField] private float shieldRegenRate = 1;

    private MechaController_Movement control;
    private float fuel = 1;
    private float shieldFuel = 1;
    private Image flyFuelMeter;
    private Image shieldFuelMeter;
    private Image shieldBrokenMeter;


    void Start() {
        control = GetComponent<MechaController_Movement>();
        flyFuelMeter = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
        shieldFuelMeter = transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>();
        shieldBrokenMeter = transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Image>();
    }

    void Update() {
        RegenFuel();
        RegenShield();
    }

    public bool ConsumeFuel(float rate) {
        fuel -= rate * Time.deltaTime;
        if (fuel < 0) {
            fuel = 0;
            return false;
        }
        else {
            return true;
        }
    }

    void RegenFuel() {
        if (fuel < 1 && control.MechaGrounded) {
            fuel += fuelRegenRate * Time.deltaTime;
            if (fuel > 1) {
                fuel = 1;
            }
        }
        // Fuel meter
        flyFuelMeter.fillAmount = fuel;
        if (fuel == 1) {
            flyFuelMeter.transform.parent.gameObject.SetActive(false);
        }
        else {
            flyFuelMeter.transform.parent.gameObject.SetActive(true);
        }
    }

    void RegenShield() {
        if (shieldFuel < 1) { // && !shield.shielding) {
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
        if (shieldFuel < 0.5f) { // && shield.shieldBroken
            shieldBrokenMeter.fillAmount = shieldFuel;
        }
        else {
            shieldBrokenMeter.fillAmount = 0;
        }
    }

    public void DeactivationCleanup() {
        fuel = 1;
        shieldFuel = 1;
        shieldFuelMeter.transform.parent.gameObject.SetActive(false);
        flyFuelMeter.transform.parent.gameObject.SetActive(false);
        shieldBrokenMeter.transform.parent.gameObject.SetActive(false);
    }
}
