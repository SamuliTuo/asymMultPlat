using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaShoot : MonoBehaviour {

    private bool reloading = false;
    private GameObject projectile_current;
    private GameObject projectile_mechaCannon;

    void Start() {
        LoadProjectiles();
    }

    public void ShootUpdate(Vector3 aimDir) {
        if (Gamepad.current.rightTrigger.isPressed) {
            if (!reloading) {
                if (aimDir == Vector3.zero) {
                    aimDir = -transform.right;
                }
                Vector3 spawnPos = transform.position + aimDir * 0.6f;
                for (int i = 0; i < projectile_current.GetComponent<GunnerProjectile>().projectileCountPerShot; i++) {
                    GameObject clone = Instantiate(projectile_current, spawnPos, Quaternion.identity);
                    var scr = clone.GetComponent<GunnerProjectile>();
                    scr.InitProjectile(aimDir);
                    //recoilDirection = -aimDir;
                    //recoilT = scr.recoilAmount;
                    //gunScr.StartTurretRecoil(2);
                    StartCoroutine(Reload(scr.reloadTime));
                    CameraShake.current.AddTrauma(scr.screenShakeAmount, scr.screenShakeMax);
                }
                VFXManager.current.SpawnParticles(transform.position + new Vector3(0, 0, -0.3f) - aimDir * 0.4f, ParticleType.SHELL_POOPER);
                EventManager.current.GunnerShoot(spawnPos);
                ShootFX(projectile_current, spawnPos);
            }
        }
    }

    IEnumerator Reload(float reloadTime) {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    void ShootFX(GameObject bullet, Vector3 pos) {
        SFXManager.current.PlayAudio(AudioClip.CANNON_SHOOT);
        VFXManager.current.SpawnParticles(pos, ParticleType.MUZZLEF_LASER);
    }

    void LoadProjectiles() {
        projectile_mechaCannon = Resources.Load("projectiles_mecha/projectile_mechaCannon") as GameObject;
        projectile_current = projectile_mechaCannon;
    }
}
