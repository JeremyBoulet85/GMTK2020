using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float normalSpeed = 4.0f;

    [SerializeField]
    private float runningSpeed = 8.0f;

    [SerializeField]
    public float dashSpeed = 20.0f;

    [SerializeField]
    public float startDashTime = 0.07f;

    [SerializeField]
    public float dashCooldownTime = 5.0f;

    public GameObject fart = null;
    public bool isRunning = false;

    public bool IsFrozen { get; private set; } = false;
    private Animator animator;
    private Rigidbody2D rb;
    private float timeBetweenFootstepSound = 0.34f;
    private Vector3 lastPosition;
    private Vector3 lastDirection;
    private bool isSneezing = false;
    private Vector3 fartPos;
    private float dashTime;
    private float dashCooldown = 0.0f;
    private bool isDashing = false;
    public GameObject circle = null;

    public GaugeBarController staminaBar;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();

        lastPosition = transform.position;
        circle.transform.localScale = new Vector3(5, 5, 1);
        circle.transform.localPosition = new Vector3(0, -3, 1);

        dashTime = startDashTime;

        staminaBar.SetMax((int) dashCooldownTime);
        staminaBar.SetValue((int)dashCooldownTime);
    }

    public void Sneeze()
    {
        isSneezing = true;
        circle.GetComponent<Animator>().SetBool("makingSound", true);
        animator.Play("Sneeze");
    }

    public void SneezeFinished()
    {
        isSneezing = false;
        circle.GetComponent<Animator>().SetBool("makingSound", false);
        animator.Play("Idle");
    }
    
    public void CollectKey()
    {
        FindObjectOfType<AudioManager>().Play("KeyCollect");
    }

    public void StopWalking()
    {
        animator.SetFloat("Speed", 0.0f);
    }

    public void Freeze(bool freeze)
    {
        IsFrozen = freeze;
    }

    public void EnteringZone(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Bathroom:
                GetComponent<FartSystem>().EnterBathroom();
                break;
            case ZoneType.Alley:
                GetComponent<SneezeSystem>().EnterAlley();
                break;
            case ZoneType.Cafetaria:
                GetComponent<HungerSystem>().EnterCafetaria();
                break;
        }
    }

    public void ExitingZone(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Bathroom:
                GetComponent<FartSystem>().ExitBathroom();
                break;
            case ZoneType.Alley:
                GetComponent<SneezeSystem>().ExitAlley();
                break;
            case ZoneType.Cafetaria:
                GetComponent<HungerSystem>().ExitCafetaria();
                break;
        }
    }

    void FixedUpdate()
    {

        if (dashCooldown > 0)
        {
            dashCooldown -= Time.fixedDeltaTime;
            staminaBar.IncreaseValue(Time.fixedDeltaTime);
        }

        if (IsFrozen)
        {
            StopWalking();
            return;
        }

        if (isSneezing)
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            return;
        }

        float effectiveSpeed = normalSpeed;
        float effectiveTimeBetweenFootstepSound = timeBetweenFootstepSound;

        if (Input.GetKey(KeyCode.LeftShift)) {
            isRunning = true;
            circle.GetComponent<Animator>().SetBool("makingSound", true);
            animator.speed = 2.0f;
            effectiveSpeed = runningSpeed;
            effectiveTimeBetweenFootstepSound = timeBetweenFootstepSound / 2.0f;
        } else
        {
            isRunning = false;
            circle.GetComponent<Animator>().SetBool("makingSound", false);

            animator.speed = 1.0f;
        }

        if ((Input.GetKey(KeyCode.Space) && dashCooldown <= 0) || isDashing) 
        {
            if (dashCooldown != dashCooldownTime)
                dashCooldown = dashCooldownTime;
            Dash();
        }
        else
        {
            dashTime = startDashTime;
            MovePosition(effectiveSpeed, effectiveTimeBetweenFootstepSound);
        }

    }

    private void Dash() 
    {
        if (!isDashing)
        {
            fartPos = transform.position;
            Instantiate(fart, fartPos, transform.rotation);
        }
        else 
        {
            staminaBar.SetValue(0);
        }

        circle.GetComponent<Animator>().SetBool("makingSound", true);

        FindObjectOfType<AudioManager>().Play("Dash");
        rb.velocity = lastDirection * dashSpeed;
        dashTime -= Time.fixedDeltaTime;

        isDashing = dashTime > 0;
        if (!isDashing) 
            dashTime = startDashTime;
    }
    
    private void MovePosition(float effectiveSpeed, float effectiveTimeBetweenFootstepSound) 
    {
        Vector3 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;

        lastDirection = velocity;

        if (velocity.sqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
        }

        rb.MovePosition(rb.transform.position + velocity.normalized * effectiveSpeed * Time.fixedDeltaTime);

        float distanceTravelled = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distanceTravelled >= 0.001f)
        {
            animator.SetFloat("Speed", effectiveSpeed);
            //FindObjectOfType<AudioManager>().PlayFootstepSound(false, effectiveTimeBetweenFootstepSound);
        } else {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}
