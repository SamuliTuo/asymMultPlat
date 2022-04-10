using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour {

    private GameObject projectile;

    void Start() {
        projectile = Resources.Load("enemies/projectiles/EnemyProjectile_basic") as GameObject;

        StartCoroutine(ProjectileTest());
    }

    IEnumerator ProjectileTest() {
        float wait = Random.Range(1.5f, 3);
        yield return new WaitForSeconds(wait);

        //make targeting nicer, add aggro stuff etc.
        var pilot = GameObject.Find("Player_pilot");
        var target = pilot.transform.GetChild(0).gameObject.activeSelf ? pilot : GameObject.Find("mechaUltimate");
        var dir = (target.transform.position - transform.position).normalized;
        //

        GameObject clone = Instantiate(projectile, transform.position, Quaternion.identity);
        clone.GetComponent<Rigidbody>().AddForce(dir * 3, ForceMode.Impulse);

        StartCoroutine(ProjectileTest());
    }
}
