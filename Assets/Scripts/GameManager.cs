using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        // scoreText.text = "00";
    }

    public void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }
}
