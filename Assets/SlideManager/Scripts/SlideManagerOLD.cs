using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlideManagerOLD : MonoBehaviour
{
    [SerializeField] private Transform headerText = default;
    [SerializeField] private Navigation navigation = default;
    [SerializeField] private GameObject slides = default;
    [SerializeField] private int currentSlideIndex = 0;

    private int numSlides;
    private static Camera mainCamera;
    private HashSet<SimulationSlideController> currentSlideControllers;

    private void Awake()
    {
        // Get reference to the main camera (to pass on to camera controllers)
        mainCamera = Camera.main;

        currentSlideControllers = new HashSet<SimulationSlideController>();
    }

    private void Start()
    {
        // Give all simulations a chance to call Awake() before turning them off
        if (slides != null)
        {
            numSlides = slides.transform.childCount;

            // Turn off all simulations initially
            foreach (Transform slide in slides.transform)
            {
                // A single slide could have multiple SimulationSlideControllers
                foreach (SimulationSlideController slideController in slide.GetComponents<SimulationSlideController>())
                {
                    slideController.DeactivateSimulation();
                    slideController.enabled = false;
                }
            }
        }

        if (navigation != null)
        {
            navigation.GenerateBreadcrumbs(numSlides);
        }

        LoadSlide(currentSlideIndex);
    }

    public void LoadSlide(int slideIndex)
    {
        //Debug.Log("Loading slide " + slideIndex);
        SetHeaderTextVisibility(slideIndex);

        if (navigation != null)
        {
            navigation.SetButtonVisibility(slideIndex, numSlides);
            navigation.SetActiveBreadcrumb(slideIndex);
        }
        else
        {
            Debug.LogWarning("No navigation assigned.");
        }

        Transform currentSlide = SetSlideVisibility(slideIndex);
        SetSimulationVisibility(currentSlide);

        // Update at the end to give the various components a chance to check
        // whether we are moving backward, forward, or staying still
        currentSlideIndex = slideIndex;
    }

    public void GoBackOneSlide()
    {
        LoadSlide(currentSlideIndex - 1);
    }

    public void GoForwardOneSlide()
    {
        LoadSlide(currentSlideIndex + 1);
    }

    private void SetHeaderTextVisibility(int slideIndex)
    {
        if (headerText != null)
        {
            float fadeTime = (currentSlideIndex == slideIndex) ? 0 : 0.2f;
            float targetAlpha = (slideIndex != 0) ? 1 : 0;
            foreach (RectTransform transform in headerText)
            {
                StartCoroutine(Utils.FadeTMP(transform.GetComponent<TextMeshProUGUI>(), targetAlpha, fadeTime));
            }
        }
    }

    private Transform SetSlideVisibility(int slideIndex)
    {
        if (slides == null) 
        {
            return null;
        }

        Transform currentSlide;

        // Starting the app on this slide
        if (currentSlideIndex == slideIndex)
        {
            // Turn off all slides...
            foreach (Transform slide in slides.transform)
            {
                slide.GetComponent<CanvasGroup>().alpha = 0;
                //slide.GetComponent<CanvasGroup>().interactable = false;
                slide.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            // Except the current one
            currentSlide = slides.transform.GetChild(currentSlideIndex);
            CanvasGroup thisCG = currentSlide.GetComponent<CanvasGroup>();
            StartCoroutine(Utils.FadeCanvasGroup(thisCG, 1, 0));
        }
        // Transitioning slides
        else
        {
            // Fade out the current slide
            Transform previousSlideTransform = slides.transform.GetChild(currentSlideIndex);
            if (previousSlideTransform.TryGetComponent(out CameraController previousCC))
            {
                previousCC.ReleaseCameraReference();
            }
            CanvasGroup previousCG = previousSlideTransform.GetComponent<CanvasGroup>();
            StartCoroutine(Utils.FadeCanvasGroup(previousCG, 0, 0.2f));

            // Fade in the next slide
            currentSlide = slides.transform.GetChild(slideIndex);
            CanvasGroup nextCG = currentSlide.GetComponent<CanvasGroup>();
            StartCoroutine(Utils.FadeCanvasGroup(nextCG, 1, 0.2f));
        }

        // Put the camera in the right place for this slide
        if (currentSlide.TryGetComponent(out CameraController currentCC))
        {
            currentCC.AssignCameraReference(mainCamera);
            currentCC.InitializeCamera();
        }

        return currentSlide;
    }

    private void SetSimulationVisibility(Transform currentSlide)
    {
        // Deactivate all current simulations
        foreach (SimulationSlideController slideController in currentSlideControllers)
        {
            slideController.DeactivateSimulation();
            slideController.enabled = false;
        }

        // Activate all simulations corresponding to this slide
        currentSlideControllers = new HashSet<SimulationSlideController>();
        foreach (SimulationSlideController slideController in currentSlide.GetComponents<SimulationSlideController>())
        {
            slideController.ActivateSimulation();
            slideController.enabled = true;
            currentSlideControllers.Add(slideController);
        }
    }
}
