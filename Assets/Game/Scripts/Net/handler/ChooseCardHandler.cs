using System.Collections;
using KXSGCodec;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Net.handler
{
    public class ChooseCardHandler
    {
        #region Public Fields

        //public static bool IsHeroFirstLoginGive = false;
        //public static ThriftSCMessage HeroFirstLoginGiveMsg;

        #endregion

        #region Public Methods

        public static void OnLotteryList(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryList;
            if (themsg != null)
            {
                PlayerModelLocator.Instance.Famous = themsg.Famous;
                PlayerModelLocator.Instance.SuperChip = themsg.SuperChip;
                var window=WindowManager.Instance.GetWindow<ChooseCardWindow>();
                window.ScLotteryList = themsg;
                window.InitializeToggleButtons();
            }
        }

        public static void OnLottery(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLottery;
            if (themsg != null)
            {
                var window = WindowManager.Instance.GetWindow<ChooseCardEffectWindow>();
                if (NGUITools.GetActive(window.gameObject))
                {
                    window.CleanUp();

                }
                window = WindowManager.Instance.Show<ChooseCardEffectWindow>(true);
                window.Refresh(themsg);
            }
        }

        public static void OnHeroFirstLoginGive(ThriftSCMessage msg)
        {
            //PopTextManager.PopTip("Get hero in first login today!");
            var themsg = msg.GetContent() as SCHeroFristLoginGive;
            if (themsg != null)
            {
                Logger.Log("!!!!!!!!!!!!!!!!themsg is:" + themsg.HeroInfos[0].Uuid + "," + themsg.HeroInfos[1].Uuid + "," + themsg.HeroInfos[2].Uuid);
                var window = WindowManager.Instance.Show<ChooseCardSuccWindow>(true);
                window.StoredHeroFristLoginGiveMsg = themsg;
                window.ShowHeroFirstGive();
                if (HeroModelLocator.AlreadyRequest)
                {
                    AddToCacheHeroList(themsg.HeroInfos);
                }
            }
        }

        public static void OnLotteryRefreshTimes(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryRefreshTimes;
            var window = WindowManager.Instance.GetWindow<ChooseCardWindow>();
            window.HeroAndItemSummitHandler.RefreshTimes(themsg.LastFreeLotteryTime, themsg.Get4StarHeroRestTimes);
        }

        public static void OnAddItemsAndHeros(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCAddItemsAndHeros;
            if (themsg != null)
            {
                var heroInfos = themsg.HeroInfos;
                AddToCacheHeroList(heroInfos);

                var itemInfos = themsg.ItemInfos;
                AddToCacheItemList(itemInfos);
            }
        }

        public static void AddToCacheItemList(List<ItemInfo> itemInfos)
        {
            if(itemInfos != null && itemInfos.Count > 0)
            {
                if(ItemModeLocator.Instance.ScAllItemInfos == null)
                {
                    ItemModeLocator.Instance.ScAllItemInfos = new SCAllItemInfos();
                }
                var cachedInfos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                if(cachedInfos == null)
                {
                    ItemModeLocator.Instance.ScAllItemInfos.ItemInfos = new List<ItemInfo>();
                    cachedInfos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
                }
                for(var i = 0; i < itemInfos.Count; i++)
                {
                    cachedInfos.Add(itemInfos[i]);
                }
            }
        }

        public static void AddToCacheHeroList(List<HeroInfo> heroInfos)
        {
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
                for(var i = 0; i < heroInfos.Count; i++)
                {
                    cachedInfos.Add(heroInfos[i]);
                }
            }
        }

        public static void OnLotteryNotFree(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryCannotFree;
            if (themsg != null)
            {
                WindowManager.Instance.GetWindow<ChooseCardWindow>().HeroAndItemSummitHandler.LotteryCannotFree();
            }
        }

        /// <summary>
        /// Response event of SCLotteryComposeList message.
        /// </summary>
        /// <param name="msg"></param>
        public static void OnLotteryComposeList(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryComposeList;
            if (themsg != null)
            {
                var window=WindowManager.Instance.GetWindow<ChooseCardWindow>();
                window.ScLotteryComposeList = themsg;
                window.FragmentCombineHandler.Refresh(themsg);
            }
        }

        /// <summary>
        /// Response event of SCLotteryComposeSucc message.
        /// </summary>
        /// <param name="msg"></param>
        public static void OnLotteryComposeSucc(ThriftSCMessage msg)
        {
            var themsg = msg.GetContent() as SCLotteryComposeSucc;
            if (themsg != null)
            {
                WindowManager.Instance.GetWindow<ChooseCardWindow>().FragmentCombineHandler.Refresh(themsg);
            }
        }

        #endregion
    }
}
