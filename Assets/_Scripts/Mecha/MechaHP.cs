using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechaHP : MonoBehaviour {

    public float hpDecayRate = 1;

    [SerializeField] private Image hpBar = null;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float invulnerabilityTime = 1;

    private bool invulnerable = false;
    private float hp;
    private Material mat;
    private Color baseCol;


    void Start() {
        mat = GetComponentInChildren<Renderer>().material;
        baseCol = mat.color;
        transform.GetChild(0).gameObject.SetActive(false);
        hp = 0;
        RefreshHPBar();
    }

    public float AddHP(float amount) {
        hp += amount;
        RefreshHPBar();
        return hp;
    }

    public void Hurt(ArrayList hurtList) {
        Hurt((float)hurtList[0], (Vector3)hurtList[1]);
    }

    public void Hurt(float amount, Vector3 sourcePos) {
        if (!invulnerable) {
            hp -= amount;
            HitStop.current.StopTime(0.05f, 3, 0.15f);
            RefreshHPBar();
            invulnerable = true;
            StartCoroutine(HurtFlash());
        }
    }

    public void FillHP() {
        hp = maxHp;
        RefreshHPBar();
    }

    void RefreshHPBar() {
        hpBar.fillAmount = Mathf.Round(hp) / maxHp;
    }

    public IEnumerator HurtFlash() {
        mat.color = Color.white;
        yield return new WaitForSecondsRealtime(0.1f);
        mat.color = Color.grey;
        yield return new WaitForSecondsRealtime(invulnerabilityTime - 0.1f);
        invulnerable = false;
        mat.color = baseCol;
    }
}
