using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;

    private Vector2 direction;
    private Animator animator;
    private Vector2[] directions = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
    private float footstepSoundDistance = 10.0f;
    private float footstepSoundInterval = 0.8f;

    void Start()
    {
        animator = GetComponent<Animator>();
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

    public bool WalkTowardTarget(Vector2 target, Vector2 playerLocation)
    {
        if (HasReachedTarget(target, 0.5f))
        {
            return true;
        }

        UpdateDirection(target);
        if (Vector3.Distance(transform.position, playerLocation) < footstepSoundDistance)
        {
            FindObjectOfType<AudioManager>().PlayFootstepSound(true, footstepSoundInterval);
        }
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
