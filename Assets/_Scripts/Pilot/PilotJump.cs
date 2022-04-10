using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PilotJump : MonoBehaviour {

    public float lateJumpTimer = 0;
    public bool justJumped = false;

    [Space]
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float lateJumpMaxTime = 1;

    private PilotController control;
    private Rigidbody rb;

    void Start() {
        control = GetComponent<PilotController>();
        rb = GetComponent<Rigidbody>();
    }

    public void Jump() {
        lateJumpTimer += Time.deltaTime;
        if (
            Gamepad.current.aButton.wasPressedThisFrame &&
            (control.PilotGrounded || lateJumpTimer <= lateJumpMaxTime) &&
            !justJumped
            )
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
        lateJumpTimer = lateJumpMaxTime;
    }

    IEnumerator JustJumped() {
        justJumped = true;
        yield return new WaitForSeconds(0.2f);
        justJumped = false;
    }
}
