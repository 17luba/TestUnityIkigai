using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            GameManager.Instance.PlaySound(GameManager.Instance.wallHitSound);

            // Determiner le mur actuel
            WallSide currentWall = (collision.transform.position.x < 0) ? WallSide.Left : WallSide.Right;

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
            GameManager.Instance.AddScore();
            GameManager.Instance.IncreaseDifficulty();
        }

        if (collision.gameObject.CompareTag("spike"))
        {
            Die();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
        animator.SetTrigger("IsDead");
        GameManager.Instance.ShowGameOver();
        // Invoke(nameof(ReloadScene), 5f);

        GameManager.Instance.PlaySound(GameManager.Instance.deathSound);
    }

    //void ReloadScene()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}
}
