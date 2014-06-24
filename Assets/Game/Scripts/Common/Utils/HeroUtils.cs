using KXSGCodec;
using Property;
using OrderType = ItemHelper.OrderType;

public class HeroUtils
 {
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
         var atk = heroInfo.Prop[RoleProperties.HERO_ATK];
         var hp = heroInfo.Prop[RoleProperties.HERO_HP]; ;
         var recover = heroInfo.Prop[RoleProperties.HERO_RECOVER]; ;
         ShowHero(orderType, heroTran, quality, level, job, atk, hp, recover);
     }
 }
