using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.factory.creater;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class PersistenceConfirmWindow : Window
{
    #region Private Fields

    private UIEventListener okBTN;
    private UIEventListener cancelBTN;
    private UILabel confirmLabel;

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        okBTN.onClick = OnOK;
        cancelBTN.onClick = OnCancel;
    }

    private void UnInstallHandlers()
    {
        okBTN.onClick = null;
        cancelBTN.onClick = null;
    }

    private void OnOK(GameObject go)
    {
        if (Mode == PersistenceMode.ReStartBattle || Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            OnReStratBattle();
        }
        else if (Mode == PersistenceMode.ReSendMessageNext||Mode==PersistenceMode.ReSendMessageNow)
        {
            OnReSendMessage();
        }
    }

    private void OnReStratBattle()
    {
        WindowManager.Instance.Show<PersistenceConfirmWindow>(false);

        BattleWindow.LoadMissionModelLocator();
        var battleStartMsg = BattleWindow.LoadStartBattleMessage();

        PopTextManager.PopTip("返回战斗数据");
        BattleModelLocator.Instance.BattleType = battleStartMsg.BattleType;
        BattleModelLocator.Instance.EnemyGroup = battleStartMsg.MonsterGroup;
        BattleModelLocator.Instance.RaidID = battleStartMsg.RaidID;
        BattleModelLocator.Instance.Uuid = battleStartMsg.Uuid;

        // server logic data.
        var type = BattleType.getValue(battleStartMsg.BattleType);
        BattleModelLocator.Instance.Source = new BattleSource(type)
        {
            Uuid = battleStartMsg.Uuid
        };

        BattleModelLocator.Instance.HeroList = FighterInfoCreater.createListFromMsgHero(BattleSideEnum.SIDEA, battleStartMsg.FighterList);
        BattleModelLocator.Instance.EnemyList = FighterInfoCreater.createListFormMsgMonster(BattleSideEnum.SIDEB, battleStartMsg.MonsterGroup, battleStartMsg.MonsterList);

        var allFighterList = new List<FighterInfo>();
        allFighterList.AddRange(BattleModelLocator.Instance.HeroList);
        allFighterList.AddRange(BattleModelLocator.Instance.EnemyList);
        var source = BattleModelLocator.Instance.Source;
        BattleModelLocator.Instance.Source.FighterProp = allFighterList;

        source.heroSkillList.AddRange(battleStartMsg.HeroSkillList);
        source.MonsterAIList.AddRange(battleStartMsg.MonsterAIList);
        source.monsterSkillList.AddRange(battleStartMsg.MonsterSkillList);
        if (battleStartMsg.BuffList != null)
        {
            source.buffList.AddRange(battleStartMsg.BuffList);
        }
        FighterInfoCreater.initBattleSkillService(source);

        var factory = type.Factory;
        BattleModelLocator.Instance.MainBattle = factory.createBattle(BattleModelLocator.Instance.Source);

        Dictionary<string, float> tempPersistence = new Dictionary<string, float>();
        if (Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            tempPersistence = BattleWindow.LoadPersistence();
            BattleModelLocator.Instance.MainBattle.start((int)tempPersistence["TopData"]);
        }
        else
        {
            BattleModelLocator.Instance.MainBattle.start();
        }

        BattleModelLocator.Instance.MonsterIndex = 0;

        // client side show.
        var window = WindowManager.Instance.Show(typeof(BattleWindow), true).gameObject;
        WindowManager.Instance.Show(typeof(MissionTabWindow), false);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), false);
        WindowManager.Instance.Show(typeof(BattleConfirmTabWindow), false);

        if (Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            window.GetComponent<BattleWindow>().Battle.PersisitenceSet(tempPersistence);
        }
    }

    private void OnReSendMessage()
    {
        if (Mode == PersistenceMode.ReSendMessageNext)
        {
            BattleWindow.LoadMissionModelLocator();
            var battleEndMsg = BattleWindow.LoadBattleEndMessage();

            NetManager.SendMessage(battleEndMsg);
            MtaManager.TrackEndPage(MtaType.BattleScreen);

            //Check battle end succeed.
            StartCoroutine(BattleWindow.MakeBattleEndSucceed(battleEndMsg));
        }

        if (Mode == PersistenceMode.ReSendMessageNow)
        {
            IsReSendMessageNowClickDown = true;
        }
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<PersistenceConfirmWindow>(false);
        if (Mode != PersistenceMode.ReSendMessageNow)
        {
            new FileInfo(BattleWindow.MissionModelLocatorPath).Delete();
            new FileInfo(BattleWindow.StartBattleMessagePath).Delete();
            new FileInfo(BattleWindow.PersistencePath).Delete();
            new FileInfo(BattleWindow.BattleEndMessagePath).Delete();
        }
    }

    #endregion

    #region Public Fields

    public enum PersistenceMode
    {
        ReSendMessageNow,
        ReStartBattle,
        ReStartBattleWithPersistence,
        ReSendMessageNext
    }

    public static PersistenceMode Mode;

    public static bool IsReSendMessageNowClickDown = false;

    #endregion

    #region Public Methods

    public void SetLabel()
    {
        if (Mode == PersistenceMode.ReStartBattle || Mode == PersistenceMode.ReStartBattleWithPersistence)
        {
            confirmLabel.text = "在当前账号下检测到未完成的副本，是否继续副本？";
        }
        else if (Mode == PersistenceMode.ReSendMessageNext)
        {
            confirmLabel.text = "在当前账号下检测到未发送的战斗结束消息，是否继续发送？";
        }
        else if (Mode == PersistenceMode.ReSendMessageNow)
        {
            confirmLabel.text = "发送消息失败，是否继续发送？";
        }
        else
        {
            Logger.LogError("Error to persistence way.");
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    void Awake()
    {
        okBTN = UIEventListener.Get(transform.Find("Buttons/Button-Ok").gameObject);
        cancelBTN = UIEventListener.Get(transform.Find("Buttons/Button-Cancel").gameObject);
        confirmLabel = transform.Find("ConfirmLabel").gameObject.GetComponent<UILabel>();
    }

    #endregion
}
