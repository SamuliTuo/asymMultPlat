using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownSwordTrigger : MonoBehaviour {

    public List<GameObject> objectsHit = new List<GameObject>();


    void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy" && !objectsHit.Contains(other.transform.root.gameObject)) {
            HitEffects(other);
            objectsHit.Add(other.transform.root.gameObject);
            StartCoroutine(RemoveFromListWithDelay(other.transform.root.gameObject));
            MechaActivation.current.FillMeter(1);
            var dir = (transform.position - other.transform.position).normalized;
            other.transform.root.GetComponent<Enemy_Hurt>().HurtWithDir(3, dir, DamageTypes.BLUE);
        }
        else if (other.tag == "Projectile_enemy") {
            HitEffects(other);
            Destroy(other.gameObject);
        }
        else if (other.tag == "Breakable" && !objectsHit.Contains(other.gameObject)) {
            HitEffects(other);
            objectsHit.Add(other.gameObject);
            StartCoroutine(RemoveFromListWithDelay(other.gameObject));
            other.GetComponent<Breakable>().TakeDamage(1, other.transform.position);
        }
    }

    void HitEffects(Collider other) {
        SFXManager.current.PlayAudio(AudioClip.SWORD_HIT);
        VFXManager.current.SpawnParticles(
                other.ClosestPointOnBounds(transform.position),
                ParticleType.HIT_SWORD);
    }

    IEnumerator RemoveFromListWithDelay(GameObject obj) {
        yield return new WaitForSeconds(0.3f);
        if (objectsHit.Contains(obj)) {
            objectsHit.Remove(obj);
        }
    }

    void OnDisable() {
        objectsHit.Clear();
    }
}