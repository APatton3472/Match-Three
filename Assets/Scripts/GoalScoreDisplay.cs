using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalScoreDisplay : MonoBehaviour
{
    Text ScoreDisplay1;
    public int goalScore=1000;
    public static int GoalScore;
    void Start()
    {
        ScoreDisplay1 = GetComponent<Text>();
        GoalScore = goalScore;
    }

    // Update is called once per frame
    void Update()
    {
        ScoreDisplay1.text = "Goal Score: " + goalScore;
    }
}
