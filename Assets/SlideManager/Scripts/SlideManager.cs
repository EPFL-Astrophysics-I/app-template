using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideManager : MonoBehaviour
{
    public void ToggleLanguage()
    {
        Debug.Log("Slide Manager > ToggleLanguage()");
    }

    public void LoadSlide(int slideIndex)
    {
        Debug.Log("Slide Manager > Loading Slide " + slideIndex);
    }
}
