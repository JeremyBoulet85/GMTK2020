﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.0f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;

        float speedSqrMagnitude = velocity.sqrMagnitude;
        animator.SetFloat("Speed", speedSqrMagnitude);
        if (speedSqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
        }

        transform.Translate(velocity.normalized * speed * Time.deltaTime);
    }
}
