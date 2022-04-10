using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hurt : MonoBehaviour {

    private Enemy_HP hp;
    private Enemy_Knockback kb;

    void Start() {
        hp = GetComponent<Enemy_HP>();
        kb = GetComponent<Enemy_Knockback>();
    }


    public void Hurt(float damage, Vector3 sourcePos, DamageTypes type) {
        if (damage < kb.smallToBigKbLimit) {
            kb.SmallKB(sourcePos);
        }
        else {
            SendMessage("GotHit");
            kb.BigKB(sourcePos);
            HitStop.current.StopTime(0, 10, 0.1f);
        }
        hp.AddHp(-damage, type);
    }

    public void HurtWithDir(float damage, Vector3 dir, DamageTypes type) {
        if (damage < kb.smallToBigKbLimit) {
            kb.SmallKB_WithDir(dir);
        }
        else {
            SendMessage("GotHit");
            kb.BigKB(dir);
            HitStop.current.StopTime(0, 10, 0.1f);
        }
        hp.AddHp(-damage, type);
    }
}
