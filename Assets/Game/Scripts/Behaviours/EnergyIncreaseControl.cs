using System;
using System.Collections;
using UnityEngine;

public class EnergyIncreaseControl : Singleton<EnergyIncreaseControl>
{
    private DateTime updateTime;
    private sbyte eachRecoverTime;
    private const int SecondsPerMinute = 60;

    private int energy;
    public int Energy
    {
        get { return energy + GetIncreased(); }
        set
        {
            energy = value;
            updateTime = DateTime.Now;
        }
    }

    public delegate void EnergyInCrease(int energyValue);

    public EnergyInCrease EnergyIncreaseHandler;

    public void Init(sbyte recoverTime)
    {
        eachRecoverTime = recoverTime;
    }

    public int GetIncreased()
    {
        var timeSpan = DateTime.Now.Subtract(updateTime);
        return (int)timeSpan.TotalMinutes / eachRecoverTime;
    }

    public void StartMonitor()
    {
        StartCoroutine(MonitorIncrease());
    }

    public void StopMonitor()
    {
        StopCoroutine("MonitorIncrease");
    }

    private IEnumerator MonitorIncrease()
    {
        var timeSpan = DateTime.Now.Subtract(updateTime);
        var increased = (int) timeSpan.TotalSeconds / eachRecoverTime;
        var nextTime = new TimeSpan(0,  (increased + 1) * eachRecoverTime, 0) - timeSpan;
        yield return new WaitForSeconds((float)nextTime.TotalSeconds);
        var value = Energy;
        if(EnergyIncreaseHandler != null)
        {
            EnergyIncreaseHandler(value);
        }
        var recoverSeconds = eachRecoverTime * SecondsPerMinute;
        while(true)
        {
            yield return new WaitForSeconds(recoverSeconds);
            value = Energy;
            if (EnergyIncreaseHandler != null)
            {
                EnergyIncreaseHandler(value);
            }
        }
    }

}
