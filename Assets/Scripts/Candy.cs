using UnityEngine;

public class Candy : MonoBehaviour
{

    public static Candy Instance;

    private void Awake()
    {
        Instance = this;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !BirdController.Instance.isDead)
        {
            GameManager.Instance.IncreaseScore(1);
            GameManager.Instance.PlaySound(GameManager.Instance.CandySound);
            gameObject.SetActive(false);
            // GameManager.Instance.ClearCandy();
            GameManager.Instance.TrySpawnCandy();
            // GameManager.Instance.SpawnNewCandy();

        }
    }
}
