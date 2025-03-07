using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Condition health { get { return uiCondition.health; } }
    public Condition temperature { get { return uiCondition.temperature; } }
    public Condition stamina { get { return uiCondition.stamina; } }
    
    public float coldHealthDecay;
    public float notemperatureHealthDecay;

    public Coroutine addCoroutine;       

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

    public void Heal(float amount, float duration)
    {
        if (duration == 0f) health.Add(amount);
        // 아니면 duration 시간 동안 매초 amount / duration 만큼 Add
        else
        {
            ApplyAddOverTime(health, amount, duration);
            RemoveAddOverTime(health, amount, duration);
        }
    }

    public void Warm(float amount, float duration)
    {
        if (duration == 0f) temperature.Add(amount);
        // 아니면 duration 시간 동안 매초 amount / duration 만큼 Add
        else
        {
            ApplyAddOverTime(temperature, amount, duration);
            RemoveAddOverTime(temperature, amount, duration);
        }
    }

    public void Energetic(float amount, float duration)
    {
        if (duration == 0f) stamina.Add(amount);
        // 아니면 duration 시간 동안 매초 amount / duration 만큼 Add
        else
        {
            ApplyAddOverTime(stamina, amount, duration);
            RemoveAddOverTime(stamina, amount, duration);
        }
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

    // 컨디션에 매초 Add해야할만큼 Add하는 IEnumerator
    public IEnumerator AddValueOverTime(Condition condition, float amount, float duration)
    {
        while (duration > 0f)
        {
            condition.Add(amount / duration);
            yield return new WaitForSeconds(1f);
            duration--;
        }
    }

    // 아이템 사용 위한 코루틴 적용
    public void ApplyAddOverTime(Condition condition, float amount, float duration)
    {
        addCoroutine = StartCoroutine(AddValueOverTime(condition, amount, duration));
    }
    
    // 아이템 사용 코루틴 중단
    public void RemoveAddOverTime(Condition condition, float amount, float duration)
    {
        if (addCoroutine != null) StopCoroutine(AddValueOverTime(condition, amount, duration));
    }

    // 플레이어 특정 행동에 Stamina 사용할 수 있게 하는 bool메서드
    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0) return false;

        stamina.Subtract(amount);
        return true;
    }
}
