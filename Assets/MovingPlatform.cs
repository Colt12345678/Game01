using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;

    private Transform currentTarget;

    private void Start()
    {
        // Start by heading towards Point B
        currentTarget = pointB;
    }

    private void Update()
    {
        // Move the platform towards the target point
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Check if we arrived at the target point
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            // Switch targets to bounce back and forth
            if (currentTarget == pointA) currentTarget = pointB;
            else currentTarget = pointA;
        }
    }

    // When the player steps on the platform, make them a child so they don't slide off
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    // When the player jumps off, remove them as a child
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}