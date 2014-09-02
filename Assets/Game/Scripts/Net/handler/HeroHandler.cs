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
                        for(var index = 0; index < heroList.Count; index++)
                        {
                            if(sellList.Contains(heroList[index].Uuid))
                            {
                                heroList.Remove(heroList[index]);
                                index--;
                            }
                        }
                        WindowManager.Instance.Show<UISellDialogWindow>(false);
                        var sellhero = WindowManager.Instance.GetWindow<UIHeroCommonWindow>().SellHeroHandler;
                        sellhero.CleanUp();
                        var window=WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
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
                        //var heroDetail = WindowManager.Instance.GetWindow<UIHeroDetailWindow>();
                        //heroDetail.HeroInfo.EquipUuid[heroDetail.CurEquipIndex] = hChangeEquipMsg.EquipUuid;
                        //heroDetail.EquipOver(heroDetail.HeroInfo);
                        for (int k = 0; k < ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Count; k++)
                        {
                            if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos[k].Id == hChangeEquipMsg.EquipUuid)
                            {
                                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos[k].EquipStatus = 1;
                                for (int i = 0; i < HeroModelLocator.Instance.SCHeroList.HeroList.Count; i++)
                                {
                                    if (HeroModelLocator.Instance.SCHeroList.HeroList[i].Uuid == hChangeEquipMsg.HeroUuid)
                                    {
                                        HeroModelLocator.Instance.SCHeroList.HeroList[i].EquipUuid[hChangeEquipMsg.Index] =
                                            hChangeEquipMsg.EquipUuid;
                                        HeroModelLocator.Instance.SCHeroList.HeroList[i].EquipTemplateId[hChangeEquipMsg.Index] =
                                            ItemModeLocator.Instance.ScAllItemInfos.ItemInfos[k].TmplId;
                                    }
                                }
                            }
                            if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos[k].Id == hChangeEquipMsg.UnEquipUuid)
                            {
                                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos[k].EquipStatus = 0;
                            }
                        }
                    }
                    break;
                case (short) MessageType.SC_HERO_BIND_SUCC:
                    var heroBindSucc = msg.GetContent() as SCHeroBindSucc;
                    if(heroBindSucc != null)
                    {
                        var changedList = HeroLockHandler.ChangedLockList;
                        for (int i = 0; i < changedList.Count; i++)
                        {
                            var uid = changedList[i];
                            var heroInfo = HeroModelLocator.Instance.FindHero(uid);
                            heroInfo.Bind = !heroInfo.Bind;
                        }
                        //WindowManager.Instance.GetWindow<UIHerosPageWindow>().Refresh(HeroModelLocator.Instance.SCHeroList.HeroList);
                    }
                    break;
            }
        }
    }
}


