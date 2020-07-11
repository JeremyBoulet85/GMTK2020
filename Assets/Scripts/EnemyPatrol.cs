using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    Transform pointA = null;

    [SerializeField]
    Transform pointB = null;

    [SerializeField]
    float speed = 2f;

    [SerializeField]
    float waitTime = 2f;

    [SerializeField]
    GameObject exclamation = null;
    private SoundManager soundManager;
    private Vector3 soundLocation;
    private float footstepSoundInterval = 0.8f;
    private float footstepSoundDistance = 10.0f;
    private Animator animator;

    enum PatrolState
    {
        Walking,
        Waiting,
        FoundPlayer,
        Stop,
        Investigating,
        LookAround,
        Chase
    }

    private PatrolState state;
    private Transform currentTarget = null;
    private Vector3 direction;
    private float waitTimer = 0f;
    private float foundTimer = 0f;
    private GameObject player;
    private GameObject exclamationInstance = null;
    private Canvas canvas;
    private Camera cam;
    private Vector3[] directions = { new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0) };
    private int lookAroundCounter = 0;
    private float lookAroundTime = 1.5f;

    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        animator = GetComponent<Animator>();
        soundManager = GetComponent<SoundManager>();
        SwitchToWalking();
    }

    private void Update()
    {
        switch (state)
        {
            case PatrolState.Waiting:
                Wait();
                SeePlayer();
                break;
            case PatrolState.Investigating:
                Investigate();
                SeePlayer();
                break;
            case PatrolState.LookAround:
                LookAround();
                SeePlayer();
                break;
            case PatrolState.FoundPlayer:
                FoundPlayer();
                break;
            case PatrolState.Stop:
                Stop();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PatrolState.Walking:
                Walk();
                SeePlayer();
                break;
            case PatrolState.Chase:
                Chase();
                SeePlayer();
                break;
        }
    }

    private void Wait()
    {
        if (state != PatrolState.Waiting)
            return;

        animator.SetFloat("Speed", 0.0f);

        waitTimer += Time.deltaTime;
        if (waitTimer > waitTime)
        {
            waitTimer = 0f;
            SwitchToWalking();
        }
    }

    private void Walk()
    {
        if (state != PatrolState.Walking)
            return;

        if (HasReachedTarget(currentTarget.position, 0.5f))
        {
            SwitchToWaiting();
            return;
        }

        MoveCharacter();
    }

    private void MoveCharacter()
    {
        UpdateAnimationDirection();
        if (Vector3.Distance(transform.position, player.transform.position) < footstepSoundDistance)
        {
            soundManager.PlayFootstepSound(footstepSoundInterval, 0.15f);
        }
        transform.Translate(speed * direction * Time.deltaTime, Space.World);
    }

    private void FoundPlayer()
    {
        if (state != PatrolState.FoundPlayer)
            return;

        if (foundTimer == 0)
        {
            UpdateDirection(player.transform.position);

            if (canvas != null)
            {
                exclamationInstance = Instantiate(exclamation, canvas.transform);
                exclamationInstance.transform.position = cam.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + 1.7f));
            }
        }

        foundTimer += Time.deltaTime;
        if (foundTimer > waitTime)
        {
            if (exclamationInstance != null)
            {
                Destroy(exclamationInstance);
            }
            foundTimer = 0f;

            state = PatrolState.Stop;
        }
    }

    private void Stop()
    {
        if (state != PatrolState.Stop)
            return;

        GameManager.instance.StrikeCount++;
        SwitchToWalking();
    }

    private void Investigate()
    {
        if (state != PatrolState.Investigating)
            return;

        float distance = Vector3.Distance(transform.position, soundLocation);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, Vector3.Distance(transform.position, soundLocation));

        if (hit.collider != null && hit.collider.CompareTag("Sound"))
        {
            // No obstacle between enemy and sound = Walk toward it and look around
            state = PatrolState.Chase;
        }
        else
        {
            // Obstacle between enemy and sound = Stay in place and look around
            state = PatrolState.LookAround;
        }
    }

    private void Chase()
    {
        if (state != PatrolState.Chase)
            return;

        if (HasReachedTarget(soundLocation, 0.5f))
        {
            state = PatrolState.LookAround;
        }
        else
        {
            UpdateDirection(soundLocation);
            MoveCharacter();
        }
    }

    private void LookAround()
    {
        if (state != PatrolState.LookAround)
            return;

        waitTimer += Time.deltaTime;
        if (waitTimer > lookAroundTime)
        {
            waitTimer = 0f;
            if (lookAroundCounter < 4)
            {
                direction = directions[lookAroundCounter];
                UpdateAnimationDirection();
                lookAroundCounter++;
            }
            else
            {
                lookAroundCounter = 0;
                SwitchToWalking();
            }
        }
    }

    private void SwitchToWaiting()
    {
        state = PatrolState.Waiting;
    }

    private void SwitchToWalking()
    {
        state = PatrolState.Walking;
        if (currentTarget == null)
        {
            currentTarget = pointA;
        }
        else
        {
            currentTarget = currentTarget.Equals(pointA) ? pointB : pointA;
        }
        UpdateDirection(currentTarget.position);
    }

    private bool HasReachedTarget(Vector3 target, float distance)
    {
        return Vector3.Distance(transform.position, target) < distance;
    }

    private void UpdateDirection(Vector3 target)
    {
        direction = (target - transform.position).normalized;
        UpdateAnimationDirection();
    }

    private void SeePlayer()
    {
        if (PlayerInsideSphere() && IsInsideVisionCone() && CanSeePlayer())
        {
            state = PatrolState.FoundPlayer;
        }
    }

    private bool PlayerInsideSphere()
    {
        return Vector3.Distance(player.transform.position, transform.position) < 8f;
    }

    private bool IsInsideVisionCone()
    {
        Vector2 targetDir = player.transform.position - transform.position;
        Vector2 forward = direction.normalized;
        
        return Vector2.Angle(targetDir, forward) < 10f;
    }

    //private void HearPlayerSound()
    //{
    //    if (player.GetComponent<SoundManager>().madeSound && PointInsideSphere(player.transform.position, transform.position, 8f))
    //    {
    //        UpdateDirection(player.transform.position);
    //        if (CanSeePlayer())
    //        {
    //            state = PatrolState.FoundPlayer;
    //        }
    //        else
    //        {
    //            state = PatrolState.LookAround;
    //        }
    //    }
    //}

    private bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;

        }
        return hit.collider == null;
    }

    private void UpdateAnimationDirection()
    {
        float directionSqrMagnitude = direction.sqrMagnitude;
        animator.SetFloat("Speed", directionSqrMagnitude);
        if (directionSqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
    }
}
