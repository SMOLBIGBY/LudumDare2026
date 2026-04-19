using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class BlackScreenFade : MonoBehaviour
{
    public static bool IsTransitionActive { get; private set; } // <- to sprawdza GameMenuManager

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float disableAfterFadeOutDelay = 0.05f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private Coroutine disableCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on this GameObject!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        FadeOut();
    }

    public void FadeIn()
    {
        // upewnij się że obiekt aktywny zanim ruszą coroutines
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StopAllFadeCoroutines();

        IsTransitionActive = true;

        // podczas fade blokuj klikanie "pod spodem"
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        fadeCoroutine = StartCoroutine(FadeAlphaRoutine(1f, onComplete: null));
    }

    public void FadeOut()
    {
        // Jeśli obiekt jest nieaktywny, to FadeOut i tak nie wystartuje coroutine,
        // więc najpierw aktywujemy.
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StopAllFadeCoroutines();

        IsTransitionActive = true;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        fadeCoroutine = StartCoroutine(FadeAlphaRoutine(0f, onComplete: () =>
        {
            // gdy skończy się fade-out -> odblokuj i wyłącz obiekt po chwili
            IsTransitionActive = false;
            disableCoroutine = StartCoroutine(DisableAfterDelay(disableAfterFadeOutDelay));
        }));
    }

    private void StopAllFadeCoroutines()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
    }

    private IEnumerator FadeAlphaRoutine(float targetAlpha, System.Action onComplete)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        // używamy unscaled, żeby działało nawet jak ktoś ma Time.timeScale = 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // jeśli jest całkiem przezroczysty, to nie blokuj raycastów
        if (targetAlpha <= 0.001f)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = false;
        }

        onComplete?.Invoke();
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // jeśli w międzyczasie znowu zaczęło się FadeIn, nie wyłączaj
        if (!IsTransitionActive && canvasGroup.alpha <= 0.001f)
            gameObject.SetActive(false);
    }
}
