using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private bool canSpawn = true;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SpawnNPC());
    }

    private IEnumerator SpawnNPC()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnRate);

        while (canSpawn)
        {
            yield return wait;
            int rand = Random.Range(0, npcPrefabs.Length);
            GameObject npcToSpawn = npcPrefabs[rand]; // Definir indice para seleccionar npc

            Instantiate(npcToSpawn, transform.position, Quaternion.identity);
        }
    }
}
