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

    [SerializeField]
    GameObject interrogation = null;

    [SerializeField]
    float soundDetectionRadius = 5f;

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
        CaughtPlayer,
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
    private GameObject reactionInstance = null;

    private Vector3[] directions = { new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0) };
    private int lookAroundCounter = 0;
    private float lookAroundTime = 1.5f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        soundManager = GetComponent<SoundManager>();
        SwitchToWalking();
    }

    private void Update()
    {
        SeePlayer();
        HearPlayerSound();
        switch (state)
        {
            case PatrolState.Waiting:
                Wait();
                break;
            case PatrolState.Investigating:
                Investigate();
                break;
            case PatrolState.LookAround:
                LookAround();
                break;
            case PatrolState.FoundPlayer:
                FoundPlayer();
                break;
            case PatrolState.CaughtPlayer:
                StrikePlayer();
                break;
        }
    }

    private void FixedUpdate()
    {
        SeePlayer();
        HearPlayerSound();
        switch (state)
        {
            case PatrolState.Walking:
                Walk();
                break;
            case PatrolState.Chase:
                Chase();
                break;
        }
    }

    private void Wait()
    {
        if (state != PatrolState.Waiting)
            return;

        LookAround();

        // Code if we want to just stop:
        //animator.SetFloat("Speed", 0.0f);

        //waitTimer += Time.deltaTime;
        //if (waitTimer > waitTime)
        //{
        //    waitTimer = 0f;
        //    SwitchToWalking();
        //}
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

            InstantiateReaction(exclamation);
        }

        foundTimer += Time.deltaTime;
        if (foundTimer > waitTime)
        {
            DestroyReaction();
            foundTimer = 0f;

            state = PatrolState.CaughtPlayer;
        }
    }

    private void StrikePlayer()
    {
        if (state != PatrolState.CaughtPlayer)
            return;

        GameManager.instance.StrikeCount++;
        SwitchToWalking();
    }

    private void Investigate()
    {
        if (state != PatrolState.Investigating)
            return;

        if (foundTimer == 0)
        {
            UpdateDirection(player.transform.position);

            InstantiateReaction(interrogation);
        }
            
        foundTimer += Time.deltaTime;
        if (foundTimer > 1.5f)
        {
            foundTimer = 0f;
            DestroyReaction();

            RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                state = PatrolState.FoundPlayer;
            }
            else if (hit.collider == null)
            {
                state = PatrolState.Chase;
            }
            else
            {
                state = PatrolState.LookAround;
            }
        }
    }

    private void InstantiateReaction(GameObject reaction)
    {
        DestroyReaction();
        reactionInstance = Instantiate(reaction, gameObject.transform);
        reactionInstance.transform.position = new Vector2(transform.position.x, transform.position.y + 1.5f);
    }

    private void DestroyReaction()
    {
        if (reactionInstance != null)
        {
            Destroy(reactionInstance);
        }
    }

    private void ResetTimers()
    {
        foundTimer = 0f;
        waitTimer = 0f;
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
        if (state != PatrolState.LookAround && state != PatrolState.Waiting)
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
        if (state == PatrolState.FoundPlayer || state == PatrolState.CaughtPlayer)
        {
            return;
        }

        if (PointInsideSphere(player.transform.position, 8f) && IsInsideVisionCone() && CanSeePlayer())
        {
            ResetTimers();
            state = PatrolState.FoundPlayer;
        }
    }

    private bool PointInsideSphere(Vector2 point, float radius)
    {
        return Vector2.Distance(point, transform.position) < radius;
    }

    private bool IsInsideVisionCone()
    {
        Vector2 targetDir = player.transform.position - transform.position;
        Vector2 forward = direction.normalized;
        
        return Vector2.Angle(targetDir, forward) < 10f;
    }

    private void HearPlayerSound()
    {
        if (state == PatrolState.FoundPlayer || state == PatrolState.CaughtPlayer)
        {
            return;
        }

        if (player.GetComponent<SoundManager>().madeSound && PointInsideSphere(player.transform.position, soundDetectionRadius))
        {
            ResetTimers();
            player.GetComponent<SoundManager>().madeSound = false;
            soundLocation = player.transform.position;

            UpdateDirection(soundLocation);
            if (CanSeePlayer())
            {
                state = PatrolState.FoundPlayer;
            }
            else
            {
                state = PatrolState.Investigating;
            }
        }
    }

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
