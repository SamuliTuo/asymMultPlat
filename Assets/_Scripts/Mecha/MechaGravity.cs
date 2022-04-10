using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaGravity : MonoBehaviour {

    [SerializeField] private float fallGravMult = 2.2f - 1;
    [SerializeField] private float lowJumpGravMult = 2.6f - 1;

    private MechaController_Movement control;
    private MechaFuel fuel;
    private Rigidbody rb;

    void Start() {
        control = GetComponent<MechaController_Movement>();
        fuel = GetComponent<MechaFuel>();
        rb = GetComponent<Rigidbody>();
    }
    public void HandleGravity() {
        // First apply the gravity once,
        if (control.MechaGrounded && rb.velocity.sqrMagnitude < 0.01f) {
            rb.velocity +=
                control.contactNormal *
                (Vector3.Dot(Physics.gravity, control.contactNormal) * Time.deltaTime);
        }
        else {
            rb.velocity += Physics.gravity * Time.deltaTime;
        }
        // then apply extra gravity in different situations:
        if (rb.velocity.y < 0 && !control.MechaGrounded) {
            if (Keyboard.current.spaceKey.isPressed && fuel.ConsumeFuel(control.flyingFuelConsumptionRate)) {
                return;
            }
            rb.velocity += Vector3.up * Physics.gravity.y * fallGravMult * Time.deltaTime;
        }
        else if (rb.velocity.y > 0) {
            if (Keyboard.current.spaceKey.isPressed && control.MechaOnSteep) {
                var dot = Mathf.Abs(Vector3.Dot(Vector3.up, control.steepNormal));
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpGravMult * dot) * Time.deltaTime;
            }
            else if (Keyboard.current.spaceKey.isPressed && fuel.ConsumeFuel(control.flyingFuelConsumptionRate)) {
                return;
            }
            else {
                rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravMult * Time.deltaTime;
            }
        }
    }
}
