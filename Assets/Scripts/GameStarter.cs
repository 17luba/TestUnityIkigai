using UnityEngine;
using TMPro;

public class GameStarter : MonoBehaviour
{
    public BirdController birdController; // R�f�rence � ton oiseau
    public TextMeshProUGUI tapToStartText;

    private bool gameStarted = false;

    void Start()
    {
        // Bloque l�oiseau au d�but
        birdController.enabled = false;
        birdController.GetComponent<Rigidbody2D>().isKinematic = true;

        tapToStartText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;

        tapToStartText.gameObject.SetActive(false);

        birdController.enabled = true;
        birdController.GetComponent<Rigidbody2D>().isKinematic = false;

        GameManager.Instance.PlaySound(GameManager.Instance.startSound);

        // Tu peux aussi d�clencher un petit "jump" ou son ici si tu veux
        BirdController.Instance.GetComponent<Rigidbody2D>().velocity = new Vector2(0, birdController.jumpForce);
        //birdController.Fly(); // S'il y a une m�thode Fly() pour le premier saut
    }
}
