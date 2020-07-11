using UnityEngine;

public class SneezeSystem : MonoBehaviour
{

    public GaugeBarController sneezeBar;

    SoundManager soundManager;
    int sneezeLevel;
    const int maxSneezeLevel = 100;
    const float fillingDelayAmount = 0.25f; // seconds

    protected float timer;

    void Start()
    {
        timer = 0;
        sneezeLevel = 0;
        sneezeBar.SetMax(maxSneezeLevel);
        soundManager = GetComponent<SoundManager>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    void Update()
    {
        if (timer >= fillingDelayAmount && !sneezeBar.isDraining)
        {
            timer = 0f;
            IncreaseSneezeLevel();
        }
    }

    void IncreaseSneezeLevel()
    {
        sneezeLevel += 1;
        sneezeBar.IncreaseValue(1);

        if (sneezeLevel >= maxSneezeLevel) { 
            sneezeLevel = 0;
            soundManager.PlaySound(SoundType.Sneeze, 0.9f);
            sneezeBar.isDraining = true;
        }
    }
}
