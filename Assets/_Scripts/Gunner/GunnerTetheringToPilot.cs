using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GunnerTetheringToPilot : MonoBehaviour {

    public bool tethered = true;

    [SerializeField] private float fuelConsuRateMoving = 1f;
    [SerializeField] private float fuelConsuRateStill = 0.25f;

    private GunnerController control;
    private GunnerFuels fuel;
    private Rigidbody rb;
    
    void Start() {
        control = GetComponent<GunnerController>();
        fuel = GetComponent<GunnerFuels>();
        rb = GetComponent<Rigidbody>();
    }

    public void Tethering(float xAxis, float yAxis) {
        // Consume fuel
        if (!tethered) {
            if (xAxis == 0 && yAxis == 0) {
                fuel.ConsumeFlyFuel(fuelConsuRateStill);
            }
            else {
                fuel.ConsumeFlyFuel(fuelConsuRateMoving);
            }
        }
        // Activate and deactivate tethering
        if (Keyboard.current.spaceKey.isPressed && fuel.flyFuel > 0) {
            if (tethered && fuel.flyFuel > 0.3f) {
                tethered = false;
                rb.isKinematic = false;
                control.targetMovePosition = transform.position;
            }
        }
        else {
            TetherToPilot();
        }
    }

    public void TetherToPilot() {
        rb.isKinematic = true;
        tethered = true;
    }
}
