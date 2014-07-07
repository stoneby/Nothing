using System.Collections.Generic;
using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    public class HeroHandler
    {
        public static void OnHeroMessage(ThriftSCMessage msg)
        {
            switch (msg.GetMsgType())
            {
                case (short) MessageType.SC_HERO_LIST:
                    HeroModelLocator.Instance.SCHeroList = msg.GetContent() as SCHeroList;
                    HeroModelLocator.AlreadyRequest = true;
                    if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInBattle)
                    {
                        WindowManager.Instance.Show(typeof(BattleConfirmTabWindow), true);                
                    }
                    else if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroPanel)
                    {
                        WindowManager.Instance.Show<UIHeroCommonWindow>(true);
                        WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
                        //Utils.ShowWithoutDestory(typeof(UIHeroDispTabWindow));
                    }
                    else if(HeroModelLocator.Instance.GetHeroPos == RaidType.GetHeroInHeroCreateTeam)
                    {
                        Utils.ShowWithoutDestory(typeof(UITeamShowingWindow));
                    }
                    break;

                case (short) MessageType.SC_HERO_CREATE_ONE:
                    var createOneMsg = msg.GetContent() as SCHeroCreateOne;
                    if(createOneMsg != null)
                    {
                        if (HeroModelLocator.Instance.SCHeroList.HeroList == null)
                        {
                            HeroModelLocator.Instance.SCHeroList.HeroList = new List<HeroInfo>();
                        }
                        var infos = HeroModelLocator.Instance.SCHeroList.HeroList;
                        infos.Add(createOneMsg.NewHero);
                        var herosWindow = WindowManager.Instance.GetWindow<UIHerosPageWindow>();
                        herosWindow.Refresh(infos);
                        var viewHandler = WindowManager.Instance.GetWindow<UIHeroDispTabWindow>().HeroViewHandler;
                        herosWindow.ItemClicked = viewHandler.OnHeroItemClicked;
                        viewHandler.Refresh();
                    }
                    break;
                case (short) MessageType.SC_HERO_MODIFY_TEAM:
                    var md5Msg = msg.GetContent() as SCHeroModifyTeam;
                    if(md5Msg != null)
                    {
                        HeroModelLocator.Instance.SCHeroList.TeamList[md5Msg.TeamIndex].ListHeroUuid =
                            md5Msg.NewTeamInfo;
                        WindowManager.Instance.GetWindow<UITeamBuildWindow>().Refresh();
                        WindowManager.Instance.Show(typeof(UITeamEditWindow), false);
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
                        var heroSellHandler = WindowManager.Instance.GetWindow<UIHeroDispTabWindow>().HeroSellHandler;
                        heroSellHandler.CleanUp();
                        WindowManager.Instance.GetWindow<UIHerosPageWindow>().Refresh(heroList);
                        heroSellHandler.FreshSellStates();    
                    }
                    break;

                case (short) MessageType.SC_HERO_LVL_UP:
                    WindowManager.Instance.GetWindow<UILevelUpWindow>().ShowLevelOver();
                    break;
                case (short) MessageType.SC_HERO_MAX_EXTEND:
                    var themsg = msg.GetContent() as SCHeroMaxExtend;
                    if(themsg != null)
                    {
                        PlayerModelLocator.Instance.HeroMax = themsg.RefreshHeroCountLimit;
                        WindowManager.Instance.GetWindow<UIHeroDispTabWindow>().HeroViewHandler.Refresh();
                    }
                    break;
                case (short) MessageType.SC_HERO_CHANGE_EQUIP:
                    var hChangeEquipMsg = msg.GetContent() as SCHeroChangeEquip;
                    if(hChangeEquipMsg != null)
                    {
                        var heroBaseWindow = WindowManager.Instance.GetWindow<HeroBaseInfoWindow>();
                        heroBaseWindow.HeroInfo.EquipUuid[heroBaseWindow.CurEquipIndex] = hChangeEquipMsg.EquipUuid;
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
                        WindowManager.Instance.GetWindow<UIHerosPageWindow>().Refresh(HeroModelLocator.Instance.SCHeroList.HeroList);
                    }
                    break;
            }
        }
    }
}


