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

        // Légère montée
        rectTransform.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // Disparition après un délai
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
