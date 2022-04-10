using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaJump : MonoBehaviour {

    [Header ("Public variables:")]
    public float lateJumpTimer_mecha = 0;
    public bool justJumped_mecha = false;

    [Header ("Exposed privates:")]
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float lateJumpMaxTime = 1;

    private MechaController_Movement control;
    private Rigidbody rb;

    void Start() {
        control = GetComponent<MechaController_Movement>();
        rb = GetComponent<Rigidbody>();
    }

    public void Jump() {
        lateJumpTimer_mecha += Time.deltaTime;
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            (control.MechaGrounded || lateJumpTimer_mecha <= lateJumpMaxTime) &&
            !justJumped_mecha) 
        {
            InitJump();
        }
    }

    public void InitJump() {
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        SFXManager.current.PlayAudio(AudioClip.PILOT_JUMP);
        StopAllCoroutines();
        StartCoroutine(JustJumped());
        lateJumpTimer_mecha = lateJumpMaxTime;
    }

    IEnumerator JustJumped() {
        justJumped_mecha = true;
        yield return new WaitForSeconds(0.2f);
        justJumped_mecha = false;
    }
}
