using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HP : MonoBehaviour {

    [SerializeField] AudioClip deathAudio = AudioClip.NULL;
    [SerializeField] AudioClip hitAudio = AudioClip.NULL;
    [SerializeField] ParticleType hitParticles = ParticleType.NULL;
    [SerializeField] ParticleType deathParticles = ParticleType.NULL;
    [SerializeField] private float maxHp = 9;
    [Header("Armor")]
    [SerializeField] private DamageTypes armorType = DamageTypes.NULL;
    [SerializeField] private float maxArmor = 10;

    private GameObject armor;
    private Material armorMat;
    private float armorHp;
    private float hp;


    void Start() {
        if (armorType != DamageTypes.NULL) {
            InitArmor();
        }
        armorHp = maxArmor;
        hp = maxHp;
    }

    public void AddHp(float amount, DamageTypes type) {
        // Have armor:
        if (amount < 0 && armorHp > 0 && armorType != DamageTypes.NULL) {
            if (armorType == type) {
                armorHp += amount * 10;
            }
            else {
                armorHp += amount * 0;
            }
            //jotain efektejä VFX ja SFX jne tänne plz!
            armorMat.SetFloat("Vector1_f258786926214a7e91587ea31032eaba", armorHp / maxArmor);
            return;
        }
        // No armor:
        hp += amount;
        if (amount < 0 && hp <= 0) {
            SFXManager.current.PlayAudio(deathAudio);
            VFXManager.current.SpawnParticles(transform.position, deathParticles);
            CameraShake.current.AddTrauma(0.3f, 0.3f);
            Destroy(this.gameObject);
        }
        else if (amount < 0) {
            StopAllCoroutines();
            SFXManager.current.PlayAudio(hitAudio);
            VFXManager.current.SpawnParticles(transform.position, hitParticles);
            CameraShake.current.AddTrauma(0.1f, 0.1f);
        }
    }

    void InitArmor() {
        GameObject outliner = Resources.Load("enemies/armorOutliner") as GameObject;
        this.armor = Instantiate(outliner, transform.position, Quaternion.identity, transform.GetChild(0)) as GameObject;
        this.armor.GetComponent<MeshFilter>().mesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        this.armor.transform.localScale = new Vector3(-this.armor.transform.localScale.x, this.armor.transform.localScale.y, this.armor.transform.localScale.z);
        armorMat = this.armor.GetComponent<Renderer>().material;
        armorMat.SetColor("Color_723c80d760bd48a48f05fdc6d63bae14", ArmorColors.current.GetArmorColor(armorType));
    }
}
