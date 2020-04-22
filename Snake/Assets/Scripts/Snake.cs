using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private enum       Direction  { UP, DOWN, LEFT, RIGHT }
    private enum       SnakeState { ALIVE, DEAD }
    private int        snakeSize;
    private float      gridMoveTimer;
    private float      gridMoveTimerMax;
    private Direction  gridMoveDir;
    private LevelGrid  levelGrid;
    private Vector2Int gridPos;
    private SnakeState state;
    private List<SnakeBodyPart>    snakeBodyParts;
    private List<SnakeBodySpatial> snakeBodyPositions;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        state              = SnakeState.ALIVE;
        snakeSize          = 0;
        gridMoveTimerMax   = .15f;
        gridMoveDir        = Direction.UP;
        gridMoveTimer      = gridMoveTimerMax;
        gridPos            = new Vector2Int(10, 10);
        snakeBodyParts     = new List<SnakeBodyPart>();
        snakeBodyPositions = new List<SnakeBodySpatial>();
    }

    private void Update()
    {
        switch(state)
        {
            case SnakeState.ALIVE: 
                if (InputHandler())
                    break;
                DrawSnake();
                break;
            case SnakeState.DEAD:
                break;
        }
    }

    public bool checkCollision(Vector2Int position)
    {
        foreach (SnakeBodySpatial bodyPartPosition in snakeBodyPositions)
        {
            if ( position == bodyPartPosition.getSpatialPosition())
            {
                return true;
            }
        }
        return false;
    }

    private bool InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDir != Direction.DOWN)
            {
                gridMoveDir = Direction.UP;
                return true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDir != Direction.UP)
            {
                gridMoveDir = Direction.DOWN;
                return true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDir != Direction.LEFT)
            {
                gridMoveDir = Direction.RIGHT;
                return true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDir != Direction.RIGHT)
            {
                gridMoveDir = Direction.LEFT;
                return true;
            }
        }
        return false;
    }

    private Vector2Int convMoveDirectionToMoveVector(Direction direction)
    {
        Vector2Int directionVector;
        switch (direction)
        {
            default:
            case Direction.UP    : directionVector = new Vector2Int( 0,  1);  break;
            case Direction.DOWN  : directionVector = new Vector2Int( 0, -1); break;
            case Direction.RIGHT : directionVector = new Vector2Int( 1,  0);  break;
            case Direction.LEFT  : directionVector = new Vector2Int(-1,  0); break;
        }
        return directionVector;
    }

    SnakeBodySpatial safeGetPrevSnakeBodySpatial()
    {
        SnakeBodySpatial prevBodySpatialOrientation = null;
        if (snakeBodyPositions.Count > 0)
            prevBodySpatialOrientation = snakeBodyPositions[0];
        return prevBodySpatialOrientation;
    }

    private void DrawSnake()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer > gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            snakeBodyPositions.Insert(0, new SnakeBodySpatial(safeGetPrevSnakeBodySpatial(), gridPos, gridMoveDir));

            gridPos += convMoveDirectionToMoveVector(gridMoveDir);
            gridPos = levelGrid.getValidGridPosition(gridPos);

            if (levelGrid.checkIfFoodIsEaten(gridPos))
            {
                CreateSnakeBody();
                snakeSize++;
            }

            if (snakeBodyPositions.Count >= snakeSize + 1)
            {
                snakeBodyPositions.RemoveAt(snakeBodyPositions.Count - 1);
            }

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyParts)
            {
                if (gridPos == snakeBodyPart.getPosition())
                {
                    state = SnakeState.DEAD;
                    GameHandler.gameOver();
                }
            }

            transform.position     = new Vector3(gridPos.x, gridPos.y);
            transform.eulerAngles  = new Vector3(0, 0, CalculateHeadRotation(convMoveDirectionToMoveVector(gridMoveDir)) - 90);

            UpdateSnakeBody();

        }
    }

    private void CreateSnakeBody()
    {
        snakeBodyParts.Add(new SnakeBodyPart(snakeBodyParts.Count + 1));
    }

    private void UpdateSnakeBody()
    {
        for (int i = 0; i < snakeBodyParts.Count; i++)
        {
            snakeBodyParts[i].setBodyPosition(snakeBodyPositions[i]);
        }
    }

    private float CalculateHeadRotation(Vector2Int direction)
    {
        float eulerZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (eulerZ < 0) ? (eulerZ + 360) : eulerZ;
    }

    private class SnakeBodyPart
    {
        private Transform        transform;
        private SnakeBodySpatial spatialOrientation;

        public SnakeBodyPart(int sortingIndex)
        {
            GameObject snakeBodyObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyObject.GetComponent<SpriteRenderer>().sortingOrder = -sortingIndex;
            transform = snakeBodyObject.transform;
        }
        
        public void setBodyPosition(SnakeBodySpatial spatialOrientation)
        {
            this.spatialOrientation = spatialOrientation;
            this.spatialOrientation.transformBodyPart(transform);
        }

        public Vector2Int getPosition()
        {
            if (spatialOrientation == null)
                return new Vector2Int(-1, -1);
            return spatialOrientation.getSpatialPosition();
        }
    }

    private class SnakeBodySpatial
    {
        private SnakeBodySpatial prevOrientation;
        private Vector2Int       bodyPosition;
        private Direction        direction;

        public SnakeBodySpatial(SnakeBodySpatial prevOirentation, Vector2Int bodyPosition, Direction direction)
        {
            this.prevOrientation = prevOirentation;
            this.bodyPosition    = bodyPosition;
            this.direction       = direction;
        }

        public Vector2Int getSpatialPosition()
        {
            return bodyPosition;
        }

        public Direction getDirection()
        {
            return direction;
        }

        public void transformBodyPart(Transform transform)
        {
            transform.position = new Vector3(bodyPosition.x, bodyPosition.y);

            float angle = 0;
            Direction prevDirection = Direction.UP;
            if (prevOrientation != null)
                prevDirection = prevOrientation.direction;
            switch(direction)
            {
                default:
                case Direction.UP    : switch (prevDirection) { default              : angle = 0;    break;
                                                                case Direction.RIGHT : angle = 315; transform.position += new Vector3(-.2f,  .2f); break;
                                                                case Direction.LEFT  : angle = 45;  transform.position += new Vector3( .2f,  .2f); break;
                                                              } break;
                case Direction.DOWN  : switch (prevDirection) { default              : angle = 180;  break;
                                                                case Direction.RIGHT : angle = 45;  transform.position += new Vector3(-.2f, -.2f); break;
                                                                case Direction.LEFT  : angle = 315; transform.position += new Vector3( .2f, -.2f); break;
                                                              } break;
                case Direction.RIGHT : switch (prevDirection) { default              : angle = 90;   break;
                                                                case Direction.UP    : angle = 315; transform.position += new Vector3( .2f, -.2f); break;
                                                                case Direction.DOWN  : angle = 45;  transform.position += new Vector3( .2f,  .2f); break;
                                                              } break;
                case Direction.LEFT  : switch (prevDirection) { default              : angle = 270;  break;
                                                                case Direction.UP    : angle = 45;  transform.position += new Vector3(-.2f, -.2f); break;
                                                                case Direction.DOWN  : angle = 315; transform.position += new Vector3(-.2f,  .2f); break;
                                                              } break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
}
