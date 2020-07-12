﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float normalSpeed = 4.0f;

    [SerializeField]
    private float runningSpeed = 8.0f;

    [SerializeField]
    public float dashSpeed = 30.0f;

    [SerializeField]
    public float startDashTime = 0.07f;

    [SerializeField]
    public float dashCooldownTime = 10.0f;

    private Animator animator;
    private Rigidbody2D rb;
    private SoundManager soundManager;
    private float timeBetweenFootstepSound = 0.34f;
    private Vector3 lastPosition;
    private Vector3 lastDirection;
    private bool isSneezing = false;

    private float dashTime;
    private float dashCooldown = 0.0f;
    private bool isDashing = false;


    private void Start()
    {
        animator     = GetComponent<Animator>();
        rb           = GetComponent<Rigidbody2D>();
        soundManager = GetComponent<SoundManager>();

        lastPosition = transform.position;

        dashTime = startDashTime;
    }

    public void Sneeze()
    {
        isSneezing = true;
        animator.Play("Sneeze");
        soundManager.PlaySound(SoundType.Sneeze, 0.9f);
    }

    public void SneezeFinished()
    {
        isSneezing = false;
        animator.Play("Idle");
    }
    
    public void CollectKey()
    {
        soundManager.PlaySound(SoundType.KeyCollect, 0.7f);
    }

    private bool soundInit = false;
    
    private void InitSound()
    {
        soundInit = true;
        soundManager.PlaySound(SoundType.Music, 0.06f);
        soundManager.PlaySound(SoundType.Ambiance, 0.1f);
    }

    void FixedUpdate()
    {
        if (!soundInit)
            InitSound();

        if (isSneezing)
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            return;
        }

        float effectiveSpeed = normalSpeed;
        float effectiveTimeBetweenFootstepSound = timeBetweenFootstepSound;

        if (Input.GetKey(KeyCode.LeftShift)) {
            animator.speed = 2.0f;
            effectiveSpeed = runningSpeed;
            effectiveTimeBetweenFootstepSound = timeBetweenFootstepSound / 2.0f;
        } else
        {
            animator.speed = 1.0f;
        }

        if (dashCooldown > 0)
            dashCooldown -= Time.fixedDeltaTime;

        if ((Input.GetKey(KeyCode.Space) && dashCooldown <= 0) || isDashing) 
        {
            if (dashCooldown != dashCooldownTime)
                dashCooldown = dashCooldownTime;
            Dash();
        }
        else
        {
            dashTime = startDashTime;
            MovePosition(effectiveSpeed, effectiveTimeBetweenFootstepSound);
        }

    }

    private void Dash() 
    {
        rb.velocity = lastDirection * dashSpeed;
        dashTime -= Time.fixedDeltaTime;

        isDashing = dashTime > 0;
        if (!isDashing) 
            dashTime = startDashTime;
    }

    private void MovePosition(float effectiveSpeed, float effectiveTimeBetweenFootstepSound) 
    {
        Vector3 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;

        lastDirection = velocity;

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
            soundManager.PlayFootstepSound(effectiveTimeBetweenFootstepSound);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}
