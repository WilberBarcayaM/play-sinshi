using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonPoolDemo : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-5f, -2f, 0f),
        new Vector3(0f, -2f, 0f),
        new Vector3(5f, -2f, 0f)
    };
    
    [Header("Demo Configuration")]
    public bool spawnOnStart = true;
    public float spawnDelay = 0.5f;
    
    private List<GameObject> activeDemons = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(WaitForPoolAndSpawn());
        }
    }

    IEnumerator WaitForPoolAndSpawn()
    {
        // Esperar a que el EnemyPool se inicialice
        while (EnemyPool.Instance == null)
        {
            yield return null;
        }
        
        // Esperar un frame adicional para que InitializePools() termine
        yield return new WaitForSeconds(0.1f);
        
        // Ahora spawnear
        StartCoroutine(SpawnDemonsWithDelay());
    }

    void Update()
    {
        // Presiona E para spawnear demons
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnDemonAtRandomPosition();
        }
        
        // Presiona R para devolver todos al pool
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnAllToPool();
        }
    }

    IEnumerator SpawnDemonsWithDelay()
    {
        yield return new WaitForSeconds(1f);
        
        foreach (Vector3 pos in spawnPositions)
        {
            GameObject demon = EnemyPool.Instance.SpawnFromPool("Demon", pos, Quaternion.identity);
            
            if (demon != null)
            {
                activeDemons.Add(demon);
            }
            
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnDemonAtRandomPosition()
    {
        if (spawnPositions.Length > 0)
        {
            Vector3 randomPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
            GameObject demon = EnemyPool.Instance.SpawnFromPool("Demon", randomPos, Quaternion.identity);
            
            if (demon != null && !activeDemons.Contains(demon))
            {
                activeDemons.Add(demon);
            }
        }
    }

    void ReturnAllToPool()
    {
        foreach (GameObject demon in activeDemons)
        {
            if (demon != null)
            {
                EnemyPool.Instance.ReturnToPool(demon);
            }
        }
        activeDemons.Clear();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 30), $"Demons activos: {activeDemons.Count}");
        GUI.Label(new Rect(10, 40, 400, 30), "Presiona E para spawnear | Presiona R para devolver al pool");
    }
}
