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
            GameManager.Instance.ShowFlotingText(transform.position);
            GameManager.Instance.PlaySound(GameManager.Instance.CandySound);
        }
        Destroy(gameObject);
        GameManager.Instance.SpawnCandyAtRandomPoint();
    }
}
