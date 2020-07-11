using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float normalSpeed = 4.0f;

    [SerializeField]
    private float runningSpeed = 8.0f;

    private Animator animator;
    private Rigidbody2D rb;
    private SoundManager soundManager;
    private Vector3 lastPosition;

    private void Start()
    {
        animator     = GetComponent<Animator>();
        rb           = GetComponent<Rigidbody2D>();
        soundManager = GetComponent<SoundManager>();

        lastPosition = transform.position;
    }

    public void Sneeze()
    {
        soundManager.PlaySound(SoundType.Sneeze, 0.9f);
    }

    void FixedUpdate()
    {
        float effectiveSpeed = normalSpeed;

        if (Input.GetKey(KeyCode.LeftShift)) {
            animator.speed = 2.0f;
            effectiveSpeed = runningSpeed;
        } else
        {
            animator.speed = 1.0f;
        }

        Vector3 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)    ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)  ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)  ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;

        if (velocity.sqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
        }

        rb.MovePosition(rb.transform.position + velocity.normalized * effectiveSpeed * Time.fixedDeltaTime);

        float distanceTravelled = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distanceTravelled >= 0.001f)
        {
            animator.SetFloat("Speed", effectiveSpeed);
            soundManager.PlayFootstepSound(0.34f);
        } else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}
