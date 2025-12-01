using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager :Singleton<GameManager>
{
    protected override void Init()
    {
        ResourceManager.Init();
    }

    private bool isDead;

    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }
    public void GameStart()
    {
        
    }

    public void GameOver()
    {
        
    }
    
}
