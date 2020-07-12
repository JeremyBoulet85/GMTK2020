using UnityEngine;

public class SneezeSystem : MonoBehaviour
{

    public GaugeBarController sneezeBar;

    int sneezeLevel;
    bool isSneezing;
    const float drainingDelayAmount = 0.01f; // seconds
    const int maxSneezeLevel = 100;
    const float fillingDelayAmount = 0.25f; // seconds
    private PlayerController player;

    protected float timer;

    void Start()
    {
        timer = 0;
        sneezeLevel = 0;
        sneezeBar.SetMax(maxSneezeLevel);
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
            FindObjectOfType<AudioManager>().Play("Sneeze");
            (player as PlayerController).Sneeze();
            sneezeBar.isDraining = true;
        }
    }
}
