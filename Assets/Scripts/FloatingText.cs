using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 20f;
    public float lifeTime = 1f;
    public Vector3 offset = new Vector3(0, 50, 0); // pixels
    private float timer = 0f;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // L�g�re mont�e
        rectTransform.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // Disparition apr�s un d�lai
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}
