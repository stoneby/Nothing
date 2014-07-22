﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MtaManager {
    //页面统计，成对调用
    public static void TrackBeginPage(string pagename)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        MtaService.TrackBeginPage(pagename);
    }
    public static void TrackEndPage(string pagename)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        MtaService.TrackEndPage(pagename);
    }

    // 统计次数的自定义事件，事件名字需要在平台配置
    // 构建自定义事件的key-value
	// Dictionary<string, string> dict = new Dictionary<string, string>();
	// dict.Add("account", "12345");
	// dict.Add("amount", "100");
	// dict.Add("item", "firearm");
    // 上报自定义事件，事件id需要在前台注册
    // 账号为12345的用户购买了一支100元的hammer
    public static void TrackCustomKVEvent(string eventname, Dictionary<string, string> dict)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        MtaService.TrackCustomKVEvent(eventname, dict);
    }

    // 统计时长的自定义事件，begin、end需要成对出现
    private static string EventName;
    private static Dictionary<string, string> EventDict;
    public static void TrackCustomBeginKVEvent(string eventname, Dictionary<string, string> beDict)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        EventName = eventname;
        EventDict = beDict;
        MtaService.TrackCustomBeginKVEvent(eventname, beDict);
    }

    public static void TrackCustomEndKVEvent(string eventname)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        if (EventName == eventname)
        {
            MtaService.TrackCustomEndKVEvent(eventname, EventDict);
        }
    }

    //App本地测试
    public static void TestSpeed(Dictionary<string, int> speedMap)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        #if UNITY_ANDROID
        MtaService.TestSpeed(speedMap);
        #endif
    }

    public static void TestServiceSpeed()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
#if UNITY_ANDROID
        Dictionary<string, int> speedMap = new Dictionary<string, int>();
        speedMap.Add("27.131.223.229", 80);
        MtaService.TestSpeed(speedMap);
#endif
    }

    //游戏参数汇报
    public static void ReportGameUser(string username, string server, string level)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) return;
        var gameUser = new MtaGameUser(username, server, level);
        MtaService.ReportGameUser(gameUser);	
    }

    


}