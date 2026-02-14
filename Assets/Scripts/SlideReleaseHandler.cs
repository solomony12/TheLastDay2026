using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SliderReleaseHandler : MonoBehaviour, IPointerUpHandler
{
    public Action<float> OnReleased;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnReleased?.Invoke(slider.value);
    }
}