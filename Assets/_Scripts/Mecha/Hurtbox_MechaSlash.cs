using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox_MechaSlash : MonoBehaviour {

    [SerializeField] private float dmg = 5f;
    [SerializeField] private MechaSwordAttack attack = null;


    void Start() {
        gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Enemy" && !attack.objectsHit.Contains(other.gameObject)) {
            attack.objectsHit.Add(other.gameObject);
            VFXManager.current.SpawnParticles(
                other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position),
                ParticleType.HIT_SWORD);
            SFXManager.current.PlayAudio(AudioClip.SWORD_HIT);
            MechaActivation.current.FillMeter(1);
            var dir = (transform.position - other.transform.position).normalized;
            other.transform.root.GetComponent<Enemy_Hurt>().Hurt(dmg, transform.position, DamageTypes.BLUE);
        }
        else if (other.tag == "Projectile_enemy") {
            Destroy(other.gameObject);
        }
    }
}
