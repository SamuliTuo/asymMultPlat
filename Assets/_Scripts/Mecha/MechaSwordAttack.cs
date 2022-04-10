using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaSwordAttack : MonoBehaviour {

    [Header("Public variables:")]
    public Vector2 lastAttackDir = Vector2.zero;
    public List<GameObject> objectsHit;

    [Header("Exposed privates:")]
    [SerializeField] private float slashLifeTime = 0.05f;
    [SerializeField] private GameObject slashObj = null;
    [SerializeField] private float attackCooldown = 1;
    [SerializeField] private float dotValue = 1;
    [SerializeField] private float upValue = 1;
    [SerializeField] private float attackSpeed = 1;

    private MechaController_Movement control;
    private bool canAttack = true;
    private Vector2 axes;
    private float t;
    

    void Start() {
        control = GetComponent<MechaController_Movement>();
        objectsHit = new List<GameObject>();
    }

    public void InitSlashAttack(Vector2 axes) {
        this.axes = axes;
        if (canAttack && Mouse.current.leftButton.wasPressedThisFrame) {
            //control.mechaState = MechaMovementStates.ATTACKING;
            canAttack = false;
            StartSlash();
        }
    }

    void StartSlash() {
        objectsHit.Clear();
        slashObj.transform.position = transform.position;
        Quaternion rotation = Quaternion.LookRotation(AttackDirection(), Vector3.forward);
        slashObj.transform.rotation = rotation;
        slashObj.SetActive(true);
        SFXManager.current.PlayAudio(AudioClip.SWORD_SWING);
        StartCoroutine(SlashLifeTime());
        StartCoroutine(AttackCooldown());
        control.SlowMovement(0, 2f);

        /// Iseasiassa muuta t�� koko homma viel� koska ly�nnin odottelu ei tunnu hyv�lt�.
        /// Tee ly�nti tapahtumaan viiveett� mutta laita sen j�lkeen hidasteluphase.
        /// Jos ly� ilmassa niin ei tota moveModifieria koska nyt se pys�ytt�� ilmaan.
        /// Sen sijaan ilmassa voi vaikka tulla vauhdilla alas sen ly�nnin per�ss� ja tehd� BOOMBOOM siihen mihin laskeutuu.
        /// Hypyst� vois muutenkin tulla pieni boom laskeutumispaikkaan.
        /// Ja isompi boom jos ly�, plus se j�� pitemm�ks aikaa paikalleen.
        /// My�s j�t� paikalleen ihan vaan laskeutuessakin (my�s se boom niin se on ihan fine varmaan)
        /// Eli joku async metodi tonne controlleriin jonka voi kutsua eri paikoista hidastaakseen menoa.
        /// Sit se async ottaa arvoiks (kuinka hitaaksi laitetaan, kuinka nopeasti palautuu)
    }

    Vector2 AttackDirection() {
        float dot = Vector2.Dot(Vector2.right, axes);
        Vector2 dir;

        if (dot > dotValue) {
            dir = Vector2.right;
        }
        else if (dot < -dotValue) {
            dir = Vector2.left;
        }
        else if (axes.y > upValue) {
            dir = Vector2.up;
        }
        else if (axes.y < -upValue && !control.MechaGrounded) {
            dir = Vector2.down;
        }
        else {
            dir = -transform.right;
        }
        lastAttackDir = dir;
        return dir;
    }

    IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator SlashLifeTime() {
        yield return new WaitForSeconds(slashLifeTime);
        slashObj.SetActive(false);
    }
}
