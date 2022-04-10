using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MechaActivation : MonoBehaviour {

    public static MechaActivation current;

    [Header("Exposed privates:")]
    [SerializeField] private Image mechaMeterUI = null;
    [SerializeField] private Slider activateSlider = null;
    [SerializeField] private PilotController pilot = null;
    [SerializeField] private GunnerController gunner = null;
    [SerializeField] private float activateDelay = 1;

    private MechaHP hp;
    private MechaFuel fuel;
    private Rigidbody rb;
    private bool pilotReady = false;
    private float mechaMeter = 0;
    private float t = 0;

    void Start() {
        hp = GetComponent<MechaHP>();
        fuel = GetComponent<MechaFuel>();
        rb = GetComponent<Rigidbody>();
        current = this;

        //for testing
        /*
        mechaMeter = 100;
        UpdateChargeMeterUI();
        //*/
    }



    public void FillMeter(float amount) {
        mechaMeter += amount * 3;
        if (mechaMeter >= 100) {
            mechaMeter = 100;
        }
        else if (mechaMeter < 0) {
            mechaMeter = 0;
        }
        UpdateChargeMeterUI();
    }

    void UpdateChargeMeterUI() {
        mechaMeterUI.fillAmount = mechaMeter * 0.01f;
    }

    public void ActivateMecha_Gunner() {
        if (Gamepad.current.startButton.isPressed && mechaMeter == 100) {
            if (pilotReady) {
                t += Time.deltaTime;
                SetActivationSlider(true, t / activateDelay);
                if (t >= activateDelay) {
                    ACTIVATE();
                }
            }
        }
        else if (activateSlider.gameObject.activeSelf) {
            t = 0;
            SetActivationSlider(false, 0);
        }
    }

    public void ActivateMecha_Pilot() {
        if (Keyboard.current.fKey.isPressed && mechaMeter == 100) {
            pilotReady = true;
        }
        else if (pilotReady) {
            t = 0;
            SetActivationSlider(false, 0);
            pilotReady = false;
        }
    }

    void ACTIVATE() {
        SetActivationSlider(false, 0);
        hp.FillHP();
        gunner.SetupGunnerForMechaMode();
        pilot.SetupPilotForMechaMode();
        transform.position = pilot.transform.position + Vector3.up * 0.1f;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DEACTIVATE() {
        fuel.DeactivationCleanup();
        rb.velocity = Vector3.zero;
        pilot.transform.position = transform.position + Vector3.up * 0.1f;
        gunner.transform.position = transform.position;
        gunner.ReturnFromMechaMode();
        pilot.ReturnFromMechaMode();
        transform.GetChild(0).gameObject.SetActive(false);
        HitStop.current.StopTime(0.1f, 2, 0.5f);
    }

    void SetActivationSlider(bool state, float value) {
        activateSlider.value = value;
        if (activateSlider.gameObject.activeSelf != state) {
            activateSlider.gameObject.SetActive(state);
        }
    }
}
