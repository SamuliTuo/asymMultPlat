using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Aggro : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Pilot") {
            SendMessageUpwards("Aggro", other.transform.root);
        }
    }
}
