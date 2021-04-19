using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snake.Map;
public class MapController : MonoBehaviour
{
    GridSystem gridSystem;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    void Awake()
    {
        gridSystem = new GridSystem(transform.position,xOffset, yOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
