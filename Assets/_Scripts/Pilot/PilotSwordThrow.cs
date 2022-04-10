using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PilotSwordThrow : MonoBehaviour {

    [SerializeField] private Transform swordTransform = null;
    [SerializeField] private float throwSpeed = 30;
    [SerializeField] private float returnSpeed = 40;
    [SerializeField] private float swordSpawnDistanceFromPilot = 1;
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private float catchDistance = 1;

    private PilotController control;
    private Rigidbody swordRB;
    private Vector3 throwDir;
    private int bounceLayerMask;
    private bool halfway, pullBackEarly;
    private float spinDirection;
    private float t;
    

    void Start() {
        control = GetComponent<PilotController>();
        swordRB = swordTransform.GetComponent<Rigidbody>();
        bounceLayerMask = (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10);
        bounceLayerMask = ~bounceLayerMask;
    }

    public void InitSwordThrow() {
        //AimBeam();
        if (control.swordIsInHand && Gamepad.current.rightTrigger.wasPressedThisFrame) {
            throwDir = Gamepad.current.rightStick.ReadValue();
            if (throwDir.magnitude < 0.2f) {
                throwDir = -transform.right;
            }
            throwDir = throwDir.normalized;
            swordTransform.position = 
                transform.position + new Vector3(throwDir.x, throwDir.y, 0) * swordSpawnDistanceFromPilot;
            Quaternion rotation = Quaternion.LookRotation(throwDir, Vector3.forward);
            swordTransform.rotation = rotation;
            swordTransform.gameObject.SetActive(true);
            swordRB.velocity = Vector3.zero;
            spinDirection = Random.value < 0.5f ? -1 : 1;
            halfway = false;
            pullBackEarly = false;
            control.swordIsInHand = false;
            t = 0;
        }
    }

    public void UpdateSwordThrow() {
        LerpThrow();
        PullBackEarly();
    }

    void AimBeam() {
        Vector2 dir = Gamepad.current.rightStick.ReadValue();
        //if (dir.SqrMagnitude > 0.15f) {
            //make em aim
        //}
    }

    void LerpThrow() {
        t += Time.deltaTime * lerpSpeed;
        // Fly away
        if (t < 1) {
            if (pullBackEarly) {
                t += Time.deltaTime * 4;
            }
            var perc = t * t;
            swordRB.velocity = throwDir * Mathf.Lerp(throwSpeed, 0, perc);
            BounceFromCollisions();
        }
        else if (!halfway) {
            t = 1;
            swordRB.velocity = Vector3.zero;
            halfway = true;
        }
        // Pull back
        else {
            var newDir = transform.position - swordTransform.position;
            if (t < 2) {
                var perc = (t - 1) * (t - 1);
                var rotSpeed = perc * 800 + 200;
                swordTransform.Rotate(0, rotSpeed * spinDirection * Time.deltaTime, 0);
                swordRB.velocity = newDir.normalized * Mathf.Lerp(0, returnSpeed, perc);
                if (newDir.sqrMagnitude < catchDistance) {
                    CatchSword();
                }
            }
            else {
                swordTransform.Rotate(0, 1000 * spinDirection * Time.deltaTime, 0);
                swordRB.velocity = newDir.normalized * returnSpeed;
                if (newDir.sqrMagnitude < catchDistance) {
                    CatchSword();
                }
            }
        }
    }

    void CatchSword() {
        swordTransform.gameObject.SetActive(false);
        control.swordIsInHand = true;
        t = 2;
    }

    void BounceFromCollisions() {
        RaycastHit hit;
        if (Physics.Raycast(swordTransform.position, throwDir, out hit, 1, bounceLayerMask)) {
            if (hit.collider.isTrigger) {
                return;
            }
            throwDir = Vector3.Reflect(throwDir, hit.normal);
            Quaternion rotation = Quaternion.LookRotation(throwDir, Vector3.forward);
            swordTransform.rotation = rotation;
            VFXManager.current.SpawnParticles(hit.point, ParticleType.HIT_SWORD);
        }
    }

    void PullBackEarly() {
        if (!Gamepad.current.rightTrigger.isPressed) {
            pullBackEarly = true;
        }
    }
}
