using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.0f;

    private Animator animator;
    private Rigidbody2D rb;
    private SoundManager soundManager;

    private void Start()
    {
        animator     = GetComponent<Animator>();
        rb           = GetComponent<Rigidbody2D>();
        soundManager = GetComponent<SoundManager>();
    }

    public void Sneeze()
    {
        soundManager.PlaySneezeSound();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)    ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)  ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)  ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1.0f : 0.0f;

        float speedSqrMagnitude = velocity.sqrMagnitude;
        animator.SetFloat("Speed", speedSqrMagnitude);
        if (speedSqrMagnitude >= 0.01f)
        {
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
        }

        rb.MovePosition(rb.position + velocity.normalized * speed * Time.fixedDeltaTime);
    }
}
