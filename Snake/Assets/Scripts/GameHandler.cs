using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;


public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;

    private static int score;
    private LevelGrid  levelGrid;

    [SerializeField] private Snake snake;

    private void Awake()
    {
        instance = this;
        resetScore();
    }

    void Start()
    {
        Debug.Log("Game Handler Started!");

        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

    void Update()
    {
        
    }

    public static void resetScore()
    {
        score = 0;
    }

    public static int getScore()
    {
        return score;
    }

    public static void incrementScore()
    {
        score += 100;
    }

    public static void gameOver()
    {
        GameOverWindow.ShowGameOverUI();
    }
}
