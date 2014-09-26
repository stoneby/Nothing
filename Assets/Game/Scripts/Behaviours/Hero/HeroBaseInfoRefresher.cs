using System.Collections.Generic;
using KXSGCodec;
using Template.Auto.Hero;
using UnityEngine;

public class HeroBaseInfoRefresher : MonoBehaviour 
{
    private KeyValuePair<int, GameObject> cachedHero = new KeyValuePair<int, GameObject>(InValid, null);
    private const int InValid = -1;
    public Transform Hero;
    public Transform Stars;
    public UILabel HeroNameLabel;
    public UISprite LockSprite;
    public UIEventListener LockLis;
    private const string LockName = "HeroLock_close";
    private const string UnLockName = "HeroLock_open";
    public static long Uuid;
    private bool isLock;
    private bool IsLock
    {
        get { return isLock; }
        set 
        {
            if(LockSprite != null)
            {
                LockSprite.spriteName = value ? LockName : UnLockName;
                isLock = value;
            }
        }
    }

    private void OnEnable()
    {
        if (LockLis)
        {
            LockLis.onClick = OnLock;
        }
    }

    private void OnDisable()
    {
        Despawn();
        if (LockLis)
        {
            LockLis.onClick = null;
        }
    }

    private void OnLock(GameObject go)
    {
        IsLock = !IsLock;
        var csHeroBindMsg = new CSHeroBind { HeroUuid = new List<long> { Uuid } };
        NetManager.SendMessage(csHeroBindMsg);
    }

    public void Refresh(HeroInfo heroInfo)
    {
        if(heroInfo == null)
        {
            Logger.LogWarning("You are refreshing hero info with null, Please check it.");
            return;
        }
        Uuid = heroInfo.Uuid;
        IsLock = heroInfo.Bind;
        var heroTemps = HeroModelLocator.Instance.HeroTemplates;
        HeroTemplate heroTemp;
        heroTemps.HeroTmpls.TryGetValue(heroInfo.TemplateId, out heroTemp);
        if (heroTemp != null)
        {

            for (var index = Stars.childCount - 1; index >= Stars.childCount - heroTemp.Star; index--)
            {
                NGUITools.SetActive(Stars.GetChild(index).gameObject, true);
            }
            for (var index = 0; index < Stars.childCount - heroTemp.Star; index++)
            {
                NGUITools.SetActive(Stars.GetChild(index).gameObject, false);
            }
            HeroNameLabel.text = heroTemp.Name;
            Despawn();
            Spawn(heroTemp.Icon - 1);
            IsLock = heroInfo.Bind;
        }
    }

    private void Spawn(int index)
    {
        var character = CharacterPoolManager.Instance.CharacterPoolList[index].Take().GetComponent<Character>();
        Utils.AddChild(Hero.gameObject, character.gameObject);
        cachedHero = new KeyValuePair<int, GameObject>(index, character.gameObject);
    }

    private void Despawn()
    {
        if (cachedHero.Key != InValid)
        {
            var go = cachedHero.Value;
            var anim = go.GetComponent<Character>().Animation;
            anim.playAutomatically = true;
            CharacterPoolManager.Instance.CharacterPoolList[cachedHero.Key].Return(go);
            cachedHero = new KeyValuePair<int, GameObject>(InValid, null);
        }
    }
}
