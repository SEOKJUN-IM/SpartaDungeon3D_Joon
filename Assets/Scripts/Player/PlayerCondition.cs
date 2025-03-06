using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public interface IWarmable
{
    void GetTempurture(int amout);
}

public class PlayerCondition : MonoBehaviour, IDamageable, IWarmable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition temperature { get { return uiCondition.temperature; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float coldHealthDecay;
    public float notemperatureHealthDecay;    

    public event Action onTakeDamage;
    public event Action onWarming;

    void Update()
    {
        temperature.Subtract(temperature.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (temperature.curValue > temperature.maxValue * 0.5f)
        {
            temperature.uiBar.color = new Color(247f / 255f, 65f / 255f, 86f / 255f, 1f);
        }

        if (temperature.curValue <= temperature.maxValue * 0.5f && temperature.curValue > 0)
        {
            health.Subtract(coldHealthDecay * Time.deltaTime);
            temperature.uiBar.color = new Color(65f/255f, 143f/255f, 247f/255f, 1f);
        }

        if (temperature.curValue == 0f)
        {
            health.Subtract(notemperatureHealthDecay * Time.deltaTime);
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

    public void Die()
    {
        Debug.Log("Player Die!");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }

    public void GetTempurture(int amout)
    {
        temperature.Add(amout);
        onWarming?.Invoke();
    }
}
