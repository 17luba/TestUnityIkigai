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
        Right
    }

    private WallSide lastWallTouched;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        rb.velocity = new Vector2(direction.x * horizontalSpeed, rb.velocity.y);

        if (Input.GetMouseButtonDown(0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            // Désactiver les pikes de la dernière paroi touchée
            if (lastWallTouched == WallSide.Left)
            {
                GameManager.Instance.HideLeftSpikes();
            }
            else
            {
                GameManager.Instance.HideRightSpikes();
            }

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
    }

    //void ReloadScene()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}
}
