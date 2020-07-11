using UnityEngine;
using UnityEngine.UI;

public class GaugeBarController : MonoBehaviour
{

    public Slider slider;
    public bool isDraining;

    float drainVal;
    float defaultDrainVal = 2;

    private void Start()
    {
        isDraining = false;
        drainVal = -1;
    }

    private void FixedUpdate()
    {
        if (isDraining)
        {
            DecreaseValue(drainVal == -1 ? defaultDrainVal : drainVal);

            if (slider.value == 0)
                isDraining = false;
        }
    }

    public void SetMax(int max)
    {
        slider.maxValue = max;
        slider.value = 0;

        drainVal = 2 * max / 100;
    }

    public void SetValue(int value)
    {
        slider.value = value;
    }

    public float GetValue()
    {
        return slider.value;
    }
    
    public void IncreaseValue(float increment)
    {
        slider.value += increment;
    }

    public void DecreaseValue(float increment)
    {
        slider.value -= increment;
    }

}
