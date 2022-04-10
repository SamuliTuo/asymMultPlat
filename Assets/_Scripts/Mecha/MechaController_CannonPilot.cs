using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaController_CannonPilot : MonoBehaviour {

    public static MechaController_CannonPilot current;

    [SerializeField] private PilotCameraTarget camTarget = null;

    private MechaShoot shoot;
    
    void Start() {
        current = this;
        shoot = GetComponent<MechaShoot>();
    }

    public void MechaUpdate_Pilot() {
        var aimDir = Gamepad.current.leftStick.ReadValue();
        if (aimDir.x < 0.2f && aimDir.x > -0.2f) {
            aimDir.x *= 0;
        }
        if (aimDir.y < 0.2f && aimDir.y > -0.2f) {
            aimDir.y *= 0;
        }

        camTarget.MoveCameraTarget_mecha(aimDir);
        shoot.ShootUpdate(aimDir);

        ///call from pilot update when mech is active
        ///
        ///pilot throws his sabre from a cannon or some shit
        ///sabre bounces around until calle back?
        ///sabre flies straight ahead and then comes back spinning?
    }

    public void SetPilot(GameObject pilot) {
        //this.pilot = pilot;
    }
}
