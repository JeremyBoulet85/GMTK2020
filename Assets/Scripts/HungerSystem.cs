using UnityEngine;

public class HungerSystem : MonoBehaviour
{
    public GaugeBarController hungerBar;

    [SerializeField]
    float hungerLevel;
    Transform player;
    Vector3 previousPos;

    const float scaling = 3.1f;
    const int maxHungerLevel = 100;

    void Start()
    {
        player = GetComponent<Transform>();
        hungerBar.SetMax(maxHungerLevel);
        hungerLevel = 0;
        previousPos = player.position;
    }

    void Update()
    {
        if (!previousPos.Equals(player.position) && !hungerBar.isDraining)
            IncreaseHungerLevel();

        previousPos = player.position;
    }

    void IncreaseHungerLevel()
    {
        // Average walking distance 0.04
        // Average running distance 0.16
        float hungerGained = Vector3.Distance(previousPos, player.position) * scaling;

        if (hungerLevel + hungerGained >= maxHungerLevel)
        {
            hungerLevel = maxHungerLevel;
            hungerBar.SetValue(maxHungerLevel);
        }
        else
        {
            hungerLevel += hungerGained;
            hungerBar.IncreaseValue(hungerGained);
        }

        if (hungerLevel == maxHungerLevel)
        {
            hungerLevel = 0;
            hungerBar.isDraining = true;
        }

    }
}
