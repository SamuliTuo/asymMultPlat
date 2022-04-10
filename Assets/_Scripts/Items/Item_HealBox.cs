using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_HealBox : MonoBehaviour {

    [SerializeField] private float lifeTime = 7;
    
    public void InitItem(PilotHP hp) {
        BroadcastMessage("Init", hp);
        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
