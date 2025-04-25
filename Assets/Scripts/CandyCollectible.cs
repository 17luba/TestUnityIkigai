using UnityEngine;

public class CandyCollectible : MonoBehaviour
{
    private CandySystem candySystem;

    public void Init(CandySystem system)
    {
        candySystem = system;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            candySystem.OnCandyCollected();
            Destroy(gameObject);
        }
    }
}
