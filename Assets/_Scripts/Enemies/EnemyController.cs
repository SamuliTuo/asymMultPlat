using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float screenShake_HIT = 0.01f;
    [SerializeField] private float screenShake_DEAD = 0.1f;
    [SerializeField] private float maxScreenShakeMagnitude_HIT = 0.4f;
    [SerializeField] private float maxScreenShakeMagnitude_DEAD = 0.4f;

    private float t = 0;
    private float nextDirectionTimer = 1;
    private Vector3 moveDir;

    void Start() {
        moveDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        moveDir = moveDir.normalized;
    }

    void FixedUpdate() {
        Move();
    }

    void GotHit() {
        //olen vain testi-lentelijä, älä minnuu huomiijoi...
    }

    void Move() {
        t += Time.deltaTime;
        if (t >= nextDirectionTimer) {
            moveDir += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            moveDir = moveDir.normalized;
            nextDirectionTimer = Random.Range(0.5f, 2.0f);
            t = 0;
        }
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}