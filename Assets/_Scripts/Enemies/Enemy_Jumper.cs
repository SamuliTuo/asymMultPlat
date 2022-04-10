using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jumper : MonoBehaviour {

    [SerializeField] private LayerMask groundCheckLayermask;
    [Tooltip("Random angle is chosen between the offset and offset + angleRangeRadians:")]
    [SerializeField] private float angleRangeRadians = 1.570796f;
    [Tooltip("Offset is counted counter clockwise starting from east:")]
    [SerializeField] private float angleOffsetRadians = 0.7853982f;
    [SerializeField] private float waitTimeMin = 0.50f;
    [SerializeField] private float waitTimeMax = 0.50f;
    [SerializeField] private float jumpForceMin = 4.0f;
    [SerializeField] private float jumpForceMax = 9.0f;

    private Enemy_ColorChanges colorScript;
    private Rigidbody rb;
    private Transform player;
    private Material mat;
    private Color baseColor;
    private bool aggro = false;
    private bool grounded = false;
    private float groundCheckerRadius;
    
    private float waitT = 1;
    private float t = 0;


    void Start() {
        colorScript = GetComponent<Enemy_ColorChanges>();
        rb = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<Renderer>().material;
        baseColor = mat.color;
        groundCheckerRadius = GetComponentInChildren<SphereCollider>().radius + 0.1f;
    }

    void Update() {
        if (!aggro) {
            JumpAround();
        }
        else if (aggro) {
            //jump towards player
        }
        else if (true) {
            //took damage -> some other behav?
        }
    }

    void FixedUpdate() {
        if (Physics.OverlapSphere(transform.position, groundCheckerRadius, groundCheckLayermask).Length > 0) {
            grounded = true;
        }
        else {
            grounded = false;
        }
    }

    void JumpAround() {
        if (!grounded) {
            return;
        }

        if (t < waitT) {
            t += Time.deltaTime;
        }
        else {
            t = 0;
            waitT = Random.Range(waitTimeMin, waitTimeMax);
            StartCoroutine(Jump());
        }
        
    }

    IEnumerator Jump() {
        yield return new WaitForSecondsRealtime(Random.Range(0.10f, 0.25f));
        rb.velocity = RandomVector2(angleRangeRadians, angleOffsetRadians) * Random.Range(jumpForceMin, jumpForceMax);
    }

    public void GotHit() {
        t += 1;
        colorScript.HitFlash();
        grounded = true;
    }

    public void Aggro(Transform player) {
        //print("aggroed!");
        this.player = player;
        //aggro = true;
    }

    Vector2 RandomVector2(float angle, float angleMin) {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
}
