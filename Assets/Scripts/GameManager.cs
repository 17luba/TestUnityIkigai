using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gestion de score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    // public TextMeshProUGUI gameOverText;

    [Header("UI")]
    public GameObject replayButton;

    [Header("Obstacles")]
    public GameObject[] leftWallSpikes;
    public GameObject[] rightWallSpikes;

    private int score = 0;

    [Header("Gestion de son")]
    public AudioSource audioSource;
    public AudioClip jumpSound, wallHitSound, deathSound, startSound, CandySound;

    [Header("Gestion de dificult�")]
    private float bouncesCount = 0f;
    public int baseSpikesToActivate = 2;
    private int currentSpikesToActivate = 2;

    public float difficultyIncrementEvery = 5f;
    public float speedIncreaseAmount = 0.5f;

    [Header("Gestion de bonbons")]
    public GameObject candyPrefabs;
    private GameObject currentCandy;
    public Transform[] spawnPoints;

    int lastIndex = -1;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        score = 0;
        HideSpikes();
        UpdateScoreText();

        // PlayerPrefs.DeleteKey("BestScore");

        // Afficher le meilleur score
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = $"Best score : {best}";
        bestScoreText.gameObject.SetActive(false);

        replayButton.SetActive(false);

        PlaySound(startSound);

        SpawnCandyAtRandomPoint();
    }

    // Gestion des bonbons
    public void SpawnCandyAtRandomPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        // Supprime l'ancien bonbon s'il existe
        if (currentCandy != null)
        {
            Destroy(currentCandy);
            currentCandy = null;
        }

        // Cr�e une liste temporaire d�indices disponibles
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i != lastIndex) // On �vite la r�p�tition imm�diate
                availableIndices.Add(i);
        }

        // Si tous les indices ont �t� exclus (ex: 1 seul point), on autorise la r�p�tition
        if (availableIndices.Count == 0)
        {
            availableIndices.Add(lastIndex);
        }

        // Choisir un index al�atoire parmi ceux disponibles
        int chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        lastIndex = chosenIndex;

        Transform spawnTransform = spawnPoints[chosenIndex];

        currentCandy = Instantiate(candyPrefabs, spawnTransform.position, Quaternion.identity);
    }

    public void ShowGameOver()
    {
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

        // S�lectionner des indices uniques
        HashSet<int> usedIndices = new HashSet<int>();
        while (usedIndices.Count < currentSpikesToActivate)
        {
            int index = Random.Range(0, wall.Length);
            usedIndices.Add(index);
        }

        foreach (int i in usedIndices)
        {
            Vector3 toPosition = wall[i].transform.localPosition;
            Vector3 fromPosition = toPosition + new Vector3(onRightWall ? 1f : -1f, 0, 0);
            SlideSpikeIn(wall[i], fromPosition, toPosition);
        }
    }

    public void AddScore()
    {
        IncreaseScore(1);
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        currentCandy = null; // D�truire le bonbon apr�s l'avoir mang�
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

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void SlideSpikeIn(GameObject spike, Vector3 from, Vector3 to)
    {
        StartCoroutine(SlideSpike(spike, from, to));
    }

    public void SlideSpikeOut(GameObject[] spikes, bool toLeftWall)
    {
        foreach (GameObject spike in spikes)
        {
            if (spike.activeSelf)
            {
                Vector3 from = spike.transform.localPosition;
                Vector3 to = from + (toLeftWall ? Vector3.left : Vector3.right) * 0.2f; // D�placer le spike hors de l'�cran
            }
        }
    }

    // Difficult�
    public void IncreaseDifficulty()
    {
        bouncesCount++;

        if (bouncesCount % difficultyIncrementEvery == 0)
        {
            // Augmenter la vitesse
            currentSpikesToActivate = Mathf.Min(currentSpikesToActivate + 1, 7);
            BirdController.Instance.IncreaseSpeed(speedIncreaseAmount);
        }
    }

    IEnumerator SlideSpike(GameObject spike, Vector3 from, Vector3 to)
    {
        spike.transform.localPosition = from;
        spike.SetActive(true);

        float duration = 1.5f; // Dur�e de l'animation
        float elapsed = 0.5f;

        while (elapsed < duration)
        {
            spike.transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spike.SetActive(false);
    }
}
