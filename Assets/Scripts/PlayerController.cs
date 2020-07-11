﻿using System.Collections;
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
    private float timeBetweenFootstepSound = 0.34f;
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
            soundManager.PlayFootstepSound(effectiveTimeBetweenFootstepSound);
        } else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}
