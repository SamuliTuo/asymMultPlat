using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Transform pilotCamTarget = null;
    [SerializeField] private Transform gunnerCamTarget = null;
    [Space]
    [SerializeField] private float followSpeed = 5f;
    //[SerializeField] private float gunnerAimHorizInfluence = 0.4f;
    //[SerializeField] private float gunnerAimVerticInfluence = 1.5f;
    //[SerializeField] private float pilotAimHorizInfluence = 2f;
    [SerializeField, Range(0.0f, 1.0f)]
    [Header ("0 = target fully pilot, 1 = target fully gunner")]
    private float camTargetPreferPerc = 0.33f;

    //private Transform playerGunner;
    //private GunnerController gunnerScr;
    private CameraShake shake;
    private Vector3 camTargetPos;

    void Start() {
        //playerGunner = GameObject.Find("Player_gunner").transform;
        //gunnerScr = playerGunner.GetComponent<GunnerController>();
        shake = GetComponent<CameraShake>();
    }

    void LateUpdate() {
        CalculateCameraTargetPos();
        MoveCamera();
    }

    void CalculateCameraTargetPos() {
        camTargetPos = pilotCamTarget.position + (gunnerCamTarget.position - pilotCamTarget.position) * camTargetPreferPerc;
    }

    void MoveCamera() {
        Vector3 newPos = transform.position + (Time.deltaTime * followSpeed * (camTargetPos - transform.position));
        newPos.z = -100;
        newPos += shake.CamShakeOffsetVec3();
        transform.localRotation = Quaternion.Euler(0, 0, 0 + shake.CamShakeRotationAngle());
        transform.position = newPos;
    }
}