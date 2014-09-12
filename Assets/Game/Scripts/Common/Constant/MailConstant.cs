using System;
using System.Collections.Generic;
using KXSGCodec;
using Property;
using UnityEngine;

public class MailConstant
{
    public const int ListTypeRefresh = 0;
    public const int ListTypeUpdate = 1;
    public const int HeroAttachType = 1;
    public const int ItemAttachType = 2;
    public const int CurrencyAttachType = 3;

    public const int DisplayOperId = 0;
    public const int ReceiveOperId = 1;
    public const int DeleteOperId = 2;

    public const sbyte NeedContent = 1;
    public const sbyte DontNeedContent = 0;

    public const string EnergyAttachKey = "UIEMailEntry.Energy";
    public const string CoinsAttachKey = "UIEMailEntry.Coins";
    public const string DiamondAttachKey = "UIEMailEntry.Diamond";
    public const string RemainTimeKey = "UIEMailEntry.RemainTime";
    public const string MailOutOfDateKey = "UIEmailEntry.OutOfDate";
    public const string MailOperSucceedKey = "UIEmailEntry.Succeed";
    public const string MailNotExitKey = "UIEmailEntry.NotExit";
    public const string MailNoNeedUpdateKey = "UIEmailEntry.NoNeedUpdate";
    public const string MailDelNoReadKey = "UIEmailEntry.DeleteNoRead";
    public const string MailDelAttachKey = "UIEmailEntry.DelAttach";

    public const string MailHeroItemPath = "Prefabs/UI/TabPanel/Email/MailHero";
    public const string MailItemPath = "Prefabs/UI/TabPanel/Email/MailItem";
    public const string MailCurrencyPath = "Prefabs/UI/TabPanel/Email/CurrencyItem";

    public const string MailDiamondSpriteName = "Diamond1";
    public const string MailCoinsSpriteName = "Coins1";
    public const string MailEnergySpriteName = "Energy1";

    public enum MailState
    {
        UnShow,
        UnRead,
        Read,
        Gain,
        Delete
    }

    public enum OptionResult
    {
        OutOfDate,
        Succeed,
        NotExit,
        NoNeedToUpdate
    }

    public static string ConvertTimeSpanToString(TimeSpan timeSpan)
    {
        if ((float)timeSpan.TotalDays >= 1)
        {
            return Mathf.FloorToInt((float)timeSpan.TotalDays) +
           LanguageManager.Instance.GetTextValue("UIEMailEntry.Days");
        }
        if ((float)timeSpan.TotalHours >= 1)
        {
            return Mathf.FloorToInt((float) timeSpan.TotalHours) +
                   LanguageManager.Instance.GetTextValue("UIEMailEntry.Hours");
        }
        return Mathf.FloorToInt((float) timeSpan.TotalMinutes) +
               LanguageManager.Instance.GetTextValue("UIEMailEntry.Minutes");
    }

    public static void SortMailList(List<MailMsgInfo> mailList)
    {
        if (mailList == null || mailList.Count == 0)
        {
            return;
        }
        mailList.Sort((mailL, mailR) => mailR.CreateTime.CompareTo(mailL.CreateTime));
    }

    public static void ShowAttachments(IEnumerable<MailAttachment> attachments, Transform parent, float interval)
    {
        var index = 0;
        GameObject item = null;
        foreach (var mailAttachment in attachments)
        {
            var type = mailAttachment.Type;
            switch (type)
            {
                case HeroAttachType:
                    item = NGUITools.AddChild(parent.gameObject, Resources.Load(MailHeroItemPath) as GameObject);
                    var baseHero = item.GetComponent<HeroItemBase>();
                    baseHero.Show(mailAttachment.ItemId);
                    var desc = baseHero.transform.Find("Desc").GetComponent<UILabel>();
                    desc.text = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[mailAttachment.ItemId].Name + "X" +
                                mailAttachment.Count;
                    break;
                case ItemAttachType:
                    item = NGUITools.AddChild(parent.gameObject, Resources.Load(MailItemPath) as GameObject);
                    var baseItem = item.GetComponent<ItemBase>();
                    baseItem.InitItem(mailAttachment.ItemId);
                    var itemDesc = baseItem.transform.Find("Desc").GetComponent<UILabel>();
                    itemDesc.text = ItemModeLocator.Instance.GetName(mailAttachment.ItemId) + "X" +
                                    mailAttachment.Count;
                    break;
                case CurrencyAttachType:
                    item = NGUITools.AddChild(parent.gameObject, Resources.Load(MailCurrencyPath) as GameObject);
                    var icon = item.transform.Find("Icon").GetComponent<UISprite>();
                    var value = item.transform.Find("Value").GetComponent<UILabel>();
                    var curDesc = item.transform.Find("Desc").GetComponent<UILabel>();
                    if (mailAttachment.ItemId == RoleProperties.ROLEBASE_DIAMOND)
                    {
                        icon.spriteName = MailDiamondSpriteName;
                        curDesc.text = LanguageManager.Instance.GetTextValue(DiamondAttachKey);
                    }
                    else if (mailAttachment.ItemId == RoleProperties.ROLEBASE_GOLD)
                    {
                        icon.spriteName = MailCoinsSpriteName;
                        curDesc.text = LanguageManager.Instance.GetTextValue(CoinsAttachKey);
                    }
                    else if (mailAttachment.ItemId == RoleProperties.ROLEBASE_ENERGY)
                    {
                        icon.spriteName = MailEnergySpriteName;
                        curDesc.text = LanguageManager.Instance.GetTextValue(EnergyAttachKey);
                    }
                    value.text = mailAttachment.Count.ToString();
                    icon.MakePixelPerfect();
                    break;
            }
            if (item != null)
            {
                item.transform.localPosition = new Vector3(index * interval, 0, 0);
            }
            index++;
        }
    }
}
