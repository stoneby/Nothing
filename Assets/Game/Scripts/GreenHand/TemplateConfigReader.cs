using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using System.Collections;

public class TemplateConfigReader : GreeenHandConfigReader
{
    private const string ConfigFilePath = "";
    private bool isLoaded;

    private class GreenHandConfig
    {
        public int Index;
        public string ConfigMode;
        public List<string> TextList = new List<string>();
        public string NextConfigTriggerObjectTag;
        public List<int> CanSelectIndexList = new List<int>();
        public List<int> ValidateIndexList = new List<int>();
        public List<int> MoveTraceIndexList = new List<int>();
        public int TagObjectIndex;
        public Vector3 NormalMoveVec = new Vector3();
        public bool IsWaitInFindTagObject;
    }
    private readonly List<GreenHandConfig> greenHandConfigList = new List<GreenHandConfig>();

    public bool ReadConfig(GreenHandGuideHandler handler, int index)
    {
        if (!isLoaded)
        {
            Logger.Log("!!!!!!!!!!!!!Load GreenHandGuide config.");
            LoadConfig();
            isLoaded = true;
        }

        foreach (var item in greenHandConfigList.Where(item => item.Index == index))
        {
            handler.ConfigMode = item.ConfigMode;
            handler.TextList = item.TextList;
            handler.NextConfigTriggerObjectTag = item.NextConfigTriggerObjectTag;
            handler.CanSelectIndexList = item.CanSelectIndexList;
            handler.ValidateIndexList = item.ValidateIndexList;
            handler.MoveTraceIndexList = item.MoveTraceIndexList;
            handler.TagObjectIndex = item.TagObjectIndex;
            handler.NormalMoveVec = item.NormalMoveVec;
            handler.IsWait = item.IsWaitInFindTagObject;

            //Foreach IEnumerable's num equal to 1.
            return true;
        }

        Logger.LogWarning("Can't read config in configIndex:" + index + ", read config cancelled.");
        return false;
    }

    public void ClearLoadedConfig()
    {
        isLoaded = false;
    }

    private void LoadConfig()
    {
        greenHandConfigList.Clear();

        XDocument xDocument = XDocument.Load(ConfigFilePath);
        var splitInt = new[] { ',' };
        var splitString = new[] { '@' };

        var indexElements = xDocument.Descendants("ConfigElement");
        foreach (var item in indexElements)
        {
            var tempConfig = new GreenHandConfig();

            if (item.Attribute("Index") != null)
            {
                tempConfig.Index = int.Parse(item.Attribute("Index").Value);
            }

            if (item.Attribute("ConfigMode") != null)
            {
                tempConfig.ConfigMode = item.Attribute("ConfigMode").Value;
            }

            if (item.Attribute("TextList") != null)
            {
                tempConfig.TextList = item.Attribute("TextList").Value.Split(splitString, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (item.Attribute("NextConfigTriggerObjectTag") != null)
            {
                tempConfig.NextConfigTriggerObjectTag = item.Attribute("NextConfigTriggerObjectTag").Value;
            }

            if (item.Attribute("CanSelectIndexList") != null)
            {
                tempConfig.CanSelectIndexList = item.Attribute("CanSelectIndexList").Value.Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }

            if (item.Attribute("ValidateIndexList") != null)
            {
                tempConfig.ValidateIndexList = item.Attribute("ValidateIndexList").Value.Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }

            if (item.Attribute("MoveTraceIndexList") != null)
            {
                tempConfig.MoveTraceIndexList = item.Attribute("MoveTraceIndexList").Value.Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
            }

            if (item.Attribute("TagObjectIndex") != null)
            {
                tempConfig.TagObjectIndex = int.Parse(item.Attribute("TagObjectIndex").Value);
            }

            if (item.Attribute("NormalMoveVec") != null)
            {
                List<float> vecList = item.Attribute("NormalMoveVec").Value.Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToList();
                tempConfig.NormalMoveVec = new Vector3(vecList[0], vecList[1], vecList[2]);
            }

            if (item.Attribute("TagObjectIndex") != null)
            {
                tempConfig.IsWaitInFindTagObject = bool.Parse(item.Attribute("IsWaitInFindTagObject").Value);
            }

            greenHandConfigList.Add(tempConfig);
        }
    }
}
