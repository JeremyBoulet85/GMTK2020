using System.Collections;
using System.Collections.Generic;
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

    private Animator animator;

    enum PatrolState
    {
        Walking,
        Waiting,
        Chase
    }

    private PatrolState state;
    private Transform currentTarget;
    private Vector3 direction;
    private float timer = 0f;
    private Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = pointA;
        UpdateDirection(currentTarget.position);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch(state)
        {
            case PatrolState.Waiting:
                Wait();
                break;
        }
    }
    
    private void FixedUpdate()
    {
        DetectPlayer();
        switch (state)
        {
            case PatrolState.Walking:
                Walk();
                break;
            case PatrolState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void Wait()
    {
        animator.SetFloat("Speed", 0.0f);

        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            timer = 0f;
            state = PatrolState.Walking;
        }
    }

    private void Walk()
    {
        if (HasReachedTarget(currentTarget.position, 0.5f))
        {
            SwitchToWaiting();
            return;
        }

        UpdateAnimationDirection();
        transform.Translate(speed * direction * Time.deltaTime, Space.World);
    }

    private void DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized);
        if (hit.collider != null && hit.transform.tag == "Player")
        {
            UpdateDirection(player.position);
            state = PatrolState.Chase;
        }
    }

    private void ChasePlayer()
    {
        // For now... Exclamation!
        UpdateAnimationDirection();
        animator.SetFloat("Speed", 0.0f);
    }

    private void SwitchToWaiting()
    {
        currentTarget = currentTarget.Equals(pointA) ? pointB : pointA;
        UpdateDirection(currentTarget.position);
        state = PatrolState.Waiting;
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
        if (state == PatrolState.Walking)
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
