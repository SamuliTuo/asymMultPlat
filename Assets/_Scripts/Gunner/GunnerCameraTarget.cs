using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerCameraTarget : MonoBehaviour
{
    [Header("Gunner form")]
    [SerializeField] private Transform gunner = null;
    [SerializeField] private float targetMoveSpeed = 1;
    [SerializeField] private float vertiMaxRange = 1;
    [SerializeField] private float horizMaxRange = 1;
    [Header("Mecha form")]
    [SerializeField] private Transform mecha = null;
    [SerializeField] private float targetMoveSpeed_mecha = 1;
    [SerializeField] private float returnToZeroTargMovSpd_mecha = 0.3f;
    [SerializeField] private float targetMaxRange_mecha = 1;

    private float currentTargetMoveSpeed = 0;
    private float distanceFromPilot = 0;
    private float aimX = 0;
    private float aimY = 0;

    public void MoveCameraTarget(Vector3 aimPos) {
        aimX = Mathf.MoveTowards(aimX, aimPos.x, targetMoveSpeed * Time.deltaTime);
        aimY = Mathf.MoveTowards(aimY, aimPos.y, targetMoveSpeed * Time.deltaTime);
        aimX = Mathf.Clamp(aimX, gunner.position.x - horizMaxRange * 0.5f, gunner.position.x + horizMaxRange * 0.5f);
        aimY = Mathf.Clamp(aimY, gunner.position.y - vertiMaxRange * 0.5f, gunner.position.y + vertiMaxRange * 0.5f);
        transform.position = new Vector3(aimX, aimY, 0);
    }

    public void MoveCameraTarget_mecha(float xAxis) {
        if (xAxis > 0.3f || xAxis < -0.3f) {
            currentTargetMoveSpeed = targetMoveSpeed_mecha;
        }
        else {
            currentTargetMoveSpeed = returnToZeroTargMovSpd_mecha;
        }

        distanceFromPilot = Mathf.MoveTowards(distanceFromPilot, xAxis * targetMaxRange_mecha, currentTargetMoveSpeed * Time.deltaTime);
        transform.position = new Vector3(mecha.position.x + distanceFromPilot, mecha.position.y, 0);
    }
}
