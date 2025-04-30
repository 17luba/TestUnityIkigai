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
    public TextMeshProUGUI affichageScoreText;
    [SerializeField] private GameObject newBestText;
    public TextMeshProUGUI gameOverText;

    public GameObject floatingTextPrefab;
    public RectTransform floatingTextParent; // Canva

    [Header("UI")]
    public GameObject replayButton;
    public GameObject menuButton;
    private bool isGameOver = false;

    [Header("Obstacles")]
    public GameObject[] leftWallSpikes;
    public GameObject[] rightWallSpikes;

    public int score = 0;

    [Header("Gestion de son")]
    public AudioSource audioSource;
    public AudioClip jumpSound, wallHitSound, deathSound, startSound, CandySound;

    [Header("Gestion de dificulté")]
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
        //int best = PlayerPrefs.GetInt("BestScore", 0);
        //bestScoreText.text = $"Best score : {best}";
        //bestScoreText.gameObject.SetActive(false);

        replayButton.SetActive(false);

        PlaySound(startSound);

        SpawnCandyAtRandomPoint();
    }

    public void ShowFlotingText(Vector3 worldPosition)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        GameObject textG0 = Instantiate(floatingTextPrefab, screenPos, Quaternion.identity ,floatingTextParent);
        Debug.Log("FloatingText instancié !");
        textG0.GetComponent<FloatingText>().SetText("+1");
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

        // Crée une liste temporaire d’indices disponibles
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i != lastIndex) // On évite la répétition immédiate
                availableIndices.Add(i);
        }

        // Si tous les indices ont été exclus (ex: 1 seul point), on autorise la répétition
        if (availableIndices.Count == 0)
        {
            availableIndices.Add(lastIndex);
        }

        // Choisir un index aléatoire parmi ceux disponibles
        int chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        lastIndex = chosenIndex;

        Transform spawnTransform = spawnPoints[chosenIndex];

        currentCandy = Instantiate(candyPrefabs, spawnTransform.position, Quaternion.identity);
    }

    public void ShowGameOver()
    {
        isGameOver = true;

        int currentScore = score;
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bool isNewBest = false;

        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            bestScoreText.text = currentScore.ToString();
            isNewBest = true;
        }

        bestScoreText.text = $"Best : {bestScore}";
        affichageScoreText.text = $"Score : {currentScore}";
        affichageScoreText.gameObject.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        replayButton.SetActive(true);
        menuButton.SetActive(true);
        newBestText.SetActive(isNewBest);
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
        IncreaseScore(1);
    }

    public void IncreaseScore(int amount)
    {
        if (isGameOver) return;

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

    public void OnMenuButtonCliked()
    {
        SceneManager.LoadScene("Menu");
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
        }
    }

    IEnumerator SlideSpike(GameObject spike, Vector3 from, Vector3 to)
    {
        spike.transform.localPosition = from;
        spike.SetActive(true);

        float duration = 1.5f; // Durée de l'animation
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
