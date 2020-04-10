using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void play()
    {
        SceneManager.LoadScene(1);
    }
    public void MM()
    {
        SceneManager.LoadScene(0);
    }
    public void exit()
    {
        Application.Quit();
    }
    public void credits()
    {
        SceneManager.LoadScene(6);
    }
    
}
