using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldObject : MonoBehaviour {

    [SerializeField] private GunnerFuels fuel = null;
    [SerializeField] private GunnerEnergyShield shieldScript = null;
    [SerializeField] private float extraDirectionalSpeed = 10f;
    [SerializeField] private float moveAwayOnBounceDistance = 0.6f;
    [SerializeField] private float shieldFuelConsumption = 0.06f;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Projectile_enemy") {
            var script = other.GetComponent<EnemyProjectile>();
            if (!script.bouncedBack) {
                script.bouncedBack = true;
                other.transform.position += shieldScript.dir * moveAwayOnBounceDistance;
                var rb = other.GetComponent<Rigidbody>();
                rb.velocity = Vector3.Reflect(rb.velocity, (other.transform.position - transform.position).normalized);
                rb.AddForce(shieldScript.dir * extraDirectionalSpeed, ForceMode.Impulse);
                fuel.shieldFuel -= shieldFuelConsumption;
                if (fuel.shieldFuel < 0) {
                    shieldScript.BreakShield();
                }
            }
        }
    }
}
