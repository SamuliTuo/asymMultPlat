using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PilotKnockback : MonoBehaviour {

    [Header("KB when getting hurt:")]
    [SerializeField] private float hurtKBHorizForce = 1;
    [SerializeField] private float hurtKBVertiForce = 1;
    [SerializeField] private float gotHurtKBDuration = 0.5f;
    [Header("KB when attacking with sword:")]
    [SerializeField] private float kbHorizontalForce = 1;
    [SerializeField] private float kbVerticalForce = 1;
    [SerializeField] private float cooldown = 1;

    private PilotController control;
    private PilotGravity gravity;
    private PilotAttack attack;
    private Rigidbody rb;
    private bool canKB = true;


    void Start() {
        control = GetComponent<PilotController>();
        gravity = GetComponent<PilotGravity>();
        attack = GetComponent<PilotAttack>();
        rb = GetComponent<Rigidbody>();
    }

    public void SelfInflictedAttackKB() {
        if (!canKB) {
            return;
        }
        switch (attack.lastAttackDir) {
            case Vector2 dir when dir.Equals(Vector2.down):
                gravity.swordJumped = true;
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                rb.AddForce(Vector3.up * kbVerticalForce, ForceMode.Impulse);
                StartCooldown();
                StartCoroutine(Cooldown());
                break;
            case Vector2 dir when dir.Equals(Vector2.up):
                if (rb.velocity.y > 0) {
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.4f, 0);
                    StartCooldown();
                }
                break;
            case Vector2 dir when dir.Equals(Vector2.right):
                rb.velocity = new Vector3(rb.velocity.x * 0.5f, rb.velocity.y, 0);
                rb.AddForce(Vector3.left * kbHorizontalForce, ForceMode.Impulse);
                StartCooldown();
                break;
            case Vector2 dir when dir.Equals(Vector2.left):
                rb.velocity = new Vector3(rb.velocity.x * 0.5f, rb.velocity.y, 0);
                rb.AddForce(Vector3.right * kbHorizontalForce, ForceMode.Impulse);
                StartCooldown();
                break;
            default:
                break;
        }
    }

    public void GotHurtKB(Vector3 sourcePos) {
        var dir = (transform.position - sourcePos);
        dir.y *= 0;
        dir = dir.normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce((dir * hurtKBHorizForce + Vector3.up * hurtKBVertiForce), ForceMode.Impulse);
        control.ResetPilot();
        control.pilotState = PilotStates.NOT_IN_CONTROL;
        StartCoroutine(GotHurtRoutine());
    }

    void StartCooldown() {
        canKB = false;
        StartCoroutine(Cooldown());
    }

    IEnumerator GotHurtRoutine() {
        yield return new WaitForSecondsRealtime(gotHurtKBDuration * 0.6f);
        if (control.PilotGrounded) {
            control.pilotState = PilotStates.NORMAL;
            yield break;
        }
        yield return new WaitForSecondsRealtime(gotHurtKBDuration * 0.4f);
        if (control.pilotState != PilotStates.IN_MECHA) {
            control.pilotState = PilotStates.NORMAL;
        }
        
    }

    IEnumerator Cooldown() {
        yield return new WaitForSeconds(cooldown);
        canKB = true;
    }
}
