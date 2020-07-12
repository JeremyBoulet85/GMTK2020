using UnityEngine;

public class FartSystem : MonoBehaviour
{
    public GaugeBarController fartBar;

    float fartLevel;
    const int maxFartLevel = 100;
    const float fillingDelayAmount = 0.25f; // seconds
    private PlayerController player;
    private bool isInBathroom = false;
    protected float timer;

    void Start()
    {
        timer = 0;
        fartLevel = 0f;
        fartBar.SetMax(maxFartLevel);
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
        if (timer >= fillingDelayAmount && !fartBar.isDraining)
        {
            timer = 0f;
            IncreaseSneezeLevel();
        }
    }

    public void EnterBathroom()
    {
        isInBathroom = true;
    }

    public void ExitBathroom()
    {
        isInBathroom = false;
    }

    void IncreaseSneezeLevel()
    {
        if (!isInBathroom)
        {
            fartLevel += 0.5f;
            fartBar.IncreaseValue(0.5f);
        } else
        {
            fartLevel -= 2.0f;
            fartBar.DecreaseValue(2.0f);
        }

        if (fartLevel >= maxFartLevel)
        {
            fartLevel = 0;
            FindObjectOfType<AudioManager>().Play("Fart");
            (player as PlayerController).Sneeze();
            fartBar.isDraining = true;
        }
    }
}
