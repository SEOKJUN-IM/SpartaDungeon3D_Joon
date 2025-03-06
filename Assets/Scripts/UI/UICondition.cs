using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition temperature;
    public Condition hunger;

    void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}
