using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotLedgeClimber : MonoBehaviour
{
    private PilotController controller;

    void Start()
    {
        controller = GetComponentInParent<PilotController>();
    }

    void OnTriggerEnter(Collider other) {
        
    }
}