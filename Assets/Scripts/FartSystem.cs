using UnityEngine;

public class FartSystem : MonoBehaviour
{
    public GaugeBarController fartBar;

    SoundManager soundManager;
    float fartLevel;
    const int maxFartLevel = 100;
    const float fillingDelayAmount = 0.25f; // seconds

    protected float timer;

    void Start()
    {
        timer = 0;
        fartLevel = 0f;
        fartBar.SetMax(maxFartLevel);
        soundManager = GetComponent<SoundManager>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    void Update()
    {
        if (timer >= fillingDelayAmount && !fartBar.isDraining)
        {
            timer = 0f;
            IncreaseSneezeLevel();
        }
    }

    void IncreaseSneezeLevel()
    {
        fartLevel += 0.5f;
        fartBar.IncreaseValue(0.5f);

        if (fartLevel >= maxFartLevel)
        {
            fartLevel = 0;
            //soundManager.PlaySound(SoundType.Sneeze, 0.9f);
            fartBar.isDraining = true;
        }
    }
}
