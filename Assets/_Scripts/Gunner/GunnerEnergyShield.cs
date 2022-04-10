using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunnerEnergyShield : MonoBehaviour {

    public Vector3 dir;
    public bool shieldBroken = false;
    public bool shielding = false;

    [SerializeField] private Transform shield = null;
    
    private GunnerController control;
    private GunnerFuels fuel;
    private Transform pilot;
    

    void Start() {
        control = GetComponent<GunnerController>();
        fuel = GetComponent<GunnerFuels>();
        shield.gameObject.SetActive(false);
    }

    public void UpdateEnergyShield() {
        if (shieldBroken) {
            DeactivateShield();
            if (fuel.shieldFuel > 0.5f) {
                shieldBroken = false;
            }
            return;
        }
        if (Mouse.current.rightButton.isPressed) {
            Dir();
            shield.transform.position = pilot.position;
            RotateShield();
            if (!shield.gameObject.activeSelf) {
                shield.gameObject.SetActive(true);
            }
            if (shield.gameObject.activeSelf) {
                shielding = true;
            }
        }
        else if (shield.gameObject.activeSelf) {
            DeactivateShield();
        }
    }

    void Dir() {
        var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        dir = (mousePos - transform.position).normalized;
    }

    void RotateShield() {
        shield.LookAt(new Vector3(shield.position.x + dir.x, shield.position.y + dir.y, 0));
    }

    public void BreakShield() {
        shieldBroken = true;
        shield.gameObject.SetActive(false);
        control.gunnerState = GunnerStates.NORMAL;
    }

    void DeactivateShield() {
        shield.gameObject.SetActive(false);
        shielding = false;
    }

    public void SetPilot(Transform pilot) {
        this.pilot = pilot;
    }
}
