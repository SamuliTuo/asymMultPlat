using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PilotDash : MonoBehaviour {

    [Header ("Public variables:")]
    public bool canDash = true;
    public float dashCooldown = 1f;
    public float currentDashCooldown = 0;

    [Header("Exposed privates:")]
    [SerializeField] private float dashGravityReturnTime = 0.3f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private Image dashSymbol = null;

    private PilotController control;
    private Rigidbody rb;

    void Start() {
        control = GetComponent<PilotController>();
        rb = GetComponent<Rigidbody>();
    }

    public void Dash() {
        if (currentDashCooldown > 0) {
            currentDashCooldown -= Time.deltaTime;
        }
        else if (Gamepad.current.bButton.wasPressedThisFrame && canDash) {
            InitDash();
        }
    }

    public void InitDash() {
        rb.velocity = Vector3.zero;
        rb.AddForce(-transform.right * dashSpeed, ForceMode.Impulse);
        currentDashCooldown = dashCooldown;
        control.pilotState = PilotStates.DASH;
        ToggleCanDash(false);
        StartCoroutine(DashStages());
    }

    IEnumerator DashStages() {
        yield return new WaitForSecondsRealtime(dashGravityReturnTime);
        control.pilotState = PilotStates.NORMAL;
        if (control.PilotGrounded) {
            ToggleCanDash(true);
        }
    }

    public void ToggleCanDash(bool state) {
        canDash = state;
        dashSymbol.gameObject.SetActive(state);
    }
}
