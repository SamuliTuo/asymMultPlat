using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PilotAttack : MonoBehaviour {

    [Header("Public variables:")]
    public Vector2 lastAttackDir = Vector2.zero;
    public List<GameObject> objectsHit;

    [Header("Exposed privates:")]
    [SerializeField] private float slashLifeTime = 0.05f;
    [SerializeField] private GameObject slashObj = null;
    [SerializeField] private float attackCooldown = 1;
    [SerializeField] private float dotValue = 1;
    [SerializeField] private float upValue = 1;

    private PilotController control;
    private bool canAttack = true;
    

    void Awake() {
        control = GetComponent<PilotController>();
        objectsHit = new List<GameObject>();
    }

    public void InitSlashAttack() {
        if (canAttack &&
            control.swordIsInHand && 
            Gamepad.current.xButton.wasPressedThisFrame)
        {
            StartSlash();
            SFXManager.current.PlayAudio(AudioClip.SWORD_SWING);
            StartCoroutine(AttackCooldown());
        }
    }

    void StartSlash() {
        objectsHit.Clear();
        slashObj.transform.position = transform.position;
        Quaternion rotation = Quaternion.LookRotation(AttackDirection(), Vector3.forward);
        slashObj.transform.rotation = rotation;
        slashObj.SetActive(true);
        StartCoroutine(SlashLifeTime());
    }

    Vector2 AttackDirection() {
        Vector2 input = Gamepad.current.leftStick.ReadValue();
        float dot = Vector2.Dot(Vector2.right, input);
        Vector2 dir;

        if (dot > dotValue) {
            dir = Vector2.right;
        }
        else if (dot < -dotValue) {
            dir = Vector2.left;
        }
        else if (input.y > upValue) {
            dir = Vector2.up;
        }
        else if (input.y < -upValue && !control.PilotGrounded) {
            dir = Vector2.down;
        }
        else {
            dir = -transform.right;
        }
        lastAttackDir = dir;
        return dir;
    }

    IEnumerator AttackCooldown() {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator SlashLifeTime() {
        yield return new WaitForSeconds(slashLifeTime);
        slashObj.SetActive(false);
    }
}
