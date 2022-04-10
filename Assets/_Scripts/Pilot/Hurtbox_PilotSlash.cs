using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox_PilotSlash : MonoBehaviour {

    [SerializeField] private PilotAttack attack = null;
    [SerializeField] private PilotKnockback kb = null;


    void Start() {
        gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "Enemy" && !attack.objectsHit.Contains(other.transform.root.gameObject)) {
            attack.objectsHit.Add(other.transform.root.gameObject);
            VFXManager.current.SpawnParticles(
                other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position),
                ParticleType.HIT_SWORD);
            SFXManager.current.PlayAudio(AudioClip.SWORD_HIT);
            MechaActivation.current.FillMeter(1);
            other.transform.root.GetComponent<Enemy_Hurt>().Hurt(3, transform.position, DamageTypes.BLUE);
            kb.SelfInflictedAttackKB();
        }
        else if (other.tag == "Spike") {
            attack.objectsHit.Add(other.gameObject);
            VFXManager.current.SpawnParticles(
                other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position),
                ParticleType.HIT_SWORD);
            SFXManager.current.PlayAudio(AudioClip.SWORD_HIT);
            kb.SelfInflictedAttackKB();
        }
        else if (other.tag == "Projectile_enemy") {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Breakable" && !attack.objectsHit.Contains(other.gameObject)) {
            attack.objectsHit.Add(other.gameObject);
            kb.SelfInflictedAttackKB();
            other.GetComponent<Breakable>().TakeDamage(1, other.transform.position);
        }
    }
}
