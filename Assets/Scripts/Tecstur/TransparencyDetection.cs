using System.Collections;
using UnityEngine;

public class TransparencyDetection : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float trancparencyAmount = 0.8f;
    public float fadeTime = 0.5f;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, trancparencyAmount));
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, 1.0f));
        }
    }

    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float fadeTime, float startTransparencyAmount, float targetTransAmount)
    {
        float elapasedTime = 0;
        while (elapasedTime < fadeTime)
        {
            elapasedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startTransparencyAmount, targetTransAmount, elapasedTime/fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);

            yield return null;
        }
    }
}
