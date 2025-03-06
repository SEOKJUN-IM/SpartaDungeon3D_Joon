using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition temperature { get { return uiCondition.temperature; } }
    Condition hunger { get { return uiCondition.hunger; } }

    public float coldHealthDecay;
    public float noHungerHealthDecay;

    void Update()
    {
        temperature.Subtract(temperature.passiveValue * Time.deltaTime);
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);

        if (temperature.curValue <= temperature.maxValue * 0.5f)
        {
            health.Subtract(coldHealthDecay * Time.deltaTime);
        }

        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Warm(float amount)
    {
        temperature.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("Player Die!");
    }
}
