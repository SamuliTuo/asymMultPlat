using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PilotHP : MonoBehaviour {

    public float damageTakeFromSpikes = 2;

    [SerializeField] private Image hpBar = null;
    [SerializeField] private float maxHp = 3;

    private PilotKnockback kb;
    private Renderer ren;
    private Material mat;
    private Color baseColor;
    private bool invulnerable = false;
    private float hp;
    

    void Start() {
        kb = GetComponent<PilotKnockback>();
        ren = GetComponentInChildren<Renderer>();
        mat = ren.material;
        baseColor = mat.color;
        hp = maxHp;
        RefreshHPBar();
    }

    public void AddHP(float amount) {
        hp += amount;
        if (hp > maxHp) {
            hp = maxHp;
        }
        RefreshHPBar();
    }

    public void Hurt(ArrayList hurtList) {
        Hurt((float)hurtList[0], (Vector3)hurtList[1]);
    }

    public void Hurt(float damage, Vector3 sourcePosition) {
        if (invulnerable) {
            return;
        }
        invulnerable = true;
        hp -= damage;
        HitStop.current.StopTime(0.05f, 3, 0.3f);
        VFXManager.current.SpawnParticles(sourcePosition, ParticleType.PILOT_HIT);
        kb.GotHurtKB(sourcePosition);
        RefreshHPBar();
        StartCoroutine(HitRoutine());
        if (hp <= 0) {
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator HitRoutine() {
        mat.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        mat.color = Color.grey;
        yield return new WaitForSeconds(1.1f);
        invulnerable = false;
        mat.color = baseColor;
    }

    void RefreshHPBar() {
        hpBar.fillAmount = hp / maxHp;
    }
}
