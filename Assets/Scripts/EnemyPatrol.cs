using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    enum PatrolState
    {
        Walking,
        Looking
    }

    private PatrolState state;

    [SerializeField]
    float speed;

    [SerializeField]
    Transform pointB;

    Transform pointA;
    Vector3 currentTarget;

    Vector3 direction;

    float waitTime = 2f;
    float timer = 0f;
    int lookingCounter = 0;

    void Awake()
    {
        pointA = transform;
        currentTarget = pointB.position;

        Rotate(currentTarget);
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == PatrolState.Walking)
        {
            if (Vector3.Distance(transform.position, currentTarget) < 0.5f)
            {
                state = PatrolState.Looking;
            }
        }
        else if (state == PatrolState.Looking)
        {
            timer += Time.deltaTime;
            if (timer > waitTime)
            {
                timer = 0f;
                Rotate(Quaternion.AngleAxis(90, Vector3.forward) * direction);
                lookingCounter++;
                if (lookingCounter > 2)
                {
                    lookingCounter = 0;
                    currentTarget = currentTarget == pointA.position ? pointB.position : pointA.position;
                    state = PatrolState.Walking;
                }
            }
        }
    }

    
    private void FixedUpdate()
    {
        if (state == PatrolState.Walking)
        {
            transform.Translate(speed * direction * Time.deltaTime, Space.World);
        }
    }

    private void Rotate(Vector3 target)
    {
        UpdateDirection(target);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        direction = direction.normalized;
    }

    private void UpdateDirection(Vector3 target)
    {
        direction = target - transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == PatrolState.Walking)
        {
            state = PatrolState.Looking;
        }
    }
}
