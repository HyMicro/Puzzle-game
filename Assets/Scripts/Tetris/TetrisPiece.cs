using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    public PipeType pipeType;
    public int rotation = 0;

    private float fallTimer = 0f;
    private float fallInterval = 1f;

    // Touch input
    private Vector2 lastTouchPos;
    private float swipeThreshold = 50f;
    private float tapThreshold = 20f;

    public void Initialize(PipeType type)
    {
        pipeType = type;
        rotation = 0;
    }

    void Update()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= fallInterval)
        {
            Move(0, -1);
            fallTimer = 0f;
        }

        HandleMobileInput();
    }

    public void Move(int deltaX, int deltaY)
    {
        if (CanMove(deltaX, deltaY))
        {
            transform.position += new Vector3(deltaX, deltaY, 0) * GameManager.Instance.cellSize;
        }
    }

    public void Rotate()
    {
        rotation = (rotation + 1) % 4;
        transform.rotation = Quaternion.Euler(0, 0, rotation * 90);
    }

    public bool CanMove(int deltaX, int deltaY)
    {
        Vector3 newPos = transform.position + new Vector3(deltaX, deltaY, 0) * GameManager.Instance.cellSize;
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(newPos.x / GameManager.Instance.cellSize),
            Mathf.RoundToInt(newPos.y / GameManager.Instance.cellSize)
        );

        if (gridPos.x < 0 || gridPos.x >= GameManager.Instance.gridWidth ||
            gridPos.y < 0 || gridPos.y >= GameManager.Instance.gridHeight)
            return false;

        GridCell cell = GameManager.Instance.GetCell(gridPos.x, gridPos.y);
        return cell != null && cell.IsEmpty();
    }

    void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastTouchPos = touchPos;
                    break;

                case TouchPhase.Moved:
                    Vector2 deltaPos = touchPos - lastTouchPos;

                    if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                    {
                        if (deltaPos.x > swipeThreshold) Move(1, 0);
                        else if (deltaPos.x < -swipeThreshold) Move(-1, 0);
                    }
                    else
                    {
                        if (deltaPos.y < -swipeThreshold) Move(0, -1);
                    }

                    lastTouchPos = touchPos;
                    break;

                case TouchPhase.Ended:
                    if (Vector2.Distance(touchPos, lastTouchPos) < tapThreshold)
                    {
                        Rotate();
                    }
                    break;
            }
        }
    }
}
