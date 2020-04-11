using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScoreDisplay : MonoBehaviour
{
    Text ScoreDisplay1;
    
    public static int Score;
    public int Nextlevel = 3;
    // Start is called before the first frame update
    void Start()
    {
        ScoreDisplay1 = GetComponent<Text>();
        Score = 0;
    }

    
    void Update()
    {
        ScoreDisplay1.text = "Score: " + Score;   
        if (Score >= GoalScoreDisplay.GoalScore)
        {
            SceneManager.LoadScene(Nextlevel);
        }
        
    }
}
