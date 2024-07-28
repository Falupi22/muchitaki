using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change destination card
/// </summary>

public class ButtonDebug : MonoBehaviour
{
    #region Methods

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Run()
    {
        Debug.Log("check");
        Console.WriteLine("Check");
        CardCreator.Create();
    }

    #endregion
}
