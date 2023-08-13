using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{


    [SerializeField] private float spawnRate = 0.5f;
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private bool canSpawn = true;
    void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);
        while (canSpawn)
        {
            yield return wait;

            Instantiate(enemys[0], transform.position, Quaternion.identity);
        }
    }
}