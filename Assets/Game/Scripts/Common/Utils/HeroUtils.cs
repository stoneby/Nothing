using KXSGCodec;
using Property;
using UnityEngine;
using System.Collections;

public class HeroUtils
 {
    public static void ShowHero(short orderType, HeroItem heroItem, int quality, short level, sbyte job, int atk, int hp, int recover)
    {
        switch (orderType)
        {
            //入手顺序排序
            case 0:
                heroItem.ShowByLvl(level);
                break;

            //武将职业排序
            case 1:
                heroItem.ShowByJob(job, atk);
                break;

            //武将稀有度排序
            case 2:
                heroItem.ShowByLvl(level);
                break;

            //照队伍顺序排序
            case 3:
                heroItem.ShowByLvl(level);
                break;

            //攻击力排序
            case 4:
                heroItem.ShowByJob(job, atk);
                break;

            //HP排序
            case 5:
                heroItem.ShowByHp(hp);
                break;

            //回复力排序
            case 6:
                heroItem.ShowByRecover(recover);
                break;

            //等级排序
            case 7:
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
     public static void ShowHero(short orderType, HeroItem heroTran, HeroInfo heroInfo)
     {
         var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
         var quality = heroTemplate.Star;
         var level = heroInfo.Lvl;
         var job = heroTemplate.Job;
         var atk = heroInfo.Prop[RoleProperties.HERO_ATK];
         var hp = heroInfo.Prop[RoleProperties.HERO_HP]; ;
         var recover = heroInfo.Prop[RoleProperties.HERO_RECOVER]; ;
         ShowHero(orderType, heroTran, quality, level, job, atk, hp, recover);
     }
 }
