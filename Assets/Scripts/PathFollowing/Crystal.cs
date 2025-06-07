using UnityEngine;
using System.Collections.Generic;

public class Crystal : MonoBehaviour
{
    private List<Vector2Int> path;
    private int currentPathIndex = 0;
    private float speed;
    private Vector3 targetPosition;

    public void InitializePath(List<Vector2Int> crystalPath, float moveSpeed)
    {
        path = new List<Vector2Int>(crystalPath);
        speed = moveSpeed;
        currentPathIndex = 0;

        // Set initial position
        Vector2Int startPos = path[0];
        transform.position = new Vector3(
            startPos.x * GameManager.Instance.cellSize,
            startPos.y * GameManager.Instance.cellSize,
            0
        );

        SetNextTarget();
    }

    void Update()
    {
        if (path == null || currentPathIndex >= path.Count) return;

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if reached target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;

            if (currentPathIndex >= path.Count)
            {
                // Reached end of path
                OnReachedDestination();
            }
            else
            {
                SetNextTarget();
            }
        }
    }

    void SetNextTarget()
    {
        if (currentPathIndex < path.Count)
        {
            Vector2Int nextPos = path[currentPathIndex];
            targetPosition = new Vector3(
                nextPos.x * GameManager.Instance.cellSize,
                nextPos.y * GameManager.Instance.cellSize,
                0
            );
        }
    }

    void OnReachedDestination()
    {
        // Crystal reached collector
        GameManager.Instance.CollectCrystal(this);
        Destroy(gameObject);
    }
}