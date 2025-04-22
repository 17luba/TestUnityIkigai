using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    // public TextMeshProUGUI gameOverText;
    public GameObject replayButton;

    public GameObject[] leftWallSpikes;
    public GameObject[] rightWallSpikes;

    private int score = 0;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        score = 0;
        HideSpikes();
        UpdateScoreText();
        // gameOverText.gameObject.SetActive(false);

        // Afficher le meilleur score
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = $"Best score : {best}";
        bestScoreText.gameObject.SetActive(false);

        // bestScoreText.text = $"Best score : {PlayerPrefs.GetInt("BestScore", 0)}";

        replayButton.SetActive(false);
    }

    public void ShowGameOver()
    {
        // gameOverText.gameObject.SetActive(true);

        //gameOverUI.SetActive(true);

        int currentScore = score;
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            bestScoreText.text = currentScore.ToString();
        }

        bestScoreText.text = $"Best : {bestScore}";
        bestScoreText.gameObject.SetActive(true);
        replayButton.SetActive(true);
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

    public void HideLeftSpikes()
    {
        foreach (GameObject spike in leftWallSpikes)
        {
            spike.SetActive(false);
        }
    }

    public void HideRightSpikes()
    {
        foreach (GameObject spike in rightWallSpikes)
        {
            spike.SetActive(false);
        }
    }

    public void Replay()
    {
        Debug.Log("Replay clicked !");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
