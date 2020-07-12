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
    private PlayerController playerRef;
    private bool isInCafetaria = false;

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
        else if (isInCafetaria)
        {
            hungerLevel -= 0.5f;
            hungerBar.DecreaseValue(0.5f);
        }

        previousPos = player.position;
    }

    void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void EnterCafetaria()
    {
        isInCafetaria = true;
    }

    public void ExitCafetaria()
    {
        isInCafetaria = false;
    }

    void IncreaseHungerLevel()
    {
        if (isInCafetaria)
        {
            hungerLevel -= 2.0f;
            hungerBar.DecreaseValue(2.0f);
        }
        else
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
                FindObjectOfType<AudioManager>().Play("Hungry");
                (playerRef as PlayerController).Sneeze();
                hungerBar.isDraining = true;
            }
        }
    }
}
