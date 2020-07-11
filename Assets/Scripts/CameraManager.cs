using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float maxX;
    public float maxY;
    public float minX;
    public float minY;

    private Camera camera = null;
    private Transform playerTransform = null;

    void Start()
    {
        camera = GetComponent<Camera>();

        camera.tag = "MainCamera";
        camera.orthographic = true;
        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, - 100.0f);
        camera.orthographicSize = 5;
    }

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3
        (
            Mathf.Clamp(playerTransform.position.x, minX, maxX),
            Mathf.Clamp(playerTransform.position.y, minY, maxY),
            -100.0f
        );
    }
}
