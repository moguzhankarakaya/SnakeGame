using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    private Text score;

    private void Awake()
    {
        score = transform.Find("Score").GetComponent<Text>();
        
    }

    public void Update()
    {
        score.text = GameHandler.getScore().ToString();
    }
}
