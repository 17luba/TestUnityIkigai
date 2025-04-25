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

    [Header("Gestion de dificulté")]
    private float bouncesCount = 0f;
    public int baseSpikesToActivate = 2;
    private int currentSpikesToActivate = 2;

    public float difficultyIncrementEvery = 5f;
    public float speedIncreaseAmount = 0.5f;
    private float baseSpeed = 5;

    [Header("Gestion de bonbons")]
    public GameObject candyPrefabs;
    private GameObject currentCandy;

    // Coroutine pour le spawn de bonbons
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private float spawnDelay = 0.5f;

    // Distance entre les bonbons
    private Vector2 lastCandyPosition = Vector2.zero;
    public float minDistanceBetweenCandies = 2.5f;


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

        PlaySound(startSound);

        // Gestion de difficulté
        // baseSpeed = BirdController.Instance.horizontalSpeed;
        // currentSpikesToActivate = baseSpikesToActivate;
    }

    // Gestion des bonbons
    public void TrySpawnCandy()
    {
        if (currentCandy != null) return;

        Vector2 spawnPos = GetSafCandyPosition();
        currentCandy = Instantiate(candyPrefabs, spawnPos, Quaternion.identity);
        Candy.Instance.gameObject.SetActive(true);
    }

    private Vector2 GetSafCandyPosition()
    {
        float margin = 1.5f;
        float minX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0)).x + margin;
        float maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x - margin;
        float minY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0)).y + margin;
        float maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1)).y - margin;

        Vector2 candidatePosition;
        int safetyCounter = 0;

        do
        {
            candidatePosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            safetyCounter++;
        }
        while (Vector2.Distance(candidatePosition, lastCandyPosition) < minDistanceBetweenCandies && safetyCounter < 50);

        lastCandyPosition = candidatePosition;

        return candidatePosition;
    }


    // Gestion bonbons 2
    public void SpawnNewCandy()
    {
        Vector3 spawnPosition = GetRandomCandyPosition();
        Instantiate(candyPrefab, spawnPosition, Quaternion.identity);
        Candy.Instance.gameObject.SetActive(true);
    }

    private Vector3 GetRandomCandyPosition()
    {
        float y = Random.Range(-3f, 3f);
        float x = -2f;
   
        return new Vector3(x, y, 0f);
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

        // Sélectionner des indices uniques
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
        // score++;
        // UpdateScoreText();
        IncreaseScore(1);
    }

    //public void ClearCandy()
    //{
    //    currentCandy = null;
    //}

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        currentCandy = null; // Détruire le bonbon après l'avoir mangé
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
                Vector3 to = from + (toLeftWall ? Vector3.left : Vector3.right) * 0.2f; // Déplacer le spike hors de l'écran
            }
        }
    }

    // Difficulté
    public void IncreaseDifficulty()
    {
        bouncesCount++;

        if (bouncesCount % difficultyIncrementEvery == 0)
        {
            // Augmenter la vitesse
            currentSpikesToActivate = Mathf.Min(currentSpikesToActivate + 1, 7);
            BirdController.Instance.IncreaseSpeed(speedIncreaseAmount);

            // Augmenter le nombre de spikes à activer
            // currentSpikesToActivate++;
        }
    }

    IEnumerator SlideSpike(GameObject spike, Vector3 from, Vector3 to)
    {
        spike.transform.localPosition = from;
        spike.SetActive(true);

        float duration = 1.5f; // Durée de l'animation
        float elapsed = 0.5f;

        // spike.transform.position = from;

        while (elapsed < duration)
        {
            spike.transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // spike.transform.position = to;

        spike.SetActive(false);
    }
}
