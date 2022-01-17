using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup tips = default;
    [SerializeField] private float fadeInTime = 0.15f;
    [SerializeField] private float fadeOutTime = 0.1f;

    private Coroutine showTipCoroutine;
    private Coroutine hideTipCoroutine;

    private void Awake()
    {
        if (tips != null)
        {
            tips.alpha = 0;
            tips.interactable = false;
            tips.blocksRaycasts = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tips != null)
        {
            //tips.alpha = 1;
            if (hideTipCoroutine != null)
            {
                StopCoroutine(hideTipCoroutine);
                hideTipCoroutine = null;
            }
            showTipCoroutine = StartCoroutine(ShowTooltip(tips, fadeInTime));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tips != null)
        {
            //tips.alpha = 0;
            if (showTipCoroutine != null)
            {
                StopCoroutine(showTipCoroutine);
                showTipCoroutine = null;
            }
            hideTipCoroutine = StartCoroutine(HideTooltip(tips, fadeOutTime));
        }
    }

    private IEnumerator ShowTooltip(CanvasGroup canvasGroup, float lerpTime)
    {
        float time = 0;
        canvasGroup.alpha = 0;
        canvasGroup.transform.localScale = 0.8f * Vector3.one;

        while (time < lerpTime)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / lerpTime);
            canvasGroup.transform.localScale = Vector3.Lerp(0.8f * Vector3.one, Vector3.one, time / lerpTime);
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.transform.localScale = Vector3.one;
        showTipCoroutine = null;
    }

    private IEnumerator HideTooltip(CanvasGroup canvasGroup, float lerpTime)
    {
        float time = 0;
        float startAlpha = canvasGroup.alpha;

        while (time < lerpTime)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / lerpTime);
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.transform.localScale = Vector3.one;
        hideTipCoroutine = null;
    }
}
