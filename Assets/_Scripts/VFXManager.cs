using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType {
    NULL,
    HIT_LASER,
    HIT_CANNON,
    HIT_SWORD,
    ENEMY_HIT,
    ENEMY_DEATH,
    MUZZLEF_LASER,
    SHELL_POOPER,
    ENEMY_PROJECTILE_HIT,
    PILOT_HIT
}

public class VFXManager : MonoBehaviour {

    public static VFXManager current;

    [SerializeField] private ParticleSystem hit_laser = null;
    [SerializeField] private ParticleSystem hit_cannon = null;
    [SerializeField] private ParticleSystem hit_sword = null;
    [SerializeField] private ParticleSystem enemy_hit = null;
    [SerializeField] private ParticleSystem enemy_death = null;
    [SerializeField] private ParticleSystem muzzleFlash_laser = null;
    [SerializeField] private ParticleSystem shellSpawner = null;
    [SerializeField] private ParticleSystem enemy_projectileHit = null;
    [SerializeField] private ParticleSystem pilot_hit = null;

    private Dictionary<ParticleType, ParticleSystem> systems = new Dictionary<ParticleType, ParticleSystem>();


    void Start() {
        current = this;
        systems.Add(ParticleType.HIT_LASER, hit_laser);
        systems.Add(ParticleType.HIT_CANNON, hit_cannon);
        systems.Add(ParticleType.HIT_SWORD, hit_sword);
        systems.Add(ParticleType.ENEMY_HIT, enemy_hit);
        systems.Add(ParticleType.ENEMY_DEATH, enemy_death);
        systems.Add(ParticleType.MUZZLEF_LASER, muzzleFlash_laser);
        systems.Add(ParticleType.SHELL_POOPER, shellSpawner);
        systems.Add(ParticleType.ENEMY_PROJECTILE_HIT, enemy_projectileHit);
        systems.Add(ParticleType.PILOT_HIT, pilot_hit);
    }

    public void SpawnParticles(Vector3 pos, ParticleType type) {
        if (type != ParticleType.NULL) {
            ParticleSystem system;
            systems.TryGetValue(type, out system);
            system.transform.position = pos;
            system.Play();
        }
    }
}