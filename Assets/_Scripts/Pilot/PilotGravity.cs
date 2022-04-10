using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PilotGravity : MonoBehaviour {

    public bool swordJumped = true;

    [SerializeField] private float fallGravMult = 2.2f - 1;
    [SerializeField] private float lowJumpGravMult = 2.6f - 1;

    private PilotController control;
    private Rigidbody rb;

    void Start() {
        control = GetComponent<PilotController>();
        rb = GetComponent<Rigidbody>();
    }
    public void HandleGravity() {
        if (control.pilotState == PilotStates.GRAPPLE || control.pilotState == PilotStates.DASH) {
            return;
        }
        // First apply the gravity once,
        if (control.PilotGrounded && rb.velocity.sqrMagnitude < 0.01f) {
            rb.velocity +=
                control.contactNormal *
                (Vector3.Dot(Physics.gravity, control.contactNormal) * Time.deltaTime);
        }
        else {
            rb.velocity += Physics.gravity * Time.deltaTime;
        }
        // then apply extra gravity in different situations:
        if (rb.velocity.y < 0 && !control.PilotGrounded) {
            rb.velocity += Vector3.up * Physics.gravity.y
                * fallGravMult * Time.deltaTime;
        }
        else if (rb.velocity.y > 0) {
            if (!Gamepad.current.aButton.isPressed || swordJumped) {
                rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravMult * Time.deltaTime;
            }
            else if (Gamepad.current.aButton.isPressed && control.PilotOnSteep) {
                var dot = Mathf.Abs(Vector3.Dot(Vector3.up, control.steepNormal));
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpGravMult * dot) * Time.deltaTime;
            }
        }
    }
}
