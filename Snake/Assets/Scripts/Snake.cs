using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private int snakeSize;
    private LevelGrid levelGrid;
    private Vector2Int gridPos, gridMoveDir;
    private List<Transform> snakeBodyTranforms;
    private List<Vector2Int> snakeBodyPositions;
    private float gridMoveTimer, gridMoveTimerMax;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        gridPos            = new Vector2Int(10, 10);
        gridMoveDir        = new Vector2Int(1, 0);
        gridMoveTimerMax   = .1f;
        gridMoveTimer      = gridMoveTimerMax;
        snakeSize          = 0;
        snakeBodyPositions = new List<Vector2Int>();
        snakeBodyTranforms = new List<Transform>();
    }

    private void Update()
    {
        InputHandler();
        DrawSnake();
    }

    public bool checkCollision(Vector2Int position)
    {
        foreach (Vector2Int bodyPart in snakeBodyPositions)
        {
            if ( position == bodyPart)
            {
                return true;
            }
        }
        return false;
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDir.y != -1)
            {
                gridMoveDir.x = 0;
                gridMoveDir.y = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDir.y != 1)
            {
                gridMoveDir.x = 0;
                gridMoveDir.y = -1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDir.x != -1)
            {
                gridMoveDir.x = 1;
                gridMoveDir.y = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDir.x != 1)
            {
                gridMoveDir.x = -1;
                gridMoveDir.y = 0;
            }
        }
    }
    
    private void DrawSnake()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer > gridMoveTimerMax)
        {
            snakeBodyPositions.Insert(0, gridPos);

            gridPos += gridMoveDir;

            if (levelGrid.checkIfFoodIsEaten(gridPos))
            {
                CreateSnakeBody();
                snakeSize++;
            }

            if (snakeBodyPositions.Count >= snakeSize + 1)
            {
                snakeBodyPositions.RemoveAt(snakeBodyPositions.Count - 1);
            }

            transform.position     = new Vector3(gridPos.x, gridPos.y);
            transform.eulerAngles  = new Vector3(0, 0, CalculateHeadRotation() - 90);

            for (int i = 0; i < snakeBodyTranforms.Count; i++)
            {
                Vector3 snakeBodyPos = new Vector3(snakeBodyPositions[i].x, snakeBodyPositions[i].y);
                snakeBodyTranforms[i].position = snakeBodyPos;
            }

            gridMoveTimer         -= gridMoveTimerMax;
        }
    }

    private void CreateSnakeBody()
    {
        GameObject snakeBodyObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
        snakeBodyObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
        snakeBodyTranforms.Add(snakeBodyObject.transform);
    }

    private float CalculateHeadRotation()
    {
        float eulerZ = Mathf.Atan2(gridMoveDir.y, gridMoveDir.x) * Mathf.Rad2Deg;
        return (eulerZ < 0) ? (eulerZ + 360) : eulerZ;
    }
}
