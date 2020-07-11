using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float maxX;
    public float maxY;
    public float minX;
    public float minY;

    private Camera cam = null;
    private Transform playerTransform = null;

    void Start()
    {
        cam = GetComponent<Camera>();

        cam.tag = "MainCamera";
        cam.orthographic = true;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, - 100.0f);
        cam.orthographicSize = 5;
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
