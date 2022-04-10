using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour {

    public float smallToBigKbLimit = 2;

    [SerializeField] private float smallKbForce = 1;
    [SerializeField] private float bigKbForce = 9;
    

    private Rigidbody rb;


    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void SmallKB(Vector3 sourcePos) {
        var dir = (transform.position - sourcePos).normalized;
        rb.AddForce(dir * smallKbForce * rb.mass, ForceMode.Impulse);
    }

    public void BigKB(Vector3 sourcePos) {
        var dir = (transform.position - sourcePos).normalized;
        rb.velocity = (dir + Vector3.up) * bigKbForce;
    }

    public void SmallKB_WithDir(Vector3 dir) {
        rb.AddForce(dir * smallKbForce * rb.mass, ForceMode.Impulse);
    }

    public void BigKB_WithDir(Vector3 dir) {
        rb.velocity = dir * bigKbForce;
    }
}
