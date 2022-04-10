using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInstance_ExplosionTriggerZone : MonoBehaviour {

    private List<GameObject> objectsHit = new List<GameObject>();
    private float dmg;
    private float pushF;

    public void Init(float dmg, float pushF) {
        this.dmg = dmg;
        this.pushF = pushF;
    }

    void OnTriggerEnter(Collider other) {
        if (!objectsHit.Contains(other.transform.root.gameObject) && (other.transform.root.tag == "Pilot" || other.transform.root.tag == "Mecha")) {
            objectsHit.Add(other.transform.root.gameObject);
            ArrayList hurtList = new ArrayList();
            hurtList.Add(dmg);
            hurtList.Add(transform.position);
            other.transform.root.SendMessage("Hurt", hurtList);
        }
    }
}
