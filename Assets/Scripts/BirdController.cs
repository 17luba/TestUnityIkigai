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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            direction.x *= -1;
            Flip();
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
        GameManager.Instance.ShowGameOver();

        Invoke(nameof(ReloadScene), 5f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
