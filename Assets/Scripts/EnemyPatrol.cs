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
    GameObject exclamation;
    private SoundManager soundManager;
    private float footstepSoundInterval = 0.8f;
    private float footstepSoundDistance = 10.0f;
    private Animator animator;

    enum PatrolState
    {
        Walking,
        Waiting,
        FoundPlayer,
        Stop
    }

    private PatrolState state;
    private Transform currentTarget = null;
    private Vector3 direction;
    private float waitTimer = 0f;
    private float foundTimer = 0f;
    private Transform player;
    private GameObject exclamationInstance = null;
    private Canvas canvas;

    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
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
                DetectPlayer();
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

        UpdateDirection(player.position);
        UpdateAnimationDirection();

        animator.SetFloat("Speed", 0.0f);

        if (foundTimer == 0)
        {
            if (canvas != null)
            {
                exclamationInstance = Instantiate(exclamation, canvas.transform);
                exclamationInstance.transform.position = new Vector2(transform.position.x, transform.position.y + 1.7f);
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
        UpdateAnimationDirection();
    }

    private bool HasReachedTarget(Vector3 target, float distance)
    {
        return Vector3.Distance(transform.position, target) < distance;
    }

    private void UpdateDirection(Vector3 target)
    {
        direction = (target - transform.position).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == PatrolState.Walking && !collision.collider.CompareTag("Player"))
        {
            SwitchToWaiting();
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
