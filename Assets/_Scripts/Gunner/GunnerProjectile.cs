using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType {
    NULL,
    LASER,
    LASER_TRIPLE,
    LASER_CRAZY,
    CANNON,
}

public class GunnerProjectile : MonoBehaviour {

    public DamageTypes damageType = DamageTypes.NULL;
    public ProjectileType type = ProjectileType.NULL;
    public bool useGravity = false;
    public float projectileSpeed = 1f;
    public int projectileCountPerShot = 1;
    public int projectilePenetration = 0;
    public float reloadTime = 1f;
    public float lifeTime = 3f;
    public float projectileDamage = 0.3f;
    [Tooltip ("0 for perfect accuracy, bigger number = bigger spread")]
    public float accuracy = 10.0f;
    public float recoilAmount = 0.3f;
    public float screenShakeAmount = 0.5f;
    public float screenShakeMax = 0.5f;
    public float mechaMeterFillAmount = 1;

    private Vector3 shootDir;

    public void InitProjectile(Vector3 shootDir) {
        this.shootDir = shootDir;
        this.shootDir = Quaternion.Euler(0, 0, Random.Range(-accuracy, accuracy)) * this.shootDir;
        transform.rotation = Quaternion.LookRotation(this.shootDir, Vector3.up);
    }

    void Update() {
        Propagate();
        LiveOrDie();
    }

    void FixedUpdate() {
        Gravity();
    }

    void Propagate() {
        transform.position += shootDir * projectileSpeed * Time.deltaTime;
    }

    void LiveOrDie() {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            Destroy(this.gameObject);
        }
    }

    void Gravity() {
        if (useGravity) {
            shootDir += Vector3.down * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.isTrigger) {
            return;
        }
        if (other.tag == "Enemy") {
            other.transform.root.GetComponent<Enemy_Hurt>().Hurt(projectileDamage, transform.position, damageType);
            ImpactEffects();
            MechaActivation.current.FillMeter(mechaMeterFillAmount);
            if (projectilePenetration > 0) {
                projectilePenetration--;
            }
            else {
                Destroy(this.gameObject);
            }
        }
        else if (other.tag == "Breakable") {
            other.GetComponent<Breakable>().TakeDamage(projectileDamage, other.transform.position);
            ImpactEffects();
            Destroy(this.gameObject);
        }
        else if (other.tag != "Player" && 
            other.tag != "Pilot" && 
            other.tag != "Projectile_player" && 
            other.tag != "Projectile_enemy" &&
            other.tag != "Mecha"
            ) 
        {
            ImpactEffects();
            Destroy(this.gameObject);
        }
    }

    void ImpactEffects() {
        if (type == ProjectileType.LASER || type == ProjectileType.LASER_TRIPLE || type == ProjectileType.LASER_CRAZY) {
            SFXManager.current.PlayAudio(AudioClip.LASER_HIT);
            VFXManager.current.SpawnParticles(transform.position, ParticleType.HIT_LASER);
        }
        else if (type == ProjectileType.CANNON) {
            SFXManager.current.PlayAudio(AudioClip.CANNON_HIT);
            VFXManager.current.SpawnParticles(transform.position, ParticleType.HIT_CANNON);
        }
    }
}