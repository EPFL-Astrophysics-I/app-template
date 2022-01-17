using System.Collections;
using TMPro;
using UnityEngine;

public class SlideManager : MonoBehaviour
{
    [SerializeField] private bool showHeader = true;
    [SerializeField] private int currentSlideIndex = 0;

    private Transform slides;

    private void Awake()
    {
        // Set header visibility
        Transform header = transform.Find("Header");
        if (header != null)
        {
            header.gameObject.SetActive(showHeader);
        }

        // Get reference to the Slides container if it exists
        slides = transform.Find("Slides");
        if (slides == null)
        {
            Debug.LogWarning("SlideManager did not find a child GameObject called Slides.");
        }
    }

    private void Start()
    {
        SetLanguage("EN");
        InitializeSlides();
        GenerateNavigationUI();
    }

    public void SetLanguage(string language)
    {
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (!text.CompareTag("Untagged"))
            {
                text.gameObject.SetActive(text.CompareTag(language));
            }
        }
    }

    private void InitializeSlides()
    {
        if (slides == null)
        {
            return;
        }

        foreach (Transform slide in slides)
        {
            if (slide.TryGetComponent(out CanvasGroup canvasGroup))
            {
                if (slide.GetSiblingIndex() != currentSlideIndex)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                }
                else
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
    }

    private void GenerateNavigationUI()
    {
        // Create navigation bubbles and set the correct one active
        Transform navigation = transform.Find("Navigation");
        if (navigation == null)
        {
            Debug.LogWarning("SlideManager did not find a child GameObject called Navigation.");
            return;
        }

        if (navigation.TryGetComponent(out Navigation nav))
        {
            nav.GenerateBubbles(slides.childCount);
            nav.SetCurrentSlideIndex(currentSlideIndex);
            nav.ChangeSlide(currentSlideIndex, false);
        }
    }

    private IEnumerator FadeSlide(CanvasGroup canvasGroup, float targetAlpha, float fadeTime, float startDelay = 0)
    {
        yield return new WaitForSeconds(startDelay);

        float time = 0;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void LoadSlide(int slideIndex)
    {
        Debug.Log("Slide Manager > Loading Slide " + slideIndex);
        if (slides == null || currentSlideIndex == slideIndex)
        {
            return;
        }

        // Turn off the current slide
        if (slides.GetChild(currentSlideIndex).TryGetComponent(out CanvasGroup currentCG))
        {
            currentCG.blocksRaycasts = false;
            StartCoroutine(FadeSlide(currentCG, 0, 0.3f, 0.1f));
        }

        // Turn on the new slide
        if (slides.GetChild(slideIndex).TryGetComponent(out CanvasGroup newCG))
        {
            newCG.blocksRaycasts = true;
            StartCoroutine(FadeSlide(newCG, 1, 0.3f, 0));
        }

        currentSlideIndex = slideIndex;

        //// Starting the app on this slide
        //if (currentSlideIndex == slideIndex)
        //{
        //    // Turn off all slides...
        //    foreach (Transform slide in slides.transform)
        //    {
        //        slide.GetComponent<CanvasGroup>().alpha = 0;
        //        //slide.GetComponent<CanvasGroup>().interactable = false;
        //        slide.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //    }

        //    // Except the current one
        //    currentSlide = slides.transform.GetChild(currentSlideIndex);
        //    CanvasGroup thisCG = currentSlide.GetComponent<CanvasGroup>();
        //    StartCoroutine(Utils.FadeCanvasGroup(thisCG, 1, 0));
        //}
        //// Transitioning slides
        //else
        //{
        //    // Fade out the current slide
        //    Transform previousSlideTransform = slides.transform.GetChild(currentSlideIndex);
        //    if (previousSlideTransform.TryGetComponent(out CameraController previousCC))
        //    {
        //        previousCC.ReleaseCameraReference();
        //    }
        //    CanvasGroup previousCG = previousSlideTransform.GetComponent<CanvasGroup>();
        //    StartCoroutine(Utils.FadeCanvasGroup(previousCG, 0, 0.2f));

        //    // Fade in the next slide
        //    currentSlide = slides.transform.GetChild(slideIndex);
        //    CanvasGroup nextCG = currentSlide.GetComponent<CanvasGroup>();
        //    StartCoroutine(Utils.FadeCanvasGroup(nextCG, 1, 0.2f));
        //}

        //// Put the camera in the right place for this slide
        //if (currentSlide.TryGetComponent(out CameraController currentCC))
        //{
        //    currentCC.AssignCameraReference(mainCamera);
        //    currentCC.InitializeCamera();
        //}
    }
}