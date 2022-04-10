using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotCameraTarget : MonoBehaviour {

    [Header("Pilot form")]
    [SerializeField] private Transform pilot = null;
    [SerializeField] private float targetMoveSpeed = 1;
    [SerializeField] private float returnToZeroTargMovSpd = 0.3f;
    [SerializeField] private float targetMaxRange = 1;
    [Header("Mecha form")]
    [SerializeField] private Transform mecha = null;
    [SerializeField] private float targetMoveSpeed_mecha = 1;
    [SerializeField] private float vertiMaxRange_mecha = 1;
    [SerializeField] private float horizMaxRange_mecha = 1;
    
    private float currentTargetMoveSpeed = 0;
    private float distanceFromPilot = 0;
    private float aimX = 0;
    private float aimY = 0;

    void Start() {
        horizMaxRange_mecha *= 0.5f;
        vertiMaxRange_mecha *= 0.5f;
    }

    public void MoveCameraTarget(float xAxis) {
        if (xAxis > 0.3f || xAxis < -0.3f) {
            currentTargetMoveSpeed = targetMoveSpeed;
        }
        else {
            currentTargetMoveSpeed = returnToZeroTargMovSpd;
        }
        distanceFromPilot = Mathf.MoveTowards(distanceFromPilot, xAxis * targetMaxRange, currentTargetMoveSpeed * Time.deltaTime);
        transform.position = new Vector3(pilot.position.x + distanceFromPilot, pilot.position.y, 0);
    }

    public void MoveCameraTarget_mecha(Vector3 aimDir) {
        aimX = Mathf.MoveTowards(aimX, mecha.position.x + aimDir.x * horizMaxRange_mecha, targetMoveSpeed_mecha * Time.deltaTime);
        aimY = Mathf.MoveTowards(aimY, mecha.position.y + aimDir.y * vertiMaxRange_mecha, targetMoveSpeed_mecha * Time.deltaTime);
        aimX = Mathf.Clamp(aimX, mecha.position.x - horizMaxRange_mecha, mecha.position.x + horizMaxRange_mecha);
        aimY = Mathf.Clamp(aimY, mecha.position.y - vertiMaxRange_mecha, mecha.position.y + vertiMaxRange_mecha);
        transform.position = new Vector3(aimX, aimY, 0);
    }
}
