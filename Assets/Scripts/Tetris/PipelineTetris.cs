using UnityEngine;
using System.Collections.Generic;

public class PipelineTetris : MonoBehaviour
{
    [Header("Tetris Settings")]
    public float fallSpeed = 1f;
    public float fastFallSpeed = 10f;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public int previewCount = 3;

    private TetrisPiece currentPiece;
    private Queue<PipeType> nextPieces;
    private bool isGameActive = true;

    void Start()
    {
        InitializeNextPieces();
        SpawnNewPiece();
    }

    void Update()
    {
        if (!isGameActive || currentPiece == null) return;

        HandleInput();
        HandleFalling();
    }

    void InitializeNextPieces()
    {
        nextPieces = new Queue<PipeType>();
        for (int i = 0; i < previewCount; i++)
        {
            nextPieces.Enqueue(GetRandomPipeType());
        }
    }

    void SpawnNewPiece()
    {
        if (nextPieces.Count == 0) return;

        PipeType nextType = nextPieces.Dequeue();
        nextPieces.Enqueue(GetRandomPipeType());

        GameObject piecePrefab = PipeManager.Instance.GetPipePrefab(nextType);
        GameObject pieceObj = Instantiate(piecePrefab, spawnPoint.position, Quaternion.identity);

        currentPiece = pieceObj.GetComponent<TetrisPiece>();
        if (currentPiece == null)
            currentPiece = pieceObj.AddComponent<TetrisPiece>();

        currentPiece.Initialize(nextType);
    }

    void HandleInput()
    {
        // Touch input untuk mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchInput(touch.position);
            }
        }

        // Keyboard input untuk testing
        if (Input.GetKeyDown(KeyCode.A))
            MovePiece(-1, 0);
        if (Input.GetKeyDown(KeyCode.D))
            MovePiece(1, 0);
        if (Input.GetKeyDown(KeyCode.S))
            MovePiece(0, -1);
        if (Input.GetKeyDown(KeyCode.W))
            RotatePiece();
    }

    void HandleTouchInput(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // Determine action based on touch position relative to piece
        if (worldPos.x < currentPiece.transform.position.x - 0.5f)
            MovePiece(-1, 0);
        else if (worldPos.x > currentPiece.transform.position.x + 0.5f)
            MovePiece(1, 0);
        else if (worldPos.y > currentPiece.transform.position.y)
            RotatePiece();
        else
            MovePiece(0, -1);
    }

    void MovePiece(int deltaX, int deltaY)
    {
        if (currentPiece.CanMove(deltaX, deltaY))
        {
            currentPiece.Move(deltaX, deltaY);
        }
        else if (deltaY < 0) // Piece hit bottom
        {
            PlacePiece();
        }
    }

    void RotatePiece()
    {
        currentPiece.Rotate();
    }

    float fallTimer = 0f;

    void HandleFalling()
    {
        fallTimer += Time.deltaTime;

        float currentFallSpeed = Input.GetKey(KeyCode.S) ? fastFallSpeed : fallSpeed;

        if (fallTimer >= 1f / currentFallSpeed)
        {
            MovePiece(0, -1);
            fallTimer = 0f;
        }
    }


    void PlacePiece()
    {
        Vector2Int gridPos = WorldToGridPosition(currentPiece.transform.position);

        if (GameManager.Instance.PlacePipe(currentPiece.pipeType, gridPos.x, gridPos.y, currentPiece.rotation))
        {
            Destroy(currentPiece.gameObject);
            currentPiece = null;

            SpawnNewPiece();
        }
    }

    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / GameManager.Instance.cellSize);
        int y = Mathf.RoundToInt(worldPos.y / GameManager.Instance.cellSize);
        return new Vector2Int(x, y);
    }

    PipeType GetRandomPipeType()
    {
        PipeType[] availableTypes = {
            PipeType.Straight, PipeType.Corner,
            PipeType.T_Junction, PipeType.Cross
        };
        return availableTypes[Random.Range(0, availableTypes.Length)];
    }
}