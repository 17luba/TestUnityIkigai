using UnityEngine;

public class FadeLoop : MonoBehaviour
{
    public float fadeDuration = 1.2f;
    private CanvasGroup canvasGroup;
    private bool fadingOut = true;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup manquant sur " + gameObject.name);
        }
    }

    void Update()
    {
        if (canvasGroup == null) return;

        float alphaChange = Time.deltaTime / fadeDuration;
        if (fadingOut)
        {
            canvasGroup.alpha -= alphaChange;
            if (canvasGroup.alpha <= 0f)
            {
                canvasGroup.alpha = 0f;
                fadingOut = false;
            }
        }
        else
        {
            canvasGroup.alpha += alphaChange;
            if (canvasGroup.alpha >= 1f)
            {
                canvasGroup.alpha = 1f;
                fadingOut = true;
            }
        }
    }
}
