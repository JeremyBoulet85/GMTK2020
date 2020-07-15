using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
        UpdateConeColor(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanHearPlayer(Vector2 playerLocation, float detectionRadius, bool playerIsRunning)
    {
        if (FindObjectOfType<AudioManager>().madeSound && PointInsideSphere(playerLocation, detectionRadius) ||
            playerIsRunning && PointInsideSphere(playerLocation, detectionRadius))
        {
            FindObjectOfType<AudioManager>().madeSound = false;
            return true;
        }

        return false;
    }

    public bool CanSeePlayer(Vector2 playerLocation, float detectionRadius, Vector2 direction)
    {
        return PointInsideSphere(playerLocation, detectionRadius) && IsInsideVisionCone(playerLocation, direction) && CanSeePlayer(playerLocation);
    }

    private bool PointInsideSphere(Vector2 point, float radius)
    {
        float distance = Vector2.Distance(point, transform.position);

        return distance < radius;
    }


    private bool IsInsideVisionCone(Vector3 location, Vector2 direction)
    {
        return Vector2.Angle(location - transform.position, direction.normalized) < 10f;
    }

    private bool CanSeePlayer(Vector2 playerLocation)
    {
        int layerMask = 1 << 8;

        layerMask = ~layerMask;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, playerLocation, layerMask);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;

        }

        return hit.collider == null;
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        return (Quaternion.Euler(0, 0, degrees) * v).normalized;
    }

    public void UpdateConeColor(bool isAlerted)
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Color color = isAlerted ? Color.red : Color.yellow;
        color.a = 0.5f;

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void DrawCone(Vector2 direction, float detectionRadius)
    {
        Vector2 position = transform.localPosition;
        Vector2 u = (direction.normalized * detectionRadius).normalized;
        Vector2 A = Rotate(u, 10) * detectionRadius;
        Vector2 B = Rotate(u, -10) * detectionRadius;

        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, A);
        lineRenderer.SetPosition(2, B);
        lineRenderer.SetPosition(3, Vector2.zero);
    }

}
