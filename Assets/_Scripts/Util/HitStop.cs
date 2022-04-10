using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour {

    public static HitStop current;

    private float restoreSpeed;
    private bool restoreTime;


    void Start() {
        current = this;
        if (Time.timeScale < 1f) {
            restoreTime = true;
            restoreSpeed = 10;
        }
        else {
            restoreTime = false;
        }
    }

    void Update() {
        if (restoreTime) {
            if (Time.timeScale < 1f) {
                Time.timeScale += Time.deltaTime * restoreSpeed;
            }
            else {
                Time.timeScale = 1f;
                restoreTime = false;
            }
        }
    }

    public void StopTime(float changeTime, int restoreSpeed, float delay) {
        this.restoreSpeed = restoreSpeed;
        if (delay > 0) {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else {
            restoreTime = true;
        }
        Time.timeScale = changeTime;
    }

    IEnumerator StartTimeAgain(float amount) {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(amount);
    }
}
