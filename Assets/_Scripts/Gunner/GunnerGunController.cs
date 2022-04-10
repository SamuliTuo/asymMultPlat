using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerGunController : MonoBehaviour {

    [SerializeField] private float recoilDistance = 0.3f;

    private float t = 1;
    private float lerpSpeed = 1;
    private GunnerController controller;
    private Transform gun;

    void Start() {
        controller = GetComponentInParent<GunnerController>();
        gun = transform.GetChild(0);
    }

    void Update() {
        RotateTurret();
        LerpRecoil();
    }

    void RotateTurret() {
        transform.right = -controller.aimDir;
    }

    void LerpRecoil() {
        if (t < 0.5f) {
            t += Time.deltaTime * lerpSpeed;
            float perc = 4 * t * t * t;
            gun.localPosition = new Vector3(Mathf.Lerp(recoilDistance, 0, perc), 0, -0.25f);
        }
        else if (t >= 0.5f && t < 1) {
            t += Time.deltaTime * lerpSpeed;
            float perc = 1 - Mathf.Pow(-2 * t + 2, 3) * 0.5f;
            gun.localPosition = new Vector3(Mathf.Lerp(recoilDistance, 0, perc), 0, -0.25f);
        }
        else
            gun.localPosition = new Vector3(0, 0, -0.25f);
    }

    public void StartTurretRecoil(float lerpSpeed) {
        t = 0;
        this.lerpSpeed = lerpSpeed;
    }
}