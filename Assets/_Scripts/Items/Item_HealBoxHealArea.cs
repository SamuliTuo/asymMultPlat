using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_HealBoxHealArea : MonoBehaviour {

    [SerializeField] private float healRate = 1;
    private PilotHP hp;

    public void Init(PilotHP hp) {
        this.hp = hp;
    }

    void OnTriggerStay(Collider other) {
        if (other.transform.root.tag == "Pilot") {
            print("Pelaajaan törmätty!! Time: " + Time.time);
            hp.AddHP(healRate * Time.deltaTime);
        }
    }
}
