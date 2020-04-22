using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid
{
    private Vector2Int foodPosition;
    private GameObject foodGameObj;
    private int width, height;
    private Snake snake;

    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    private void SpawnFood()
    {
        do
        {
            foodPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.checkCollision(foodPosition));

        foodGameObj = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObj.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodGameObj.transform.position = new Vector3(foodPosition.x, foodPosition.y);
    }

    public bool checkIfFoodIsEaten(Vector2Int snakePosition)
    {
        if (snakePosition == foodPosition)
        {
            Object.Destroy(foodGameObj);
            SpawnFood();
            GameHandler.incrementScore();
            return true;
        }
        return false;
    }

    public Vector2Int getValidGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0)
            gridPosition.x = width;
        else if (gridPosition.x > width)
            gridPosition.x = 0;

        if (gridPosition.y < 0)
            gridPosition.y = height;
        else if (gridPosition.y > height)
            gridPosition.y = 0;

        return gridPosition;
    }
}
