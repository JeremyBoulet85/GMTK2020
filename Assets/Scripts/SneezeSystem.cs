﻿using UnityEngine;

public class SneezeSystem : MonoBehaviour
{

    public GaugeBarController sneezeBar;

    SoundManager soundManager;
    int sneezeLevel;
    bool isSneezing;
    const int maxSneezeLevel = 50;
    const float fillingDelayAmount = 0.05f; // seconds
    const float drainingDelayAmount = 0.01f; // seconds

    protected float timer;

    void Start()
    {
        timer = 0;
        isSneezing = false;
        sneezeLevel = 0;
        sneezeBar.SetMax(maxSneezeLevel);
        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        if (isSneezing)
        {
            Sneezing();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= fillingDelayAmount)
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
        isSneezing = true;
        sneezeLevel = 0;
        soundManager.PlaySound(SoundType.Sneeze, 0.9f);
        Sneezing();
        // do more things
    }

    void Sneezing()
    {
        timer += Time.deltaTime;

        if (timer >= drainingDelayAmount)
        {
            sneezeBar.DecreaseValue(1);

            if (sneezeBar.GetValue() == 0)
                isSneezing = false;
        }
    }
}
