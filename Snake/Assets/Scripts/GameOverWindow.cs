using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private static GameOverWindow instance;

    private void Awake()
    {
        instance = this;

        transform.Find("RetryButton").GetComponent<Button_UI>().ClickFunc = () => {
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        };

        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void ShowGameOverUI()
    {
        instance.Show();
    }
}
