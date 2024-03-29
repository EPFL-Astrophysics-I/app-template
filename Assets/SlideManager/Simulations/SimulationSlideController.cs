﻿using UnityEngine;

// Classes should inherit from SimulationSlideController and attach to a Slide
// GameObject in order for the SlideManager to control the simulation's visibility
public abstract class SimulationSlideController : MonoBehaviour
{
    [SerializeField] private bool autoPlay = true;
    [SerializeField] protected Simulation simulation;

    // Simulations are activated by SlideManager when changing slides
    public void ActivateSimulation()
    {
        if (simulation == null)
        {
            return;
        }

        if (!simulation.gameObject.activeInHierarchy)
        {
            simulation.gameObject.SetActive(true);
            InitializeSlide();

            if (autoPlay)
            {
                simulation.Resume();
            }
            else
            {
                simulation.Pause();
            }
        }
    }

    // Simulations are deactivated by SlideManager when changing slides
    public void DeactivateSimulation()
    {
        if (simulation == null)
        {
            return;
        }

        if (simulation.gameObject.activeInHierarchy)
        {
            simulation.gameObject.SetActive(false);
        }
    }

    public virtual void InitializeSlide()
    {
        Debug.LogWarning(transform.name + " has not defined InitializeSlide()");
    }
}
