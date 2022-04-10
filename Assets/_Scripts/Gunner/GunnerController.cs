using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GunnerStates {
    NORMAL,
    GRAPPLE,
    IN_MECHA
}

public class GunnerController : MonoBehaviour {

    [Header("Public variables:")]
    public Vector3 aimDir;
    public Vector3 mousePos, targetMovePosition, followPos;
    public Transform pilot;
    public GunnerStates gunnerState = GunnerStates.NORMAL;
    public float followMaxOffset = 1f;

    [Header("Exposed privates:")]
    [SerializeField] private Vector3 shieldingPos = Vector3.zero;
    [SerializeField] private float followPosUpOffset = 1f;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float untetheredMoveSpeed = 0.4f;
    [SerializeField] private float grappledMoveSpeed = 10f;
    [SerializeField] private GunnerCameraTarget camTarget = null;

    private GunnerTetheringToPilot tether;
    private GunnerFuels fuel;
    private GunnerEnergyShield shield;
    private GunnerShoot shoot;
    private PilotGrapple grapple;
    private Vector3 followOffset;
    private float xAxis, yAxis;
    private int flyingLayerMask;


    void Start() {
        shoot = GetComponent<GunnerShoot>();
        tether = GetComponent<GunnerTetheringToPilot>();
        fuel = GetComponent<GunnerFuels>();
        shield = GetComponent<GunnerEnergyShield>();
        shield.SetPilot(pilot);
        grapple = pilot.GetComponent<PilotGrapple>();
        flyingLayerMask = (1 << 7) | (1 << 8);
        flyingLayerMask = ~flyingLayerMask;
        followPos = Vector3.up * followPosUpOffset;
    }

    void Update() {
        // Piloting the mecha
        if (gunnerState == GunnerStates.IN_MECHA) {
            MechaController_Movement.current.MechaUpdate_Gunner();
            return;
        }
        MechaActivation.current.ActivateMecha_Gunner();

        // Playing as gunner
        Axes();
        AimDir();
        camTarget.MoveCameraTarget(mousePos);
        shield.UpdateEnergyShield();

        if (gunnerState == GunnerStates.GRAPPLE) {
            tether.TetherToPilot();
            Grappled_MoveMovementTarget();
        }
        else if (shield.shielding) {
            Shielding_MoveMovementTarget();
            return;
        }
        else if (gunnerState == GunnerStates.NORMAL) {
            tether.Tethering(xAxis, yAxis);
            if (tether.tethered) {
                Tethered_MoveMovementTarget();
            }
            else {
                Untethered_MoveMovementTarget();
            }
        }
        shoot.ShootingUpdate(aimDir);
    }

    void LateUpdate() {
        if (gunnerState == GunnerStates.IN_MECHA) {
            return;
        }
        FollowMovementTarget();
        shoot.Recoil();
    }

    void FixedUpdate() {
        if (gunnerState == GunnerStates.IN_MECHA) {
            MechaController_Movement.current.Mecha_Gunner_FixedUpdate();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    void AimDir() {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        aimDir = (mousePos - transform.position).normalized;
    }

    void Axes() {
        xAxis = yAxis = 0;
        if (Keyboard.current.wKey.isPressed) yAxis += 1;
        if (Keyboard.current.aKey.isPressed) xAxis -= 1;
        if (Keyboard.current.sKey.isPressed) yAxis -= 1;
        if (Keyboard.current.dKey.isPressed) xAxis += 1;
    }

    void Shielding_MoveMovementTarget() {
        targetMovePosition = pilot.position + shieldingPos;
    }

    void Tethered_MoveMovementTarget() {
        followOffset = new Vector3(xAxis, yAxis, 0).normalized * followMaxOffset;
        targetMovePosition = pilot.position + followPos + followOffset;
    }

    void Untethered_MoveMovementTarget() {
        var dir = new Vector3(xAxis, yAxis, 0);
        var dist = Mathf.Max(1, (targetMovePosition - (pilot.position + followPos)).magnitude - followMaxOffset);
        if (!Physics.Raycast(targetMovePosition, dir, out _, 0.6f, flyingLayerMask)) {
            targetMovePosition = Vector3.MoveTowards(
                targetMovePosition, 
                targetMovePosition + dir, 
                untetheredMoveSpeed / dist * Time.deltaTime);
        }
    }

    void Grappled_MoveMovementTarget() {
        var dir = new Vector3(xAxis, yAxis, 0);
        var dist = Mathf.Max(1, (targetMovePosition - (pilot.position + followPos)).magnitude - followMaxOffset);
        if (!Physics.Raycast(targetMovePosition, dir, out _, 0.6f, flyingLayerMask)) {
            targetMovePosition = Vector3.MoveTowards(
                targetMovePosition, 
                targetMovePosition + dir, 
                grapple.GetGrappleMoveModifierForGunner() * grappledMoveSpeed / dist * Time.deltaTime);
        }
    }

    void FollowMovementTarget() {
        var dir = targetMovePosition - transform.position;
        if (shield.shielding) {
            transform.position += dir * followSpeed * 2.5f * Time.deltaTime;
        }
        else {
            transform.position += dir * followSpeed * Time.deltaTime;
        }
    }

    public void SetupGunnerForMechaMode() {
        gunnerState = GunnerStates.IN_MECHA;
        for (int i = 0; i < 3; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ReturnFromMechaMode() {
        gunnerState = GunnerStates.NORMAL;
        fuel.FillUpFuels();
        for (int i = 0; i < 3; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
