using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1;
        transform.parent.gameObject.SetActive(false);
    }
}
