using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour {

    //[SerializeField] private List<string> scenes = new List<string>();
    [SerializeField] private string sceneToLoad = null;

    /*
    void OnValidate() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            scenes.Add(SceneManager.GetSceneAt(i).name);
        }
    }
    */

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Pilot") {
            if (sceneToLoad != null) {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
