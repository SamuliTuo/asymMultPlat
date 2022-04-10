using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunnerShoot : MonoBehaviour {

    private GunnerGunController gunScr;
    private GunnerEnergyShield shield;
    private float recoilT;
    private bool reloading;
    private Vector3 recoilDirection;
    private GameObject projectile_current;
    private GameObject projectile_laser;
    private GameObject projectile_laser_triple;
    private GameObject projectile_laser_crazy;
    private GameObject projectile_cannon;

    void Start() {
        gunScr = GetComponentInChildren<GunnerGunController>();
        shield = GetComponent<GunnerEnergyShield>();
        LoadProjectiles();
    }

    public void ShootingUpdate(Vector3 aimDir) {
        if (Mouse.current.leftButton.isPressed && !shield.shielding) {
            if (!reloading) {
                Vector3 spawnPos = transform.position + aimDir * 0.6f;
                for (int i = 0; i < projectile_current.GetComponent<GunnerProjectile>().projectileCountPerShot; i++) {
                    GameObject clone = Instantiate(projectile_current, spawnPos, Quaternion.identity);
                    var scr = clone.GetComponent<GunnerProjectile>();
                    scr.InitProjectile(aimDir);
                    recoilDirection = -aimDir;
                    recoilT = scr.recoilAmount;
                    gunScr.StartTurretRecoil(2);
                    StartCoroutine(Reload(scr.reloadTime));
                    CameraShake.current.AddTrauma(scr.screenShakeAmount, scr.screenShakeMax);
                }
                VFXManager.current.SpawnParticles(transform.position + new Vector3(0, 0, -0.3f) - aimDir * 0.4f, ParticleType.SHELL_POOPER);
                EventManager.current.GunnerShoot(spawnPos);
                ShootFX(projectile_current, spawnPos);
            }
        }
        else ChangeWeapon();
    }

    public void Recoil() {
        if (recoilT > 0) {
            recoilT -= Time.deltaTime;
            transform.position += recoilDirection * recoilT;
        }
    }

    IEnumerator Reload(float reloadTime) {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    void ChangeWeapon() {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            projectile_current = projectile_laser;
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            projectile_current = projectile_laser_triple;
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            projectile_current = projectile_cannon;
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) {
            projectile_current = projectile_laser_crazy;
        }
    }

    void ShootFX(GameObject bullet, Vector3 pos) {
        switch (bullet.GetComponent<GunnerProjectile>().type) {
            case ProjectileType.LASER:
                SFXManager.current.PlayAudio(AudioClip.LASER_SHOOT);
                VFXManager.current.SpawnParticles(pos, ParticleType.MUZZLEF_LASER);
                break;
            case ProjectileType.CANNON:
                SFXManager.current.PlayAudio(AudioClip.CANNON_SHOOT);
                VFXManager.current.SpawnParticles(pos, ParticleType.MUZZLEF_LASER); //// make separate one for cannon(?)
                break;
            case ProjectileType.LASER_TRIPLE:
                SFXManager.current.PlayAudio(AudioClip.LASER_SHOOT);
                VFXManager.current.SpawnParticles(pos, ParticleType.MUZZLEF_LASER);
                break;
            case ProjectileType.LASER_CRAZY:
                SFXManager.current.PlayAudio(AudioClip.LASER_SHOOT);
                VFXManager.current.SpawnParticles(pos, ParticleType.MUZZLEF_LASER);
                break;
            default:
                break;
        }
    }

    void LoadProjectiles() {
        projectile_laser = Resources.Load("projectiles_gunner/projectile_laserRifle") as GameObject;
        projectile_cannon = Resources.Load("projectiles_gunner/projectile_cannon") as GameObject;
        projectile_laser_triple = Resources.Load("projectiles_gunner/projectile_laserRifle_triple") as GameObject;
        projectile_laser_crazy = Resources.Load("projectiles_gunner/projectile_laserRifle_crazy") as GameObject;
        projectile_current = projectile_laser;
    }
}
