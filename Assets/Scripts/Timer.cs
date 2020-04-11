using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    Image timer;
    public float maxTime = 90f;
    float timeLeft;
    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponent<Image>();
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timer.fillAmount = timeLeft / maxTime;
        }
        if (timeLeft <= 0)
        {
            SceneManager.LoadScene(5);
        }
    }
}
