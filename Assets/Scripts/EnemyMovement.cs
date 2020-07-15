using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;

    private Vector2 direction;
    private Animator animator;
    private Vector2[] directions = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDirection(int index)
    {
        direction = directions[index];
        UpdateAnimationDirection();
        StopAnimation();
    }

    public void UpdateDirection(Vector3 newTarget)
    {
        direction = (newTarget - transform.position).normalized;
        UpdateAnimationDirection();
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    public bool WalkTowardTarget(Vector2 target)
    {
        if (HasReachedTarget(target, 0.5f))
        {
            return true;
        }
        UpdateDirection(target);
        transform.Translate(speed * direction * Time.deltaTime, Space.World);

        return false;
    }

    public void UpdateAnimationDirection()
    {
        float directionSqrMagnitude = direction.sqrMagnitude;
        animator.SetFloat("Speed", directionSqrMagnitude);
        if (directionSqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
    }

    public void StopAnimation()
    {
        animator.SetFloat("Speed", 0.0f);
    }

    
    private bool HasReachedTarget(Vector3 target, float distance)
    {
        return Vector3.Distance(transform.position, target) < distance;
    }
}
