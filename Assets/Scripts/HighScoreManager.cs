using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.Save();
        Debug.Log("HighScore reset!");
    }
}
