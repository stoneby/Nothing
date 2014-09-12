using System.Collections.Generic;
using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    public class HeroHandler
    {
        public delegate void MessageReceived();
        public static MessageReceived HeroListInHeroPanel;

        public delegate void HeroModifyTeam(sbyte teamIndex, List<long> uuids);
        public static HeroModifyTeam OnHeroModifyTeam;

        public static void OnHeroMessage(ThriftSCMessage msg)
        {
            switch (msg.GetMsgType())
            {
                case (short) MessageType.SC_HERO_LIST:
                    var heroModelLocator = HeroModelLocator.Instance;
                    heroModelLocator.SCHeroList = msg.GetContent() as SCHeroList;
                    TeamMemberManager.Instance.SetValue(heroModelLocator.SCHeroList.CurrentTeamIndex);
                    HeroModelLocator.AlreadyRequest = true;
                    if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInBattle)
                    {
                        WindowManager.Instance.Show<SetBattleWindow>(true);
                        WindowManager.Instance.Show<RaidsWindow>(false);    
                    }
                    else if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroPanel)
                    {
                        if(HeroListInHeroPanel != null)
                        {
                            HeroListInHeroPanel();
                        }
                        WindowManager.Instance.Show<UIHeroCommonWindow>(true);
                    }
                    else if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroCreateTeam)
                    {
                        WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
                    }
                    break;

                case (short) MessageType.SC_HERO_CREATE_ONE:
                    var createOneMsg = msg.GetContent() as SCHeroCreateOne;
                    if (createOneMsg != null)
                    {
                        ChooseCardHandler.AddToCacheHeroList(new List<HeroInfo> {createOneMsg.NewHero});
                        var herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
                        if(NGUITools.GetActive(herosWindow))
                        {
                            herosWindow.Refresh(HeroModelLocator.Instance.SCHeroList.HeroList);
                        }  
                    }
                    break;
                case (short) MessageType.SC_HERO_MODIFY_TEAM:
                    var md5Msg = msg.GetContent() as SCHeroModifyTeam;
                    if (md5Msg != null)
                    {
                        var teamIndex = md5Msg.TeamIndex;
                        var uuids = md5Msg.NewTeamInfo;
                        HeroModelLocator.Instance.SCHeroList.TeamList[teamIndex].ListHeroUuid = uuids;
                        TeamMemberManager.Instance.SetValue(md5Msg.TeamIndex);
                        PlayerModelLocator.UpdateTeamInfos(md5Msg.NewTeamInfo);
                        if (OnHeroModifyTeam != null)
                        {
                            OnHeroModifyTeam(teamIndex, uuids);
                        }
                    }
                    break;

                case (short) MessageType.SC_HERO_SELL:
                    var scHeroSell = msg.GetContent() as SCHeroSell;
                    if(scHeroSell != null)
                    {
                        var sellList = scHeroSell.SellList;
                        var heroList = HeroModelLocator.Instance.SCHeroList.HeroList;
                        var items = ItemModeLocator.Instance.ScAllItemInfos;
                        for(var index = 0; index < heroList.Count; index++)
                        {
                            var heroInfo = heroList[index];
                            if (sellList.Contains(heroInfo.Uuid))
                            {
                                if (items != null)
                                {
                                    HeroUtils.CleanEquipStatus(heroInfo, items.ItemInfos);
                                }
                                heroList.Remove(heroInfo);
                                index--;
                            }
                        }
                        WindowManager.Instance.Show<UISellDialogWindow>(false);
                        var window = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
                        var sellhero = window.SellHeroHandler;
                        sellhero.SellOverUpdate();
                        window.Refresh(heroList);
                    }
                    break;

                case (short) MessageType.SC_HERO_LVL_UP:
                    WindowManager.Instance.GetWindow<UIHeroCommonWindow>().LevelUpHeroHandler.ShowLevelOver();
                    break;
                case (short) MessageType.SC_HERO_MAX_EXTEND:
                    var themsg = msg.GetContent() as SCHeroMaxExtend;
                    if(themsg != null)
                    {
                        PlayerModelLocator.Instance.HeroMax = themsg.RefreshHeroCountLimit;
                        WindowManager.Instance.GetWindow<UIHeroCommonWindow>().RefreshHeroMaxNum();
                    }
                    break;
                case (short) MessageType.SC_HERO_CHANGE_EQUIP:
                    var hChangeEquipMsg = msg.GetContent() as SCHeroChangeEquip;
                    if(hChangeEquipMsg != null)
                    {
                        var items = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                        foreach (var item in items)
                        {
                            var isEquip = (item.Id == hChangeEquipMsg.EquipUuid);
                            var isUnEquip = (item.Id == hChangeEquipMsg.UnEquipUuid);
                            if (isEquip || isUnEquip)
                            {
                                item.EquipStatus = (sbyte)(isEquip ? 1 : 0);
                                var heros = HeroModelLocator.Instance.SCHeroList.HeroList;
                                foreach(var hero in heros)
                                {
                                    if (hero.Uuid == hChangeEquipMsg.HeroUuid)
                                    {
                                        hero.EquipUuid[hChangeEquipMsg.Index] = hChangeEquipMsg.EquipUuid;
                                        hero.EquipTemplateId[hChangeEquipMsg.Index] = isEquip ? item.TmplId : 0;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (short) MessageType.SC_HERO_BIND_SUCC:
                    var heroBindSucc = msg.GetContent() as SCHeroBindSucc;
                    if(heroBindSucc != null)
                    {
                        var heroInfo = HeroModelLocator.Instance.FindHero(HeroBaseInfoRefresher.Uuid);
                        heroInfo.Bind = !heroInfo.Bind;
                        PopTextManager.PopTip(heroInfo.Bind ? "绑定成功" : "解绑成功", false);
                    }
                    break;
            }
        }
    }
}


