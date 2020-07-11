using UnityEngine;
using UnityEngine.UI;

public class GaugeBarController : MonoBehaviour
{

    public Slider slider;

    public void SetMax(int max)
    {
        slider.maxValue = max;
        slider.value = 0;
    }
    
    public void IncreaseValue(int increment)
    {
        slider.value += increment;
    }

    public void DecreaseValue(int increment)
    {
        slider.value -= increment;
    }

}
