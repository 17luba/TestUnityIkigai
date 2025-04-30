using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class BirdController : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask wallLayer;
    public bool isDead = false;

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;
    private bool facingRight = true;

    private Animator animator;

    private enum WallSide
    {
        Left,
        Right,
        None
    }

    private int wallHitCount = 0;
    private bool candyAreActive = false;

    private WallSide lastWallTouched;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public static BirdController Instance;
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isDead) return;

        rb.velocity = new Vector2(direction.x * horizontalSpeed, rb.velocity.y);

        if (Input.GetMouseButtonDown(0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            GameManager.Instance.PlaySound(GameManager.Instance.jumpSound);
        }
    }

    public void IncreaseSpeed(float increment)
    {
        horizontalSpeed += increment;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            // Determiner le mur actuel
            WallSide currentWall = (collision.transform.position.x < 0) ? WallSide.Left : WallSide.Right;

            // Activer les bonbons après 5 rebonds sur les murs
            wallHitCount++;
            if (wallHitCount >= 5 && !candyAreActive)
            {
                candyAreActive = true;
                GameManager.Instance.SpawnCandyAtRandomPoint();
            }


            if (lastWallTouched != WallSide.None && lastWallTouched != currentWall)
            {
                // Désactiver les pikes de la dernière paroi touchée
                if (lastWallTouched == WallSide.Left)
                {
                    GameManager.Instance.SlideSpikeOut(GameManager.Instance.leftWallSpikes, true);
                }
                else
                {
                    GameManager.Instance.SlideSpikeOut(GameManager.Instance.rightWallSpikes, false);
                }
            }

            lastWallTouched = currentWall;



            // Inverser la direction
            direction.x *= -1;
            Flip();

            // Déterminer le nouveau mur opposé
            bool hittingRightWall = direction.x < 0; // Rebondire à gauche => vient du mur droit
            GameManager.Instance.ActiveRandomSpikes(!hittingRightWall);

            // Mettre à jour le dernier mur touché
            lastWallTouched = hittingRightWall ? WallSide.Right : WallSide.Left;

            // Score
            if (!isDead)
            {
                GameManager.Instance.AddScore();
                GameManager.Instance.PlaySound(GameManager.Instance.wallHitSound);
            }
            GameManager.Instance.IncreaseDifficulty();
        }

        if (collision.gameObject.CompareTag("spike"))
        {
            Die();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (GameManager.Instance.score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", GameManager.Instance.score);
            PlayerPrefs.Save();
            GameManager.Instance.bestScoreText.text = GameManager.Instance.score.ToString();
        }
        else
        {
            GameManager.Instance.bestScoreText.text = bestScore.ToString();
        }

        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
        animator.SetTrigger("IsDead");
        GameManager.Instance.ShowGameOver();
        GameManager.Instance.PlaySound(GameManager.Instance.deathSound);

       // Appliquer le rebon aleatoire
        rb.velocity = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));

        // Desactiver la collision après 3s
        Invoke(nameof(DisableCollision), 3f);

        // Geler la physique après 8s
        Invoke(nameof(DisablePhysics), 8f);

    }

    void DisableCollision()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    void DisablePhysics()
    {
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}
