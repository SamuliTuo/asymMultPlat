using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public static EventManager current;
    public event Action<Vector3> onGunnerShoot;


    void Awake() {
        current = this;
    }

    public void GunnerShoot(Vector3 pos) {
        onGunnerShoot?.Invoke(pos);
    }
}
