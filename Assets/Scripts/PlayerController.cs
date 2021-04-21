using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public delegate void MoveTick();
    public static event MoveTick OnMoveTick;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
