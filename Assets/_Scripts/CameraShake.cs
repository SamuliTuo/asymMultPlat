using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraShake : MonoBehaviour {

    public static CameraShake current;

    [SerializeField] private float directionalMultiplier = 1;
    [SerializeField] private float rotationalMultiplier = 1;

    private float trauma = 0.0f;
    private float seed;
    private float angle = 0;
    private Vector3 offset = Vector3.zero;
    
    void Start() {
        current = this;
        seed = Random.Range(0.0f, 1000.0f);
    }

    public void AddTrauma(float amount, float cap = 1) {
        if (trauma < cap) {
            trauma = Mathf.Min(trauma + amount, cap);
        }
    }

    void Update() {
        TestingInputs();
        ShakeIt();
    }

    void ShakeIt() {
        if (trauma > 0.0f) {
            trauma -= Time.deltaTime;
            if (trauma <= 0.0f) {
                trauma = 0.0f;
                offset = Vector3.zero;
                seed = Random.Range(0.0f, 1000.0f);
                angle = 0;
            }
            float shake = trauma * trauma;
            var time = Time.time * 10;
            angle = (Mathf.PerlinNoise(time + seed, time + seed + 10) - 0.5f) * shake * rotationalMultiplier;
            offset = new Vector3(
                (Mathf.PerlinNoise(time + seed + 20, time + seed + 30) - 0.5f) * shake * directionalMultiplier,
                (Mathf.PerlinNoise(time + seed + 40, time + seed + 50) - 0.5f) * shake * directionalMultiplier,
                0);
        }
    }

    void TestingInputs() {
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            AddTrauma(0.5f);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame) {
            Time.timeScale = 0.2f;
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame) {
            Time.timeScale = 1;
        }
    }

    public Vector3 CamShakeOffsetVec3() {
        return offset;
    }
    public float CamShakeRotationAngle() {
        return angle;
    }
}
