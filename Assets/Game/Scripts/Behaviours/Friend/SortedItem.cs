﻿using System.Collections.Generic;
using KXSGCodec;

public class SortedItem : FriendItem
{
    public List<string> ThreeWinnerSpriteNames;
    public string DefaultPassKey = "UIFriendEntry.DefaultRaid";
    public string Separator;
    private UILabel curPass;
    private UILabel highestHit;
    private UILabel loserLabel;
    private UISprite winerSprite;

    protected override void Awake()
    {
        base.Awake();
        curPass = transform.Find("CurPassTitle/CurPass").GetComponent<UILabel>();
        highestHit = transform.Find("MaxHitTitle/MaxHit").GetComponent<UILabel>();
        loserLabel = transform.Find("Ranking/Loser").GetComponent<UILabel>();
        winerSprite = transform.Find("Ranking/Winner").GetComponent<UISprite>();
    }

    public override void Init(FriendInfo info)
    {
        base.Init(info);
        highestHit.text = info.MaxDamage.ToString();
        var raidTemp = MissionModelLocator.Instance.RaidTemplates;
        if(raidTemp.RaidStageTmpl.ContainsKey(info.RaidStageId))
        {
            var raidStage = raidTemp.RaidStageTmpl[info.RaidStageId];
            var raid = raidTemp.RaidTmpl[raidStage.RaidId];
            curPass.text = raid.Name + Separator + raidStage.StageName;
        }
        else
        {
            curPass.text = LanguageManager.Instance.GetTextValue(DefaultPassKey);
        }
    }

    public void Init(FriendInfo info, int rank)
    {
        Init(info);
        if(rank < 3)
        {
            loserLabel.text = "";
            winerSprite.enabled = true;
            winerSprite.spriteName = ThreeWinnerSpriteNames[rank];
        }
        else
        {
            winerSprite.enabled = false;
            //The rank is base on zero, where the display number is based on one.
            loserLabel.text = "" + (rank + 1);
        }
    }
}
