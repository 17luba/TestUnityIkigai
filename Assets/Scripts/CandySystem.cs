using UnityEngine;
using UnityEngine.SceneManagement;

public class CandySystem : MonoBehaviour
{
    [Header("Références")]
    public GameObject candyPrefab;
    public BoxCollider2D spawnZone;
    public LayerMask spikeLayer;
    public AudioClip candyPickupSound;

    [Header("Paramètres")]
    public float candyRadius = 0.5f;

    private GameObject currentCandy;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SpawnCandy();
    }

    void SpawnCandy()
    {
        if (currentCandy != null) gameObject.SetActive(false);

        Vector2 spawnPos;
        int maxTries = 100;
        int tries = 0;

        do
        {
            float x = Random.Range(spawnZone.bounds.min.x + candyRadius, spawnZone.bounds.max.x - candyRadius);
            float y = Random.Range(spawnZone.bounds.min.y + candyRadius, spawnZone.bounds.max.y - candyRadius);
            spawnPos = new Vector2(x, y);
            tries++;
        }
        while (Physics2D.OverlapCircle(spawnPos, candyRadius, spikeLayer) && tries < maxTries);

        currentCandy = Instantiate(candyPrefab, spawnPos, Quaternion.identity);
        currentCandy.AddComponent<CandyCollectible>().Init(this);
    }

    public void OnCandyCollected()
    {
        if (audioSource && candyPickupSound) audioSource.PlayOneShot(candyPickupSound);
        GameManager.Instance.AddScore();
        SpawnCandy();
    }
}
