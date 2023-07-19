using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Playercontroller playerController;
    public StateMachine stateMachineEnemy;

    private void Awake()
    {
        playerController.Initialize();
        stateMachineEnemy.Initialize();
    }

    private void Start()
    {
        playerController.SecondInitialize();
        stateMachineEnemy.SecondInitialize();
    }
    
    private void Update()
    {
        
    }
}
