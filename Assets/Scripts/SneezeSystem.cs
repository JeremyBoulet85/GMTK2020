using UnityEngine;

public class SneezeSystem : MonoBehaviour
{

    public GaugeBarController sneezeBar;
    int sneezeLevel;
    const int maxSneezeLevel = 100;
    const float delayAmount = 0.1f; // seconds

    protected float timer;

    void Start()
    {
        timer = 0;
        sneezeLevel = 0;
        sneezeBar.SetMax(maxSneezeLevel);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delayAmount)
        {
            timer = 0f;
            IncreaseSneezeLevel();
        }
    }

    void IncreaseSneezeLevel()
    {
        sneezeLevel += 1;
        sneezeBar.IncreaseValue(1);

        if (sneezeLevel >= maxSneezeLevel)
            Sneeze();
    }

    void Sneeze()
    {
        sneezeLevel = 0;
        sneezeBar.SetValue(0);
        Debug.Log("Sneeze!!");
        // do more things
    }
}
