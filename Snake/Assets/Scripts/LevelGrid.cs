using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid : MonoBehaviour
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
            return true;
        }
        return false;
    }
}
