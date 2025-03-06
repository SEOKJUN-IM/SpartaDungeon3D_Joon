using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmZone : MonoBehaviour
{
    public int temperatureAmount;
    public float warmRate;

    List<IWarmable> things = new List<IWarmable>();

    void Start()
    {
        InvokeRepeating("GettingWarmer", 0, warmRate);
    }

    void GettingWarmer()
    {
        for (int i = 0; i < things.Count; i++)
        {
            things[i].GetTempurture(temperatureAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IWarmable warmable))
        {
            things.Add(warmable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IWarmable warmable))
        {
            things.Remove(warmable);
        }
    }
}
