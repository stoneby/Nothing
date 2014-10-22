using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class TemplateConfigReader : GreeenHandConfigReader
{
    private const string ConfigFilePath = "AssetBundles/TextAssets/GreenHand/GreenHandConfig";
    private bool isLoaded;

    private class GreenHandConfig
    {
        public string BattleTriggerType;

        public int Index;
        public string ConfigMode;
        public List<string> TextList;
        public string NextConfigTriggerObjectTag;
        public List<int> CanSelectIndexList;
        public List<int> ValidateIndexList;
        public List<int> MoveTraceIndexList;
        public int TagObjectIndex;
        public Vector3 NormalMoveVec = new Vector3(0, 0, 0);
        public bool IsWaitInFindTagObject;
        public float WaitSec;
    }
    private readonly List<GreenHandConfig> greenHandConfigList = new List<GreenHandConfig>();

    public string GetBattleType(int index)
    {
        if (!isLoaded)
        {
            Debug.Log("!!!!!!!!!!!!!Load GreenHandGuide config.");
            LoadConfig();
            isLoaded = true;
        }

        return greenHandConfigList.Where(item => item.Index == index).Select(item => item.BattleTriggerType).FirstOrDefault();
    }

    public bool ReadConfig(GreenHandGuideHandler handler, int index)
    {
        if (!isLoaded)
        {
            Debug.Log("!!!!!!!!!!!!!Load GreenHandGuide config.");
            LoadConfig();
            isLoaded = true;
        }

        foreach (var item in greenHandConfigList.Where(item => item.Index == index))
        {
            if (item.ConfigMode == "End")
            {
                Logger.LogWarning("Read the end config in configIndex:" + index + ".");

                //MTA track Read Guide Config.
                MtaManager.TrackCustomKVEvent(MtaType.KVEventReadGreenHandConfig + index.ToString(), new Dictionary<string, string>());

                return false;
            }

            handler.ConfigMode = item.ConfigMode;
            handler.TextList = item.TextList;
            handler.NextConfigTriggerObjectTag = item.NextConfigTriggerObjectTag;
            handler.CanSelectIndexList = item.CanSelectIndexList;
            handler.ValidateIndexList = item.ValidateIndexList;
            handler.MoveTraceIndexList = item.MoveTraceIndexList;
            handler.TagObjectIndex = item.TagObjectIndex;
            handler.NormalMoveVec = item.NormalMoveVec;
            handler.IsWait = item.IsWaitInFindTagObject;
            handler.WaitSec = item.WaitSec;

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

        var xmlData = ResoucesManager.Instance.Load(ConfigFilePath) as TextAsset;
        var xDocument = new XmlDocument();
        xDocument.LoadXml(xmlData.text);
        var indexElements = xDocument.DocumentElement.ChildNodes.OfType<XmlElement>();

        var splitInt = new[] { ',' };
        var splitString = new[] { '@' };

        foreach (var item in indexElements)
        {
            var tempConfig = new GreenHandConfig();

            if (item.GetAttribute("BattleTriggerType") != "")
            {
                tempConfig.BattleTriggerType = item.GetAttribute("BattleTriggerType");
            }

            if (item.GetAttribute("Index") != "")
            {
                tempConfig.Index = int.Parse(item.GetAttribute("Index"));
            }

            if (item.GetAttribute("ConfigMode") != "")
            {
                tempConfig.ConfigMode = item.GetAttribute("ConfigMode");
            }

            if (item.GetAttribute("TextList") != "")
            {
                tempConfig.TextList = item.GetAttribute("TextList").Split(splitString, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (item.GetAttribute("NextConfigTriggerObjectTag") != "")
            {
                tempConfig.NextConfigTriggerObjectTag = item.GetAttribute("NextConfigTriggerObjectTag");
            }

            if (item.GetAttribute("CanSelectIndexList") != "")
            {
                tempConfig.CanSelectIndexList = ParseToIntList("CanSelectIndexList", item, splitInt);
            }

            if (item.GetAttribute("ValidateIndexList") != "")
            {
                tempConfig.ValidateIndexList = ParseToIntList("ValidateIndexList", item, splitInt);
            }

            if (item.GetAttribute("MoveTraceIndexList") != "")
            {
                tempConfig.MoveTraceIndexList = ParseToIntList("MoveTraceIndexList", item, splitInt);
            }

            if (item.GetAttribute("TagObjectIndex") != "")
            {
                tempConfig.TagObjectIndex = int.Parse(item.GetAttribute("TagObjectIndex"));
            }

            if (item.GetAttribute("NormalMoveVec") != "")
            {
                List<float> vecList = item.GetAttribute("NormalMoveVec").Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(xmlItem => float.Parse(xmlItem)).ToList();
                tempConfig.NormalMoveVec = new Vector3(vecList[0], vecList[1], vecList[2]);
            }

            if (item.GetAttribute("IsWaitInFindTagObject") != "")
            {
                tempConfig.IsWaitInFindTagObject = bool.Parse(item.GetAttribute("IsWaitInFindTagObject"));
            }

            if (item.GetAttribute("WaitSec") != "")
            {
                tempConfig.WaitSec = float.Parse(item.GetAttribute("WaitSec"));
            }

            greenHandConfigList.Add(tempConfig);
        }
    }

    private static List<int> ParseToIntList(string name, XmlElement item, char[] splitInt)
    {
        return item.GetAttribute(name).Split(splitInt, StringSplitOptions.RemoveEmptyEntries).Select(xmlItem => int.Parse(xmlItem)).ToList();
    }
}
