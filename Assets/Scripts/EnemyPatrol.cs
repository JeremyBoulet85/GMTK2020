using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Transform player;
    private GameObject exclamationInstance = null;
    private Canvas canvas;
    private Camera cam;
    private Vector3 soundLocation;
    private bool alerted = false;
    private Vector3[] directions = { new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0) };
    private int lookAroundCounter = 0;
    private float lookAroundTime = 1.5f;

    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
                DetectPlayer();
                Wait();
                break;
            case PatrolState.Investigating:
                DetectPlayer();
                Investigate();
                break;
            case PatrolState.LookAround:
                DetectPlayer();
                LookAround();
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
                DetectPlayer();
                Walk();
                break;
            case PatrolState.Chase:
                DetectPlayer();
                Chase();
                break;
        }
    }

    private void Wait()
    {
        if (state != PatrolState.Waiting)
            return;

        alerted = false;

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

        alerted = false;

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
        if (Vector3.Distance(transform.position, player.position) < footstepSoundDistance)
        {
            soundManager.PlayFootstepSound(footstepSoundInterval, 0.15f);
        }
        transform.Translate(speed * direction * Time.deltaTime, Space.World);
    }

    private void DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized);
        Debug.DrawRay(transform.position, direction * 1000);
        if (hit.collider != null && hit.transform.tag == "Player")
        {
            state = PatrolState.FoundPlayer;
        }
    }

    private void FoundPlayer()
    {
        if (state != PatrolState.FoundPlayer)
            return;

        alerted = false;

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

        alerted = false;

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
                alerted = false;
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Sound") && !alerted)
        {
            alerted = true;
            soundLocation = collider.gameObject.transform.position;
            UpdateDirection(soundLocation);
            state = PatrolState.Investigating;
        }
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
