using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private float spawnInterval = 1;

    GameObject enemy;

    void Start() {
        enemy = Resources.Load("enemies/Enemy_test") as GameObject;
        StartCoroutine(SpawnEnemyAfterInterval());
    }

    IEnumerator SpawnEnemyAfterInterval() {
        yield return new WaitForSeconds(spawnInterval);
        Instantiate(enemy, new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(4.0f, 7.0f), 0), Quaternion.identity);
        StartCoroutine(SpawnEnemyAfterInterval());
    }
}