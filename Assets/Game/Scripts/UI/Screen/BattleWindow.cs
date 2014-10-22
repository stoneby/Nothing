using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.enums;
using UnityEngine;

#if !SILVERLIGHT
[Serializable]
#endif
public class BattleWindow : Window
{
    #region Public Fields

    /// <summary>
    /// Battle controller.
    /// </summary>
    public InitBattleField Battle;

    public bool EditMode;

    /// <summary>
    /// Rectangle team controller on left side.
    /// </summary>
    public TeamSelectController TeamLeft;

    /// <summary>
    /// Rectangle team controller on right side.
    /// </summary>
    public TeamSelectController TeamRight;

    /// <summary>
    /// Simple team controller.
    /// </summary>
    public TeamSimpleController TeamSimpleRight;

    #endregion

    #region Private Fields

    private const string GreenHandMtaType = "GreenHand";
    private const string NormalMtaType = "Normal";
    private const string PersistenceMtaType = "Persistence";

    #endregion

    #region Window

    public override void OnEnter()
    {
        Logger.Log("I am OnEnter with type - " + GetType().Name);

        Battle.Init();
        Battle.StartBattle();

        //Load Character audio.
        Character.LoadAudio();

        //MTA track battle start.
        var dict = new Dictionary<string, string>
            {
                {"Type", (PersistenceHandler.Instance.Mode!=PersistenceHandler.PersistenceMode.Normal) ?
                    PersistenceMtaType :
                    ((BattleModelLocator.Instance.BattleType == BattleType.GREENHANDPVE.Index) ? 
                    GreenHandMtaType : 
                    NormalMtaType)},
                {"Raid", (MissionModelLocator.Instance.BattleStageTemplate!=null) ? MissionModelLocator.Instance.BattleStageTemplate.Id.ToString() : "None"},
            };
        MtaManager.TrackCustomBeginKVEvent(MtaType.KVEventBattleDuration, dict);

        MemoryStrategy.Instance.HandleBattleBegin();
        BackGroundSoundControl.Instance.StopSound();
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit with type - " + GetType().Name);

        //Clean Character audio.
        Character.CleanUpAudio();

        //MTA track battle end.
        MtaManager.TrackCustomEndKVEvent(MtaType.KVEventBattleDuration);

        // clean up all battle fields.
        Battle.ResetAll();

        MemoryStrategy.Instance.HandleBattleEnd();
        BackGroundSoundControl.Instance.PlaySound();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Save position.
    /// </summary>
    private void SavePosition()
    {
        var formationController = TeamLeft.FormationController;
        formationController.SpawnList.Clear();
        formationController.SpawnList.AddRange(TeamLeft.CharacterList.Select(character => character.gameObject));
        formationController.Description = "RunningGameEdit";
        formationController.WriteXml();
    }

    #endregion

    #region Mono

#if UNITY_EDITOR

    private void OnGUI()
    {
        var x = Screen.width / 20;
        var y = Screen.height / 20;
        var width = Screen.width / 8;
        var height = Screen.height / 10;
        if (!EditMode)
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode On"))
            {
                EditMode = true;
                TeamLeft.EditMode = true;
                TeamSimpleRight.EditMode = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(x, y, width, height), "Edit Mode Off"))
            {
                EditMode = false;
                TeamLeft.EditMode = false;
                TeamSimpleRight.EditMode = false;
            }
        }

        if (EditMode)
        {
            if (GUI.Button(new Rect(Screen.width - x - width, y, width, height), "Save Position"))
            {
                SavePosition();
            }
        }
    }

#endif

    private void Awake()
    {

    }

    #endregion
}

