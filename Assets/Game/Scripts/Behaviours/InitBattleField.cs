using Assets.Game.Scripts.Common.Model;
using Assets.Game.Scripts.Net.handler;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.enums;
using com.kx.sglm.gs.battle.share.input;
using com.kx.sglm.gs.hero.properties;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitBattleField : MonoBehaviour, IBattleView
{
    public GameObject SpritePrefab;
    public GameObject EffectBg;
    public GameObject EffectObject;

    public GameObject BreakObject;
    public GameObject TextBGObject;
    public GameObject TextObject;
    public GameObject Picture91;
    public GameObject TextBG91;
    public GameObject TexSwardBg91;
    public GameObject Text91;

    public CameraLikeEffect CameraEffect;

    /// <summary>
    /// Warning controller.
    /// </summary>
    public WarningEffect WarningController;

    /// <summary>
    /// Rectangle team controller
    /// </summary>
    /// <remarks>Used to control our heros</remarks>
    public TeamSelectController TeamController;

    /// <summary>
    /// Simple team controller
    /// </summary>
    /// <remarks>Used to control our enemies</remarks>
    public TeamSimpleController MonsterController;

    /// <summary>
    /// Character's waiting transform.
    /// </summary>
    public Transform CharacterWaitingTrans;

    /// <summary>
    /// Battle face controller.
    /// </summary>
    public BattleFaceController FaceController;

    /// <summary>
    /// Battleground controller
    /// </summary>
    public BattlegroundController BattleController;

    /// <summary>
    /// Nine attack controller.
    /// </summary>
    public NineAttackController NineAttack;

    #region Battle Face Inner Controller

    /// <summary>
    /// Next foot manager to show next foot colors on the top left.
    /// </summary>
    private NextFootManager footManager;
    private MagicBarController hpController;
    private MagicBarController mpController;
    private TopDataController topController;
    private RecordController stepController;
    private LeaderGroupController leaderController;
    private StarController starController;

    #endregion

    private int characterAttackValue;
    //left part of battlefield
    private GameObject leftContainerObj;

    private float characterValue;

    /// <summary>
    /// Attack waiting characters list.
    /// </summary>
    private List<Character> attackWaitList;

    /// <summary>
    /// Character team list.
    /// </summary>
    private readonly Character[,] charactersLeft = new Character[3, 3];

    private readonly List<Character> originalCharacterList = new List<Character>();
    private readonly List<Character> originalMonsterList = new List<Character>();

    private bool isRecover;

    private BattleSkillRecord leaderSkillRecord;
    private BattleTeamFightRecord battleTeamRecord;
    private BattleModeHandler battleModeHandler;

    public void Init()
    {
        EffectBg.SetActive(false);
        EffectObject.SetActive(false);
        BreakObject.SetActive(false);
        TextBGObject.SetActive(false);
        TextObject.SetActive(false);
        Picture91.SetActive(false);
        TextBG91.SetActive(false);
        Text91.SetActive(false);
        TexSwardBg91.SetActive(false);

        Debug.Log("Set greenhandBattleMode.");
        battleModeHandler = new BattleModeHandler();
        //Reset current config for GreenHand.
        battleModeHandler.ResetCurrentConfig();
    }

    /// <summary>
    /// Start battle of the whole level.
    /// </summary>
    /// <remarks>Called once per map level.</remarks>
    public void StartBattle()
    {
        MtaManager.TrackBeginPage(MtaType.BattleScreen);
        leftContainerObj = GameObject.Find("BattleFieldWidgetLeft");

        // First reset for case that battle result window does not show correctly.
        ResetAll();

        // team controller initialization should be at the right beginning.
        // everything else is depend on this structure.
        InitTeamController();

        InitCharacterList();

        MonsterController.OnSelectedChanged = OnMonsterSelected;
        InitMonsterList();

        InitBattleFace();

        InitBattleground();

        ShowTopData();

        // init character list & attack wait list from team selection controller.
        SyncCharacterList();

        RequestRecords();

        StartCoroutine(InitBattleFighters());
    }

    private void InitTeamController()
    {
        // Initialize fighter list to multiple character generator before team controller initialization.
        var generator = TeamController.GroupController.Generator;
        generator.FighterList = BattleModelLocator.Instance.HeroList;

        TeamController.Total = BattleModelLocator.Instance.HeroList.Count;
        TeamController.Row = 3;
        TeamController.Col = 3;
        TeamController.Initialize();

        TeamController.CharacterList.ForEach(item => item.BuffController.OnUpdateHurtValue = OnUpdateHurtValue);
        TeamController.CharacterList.ForEach(item => item.BuffController.OnSeal = OnSeal);

        // keep original character list in memory easy for quering.
        originalCharacterList.Clear();
        originalCharacterList.AddRange(TeamController.CharacterList);

        Logger.Log("Team controller total num: " + TeamController.Total);
    }

    private void InitBattleFace()
    {
        characterValue = 0;

        // reset.
        FaceController.Reset();

        // initialize.
        leaderController.Init(BattleModelLocator.Instance.HeroList, TeamController.CharacterList);
        leaderController.LeaderList.ForEach(leader => leader.OnActiveLeaderSkill += OnActiveLeaderSkill);

        stepController.TotalValue = BattleModelLocator.Instance.MonsterGroup.Count;
    }

    private void InitBattleground()
    {
        BattleController.Reset();

        var enemyGroup = BattleModelLocator.Instance.MonsterGroup;
        BattleController.TotalStep = enemyGroup.Count;

        var stageTemplate = MissionModelLocator.Instance.GetRaidStagrByTemplateId(BattleModelLocator.Instance.RaidID);
        // Battle ID which is 1, so...
        BattleController.BattleID = stageTemplate.SceneId;
        BattleController.Initialize();
    }

    private void ShowTopData()
    {
        stepController.CurrentValue = (BattleModelLocator.Instance.LevelManager.CurrentLevel + 1);
        stepController.Show();
    }

    private void ResetCharacterList()
    {
        TeamController.CharacterList.ForEach(character =>
        {
            var idIndex = character.IDIndex;
            var pool = CharacterPoolManager.Instance.CharacterPoolList[idIndex];
            pool.Return(character.gameObject);
        });
    }

    private void InitCharacterList()
    {
        for (var i = 0; i < TeamController.Total; i++)
        {
            var hero = TeamController.CharacterList[i];
            var cc = hero.FaceObject.GetComponent<CharacterControl>();
            cc.SetCharacter((i >= TeamController.VisibleCount) ? CharacterType.Friend : CharacterType.Hero);
            cc.SetSelect(false);
        }
    }

    IEnumerator InitBattleFighters()
    {
        yield return StartCoroutine(MakeUpOneByOne(false));
        RunToNextMonsters();
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void InitMonsterList()
    {
        var monsterGroup = BattleModelLocator.Instance.MonsterGroup;
        var monsterList = BattleModelLocator.Instance.MonsterList;

        Logger.Log("Monster group count: " + monsterGroup.Count + ", current monster group index: " + BattleModelLocator.Instance.LevelManager.CurrentLevel +
                   ", total monster count: " + monsterList.Count);

        // Initialize fighter list to multiple character generator before team controller initialization.
        var generator = MonsterController.GroupController.Generator;
        generator.FighterList = BattleModelLocator.Instance.LevelManager.CurrentMonsterList;

        MonsterController.Cleanup();
        MonsterController.Initialize();

        Logger.Log("Current level enemy's count: " + MonsterController.Total);

        for (var i = 0; i < MonsterController.CharacterList.Count; ++i)
        {
            var enemyController = MonsterController.CharacterList[i].FaceObject.GetComponent<MonsterControl>();
            var enemyData = monsterList[BattleModelLocator.Instance.MonsterIndex + i];
            var maxValue = enemyData.BattleProperty[RoleAProperty.HP];
            enemyController.Reset();
            enemyController.SetBloodBar(maxValue, maxValue);
            enemyController.CharacterData.Data = enemyData;
            enemyController.BaseWidget = BattleController.GetComponent<UIWidget>();

            Logger.Log("Init enemy of index: " + (BattleModelLocator.Instance.MonsterIndex + i));
        }

        // set default current select to index of 0.
        MonsterController.SetDefaultCurrentSelect();

        originalMonsterList.Clear();
        originalMonsterList.AddRange(MonsterController.CharacterList);
    }

    private void SyncCharacterList()
    {
        if (attackWaitList == null)
        {
            attackWaitList = new List<Character>();
        }

        attackWaitList.Clear();
        for (var i = 0; i < TeamController.Total; ++i)
        {
            var character = TeamController.CharacterList[i];
            if (i >= TeamController.VisibleCount)
            {
                attackWaitList.Add(character);
            }
            else
            {
                charactersLeft[i / TeamController.Row, i % TeamController.Col] = character;
            }
        }
    }

    /// <summary>
    /// Sync back to team controller's character list.
    /// </summary>
    /// <remarks>
    /// After rearrangement, we need to sync updated character list to team selection controller for selecting.
    /// </remarks>
    private void SyncTeamController()
    {
        TeamController.CharacterList.Clear();
        for (var i = 0; i < TeamController.Col; i++)
        {
            for (var j = 0; j < TeamController.Row; j++)
            {
                var character = charactersLeft[i, j];
                character.Location = new Position { X = i, Y = j };
                TeamController.CharacterList.Add(character);
            }
        }
        foreach (var character in attackWaitList)
        {
            TeamController.CharacterList.Add(character);
        }

        Validate();
    }

    private void Validate()
    {
        var expectWaitingCount = TeamController.Total - TeamController.VisibleCount;
        var resultWaitingCount = attackWaitList.Count;
        if (expectWaitingCount != resultWaitingCount)
        {
            Logger.LogError("Expected waiting count: " + expectWaitingCount + ", is not the same as result waiting count: " + resultWaitingCount);
        }
    }

    private void ResetBattle()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                charactersLeft[i, j] = null;
            }
        }

        if (attackWaitList != null)
        {
            attackWaitList.Clear();
        }
    }

    IEnumerator MakeUpOneByOne(bool needresetcharacter = true)
    {
        float runStepTime = (needresetcharacter) ? GameConfig.RunStepNeedTime : GameConfig.ShortTime;
        float runWaitTime = (needresetcharacter) ? GameConfig.NextRunWaitTime : GameConfig.ShortTime;
        float duration = GameConfig.ShortTime;
        for (var i = 0; i < TeamController.Col; i++)
        {
            for (var j = 0; j < TeamController.Row; j++)
            {
                if (charactersLeft[i, j] == null)
                {
                    var flag = true;
                    CharacterControl cc;
                    for (int k = i + 1; k < 3; k++)
                    {
                        if (charactersLeft[k, j] != null)
                        {
                            charactersLeft[i, j] = charactersLeft[k, j];
                            var hero = charactersLeft[i, j];
                            cc = hero.FaceObject.GetComponent<CharacterControl>();
                            cc.PlayCharacter(Character.State.Run, true);
                            duration = (k - i) * runStepTime;
                            cc.SetCharacterAfter(duration);

                            // Move to target.
                            var target = charactersLeft[k, j];
                            var targetPosition =
                                TeamController.FormationController.LatestPositionList[
                                    TeamController.TwoDimensionToOne(i, j)];
                            iTween.MoveTo(target.gameObject, targetPosition, duration);

                            charactersLeft[k, j] = null;
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        charactersLeft[i, j] = attackWaitList[0];
                        attackWaitList.RemoveAt(0);

                        charactersLeft[i, j].gameObject.SetActive(true);
                        cc = charactersLeft[i, j].FaceObject.GetComponent<CharacterControl>();

                        cc.PlayCharacter(Character.State.Run, true);

                        duration = (2 - i) * GameConfig.RunStepNeedTime + GameConfig.RunStepNeedTime;

                        if (needresetcharacter)
                        {
                            cc.SetCharacterAfter(duration);
                        }

                        // Move to target from delta left.
                        var index = TeamController.TwoDimensionToOne(i, j);
                        var targetPosition = TeamController.FormationController.LatestPositionList[index];
                        var sourcePosition = TeamController.WaitingStackList[j].transform.position;
                        charactersLeft[i, j].transform.position = sourcePosition;

                        var target = charactersLeft[i, j];
                        iTween.MoveTo(target.gameObject, targetPosition, duration);
                    }
                    yield return new WaitForSeconds(runWaitTime);
                }
            }
        }

        SyncTeamController();

        battleModeHandler.SetBattleField(TeamController, MonsterController, charactersLeft, FaceController, "MakeUpOneByOne");

        if (needresetcharacter)
        {
            yield return new WaitForSeconds(duration);
        }
    }

    private static void SetColors(IList<Character> characterList)
    {
        var nextColorList = BattleModelLocator.Instance.NextList;
        if (nextColorList.Count != characterList.Count)
        {
            Logger.LogError("Next colors count: " + nextColorList.Count + " does not match client selected character list count: " + characterList.Count);
            return;
        }

        for (var i = 0; i < nextColorList.Count; ++i)
        {
            var colorIndex = nextColorList[i].Color;
            var character = characterList[i];
            character.ColorIndex = colorIndex;
            var cc = characterList[i].FaceObject.GetComponent<CharacterControl>();
            cc.SetFootIndex();
        }
    }

    void Update()
    {
        MoveCharacterUpdate();
    }

    private void OnEnable()
    {
        TeamController.OnSelect += OnSelected;
        TeamController.OnDeselect += OnDeselected;
        TeamController.OnStart += OnSelectedStart;
        TeamController.OnStop += OnSelectedStop;
    }

    private void OnDisable()
    {
        TeamController.OnSelect -= OnSelected;
        TeamController.OnDeselect -= OnDeselected;
        TeamController.OnStart -= OnSelectedStart;
        TeamController.OnStop -= OnSelectedStop;
    }

    private void OnSelected(Character character)
    {
        var characterControll = character.FaceObject.GetComponent<CharacterControl>();
        currentFootIndex = characterControll.FootIndex;
        AddAObj(characterControll);
    }

    private void OnDeselected(Character character)
    {
        var characterControll = character.FaceObject.GetComponent<CharacterControl>();
        AddAObj(characterControll, false);
    }

    private void OnSelectedStart()
    {
        ResetOneRound();
    }

    private void OnSelectedStop(bool isAttacked)
    {
        FaceController.ResetAttackLabel();

        CleanEffect();

        TeamController.SelectedCharacterList.ForEach(hero =>
        {
            var character = hero.FaceObject.GetComponent<CharacterControl>();
            character.SetSelect(false);
        });

        //Cancel attack if selectedList doesn't fit config info in greenHandGuide mode.
        if (battleModeHandler.CheckCanAttack(TeamController) == 0)
        {
            isAttacked = false;
        }

        if (isAttacked)
        {
            battleModeHandler.StopFingerMove();
            var selectedList = TeamController.SelectedCharacterList.Select(item => TeamController.TwoDimensionToOne(item.Location));
            DoAttack(selectedList.ToArray());
        }
        else
        {
            hpController.ShowForgroundBar(characterValue);
            mpController.ShowForgroundBar(leaderController.TotalLeaderCD);
        }
    }

    private ArrayList selectEffectList;
    private int currentFootIndex;

    private void CleanEffect()
    {
        while (selectEffectList != null && selectEffectList.Count > 0)
        {
            Destroy(selectEffectList[0] as GameObject);
            selectEffectList.RemoveAt(0);
        }
    }

    private void ResetOneRound()
    {
        if (selectEffectList == null)
        {
            selectEffectList = new ArrayList();
        }
        selectEffectList.Clear();

        characterAttackValue = 0;
    }

    private void DoAttack(int[] indexArr)
    {
        if (MonsterController.CharacterList.Count <= 0)
        {
            Logger.Log("Enemy list is empty. Please check it out.");
            return;
        }

        var enemy = MonsterController.CurrentSelect;
        Logger.LogWarning("Attack to emeny: " + enemy);

        var action = new ProduceFighterIndexAction
        {
            HeroIndex = indexArr,
            TargetIndex = enemy.Index
        };
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(action);
        RequestRecords();
    }

    void AddAObj(CharacterControl characterControll, bool isadd = true)
    {
        if (isadd)
        {
            // adjust attack value in case zero effect buff is on.
            characterControll.AdjustAttackValue();
            characterControll.SetSelect(true, TeamController.SelectedCharacterList.Count - 1);
            selectEffectList.Add(EffectManager.ShowEffect(EffectType.SelectEffects[currentFootIndex], 0, 0, characterControll.transform.position));
        }
        else
        {
            characterControll.SetSelect(false);
            Destroy(selectEffectList[selectEffectList.Count - 1] as GameObject);
            selectEffectList.RemoveAt(selectEffectList.Count - 1);
        }

        characterAttackValue = isadd ? characterAttackValue + characterControll.AttackValue : characterAttackValue - characterControll.AttackValue;
        FaceController.SetAttackLabel("" + characterAttackValue);

        if (currentFootIndex == (int)FootColorType.Pink)
        {
            hpController.ShowBackgroundBar(characterAttackValue);
        }
        hpController.ShowBackgroundBar(TeamController.SelectedCharacterList.Count);
    }

    /// <summary>
    /// Left side attack.
    /// </summary>
    void DoAttrackLeft()
    {
        StartCoroutine(LeftAttackCoroutineHandler());
    }

    /// <summary>
    /// Right side attack.
    /// </summary>
    void DoAttrackRight()
    {
        StartCoroutine(RightAttackCoroutineHandler());
    }

    IEnumerator OnUpdateHurtValue(GameObject sender, float value)
    {
        CharacterLoseBlood(sender.transform.localPosition, value);

        yield return StartCoroutine(DoPlayHurt(sender.GetComponent<Character>()));
    }

    private void OnSeal(GameObject sender, bool show)
    {
        var character = sender.GetComponent<Character>();
        if (character.IsLeader)
        {
            leaderController.PlaySeal(character.Index, show);
        }
    }

    private IEnumerator DoPlayHurt(Character character)
    {
        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);
        character.PlayState(Character.State.Hurt, false);
        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);
        character.PlayState(Character.State.Idle, true);
    }

    /// <summary>
    /// Display character losing blood.
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="newvalue">New value to take</param>
    /// <param name="isnotadd">Flag indicates if not added</param>
    void CharacterLoseBlood(Vector3 pos, float newvalue, bool isnotadd = true)
    {
        if (isnotadd)
        {
            var losevalue = characterValue - newvalue;

            characterValue = newvalue;
            if (losevalue > 0)
            {
                PopTextManager.ShowText("-" + losevalue, 0.6f, -25, 60, 50, pos);

                var ratio = characterValue / hpController.TotalValue;
                starController.Show(ratio);
            }
        }
        else
        {
            PopTextManager.ShowText("+" + characterAttackValue, 0.6f, 80, 100, 50, pos);
        }
        hpController.ShowForgroundBar(characterValue);
    }

    void PlayBloodFullEffect()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                EffectManager.PlayEffect(EffectType.BloodFull, GameConfig.PlayRecoverEffectTime, -20, 0, obj.transform.position);
            }
        }
    }

    void RunToNextMonsters()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var hero = charactersLeft[i, j];
                var cc = hero.FaceObject.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Run, true);
                cc.SetCharacterAfter(GameConfig.RunRoNextMonstersTime);
            }
        }

        BattleController.Play();

        foreach (var enemyController in MonsterController.CharacterList.Select(character => character.FaceObject.GetComponent<MonsterControl>()))
        {
            enemyController.Move();
        }
    }

    private IEnumerator PlayOneAction(BattleFightRecord record)
    {
        var character = GetCharacterByAction(record.getAttackAction());
        var characterControll = character.FaceObject.GetComponent<CharacterControl>();

        switch (record.getAttackAction().ActType)
        {
            case BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK:
                {
                    var monster = GetMonsterByAction(record.ActionList[0]);
                    RunReturn(character, GameConfig.ShortTime);
                    // [FIXME]: Need new SP effect.
                    yield return new WaitForSeconds(AddMoveCharacter(character, monster));
                    StartCoroutine(PlayMonsterBeenAttack(record.ActionList));
                }
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER:
                isRecover = true;
                characterControll.PlayCharacter(Character.State.Attack, false);
                yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                characterControll.PlayCharacter(Character.State.Idle, true);
                CharacterLoseBlood(character.transform.localPosition,
                    record.getAttackAction().getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED:
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE:
                break;
            default:
                {
                    var monster = GetMonsterByAction(record.ActionList[0]);
                    RunToAttackPlace(character, monster);
                    yield return new WaitForSeconds(GameConfig.RunToAttrackPosTime);
                    characterControll.PlayCharacter(Character.State.Attack, false);
                    yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                    StartCoroutine(PlayMonsterBeenAttack(record.ActionList));
                    RunReturn(character, GameConfig.HeroRunReturnTime);
                }
                break;
        }
    }

    private IEnumerator LeftAttackCoroutineHandler()
    {
        if (battleTeamRecord.SkillFighter != null && battleTeamRecord.SkillFighter.Count > 0)
        {
            yield return new WaitForSeconds(0.3f * battleTeamRecord.SkillFighter.Count + 0.5f);
        }

        BattleFightRecord record;
        isRecover = false;

        if (battleTeamRecord.RecordList.Count > 0)
        {
            record = battleTeamRecord.RecordList[0];
            if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
            {
                StartCoroutine(PlayOneAction(record));
            }
            else
            {
                yield return StartCoroutine(PlayOneAction(record));
            }
        }

        if (battleTeamRecord.RecordList.Count > 1)
        {
            var count = (battleTeamRecord.RecordList.Count != 9 || isRecover) ? battleTeamRecord.RecordList.Count : battleTeamRecord.RecordList.Count - 1;
            //var count = battleTeamRecord.RecordList.Count;
            for (var i = 1; i < count; i++)
            {
                record = battleTeamRecord.RecordList[i];
                StartCoroutine(PlayOneAction(record));
                if (!isRecover)
                {
                    yield return new WaitForSeconds(GameConfig.NextAttrackWaitTime);
                }
            }
        }

        if (battleTeamRecord.RecordList.Count == 9 && !isRecover)
        {
            // nine attack effect play.
            var lastHero = TeamController.SelectedCharacterList[TeamController.SelectedCharacterList.Count - 1];
            NineAttack.Start = lastHero.gameObject;
            NineAttack.End = MonsterController.CurrentSelect.gameObject;
            NineAttack.Initialize(lastHero.JobIndex);
            NineAttack.PlayEffect();

            // play special attack animation together.
            lastHero.PlayState(Character.State.Special, false);

            // wait util finish.
            yield return new WaitForSeconds(NineAttack.Duration);

            // cleanup nine attack stuffes.
            NineAttack.Cleanup();

            // play monster get hurt.
            record = battleTeamRecord.RecordList[battleTeamRecord.RecordList.Count - 1];
            StartCoroutine(PlayMonsterBeenAttack(record.ActionList));

            // run back to waiting list.
            RunReturn(lastHero, GameConfig.HeroRunReturnTime);
            yield return new WaitForSeconds(GameConfig.HeroRunReturnTime);
        }

        if (isRecover)
        {
            yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
            PlayBloodFullEffect();
            yield return new WaitForSeconds(GameConfig.PlayRecoverEffectTime);
            if (battleTeamRecord.RecordList.Count > 0)
            {
                foreach (var fightRecord in battleTeamRecord.RecordList)
                {
                    var hero = GetCharacterByAction(fightRecord.getAttackAction());
                    if (hero != null)
                    {
                        RunReturn(hero, GameConfig.HeroRunReturnTime);
                    }
                }
            }
        }
        var temp = (battleTeamRecord.SkillFighter != null && battleTeamRecord.SkillFighter.Count > 0) ? 0.3f : 0;
        yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime + temp);
        CheckMonsterDead();

        // Set colors to selected character list, which are appended to attack waiting list already.
        // make color here is because those characters are invisible for now.
        SetColors(TeamController.SelectedCharacterList);

        // Move foot manager one round.
        footManager.Move();

        yield return StartCoroutine(MakeUpOneByOne());

        // show enemy's debuff.
        ShowDebuff(MonsterController.CharacterList);
        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);

        leaderController.TotalLeaderCD = battleTeamRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_MP);
        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);

        ShowTopData();

        recordIndex++;
        DealWithRecord();
    }

    private void CheckMonsterDead()
    {
        var monsterList = BattleModelLocator.Instance.MonsterList;
        var monsterIndexBase = BattleModelLocator.Instance.MonsterIndex;
        var deadList = new List<int>();
        for (var i = 0; i < MonsterController.CharacterList.Count; ++i)
        {
            var monster = MonsterController.CharacterList[i];
            var enemyController = monster.FaceObject.GetComponent<MonsterControl>();
            if (enemyController.Health <= 0)
            {
                var enemyData = monsterList[monsterIndexBase + monster.Index];
                var v = enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_COIN);
                if (v > 0)
                {
                    topController.GoldCount += v;
                    EffectManager.PlayEffect(EffectType.GetMoney, 0.5f, 0, 0, monster.transform.position);
                }

                topController.BoxCount += enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_ITEM);
                topController.Show();

                ShowTopData();

                deadList.Add(i);
                Logger.LogWarning("Enemy is dead: " + monster);
            }
        }

        // remove enemy from back end order.
        for (var i = deadList.Count - 1; i >= 0; --i)
        {
            // [FIXME]: disable buff bar controller.
            var character = MonsterController.CharacterList[deadList[i]];
            var buffController = character.BuffBarController;
            buffController.gameObject.SetActive(false);

            MonsterController.ReturnAt(deadList[i]);
        }
    }

    private IEnumerator RightAttackCoroutineHandler()
    {
        SetCharacterCanSelect(false);
        if (battleTeamRecord.RecordList.Count > 0)
        {
            foreach (BattleFightRecord record in battleTeamRecord.RecordList)
            {
                var monster = GetMonsterByAction(record.getAttackAction());
                if (monster == null) continue;
                var ec = monster.FaceObject.GetComponent<MonsterControl>();

                switch (record.getAttackAction().ActType)
                {
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK:

                        //CameraEffect.LookAt = enemy.transform;
                        //CameraEffect.LookAtTime = GameConfig.MoveCameraTime;
                        //CameraEffect.LookInto();

                        //yield return new WaitForSeconds(GameConfig.MoveCameraTime);
                        //EffectManager.PlayEffect(EffectType.EnemySprite, GameConfig.PlayMonsterEffectTime, 0, 0, enemy.transform.position);
                        //yield return new WaitForSeconds(GameConfig.PlayMonsterEffectTime);

                        //CameraEffect.LookOut();

                        //yield return new WaitForSeconds(GameConfig.MoveCameraTime);
                        // [FIXME] will be removed after apple commitment.
                        yield return new WaitForSeconds(ec.PlayAttack());
                        monster.PlayState(Character.State.Idle, true);
                        yield return StartCoroutine(PlayCharacterBeenAttrack(record.ActionList));
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER:
                        ec.SetHealth(record.getAttackAction().getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED:
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE:
                        break;
                    default:
                        yield return new WaitForSeconds(ec.PlayAttack());
                        monster.PlayState(Character.State.Idle, true);
                        yield return StartCoroutine(PlayCharacterBeenAttrack(record.ActionList));
                        break;
                }

                //yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime);
                ResetMonsterStates(ec, record.getAttackAction());
            }
        }

        // show character's debuff.
        ShowDebuff(TeamController.CharacterList);
        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);

        recordIndex++;
        DealWithRecord();
        SetCharacterCanSelect(true);

        Logger.LogWarning("Team controller selected to true");
        TeamController.Enable = true;
    }

    private void ShowDebuff(List<Character> characterList)
    {
        characterList.ForEach(character =>
        {
            if (character.gameObject.activeSelf)
            {
                character.ShowDebuff();
            }
        });
    }

    private void ResetMonsterStates(MonsterControl ec, SingleActionRecord monsterRecord)
    {
        ec.SetCdLabel(monsterRecord.getIntProp(BattleRecordConstants.BATTLE_MONSTER_SKILL_ROUND));
    }

    private Character GetCharacterByAction(SingleActionRecord action)
    {
        return GetObjectByAction(originalCharacterList, action.FighterInfo);
    }

    /// <summary>
    /// Get monster object according to single action record.
    /// </summary>
    /// <param name="action">Single action record</param>
    /// <returns>The monster character</returns>
    private Character GetMonsterByAction(SingleActionRecord action)
    {
        return GetObjectByAction(originalMonsterList, action.FighterInfo);
    }

    private Character GetObjectByAction(IList<Character> characterList, SingleFighterRecord action)
    {
        if (action.Index < 0 || action.Index >= characterList.Count)
        {
            Logger.LogError("[***************] Could not find character with index: " + action.Index + " in side: " + action.Side + ", character list count: " + characterList.Count);
            return null;
        }
        var monster = characterList[action.Index];
        // only find monster which is active that alive.
        return monster.gameObject.activeSelf ? monster : null;
    }

    private IEnumerator PlayCharacterBeenAttrack(List<SingleActionRecord> actionlist)
    {
        foreach (var action in actionlist)
        {
            var hero = GetCharacterByAction(action);
            if (hero != null)
            {
                EffectManager.PlayEffect(EffectType.Attrack, 0.8f, -20, -20, hero.transform.position);
                var cc = hero.FaceObject.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Hurt, false);
                CharacterLoseBlood(hero.transform.localPosition, action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
            }
        }

        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);
        foreach (var action in actionlist)
        {
            var hero = GetCharacterByAction(action);
            if (hero != null)
            {
                var cc = hero.FaceObject.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Idle, true);
            }
        }

        battleModeHandler.SetBattleField(TeamController, MonsterController, charactersLeft, FaceController, "UnderAttack");
    }

    private static void RunToAttackPlace(Character hero, Character enemy)
    {
        var enemyController = enemy.FaceObject.GetComponent<MonsterControl>();
        var duration = GameConfig.RunToAttrackPosTime;
        var increment = enemyController.AttackLocation.transform.position - hero.transform.position;
        iTween.MoveBy(hero.gameObject, increment, duration);
    }

    private IEnumerator PlayMonsterBeenAttack(IEnumerable<SingleActionRecord> actionList)
    {
        foreach (var action in actionList)
        {
            var monster = GetMonsterByAction(action);
            if (monster == null)
            {
                continue;
            }

            var hitCount = action.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_COUNT);
            var singleDamage = action.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_SINGLE_DAMAGE);
            StartCoroutine(MultPopText(monster, hitCount, singleDamage));

            EffectManager.PlayEffect(EffectType.Attrack, 0.8f, 0, -20, monster.transform.position);

            var enemyController = monster.FaceObject.GetComponent<MonsterControl>();
            if (action.prop.ContainsKey(BattleRecordConstants.SINGLE_ACTION_PROP_HP))
            {
                enemyController.SetHealth(action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
            }

            // play hurt state.
            monster.PlayState(Character.State.Hurt, false);
            yield return new WaitForSeconds(GameConfig.MonsterAttackStepTime);
            monster.PlayState(Character.State.Idle, true);
        }
    }

    private static IEnumerator MultPopText(Character obj, int count, int value)
    {
        var v = obj.transform.localPosition;
        for (var i = 0; i < count; i++)
        {
            PopTextManager.ShowText("-" + value, 0.6f, 0, 40, 120, v);
            yield return new WaitForSeconds(0.2f);
        }

    }

    private void RunReturn(Character character, float duration)
    {
        var cc = character.FaceObject.GetComponent<CharacterControl>();
        cc.PlayCharacter(Character.State.Idle, true);

        // Move to target.
        var increment = CharacterWaitingTrans.position - character.transform.position;
        iTween.MoveBy(character.gameObject, increment, duration);

        charactersLeft[character.Location.X, character.Location.Y] = null;
    }

    private void OnActiveLeaderSkill(LeaderData data)
    {
        battleModeHandler.SetBattleField(TeamController, MonsterController, charactersLeft, FaceController, "ActiveSkill");
        RequestRecords();
    }

    IEnumerator PlayLeaderEffect()
    {
        BattleModelLocator.Instance.CanSelectHero = false;
        //EffectManager.PlayAllEffect(false);
        //GameObject effectbg = EffectBg;
        //effectbg.SetActive(true);
        //var tt = effectbg.GetComponent<UITexture>();
        //tt.alpha = 0.9f;

        //GameObject effectobj = EffectObject;
        //tt = effectobj.GetComponent<UITexture>();
        //tt.mainTexture = (Texture2D)Resources.Load(EffectType.LeaderTextures[Random.Range(0, 11)], typeof(Texture2D));
        //effectobj.transform.localPosition = new Vector3(0, 0, 0);
        //effectobj.transform.localScale = new Vector3(5, 5, 1);
        //tt.alpha = 1.0f;
        //effectobj.SetActive(true);

        //PlayTweenScale(effectobj, 0.2f, new Vector3(5, 5, 1), new Vector3(1, 1, 1));

        //yield return new WaitForSeconds(0.2f);
        //TextBGObject.SetActive(true);
        //tt = TextBGObject.GetComponent<UITexture>();
        //tt.alpha = 1;

        //PlayTweenScale(effectobj, 1.0f, new Vector3(1, 1, 1), new Vector3(0.9f, 0.9f, 1));


        //TextObject.transform.localScale = new Vector3(5, 5, 1);
        //var lb = TextObject.GetComponent<UILabel>();
        //lb.text = BattleModelLocator.Instance.Skill.Name;
        //lb.alpha = 1;
        //TextObject.SetActive(true);

        //PlayTweenScale(TextObject, 0.2f, new Vector3(5, 5, 1), new Vector3(1, 1, 1));
        //yield return new WaitForSeconds(0.2f);

        //PlayTweenScale(TextObject, 0.8f, new Vector3(1, 1, 1), new Vector3(0.9f, 0.9f, 1));

        //yield return new WaitForSeconds(0.8f);

        //BreakObject.SetActive(true);
        //var tt1 = BreakObject.GetComponent<UITexture>();
        //BreakObject.transform.localScale = new Vector3(1, 1, 1);
        //tt1.alpha = 0.9f;

        //yield return new WaitForSeconds(.1f);
        //PlayTweenAlpha(effectbg, 0.3f, 0.9f, 0);

        //PlayTweenScale(effectobj, 0.3f, new Vector3(1, 1, 1), new Vector3(5, 5, 1));
        //PlayTweenAlpha(effectobj, 0.3f, 1, 0.1f);

        //PlayTweenAlpha(BreakObject, 0.3f, 1, 0);
        //PlayTweenScale(BreakObject, 0.3f, new Vector3(1, 1, 1), new Vector3(5, 5, 1));

        //PlayTweenAlpha(TextBGObject, 0.3f, 1, 0);

        //PlayTweenAlpha(TextObject, 0.2f, 1, 0);

        //yield return new WaitForSeconds(.4f);
        //effectobj.SetActive(false);
        //BreakObject.SetActive(false);
        //effectbg.SetActive(false);
        //TextBGObject.SetActive(false);

        var attack = leaderSkillRecord.OrCreateFightRecord.getAttackAction();
        leaderController.TotalLeaderCD = leaderSkillRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_MP);
        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);

        if (attack == null)
        {
            Logger.LogError("Attack action from leader skill record is null.");
        }
        else
        {
            if (attack.ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_CHANGE_COLOR)
            {
                foreach (var t in leaderSkillRecord.OrCreateFightRecord.ActionList)
                {
                    var hero = GetCharacterByAction(t);
                    if (hero == null)
                    {
                        continue;
                    }
                    var cc = hero.FaceObject.GetComponent<CharacterControl>();
                    var k =
                        t.getIntProp(
                            BattleRecordConstants.BATTLE_HERO_PROP_COLOR_CHANGE);
                    hero.ColorIndex = k;
                    cc.SetFootIndex();
                }
            }
            else if (attack.ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
            {
                PlayBloodFullEffect();
                var obj = GetCharacterByAction(attack);
                var k = attack.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP);
                CharacterLoseBlood(obj != null ? obj.transform.localPosition : new Vector3(0, 0, 0), k);
            }
        }

        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);
        EffectManager.PlayAllEffect(true);
        BattleModelLocator.Instance.CanSelectHero = true;

        recordIndex++;
        DealWithRecord();
        yield return null;
    }

    //播放sp攻击残影拖尾效果
    ArrayList moveCharacters;
    ArrayList moveGameObjects;
    private float AddMoveCharacter(Character hero, Character enemy)
    {
        if (moveCharacters == null)
        {
            moveCharacters = new ArrayList();
            moveGameObjects = new ArrayList();
        }
        var cc = hero.FaceObject.GetComponent<CharacterControl>();
        var vo = new CharacterMoveVO();
        vo.Init(cc.GetNamePrefix(), transform.localPosition, enemy.transform.localPosition);
        moveCharacters.Add(vo);
        return vo.Duration;
    }

    private void MoveCharacterUpdate()
    {
        if (moveGameObjects == null || moveCharacters == null) return;
        GameObject obj;
        for (int i = 0; i < moveGameObjects.Count; i++)
        {
            obj = moveGameObjects[i] as GameObject;
            var sp = obj.GetComponent<UISprite>();

            if (sp.alpha > 0.5f)
            {
                sp.alpha = 0.5f;
            }
            else if (sp.alpha > 0)
            {
                sp.alpha -= 0.02f;
            }
            else
            {
                moveGameObjects.RemoveAt(i);
                Destroy(obj);
                i--;
            }
        }

        for (var i = 0; i < moveCharacters.Count; i++)
        {
            var vo = moveCharacters[i] as CharacterMoveVO;
            var str = vo.ScriptName();
            if (str == "finish")
            {
                moveCharacters.RemoveAt(i);
                i--;
            }
            else if (str != "continue")
            {
                obj = NGUITools.AddChild(leftContainerObj, SpritePrefab);
                var sp = obj.GetComponent<UISprite>();
                sp.spriteName = str;
                sp.color = new Color(1.0f, 0, 0);
                var tp = obj.GetComponent<TweenPosition>();
                tp.duration = 0.001f;
                tp.from = tp.to = vo.GetCurrentPosition();
                tp.PlayForward();

                moveGameObjects.Add(obj);
            }
        }
    }

    private IEnumerator GotoNextScene()
    {
        BattleModelLocator.Instance.CanSelectHero = false;
        yield return StartCoroutine(MakeUpOneByOne());

        ResetBuffAll();

        if (BattleModelLocator.Instance.LevelManager.HasNextLevel())
        {
            BattleModelLocator.Instance.LevelManager.GoNextLevel();

            InitMonsterList();

            RunToNextMonsters();
            yield return new WaitForSeconds(GameConfig.RunRoNextMonstersTime);

            // special animation with last level hits in.
            if (BattleModelLocator.Instance.LevelManager.IsLastLevel())
            {
                WarningController.Play();
                yield return new WaitForSeconds(WarningController.Duration);
            }

            ShowTopData();
        }
        BattleModelLocator.Instance.CanSelectHero = true;

        //Store persistence file
        PersistenceHandler.Instance.StorePersistence();

        recordIndex++;
        DealWithRecord();
    }

    /// <summary>
    /// Show record of battle skills.
    /// </summary>
    /// <param name="battleSkillRecord">Battle skill record</param>
    public void showBattleSkillRecord(BattleSkillRecord battleSkillRecord)
    {
        Logger.Log("[-----RECORD-----] - battle skill record: " + battleSkillRecord);

        if (battleSkillRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_LEFT)
        {
            leaderSkillRecord = battleSkillRecord;

            StartCoroutine(PlayLeaderEffect());
        }
        else
        {
            recordIndex++;
            DealWithRecord();
        }
    }

    /// <summary>
    /// Show record of team fight one round.
    /// </summary>
    /// <param name="battleTeamFightRecord">Battle team fight record</param>
    /// <remarks>
    /// This record is the every first record of we can do an attack.
    /// So we put lock team selection function here.
    /// </remarks>
    public void showBattleTeamFightRecord(BattleTeamFightRecord battleTeamFightRecord)
    {
        Logger.Log("[-----RECORD-----] - battle team fight record: " + battleTeamFightRecord.RecordList.Count + ", " + battleTeamFightRecord);

        // get debuff value.
        var characterList = (battleTeamFightRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_LEFT)
            ? originalCharacterList
            : originalMonsterList;

        battleTeamFightRecord.BuffAction.ForEach(action =>
        {
            var character = GetObjectByAction(characterList, action.FighterInfo);
            character.BuffController.HurtValueList.Clear();
            character.BuffController.BaseValue = characterValue;
            character.BuffController.HurtValueList.Add(action.ResultHp);
        });

        battleTeamRecord = battleTeamFightRecord;
        if (battleTeamRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_LEFT)
        {
            Logger.LogWarning("Team controller selected to false");

            // Disable team selection.
            TeamController.Enable = false;
            // add selected characters to attack waiting list.
            attackWaitList.AddRange(TeamController.SelectedCharacterList);

            DoAttrackLeft();
        }
        else
        {
            DoAttrackRight();
        }
    }

    public void showBattleDebugRecord(BattleDebugRecord battleDebugRecord)
    {
        Logger.LogWarning("[-----RECORD-----] - Battle Debug Record: \n" + battleDebugRecord);

        for (var i = 0; i < battleDebugRecord.PointList.Count; ++i)
        {
            var left = battleDebugRecord.PointList[i];
            var right = TeamController.CharacterList[i];
            if (left.Color != right.ColorIndex)
            {
                throw new Exception("Color index is not correct. please double check and fix it. logic is: " +
                                    (FootColorType)left.Color + ", presentation is: " +
                                    (FootColorType)right.ColorIndex);
            }
        }

        ++recordIndex;
        DealWithRecord();
    }


    /// <summary>
    /// Show record of round counting.
    /// </summary>
    /// <param name="roundCountRecord">Round cout record</param>
    public void showBattleRoundCountRecord(BattleRoundCountRecord roundCountRecord)
    {
        Logger.Log("[-----RECORD-----] - Round count record: " + roundCountRecord.RecordList.Count);

        if (roundCountRecord.RecordList != null && roundCountRecord.RecordList.Count > 0)
        {
            for (int i = 0; i < roundCountRecord.RecordList.Count; i++)
            {
                var action = roundCountRecord.RecordList[i];

                if (action.SideIndex == BattleRecordConstants.TARGET_SIDE_LEFT)
                {
                }
                else
                {
                    var monster = GetMonsterByAction(action);
                    if (monster != null)
                    {
                        var ec = monster.FaceObject.GetComponent<MonsterControl>();
                        ResetMonsterStates(ec, action);
                    }
                }
            }
        }
        recordIndex++;
        DealWithRecord();
    }

    /// <summary>
    /// Show record of hero's color and index.
    /// </summary>
    /// <param name="battleIndexRecord">Battle index record</param>
    public void showBattleIndexRecord(BattleIndexRecord battleIndexRecord)
    {
        Logger.Log("[-----RECORD-----] - battle index record: " + battleIndexRecord);

        var battleModelLocator = BattleModelLocator.Instance;
        // first time battle set all colors according to fill point list, which contains everything.
        if (battleModelLocator.NextList == null)
        {
            battleModelLocator.NextList = battleIndexRecord.FillPointList;
            // fill in all character list colors.
            SetColors(TeamController.CharacterList);

            // init next foot manager's on stage color list.
            footManager.OnStageColorList =
                attackWaitList.Select(item => item.ColorIndex).ToList();
            footManager.Initialize();
        }
        else
        {
            battleModelLocator.NextList = battleIndexRecord.FillPointList;
            // Remove redurant colors that already taken last round.
            // New colors are only left for selected characters.
            // Server send client colors that including waiting list color (we known) plus selected list color (unknown).
            for (var i = 0; i < TeamController.Total - TeamController.VisibleCount; ++i)
            {
                battleModelLocator.NextList.RemoveAt(0);
            }
            // init next foot manager's waiting color list.
            footManager.WaitingColorList = battleModelLocator.NextList.Select(item => item.Color).ToList();
        }

        if (battleIndexRecord.prop.ContainsKey(BattleRecordConstants.BATTLE_HERO_TOTAL_HP))
        {
            characterValue = battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_HP);
            // init total hp value the first time.
            if (hpController.TotalValue <= characterValue)
            {
                hpController.TotalValue = characterValue;
            }
            hpController.ShowForgroundBar(characterValue);
        }
        if (battleIndexRecord.prop.ContainsKey(BattleRecordConstants.BATTLE_HERO_TOTAL_MP))
        {
            // init total mp value the first time.
            if (mpController.TotalValue <= 0)
            {
                mpController.TotalValue = battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_MP);
            }
            mpController.ShowForgroundBar(0);
        }

        recordIndex++;
        DealWithRecord();
    }

    public void showBattleBuffRecord(BattleBuffRecord battleBuffRecord)
    {
        Logger.Log("[-----RECORD-----] showBattleBuffRecord: " + battleBuffRecord);

        var characterList = (battleBuffRecord.SideIndex == BattleRecordConstants.TARGET_SIDE_LEFT)
            ? originalCharacterList
            : originalMonsterList;

        battleBuffRecord.RecordList.ForEach(record =>
        {
            var character = GetObjectByAction(characterList, record);
            if (character != null)
            {
                character.BuffController.Set(record.StateUpdateList);
                character.ShowBuff();
            }
        });

        recordIndex++;
        DealWithRecord();
    }

    private void ResetBuff(List<Character> characterList)
    {
        characterList.ForEach(character => character.ResetBuff());
    }

    private void ResetBuffAll()
    {
        var characterList = new List<Character>();
        characterList.AddRange(TeamController.CharacterList);
        characterList.AddRange(MonsterController.CharacterList);
        ResetBuff(characterList);
    }

    public void showBattleErrorRecord(BattleErrorRecord battleErrorRecord)
    {
        Logger.LogWarning("I got an error.");
    }

    public void showBattleTeamInfoRecord(BattleTeamInfoRecord battletTeamInfoRecord)
    {
        Logger.Log("[-----RECORD-----] showBattleTeamInfoRecord: " + battletTeamInfoRecord + ", count: " + battletTeamInfoRecord.RecordList.Count);

        if (battletTeamInfoRecord.Side == BattleRecordConstants.TARGET_SIDE_LEFT)
        {
            battletTeamInfoRecord.RecordList.ForEach(record =>
            {
                var hero = GetObjectByAction(originalCharacterList, record);
                var characterControll = hero.FaceObject.GetComponent<CharacterControl>();
                characterControll.SetAttackLabel(record);
            });
        }

        recordIndex++;
        DealWithRecord();
    }

    /// <summary>
    /// Show record of battle ending.
    /// </summary>
    /// <param name="battleEndRecord">Battle end record</param>
    public void showBattleEndRecord(BattleEndRecord battleEndRecord)
    {
        Logger.Log("[-----RECORD-----] showBattleEndRecord: " + battleEndRecord);

        switch (battleEndRecord.EndType)
        {
            case BattleRecordConstants.BATTLE_SCENE_END:
                if (characterValue > 0)
                {
                    StartCoroutine(GotoNextScene());
                }
                else
                {
                    recordIndex++;
                    DealWithRecord();
                }
                break;
            case BattleRecordConstants.BATTLE_ALL_END:
                {
                    if (BattleModelLocator.Instance.BattleType == BattleType.GREENHANDPVE.Index)
                    {
                        GreenhandController.Instance.SendEndMessage();

                        ResetAll();

                        WindowManager.Instance.Show<GreenHandGuideWindow>(false);
                        WindowManager.Instance.Show<UIMainScreenWindow>(true);

                        Debug.Log("Go to HeroFirstLoginGive.");
                        //FirstLoginGiveHero
                        if (ChooseCardHandler.IsHeroFirstLoginGive)
                        {
                            ChooseCardHandler.OnHeroFirstLoginGive();
                        }
                    }
                    else
                    {
                        PersistenceHandler.IsRaidFinish = false;

                        var k = battleEndRecord.getIntProp(BattleRecordConstants.BATTLE_END_WIN_SIDE);

                        var msg = new CSBattlePveFinishMsg
                        {
                            Uuid = BattleModelLocator.Instance.Uuid
                        };

                        if (k == BattleRecordConstants.TARGET_SIDE_LEFT)
                        {
                            msg.BattleResult = 1;
                            if (MissionModelLocator.Instance.RaidLoadingAll != null)
                            {
                                MissionModelLocator.Instance.AddStar(starController.CurrentStar);
                                MissionModelLocator.Instance.AddFinishTime(MissionModelLocator.Instance.BattleStageTemplate.Id);
                            }
                        }
                        else
                        {
                            PersistenceHandler.IsRaidFinish = true;
                            WindowManager.Instance.Show(typeof(BattleLostWindow), true);
                            msg.BattleResult = 0;
                        }

                        recordIndex++;
                        DealWithRecord();

                        MissionModelLocator.Instance.OldExp = PlayerModelLocator.Instance.Exp;
                        MissionModelLocator.Instance.OldLevel = PlayerModelLocator.Instance.Level;
                        MissionModelLocator.Instance.StarCount = starController.CurrentStar;

                        msg.Star = (sbyte)starController.CurrentStar;

                        //Battle persistence
                        PersistenceHandler.Instance.StoreBattleEndMessage(msg);

                        NetManager.SendMessage(msg);
                        MtaManager.TrackEndPage(MtaType.BattleScreen);

                        //Battle persistence:Check battle end succeed.
                        StartCoroutine(PersistenceHandler.Instance.CheckBattleEndSucceed(msg));
                    }
                }
                break;
            default:
                recordIndex++;
                DealWithRecord();
                break;
        }
    }

    public void ResetAll()
    {
        BattleModelLocator.Instance.NextList = null;
        MonsterController.OnSelectedChanged = null;

        ResetBattle();
        ResetBuffAll();

        BattleController.Cleanup();

        ResetCharacterList();
        TeamController.Cleanup();
        MonsterController.Cleanup();

        leaderController.LeaderList.ForEach(leader => leader.OnActiveLeaderSkill -= OnActiveLeaderSkill);
    }

    private void OnMonsterSelected(Character currentSelected, Character lastSelected, bool isStartScene = false)
    {
        Logger.LogWarning("On enemy select: sender, " + currentSelected.name + ", last selected: " + lastSelected.name);
        battleModeHandler.SetBattleField(TeamController, MonsterController, charactersLeft, FaceController, "MonsterSelect");
        var lastEnemy = lastSelected.FaceObject.GetComponent<MonsterControl>();
        var currentEnemy = currentSelected.FaceObject.GetComponent<MonsterControl>();
        if (!isStartScene)
        {
            //Switch aim if select the different monster.
            if (currentSelected != lastSelected)
            {
                lastEnemy.ShowAimTo(false);
                currentEnemy.ShowAimTo(true);
            }
            //Deactive aim if select the same monster, which isShowedAim.

            else
            {
                currentEnemy.ShowAimTo(!currentEnemy.IsShowAim);
            }
        }
        //Deactive aim if default select in start a new scene.
        else
        {
            lastEnemy.ShowAimTo(false);
            currentEnemy.ShowAimTo(false);
        }
    }

    private List<IBattleViewRecord> recordList;
    private int recordIndex;

    private void RequestRecords()
    {
        recordList = BattleModelLocator.Instance.MainBattle.Record.reportRecordListAndClear();
        recordIndex = 0;
        DealWithRecord();
    }

    private void DealWithRecord()
    {
        if (recordIndex >= recordList.Count)
        {
            return;
        }
        var record = recordList[recordIndex];
        record.show(this);
    }

    private void SetCharacterCanSelect(bool flag)
    {
        foreach (var characterControl in TeamController.CharacterList.Select(character => character.FaceObject.GetComponent<CharacterControl>()))
        {
            characterControl.SetCanSelect(flag);
        }
    }

    public void PersisitenceSet(Dictionary<string, string> value)
    {
        //Sync Hp,Mp
        characterValue = float.Parse(value["Hp"]);
        leaderController.TotalLeaderCD = int.Parse(value["Mp"]);
        hpController.ShowForgroundBar(characterValue);
        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);
        //Sync top data.
        ShowTopData();
        //Sync Topinfo
        topController.BoxCount = float.Parse(value["BoxCount"]);
        topController.GoldCount = float.Parse(value["GoldCount"]);
        topController.Show();
        //Sync Star
        starController.CurrentStar = int.Parse(value["StarCount"]);
    }

    public void PersistenceStore(Dictionary<string, string> persistenceInfo)
    {
        //hp,mp
        persistenceInfo.Add("Hp", characterValue.ToString());
        persistenceInfo.Add("Mp", leaderController.TotalLeaderCD.ToString());
        //topinfo
        persistenceInfo.Add("BoxCount", topController.BoxCount.ToString());
        persistenceInfo.Add("GoldCount", topController.GoldCount.ToString());
        //star
        persistenceInfo.Add("StarCount", starController.CurrentStar.ToString());
    }

    private void Awake()
    {
        footManager = FaceController.FootManager;
        hpController = FaceController.HPController;
        mpController = FaceController.MPController;
        topController = FaceController.TopController;
        stepController = FaceController.StepRecord;
        leaderController = FaceController.LeaderController;
        starController = FaceController.StarController;
    }
}