using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;
using OrderType = ItemHelper.OrderType;
using LeaderState = HeroConstant.LeaderState;

public class HeroUtils
 {
    private const int ConversionRate = 100;

    public static void ShowHero(OrderType orderType, HeroItem heroItem, int quality, short level, sbyte job, int atk, int hp, int recover)
    {
        switch (orderType)
        {
            //入手顺序排序
            case OrderType.Time:
                heroItem.ShowByLvl(level);
                break;

            //武将职业排序
            case OrderType.Job:
                heroItem.ShowByJob(job, atk);
                break;

            //武将稀有度排序
            case OrderType.Rarity:
                heroItem.ShowByLvl(level);
                break;

            //照队伍顺序排序
            case OrderType.Team:
                heroItem.ShowByLvl(level);
                break;

            //攻击力排序
            case OrderType.Attack:
                heroItem.ShowByJob(job, atk);
                break;

            //HP排序
            case OrderType.Health:
                heroItem.ShowByHp(hp);
                break;

            //回复力排序
            case OrderType.Recover:
                heroItem.ShowByRecover(recover);
                break;

            //等级排序
            case OrderType.Level:
                heroItem.ShowByLvl(level);
                break;
        }
    }

     /// <summary>
     /// Show the info of the item.
     /// </summary>
     /// <param name="orderType">The order type of </param>
     /// <param name="heroTran">The transform of item.</param>
     /// <param name="heroInfo">The info of item.</param>
    public static void ShowHero(OrderType orderType, HeroItem heroTran, HeroInfo heroInfo)
     {
         var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
         var quality = heroTemplate.Star;
         var level = heroInfo.Lvl;
         var job = heroTemplate.Job;
         var atk = heroInfo.Prop[RoleProperties.ROLE_ATK];
         var hp = heroInfo.Prop[RoleProperties.ROLE_HP]; ;
         var recover = heroInfo.Prop[RoleProperties.ROLE_RECOVER]; ;
         ShowHero(orderType, heroTran, quality, level, job, atk, hp, recover);
     }
    
    /// <summary>
    /// Get the leader state of the hero info with special uuid, current team uuids and all team uuids.
    /// </summary>
    /// <param name="uuid">The uuid of the hero info whose leader state is needed to get.</param>
    /// <param name="curTeam">The uuids of current team.</param>
    /// <param name="allTeams">The all uuids of all teams.</param>
    /// <returns></returns>
    public static LeaderState GetLeaderState(long uuid, List<long> curTeam, List<long> allTeams)
    {
        if(curTeam.Contains(uuid))
        {
            var index = curTeam.IndexOf(uuid);
            switch (index)
            {
                case  HeroConstant.LeaderPosInTeam:
                    {
                        return LeaderState.MainLeader;
                    }

                case HeroConstant.SecondLeaderPosInTeam:
                    {
                        return LeaderState.SecondLeader;
                    }

                case HeroConstant.ThirdLeaderPosInTeam:
                    {
                        return LeaderState.ThirdLeader;
                    }
                default:
                    {
                        return LeaderState.Member;
                    }
            }
        }

        if(allTeams.Contains(uuid))
        {
            return LeaderState.MemberInOtherTeam;
        }
        return LeaderState.NotInTeam;
    }

    /// <summary>
    /// Spawn or despawn the new game object, and install or uninstall handler. 
    /// </summary>
    /// <param name="parent">The parent of all items.</param>
    /// <param name="childPrefab">The prefab of child item.</param>
    /// <param name="isAdd">If true, add child to the parent.</param>
    /// <param name="count">The number of item to be added or deleted.</param>
    /// <param name="poolName">The name of pool.</param>
    /// <param name="dDelegate">The handler to install or uninstall.</param>
    public static void AddOrDelItems(Transform parent, Transform childPrefab, bool isAdd, int count, string poolName, UIEventListener.VoidDelegate dDelegate)
    {
        if (isAdd)
        {
            for (int i = 0; i < count; i++)
            {
                var item = PoolManager.Pools[poolName].Spawn(childPrefab);
                Utils.MoveToParent(parent, item);
                NGUITools.SetActive(item.gameObject, true);
                UIEventListener.Get(item.gameObject).onClick += dDelegate;
            }
        }
        else
        {
            if (PoolManager.Pools.ContainsKey(poolName))
            {
                var list = parent.Cast<Transform>().ToList();
                for (int index = 0; index < count; index++)
                {
                    var item = list[index];
                    UIEventListener.Get(item.gameObject).onClick -= dDelegate;
                    item.parent = PoolManager.Pools[poolName].transform;
                    PoolManager.Pools[poolName].Despawn(item);
                }
            }
        }
    }

    public static void AddOrDelItems(Transform parent, Transform childPrefab, bool isAdd, int count, string poolName, UIEventListener.VoidDelegate normalPress, UIEventListener.VoidDelegate longPress)
    {
        if (isAdd)
        {
            for (int i = 0; i < count; i++)
            {
                var item = PoolManager.Pools[poolName].Spawn(childPrefab);
                Utils.MoveToParent(parent, item);
                NGUITools.SetActive(item.gameObject, true);
                var longPressDetecter = item.GetComponent<NGUILongPress>();
                longPressDetecter.OnNormalPress += normalPress;
                longPressDetecter.OnLongPress += longPress;
            }
        }
        else
        {
            if (PoolManager.Pools.ContainsKey(poolName))
            {
                var list = parent.Cast<Transform>().ToList();
                for (int index = 0; index < count; index++)
                {
                    var item = list[index];
                    var longPressDetecter = item.GetComponent<NGUILongPress>();
                    longPressDetecter.OnNormalPress -= normalPress;
                    longPressDetecter.OnLongPress -= longPress;
                    item.parent = PoolManager.Pools[poolName].transform;
                    PoolManager.Pools[poolName].Despawn(item);
                }
            }
        }
    }

    public static void GetMaxProperties(HeroInfo info, out int maxAtk, out int maxHp, out int maxRecover, out int maxMp)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[info.TemplateId];
        var addtionTimes = heroTemplate.LvlLimit - info.Lvl;
        maxAtk = info.Prop[RoleProperties.ROLE_ATK] + GetConverted(heroTemplate.AttackAddtion) * addtionTimes;
        maxHp = info.Prop[RoleProperties.ROLE_HP] + GetConverted(heroTemplate.HPAddtion) * addtionTimes;
        maxRecover = info.Prop[RoleProperties.ROLE_RECOVER] + GetConverted(heroTemplate.RecoverAddtion) * addtionTimes;
        maxMp = info.Prop[RoleProperties.ROLE_MP] + GetConverted(heroTemplate.MPAddtion) * addtionTimes;
    }

    private static int GetConverted(int value)
    {
        return Mathf.RoundToInt((float)value / ConversionRate);
    }
 }
