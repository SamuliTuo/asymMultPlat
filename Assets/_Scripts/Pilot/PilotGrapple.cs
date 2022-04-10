using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PilotGrapple : MonoBehaviour {

    [Header("Public variables:")]
    public bool canGrapple = false;
    public float anticipationMoveSpeedMultiplier = 1;

    [Header("Exposed privates:")]
    [SerializeField] private Transform gunner = null;
    [SerializeField] private float anticipationTime = 0.3f;
    [SerializeField] private float hangTime = 1;
    [SerializeField] private float grappleSpeed = 1;
    [SerializeField] private Image grappleImage = null;

    private PilotController pilotControl;
    private GunnerController gunnerControl;
    private PilotJump jump;
    private PilotDash dash;
    private Rigidbody rb;
    private int layerMask;
    private bool started = false;
    private Vector3 startPos;
    private float t;
    private float maxT;

    void Start() {
        rb = GetComponent<Rigidbody>();
        pilotControl = GetComponent<PilotController>();
        gunnerControl = gunner.GetComponent<GunnerController>();
        jump = GetComponent<PilotJump>();
        dash = GetComponent<PilotDash>();
        layerMask = (1 << 7) | (1 << 8);
        layerMask = ~layerMask;
    }

    public void InitGrapple() {
        
        if (Gamepad.current.yButton.wasPressedThisFrame && canGrapple) {
            t = 0 - anticipationTime;
            maxT = (transform.position - gunner.position).magnitude;
            ToggleCanGrapple(false);
            started = false;
            pilotControl.pilotState = PilotStates.GRAPPLE;
            gunnerControl.gunnerState = GunnerStates.GRAPPLE;
        }
    }

    public void UpdateGrapple() {
        // Anticipation
        if (t < 0) {
            t += Time.deltaTime;
            anticipationMoveSpeedMultiplier = t * (-1) / anticipationTime;
        }
        else if (!started) {
            // Check the line of sight to gunner
            var dir = gunner.position - transform.position;
            var len = dir.magnitude;
            if (!Physics.Raycast(transform.position, dir.normalized, out _, len, layerMask)) {
                startPos = transform.position;
                anticipationMoveSpeedMultiplier = 1;
                started = true;
            }
            else {
                EndStepCleanup();
            }
        }
        // Lerp the grapple
        else if (t < maxT) {
            t += Time.deltaTime * grappleSpeed;
            var perc = (t / maxT) * (t / maxT);
            transform.position = Vector3.Lerp(startPos, gunner.position, perc);
        }
        else if (t < maxT + hangTime) {
            t += Time.deltaTime;
            transform.position = gunner.position;
            HangStepActions();
        }
        else {
            EndStepCleanup();
        }
    }

    void HangStepActions() {
        if (Gamepad.current.aButton.wasPressedThisFrame) {
            EndStepCleanup();
            jump.InitJump();
        }
        else if (Gamepad.current.bButton.wasPressedThisFrame && dash.canDash) {
            EndStepCleanup();
            dash.InitDash();
        }
        else if (Gamepad.current.yButton.wasPressedThisFrame) {
            EndStepCleanup();
        }
    }

    void EndStepCleanup() {
        rb.velocity = Vector3.zero;
        anticipationMoveSpeedMultiplier = 1;
        pilotControl.pilotState = PilotStates.NORMAL;
        gunnerControl.gunnerState = GunnerStates.NORMAL;
    }

    public void ToggleCanGrapple(bool state) {
        if (pilotControl.pilotState == PilotStates.GRAPPLE) {
            return;
        }
        canGrapple = state;
        grappleImage.gameObject.SetActive(state);
    }

    public float GetGrappleMoveModifierForGunner() {
        if (t < 0) {
            return anticipationMoveSpeedMultiplier;
        }
        else if (t < maxT) {
            return 0.3f;
        }
        else if (t < maxT + hangTime) {
            return 0.1f;
        }
        else {
            return 1;
        }
    }

    public void EndGrappleEarly() {
        t = maxT;
        anticipationMoveSpeedMultiplier = 1;
        gunnerControl.gunnerState = GunnerStates.NORMAL;
    }
}
