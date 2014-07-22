using KXSGCodec;
using System.Collections.Generic;

namespace Assets.Game.Scripts.Net.handler
{
    public class ChooseCardHandler
    {
         public static void OnLotteryList(ThriftSCMessage msg)
         {
            var themsg = msg.GetContent() as SCLotteryList;
            if (themsg != null)
            {
                PlayerModelLocator.Instance.Famous = themsg.Famous;
                PlayerModelLocator.Instance.SuperChip = themsg.SuperChip;
                WindowManager.Instance.Show<ChooseCardWindow>(false);
                WindowManager.Instance.Show<ChooseHeroCardWindow>(true);
                WindowManager.Instance.GetWindow<ChooseHeroCardWindow>().Refresh(themsg);
            }
         }

         public static void OnLottery(ThriftSCMessage msg)
         {
            var themsg = msg.GetContent() as SCLottery;
            if (themsg != null)
            {
                if (themsg.LotteryMode == LotteryConstant.LotteryModeFree || themsg.LotteryMode == LotteryConstant.LotteryModeOnceCharge)
                {
                    var resultWin = WindowManager.Instance.Show<InfoShowingWindow>(true);
                    //resultWin.Refresh(themsg);
                }
                else if (themsg.LotteryMode == LotteryConstant.LotteryModeTenthCharge)
                {
                    var resultWin = WindowManager.Instance.Show<TenLotteryResultDispWindow>(true);
                    resultWin.Refresh(themsg);
                }
                //WindowManager.Instance.GetWindow<ChooseHeroCardWindow>().RefreshFamousAndChips();
            }
         }

         public static void OnLotteryRefreshTimes(ThriftSCMessage msg)
         {
            var themsg = msg.GetContent() as SCLotteryRefreshTimes;
            if (themsg != null)
            {
                WindowManager.Instance.GetWindow<ChooseHeroCardWindow>().RefreshTimes(themsg.LastFreeLotteryTime,
                                                                                      themsg.Get4StarHeroRestTimes);
            }
         }

         public static void OnAddItemsAndHeros(ThriftSCMessage msg)
         {
            var themsg = msg.GetContent() as SCAddItemsAndHeros;
            if (themsg != null)
            {
                var heroInfos = themsg.HeroInfos;
                if(heroInfos != null && heroInfos.Count > 0)
                {
                    if(HeroModelLocator.Instance.SCHeroList == null)
                    {
                        HeroModelLocator.Instance.SCHeroList = new SCHeroList();
                    }
                    var cachedInfos = HeroModelLocator.Instance.SCHeroList.HeroList;
                    if(cachedInfos == null)
                    {
                        HeroModelLocator.Instance.SCHeroList.HeroList = new List<HeroInfo>();
                        cachedInfos = HeroModelLocator.Instance.SCHeroList.HeroList;
                    }
                    for (var i = 0; i < heroInfos.Count; i++)
                    {
                        cachedInfos.Add(heroInfos[i]);
                    }
                }

                var itemInfos = themsg.ItemInfos;
                if(itemInfos != null && itemInfos.Count > 0)
                {
                    if (ItemModeLocator.Instance.ScAllItemInfos == null)
                    {
                        ItemModeLocator.Instance.ScAllItemInfos = new SCAllItemInfos();
                    }
                    var cachedInfos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                    if (cachedInfos == null)
                    {
                        ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>();
                        cachedInfos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                    }
                    for (var i = 0; i < itemInfos.Count; i++)
                    {
                        cachedInfos.Add(itemInfos[i]);
                    }
                }
            }
         }
   
        public static void OnLotteryNotFree(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryCannotFree;
            if (themsg != null)
            {
                var chooseHeroCard = WindowManager.Instance.GetWindow<ChooseHeroCardWindow>();
                chooseHeroCard.LotteryCannotFree();
            }
        }

        //Response event of SCLotteryComposeList message.
        public static void OnLotteryComposeList(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryComposeList;
            if (themsg != null)
            {
                WindowManager.Instance.Show<ChooseCardWindow>(false);
                WindowManager.Instance.Show<FragmentListWindow>(true);
                WindowManager.Instance.GetWindow<FragmentListWindow>().Refresh(themsg);
            }
        }

        //Response event of SCLotteryComposeSucc message.
        public static void OnLotteryComposeSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryComposeSucc;
            if (themsg != null)
            {
                WindowManager.Instance.Show<FragmentConfirmWindow>(false);
                var tempWindow = WindowManager.Instance.GetWindow<FragmentListWindow>();
                tempWindow.Refresh(themsg);
            }
        }
    }
}
