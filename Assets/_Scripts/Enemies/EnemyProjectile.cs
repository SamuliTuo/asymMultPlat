using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    [SerializeField] private DamageTypes projectileType = DamageTypes.NULL;

    public bool bouncedBack = false;


    void OnTriggerEnter(Collider other) {
        if (other.tag == "Pilot" && !bouncedBack) {
            other.transform.root.GetComponent<PilotHP>().Hurt(1, transform.position);
            Destroy(this.gameObject);
        }
        else if (other.tag == "Mecha") {
            other.transform.root.GetComponent<MechaHP>().Hurt(7f, transform.position);
            other.transform.root.GetComponent<MechaController_Movement>().SlowMovement(0.7f, 1);
            Destroy(this.gameObject);
        }
        else if (bouncedBack && other.tag == "Enemy") {
            other.transform.root.GetComponent<Enemy_Hurt>().Hurt(1, transform.position, projectileType);
            Destroy(this.gameObject);
        }
        else if (!other.isTrigger && other.tag != "Enemy" && other.tag != "Projectile_enemy") {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy() {
        if (VFXManager.current != null) {
            VFXManager.current.SpawnParticles(transform.position, ParticleType.ENEMY_PROJECTILE_HIT);
        }
    }
}
