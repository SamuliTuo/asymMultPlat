using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DamageInstance_Explosion : MonoBehaviour {

    private float lerpSpeed;
    private float minSize;
    private float maxSize;
    private float t;


    public void InitDamageInstance(float lerpSpeed, float minSize, float maxSize, float pushF, float dmg) {
        GetComponentInChildren<DamageInstance_ExplosionTriggerZone>().Init(dmg, pushF);
        this.lerpSpeed = lerpSpeed;
        this.minSize = minSize;
        this.maxSize = maxSize;
        t = 0;
        LerpExplosion();
    }

    public async void LerpExplosion() {
        while (t < 1) {
            t += Time.deltaTime * lerpSpeed;
            var perc = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.GetChild(0).localScale = 
                Vector3.Lerp(new Vector3(minSize, minSize, minSize), 
                new Vector3(maxSize, maxSize, maxSize),
                perc);

            await Task.Yield();
        }
        if (gameObject != null) {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy() {
        t = 1;
    }
}
