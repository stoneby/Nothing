using System;
using UnityEngine;
using System.Collections;

public class HeartController : MonoBehaviour
{
    //public static bool StartCountFlag = false;
    private static int eneryStepTime = -1;  //以秒为单位的时间间隔
    private static float eneryLastTime = -1;//上一次的时间

    private static int[] TimeEvents = new[] {TimeEventType.Zero};

    public static void SetEneryTimer(int steptime)
    {
        eneryLastTime = Time.realtimeSinceStartup;
        eneryStepTime = steptime*60;
        //PopTextManager.PopTip("体力恢复时间间隔为：" + eneryStepTime);
    }
	// Use this for initialization
	void Start ()
	{
	    oldTime = GetTime();
	}

    private int GetTime()
    {
        return DateTime.Now.Hour*10000 + DateTime.Now.Minute*100 + DateTime.Now.Second;
    }

    private int flag = 0;
    private int oldTime = -1;
	// Update is called once per frame
	void Update () 
    {
        if (flag < 60)
        {
            flag++;
            return;
        }
	    var newtime = GetTime();
	    for (int i = 0; i < TimeEvents.Length; i++)
	    {
            if ((oldTime > TimeEvents[i] && newtime <= TimeEvents[i]) || (oldTime < TimeEvents[i] && newtime >= TimeEvents[i]))
	        {
                PopTextManager.PopTip(TimeEvents[i].ToString());
	            EventHandler(TimeEvents[i]);
	        }
	    }
	    oldTime = newtime;
        
        //PopTextManager.PopTip("Minute : " + DateTime.Now.Minute);
        //PopTextManager.PopTip("Second : " + DateTime.Now.Second);
	    flag = 0;
	    if (eneryStepTime > 0)
	    {
	        var realtime = Time.realtimeSinceStartup;
	        if (realtime - eneryLastTime >= eneryStepTime)
	        {
                var levelTemps = LevelModelLocator.Instance.LevelUpTemplates.LevelUpTmpls;
	            var value = PlayerModelLocator.Instance.Level;
                if (levelTemps.ContainsKey(value))
                {
                    var levelTemp = levelTemps[value];
                    if (PlayerModelLocator.Instance.Energy < levelTemp.MaxEnergy)
                    {
                        PlayerModelLocator.Instance.Energy++;
                        eneryLastTime += eneryStepTime;
                        //PopTextManager.PopTip("体力恢复为：" + PlayerModelLocator.Instance.Energy);
                        if (CommonHandler.PlayerPropertyChanged != null)
                        {
                            CommonHandler.PlayerPropertyChanged(null);
                        }
                    }
                    
                }
                
	        }
	    }
	}

    private void EventHandler(int thetype)
    {
        switch (thetype)
        {
            case TimeEventType.Zero:
                MissionModelLocator.Instance.CleanFinishTime();
                break;
            case TimeEventType.Test:
                
                break;
        }
    }
}
