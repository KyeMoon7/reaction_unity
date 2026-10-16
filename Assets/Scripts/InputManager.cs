using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButtonDown(0 ))
        {
            gameManager.Trigger();
        }
    }
}
