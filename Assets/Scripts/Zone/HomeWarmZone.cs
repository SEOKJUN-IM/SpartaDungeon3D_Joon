using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomeWarmZone : MonoBehaviour
{    
    public int temperatureAmount;
    public float warmRate;
    public bool turnOnFire = false;

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

    public void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IWarmable warmable))
        {
            if (turnOnFire)
            {
                things.Add(warmable);
                things = things.Distinct().ToList();
            }
            else things.Clear();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IWarmable warmable))
        {
            things.Clear();
        }
    }
}
