using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Slider))]
public class SliderScr : MonoBehaviour
{

    #region Public Fields
    public TextMeshProUGUI valueTxt;
    public event Action<float> OnRealValueChanged;
    public valueType type;
    public string format;
    #endregion
    private Slider slider;

    public enum valueType
    {
        cubic,
        real,
        realCubic
    }


    private void Start()
    {
        slider = GetComponent<Slider>();
        if (type == valueType.real)
            slider.onValueChanged.AddListener(fixedValue);
        else if (type == valueType.cubic)
            slider.onValueChanged.AddListener(cubicValue);
        else 
            slider.onValueChanged.AddListener(realCubicValue);
    }

    private void fixedValue(float value)
    {
        valueTxt.text = value.ToString(format);
        OnRealValueChanged?.Invoke(value);
    } 
    private void cubicValue(float value)
    {
        value = value * value * value - 1;
        fixedValue(value);
    }
    private void realCubicValue(float value)
    {
        value = value * value * value;
        fixedValue(value);
    }

}
