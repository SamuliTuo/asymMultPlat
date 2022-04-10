using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    [SerializeField] private float hp = 4;
    [SerializeField] private GameObject noDmg = null;
    [SerializeField] private GameObject lowDmg = null;
    [SerializeField] private GameObject midDmg = null;
    [SerializeField] private GameObject highDmg = null;


    public void TakeDamage(float amount, Vector3 hitPos) {
        hp -= amount;
        if (hp <= 0) {
            VFXManager.current.SpawnParticles(hitPos, ParticleType.HIT_CANNON);
            Destroy(gameObject);
        }
        else {
            VFXManager.current.SpawnParticles(hitPos, ParticleType.HIT_SWORD);
        }

        if (hp > 2 && hp <= 3) {
            noDmg.SetActive(false);
            lowDmg.SetActive(true);
            midDmg.SetActive(false);
            highDmg.SetActive(false);
        }
        else if (hp > 1 && hp <= 2) {
            noDmg.SetActive(false);
            lowDmg.SetActive(false);
            midDmg.SetActive(true);
            highDmg.SetActive(false);
        }
        else if (hp > 0 && hp <= 1) {
            noDmg.SetActive(false);
            lowDmg.SetActive(false);
            midDmg.SetActive(false);
            highDmg.SetActive(true);
        }
    }
}
