using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;

    public GameObject[] leftWallSpikes;
    public GameObject[] rightWallSpikes;

    private int score = 0;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HideSpikes();
        UpdateScoreText();
        gameOverText.gameObject.SetActive(false);
        // scoreText.text = "00";
    }

    public void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public void HideSpikes()
    {
        foreach (GameObject spike in leftWallSpikes)
        {
            spike.SetActive(false);
        }

        foreach (GameObject spike in rightWallSpikes)
        {
            spike.SetActive(false);
        }
    }

    public void ActiveRandomSpikes(bool onRightWall)
    {
        GameObject[] wall = onRightWall ? rightWallSpikes : leftWallSpikes;

        foreach (GameObject spike in wall)
        {
            spike.SetActive(false);
        }

        int first = Random.Range(0, wall.Length);
        int second;

        do
        {
            second = Random.Range(0, wall.Length);
        } while (first == second);

        wall[first].SetActive(true);
        wall[second].SetActive(true);
    }

    public void AddScore()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString("D2");
    }
}
