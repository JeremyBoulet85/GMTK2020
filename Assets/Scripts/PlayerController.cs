using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = new Vector3();

        velocity.y += Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        velocity.x -= Input.GetKey(KeyCode.A) ? 1.0f : 0.0f;
        velocity.y -= Input.GetKey(KeyCode.S) ? 1.0f : 0.0f;
        velocity.x += Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;

        transform.Translate(velocity.normalized * speed * Time.deltaTime);
    }
}
