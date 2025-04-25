using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = new Vector2(Random.Range(-4f, 4f), 6f); // Rebond initial
        rb.gravityScale = 1.5f;
        rb.angularVelocity = 200f;

        Invoke("DisableCollision", 3f); // Laisser rebondir
        Invoke("DisablePhysics", 8f);   // Sortie de l'écran
    }

    void DisableCollision()
    {
        col.enabled = false;
    }

    void DisablePhysics()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }
}
