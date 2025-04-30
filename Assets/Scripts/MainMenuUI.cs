using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject optionsPanel;
    public TMP_Text bestScoreText;
    public Toggle soundToggle;

    [SerializeField] private TextMeshProUGUI versionText;

    void Start()
    {
        // GameManager.Instance.PlaySound(GameManager.Instance.menuSound);

        optionsPanel.SetActive(false);

        
        // Charger et afficher le meilleur score
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = bestScore.ToString("D2");

        soundToggle.isOn = AudioManager.Instance.IsMusicOn();
        soundToggle.onValueChanged.AddListener(OnToggleSound);


        if (menuButtons == null)
            menuButtons = GameObject.Find("MenuButtons");
        if (optionsPanel == null)
            optionsPanel = GameObject.Find("OptionPanel");

        versionText.text = Application.version;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        menuButtons.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuButtons.SetActive(true);
    }

    public void ResetBestScore()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        bestScoreText.text = 0.ToString("D2");
    }

    public void OnToggleSound(bool isOn)
    {
        AudioManager.Instance.SetMusicState(isOn);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
