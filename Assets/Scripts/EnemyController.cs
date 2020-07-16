using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    Transform pointA = null;
    [SerializeField]
    Transform pointB = null;
    [SerializeField]
    GameObject foundPlayerReaction = null;
    [SerializeField]
    GameObject alertedReaction = null;
    [SerializeField]
    bool isMale = false;

    private EnemyState state;
    private EnemyMovement movement;
    private PlayerDetection detection;
    private Transform currentTarget = null;
    private GameObject player;
    private GameObject reactionInstance = null;
    private Vector2 alertLocation;

    private float waitTimer = 0f;
    private float alertedTimer = 0f;
    private float foundTimer = 0f;
    private float lookAroundTimer = 0f;
    private int lookAroundCounter = 0;
    private const float soundDetectionRadius = 3f;
    private const float visionDetectionRadius = 4.5f;

    enum EnemyState
    {
        // Don't move the order!
        Patrolling,
        Waiting,
        SwitchingTarget,
        Alerted,
        Investigating,
        LookingAround,
        FoundPlayer
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movement = GetComponent<EnemyMovement>();
        detection = GetComponent<PlayerDetection>();
        player.gameObject.GetComponent<PlayerController>().OnSoundProduced += ReactToSound;

        if (currentTarget == null)
        {
            currentTarget = pointA;
        }
        else
        {
            currentTarget = currentTarget.Equals(pointA) ? pointB : pointA;
        }
        movement.UpdateDirection(currentTarget.position);
        state = EnemyState.Patrolling;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case EnemyState.Alerted:
                Alerted();
                break;
            case EnemyState.Investigating:
                FindPlayer();
                Investigate();
                break;
            case EnemyState.LookingAround:
                FindPlayer();
                LookAround();
                break;
            case EnemyState.Waiting:
                ReactToVision();
                Wait();
                break;
            case EnemyState.SwitchingTarget:
                ReactToVision();
                SwitchTarget();
                break;
            case EnemyState.FoundPlayer:
                FoundPlayer();
                break;
        }
        UpdateVisionCone();
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Patrolling)
        {
            ReactToVision();
            Patrol();
        }
        detection.DrawCone(movement.GetDirection(), visionDetectionRadius);
    }

    private void Patrol()
    {
        if (state != EnemyState.Patrolling)
        {
            return;
        }

        if (movement.WalkTowardTarget(currentTarget.position, player.transform.position))
        {
            // If reached target, wait
            state = EnemyState.Waiting;
        }
    }

    private void SwitchTarget()
    {
        if (state != EnemyState.SwitchingTarget)
        {
            return;
        }

        // Set new target
        currentTarget = currentTarget == null || currentTarget.Equals(pointA) ? pointB : pointA;

        // Set direction toward target
        movement.UpdateDirection(currentTarget.position);
        state = EnemyState.Patrolling;
    }

    private void Alerted()
    {
        if (state != EnemyState.Alerted)
        {
            return;
        }

        if (alertedTimer == 0)
        {
            InstantiateReaction(alertedReaction);
        }

        alertedTimer += Time.deltaTime;
        if (alertedTimer > 0.7f)
        {
            alertedTimer = 0f;
            state = EnemyState.Investigating;
        }
    }

    private void Investigate()
    {
        if (state != EnemyState.Investigating)
        {
            return;
        }

        RaycastHit2D alertHit = Physics2D.Linecast(transform.position, alertLocation);
        if (alertHit.collider != null && alertHit.collider.CompareTag("Player"))
        {
            DestroyReaction();
            state = EnemyState.FoundPlayer;
        }
        else if (alertHit.collider == null )
        {
            // If no obstacle between enemy and alert location, walk to it
            if (movement.WalkTowardTarget(alertLocation, player.transform.position))
            {
                state = EnemyState.LookingAround;
            }
        }
        else
        {
            // If there are obstacles between enemy and alert location, look around
            state = EnemyState.LookingAround;
        }
    }

    private void LookAround()
    {
        if (state != EnemyState.LookingAround)
        {
            return;
        }

        lookAroundTimer += Time.deltaTime;
        if (lookAroundTimer > 1f)
        {
            lookAroundTimer = 0f;
            if (lookAroundCounter < 4)
            {
                movement.UpdateDirection(lookAroundCounter);
                lookAroundCounter++;
            }
            else
            {
                lookAroundCounter = 0;
                DestroyReaction();
                state = EnemyState.Patrolling;
            }
        }
    }

    private void Wait()
    {
        if (state != EnemyState.Waiting)
        {
            return;
        }

        movement.StopAnimation();

        waitTimer += Time.deltaTime;
        if (waitTimer > 1.5f)
        {
            waitTimer = 0f;
            state = EnemyState.SwitchingTarget;
        }
    }

    private void FoundPlayer()
    {
        if (state != EnemyState.FoundPlayer)
        {
            return;
        }

        player.GetComponent<PlayerController>().Freeze(true);

        if (foundTimer == 0)
        {
            GameManager.instance.GetStriked();
            movement.UpdateDirection(player.transform.position);
            FindObjectOfType<AudioManager>().Play(isMale ? "MaleAngry" : "FemaleAngry");
            InstantiateReaction(foundPlayerReaction);
        }

        foundTimer += Time.deltaTime;
        if (foundTimer > 2f)
        {
            DestroyReaction();
            foundTimer = 0f;

            if (!GameManager.instance.IsGameOver)
            {
                GameManager.instance.Respawn();
            }
            else
            {
                GameManager.instance.GameOver();
            }
            state = EnemyState.Patrolling;
            player.GetComponent<PlayerController>().Freeze(false);
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

    private void FindPlayer()
    {
        if (detection.CanSeePlayer(player.transform.position, visionDetectionRadius, movement.GetDirection()))
        {
            state = EnemyState.FoundPlayer;
        }
    }

    private void ReactToVision()
    {

        Vector2 playerLocation = player.transform.position;
        if (detection.CanSeePlayer(playerLocation, visionDetectionRadius, movement.GetDirection()))
        {
            if (state == EnemyState.Alerted)
            {
                alertLocation = playerLocation;
                state = EnemyState.Investigating;
            }
            else
            {
                alertLocation = playerLocation;
                state = EnemyState.Alerted;
            }
        }
    }

    private void ReactToSound(Transform player)
    {
        if (detection.CanHearPlayer(player.position, soundDetectionRadius))
        {
            if (state == EnemyState.Alerted || state == EnemyState.Investigating)
            {
                alertLocation = player.position;
                state = EnemyState.Investigating;
            }
            else
            {
                alertLocation = player.position;
                state = EnemyState.Alerted;
            }
        }
    }

    private void UpdateVisionCone()
    {
        detection.UpdateConeColor(state >= EnemyState.Alerted);
        detection.DrawCone(movement.GetDirection(), visionDetectionRadius);
    }
}
