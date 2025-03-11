using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSpeedTool : Equip
{
    public float boardMoveSpeed;
    private float firstMoveSpeed;    

    public PlayerController controller;

    void Awake()
    {
        controller = CharacterManager.Instance.Player.controller;        
        firstMoveSpeed = controller.moveSpeed;
    }

    void Update()
    {
        FasterPlayer();                
    }

    void OnDestroy()
    {
        ResetPlayerSpeed();
    }

    void FasterPlayer()
    {
        controller.moveSpeed = boardMoveSpeed;        
    }

    void ResetPlayerSpeed()
    {
        controller.moveSpeed = firstMoveSpeed;
    }
}
