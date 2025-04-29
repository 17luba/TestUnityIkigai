using UnityEngine;

public class CandySpawner : MonoBehaviour
{
    public GameObject candyPrefab;
    public Transform[] spawnPoints;

    private GameObject currentCandy;

    public void TrySpawnCandy()
    {
        if (currentCandy != null) return;

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        currentCandy = Instantiate(candyPrefab, randomSpawnPoint.position, Quaternion.identity);
    }

    public void OnCandyCollected()
    {
        if (currentCandy != null)
        {
            Destroy(currentCandy);
            currentCandy = null;

            // Si tu veux faire réapparaitre après 3 secondes :
            // Invoke(nameof(TrySpawnCandy), 3f);

            // Sinon, réapparition immédiate :
            TrySpawnCandy();
        }
    }
}
