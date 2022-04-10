using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ColorChanges : MonoBehaviour {

    private Material mat;
    private Color baseColor;
    private Coroutine coroutine;

    void Start() {
        mat = transform.GetChild(0).GetComponent<Renderer>().material;
        baseColor = mat.color;
    }

    public void HitFlash() {
        if (mat.color == baseColor) {
            StartCoroutine(HitFlashRoutine());
        }
    }

    public void ChangeColor(Color col) {
        StopAllCoroutines();
        mat.color = col;
    }

    public void ReturnToDefault() {
        mat.color = baseColor;
    }

    IEnumerator HitFlashRoutine() {
        mat.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        mat.color = baseColor;
    }
}
