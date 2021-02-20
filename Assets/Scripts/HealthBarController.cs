using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    Camera camToLook;
    [SerializeField]
    bool copyAnotherHealthBar = false;
    [SerializeField]
    Slider sliderToCopy;

    Slider curSlider;

    Image curSliderFillImage;
    Image sliderToCopyFillImage;


    void Start()
    {
        if (copyAnotherHealthBar)
        {
            curSlider = GetComponent<Slider>();
            curSlider.maxValue = sliderToCopy.maxValue;

            sliderToCopyFillImage = sliderToCopy.fillRect.gameObject.GetComponent<Image>();
            curSliderFillImage = curSlider.fillRect.gameObject.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!camToLook)
        {
            camToLook = GetComponentInParent<Canvas>().worldCamera;
        }

        transform.LookAt(camToLook.transform);
        if (copyAnotherHealthBar)
        {
            curSlider.value = sliderToCopy.value;
            curSliderFillImage.color = sliderToCopyFillImage.color;
        }
    }
}
