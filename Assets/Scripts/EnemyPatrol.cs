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
    float speed = 1f;

    [SerializeField]
    float waitTime = 2f;

    private Animator animator;

    enum PatrolState
    {
        Walking,
        Waiting
    }

    private PatrolState state;
    private Transform currentTarget;
    private Vector3 direction;
    private float timer = 0f;

    void Awake()
    {
        currentTarget = pointA;
        UpdateDirection(currentTarget.position);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (state == PatrolState.Waiting)
        {
            animator.SetFloat("Speed", 0.0f);

            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                timer = 0f;
                SwitchToWalking();
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (state == PatrolState.Walking)
        {
            if (HasReachedTarget())
            {
                state = PatrolState.Waiting;
            }
            else
            {
                Walk();
            }
        }
    }

    private void SwitchToWalking()
    {
        currentTarget = currentTarget.Equals(pointA) ? pointB : pointA;
        UpdateDirection(currentTarget.position);
        state = PatrolState.Walking;
    }

    private bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, currentTarget.position) < 0.05f;
    }

    private void Walk()
    {
        float directionSqrMagnitude = direction.sqrMagnitude;
        animator.SetFloat("Speed", directionSqrMagnitude);
        if (directionSqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }

        transform.Translate(speed * direction * Time.deltaTime, Space.World);
    }

    private void UpdateDirection(Vector3 target)
    {
        direction = target - transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == PatrolState.Walking)
        {
            state = PatrolState.Waiting;
        }
    }
}
