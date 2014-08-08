using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.input;
using com.kx.sglm.gs.hero.properties;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitBattleField : MonoBehaviour, IBattleView
{
    public GameObject CharacterPrefab;
    public GameObject EnemyPrefab;
    public GameObject DragBarPrefab;
    public GameObject LeaderPrefab;
    public GameObject SpriteStartPrefab;
    public GameObject SpritePrefab;
    public GameObject EffectBg;
    public GameObject EffectObject;

    public GameObject BattleBG;

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
    public TeamSimpleController EnemyController;


    /// <summary>
    /// Character's waiting transform.
    /// </summary>
    public Transform CharacterWaitingTrans;

    /// <summary>
    /// Battle face controller.
    /// </summary>
    public BattleFaceController FaceController;

    #region Battle Face Inner Controller

    /// <summary>
    /// Next foot manager to show next foot colors on the top left.
    /// </summary>
    private NextFootManager footManager;
    private MagicBarController hpController;
    private MagicBarController mpController;
    private TopDataController topController;
    private PopMenuController popController;
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
    private List<GameObject> attackWaitList;

    /// <summary>
    /// Character team list.
    /// </summary>
    private readonly GameObject[,] charactersLeft = new GameObject[3, 3];

    private readonly List<Character> originalCharacterList = new List<Character>();
    private readonly List<Character> originalEnemyList = new List<Character>();

    /// <summary>
    /// Current enemy group index.
    /// </summary>
    private int currentEnemyGroupIndex;

    private bool isRecover;
    private float realTime;

    private BattleSkillRecord leaderSkillRecord;
    private BattleTeamFightRecord battleTeamRecord;

    private GameObject tempGameObj;

    private static int bgIndex;

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
    }

    /// <summary>
    /// Start battle of the whole level.
    /// </summary>
    /// <remarks>Called once per map level.</remarks>
    public void StartBattle()
    {
        MtaManager.TrackBeginPage(MtaType.BattleScreen);
        leftContainerObj = GameObject.Find("BattleFieldWidgetLeft");

        // team controller initialization should be at the right beginning.
        // everything else is depend on this structure.
        InitTeamController();

        InitCharacterList();

        // [FIXME] enemy ground controller.
        currentEnemyGroupIndex = 0;

        EnemyController.OnSelectedChanged = OnEnemySelected;
        InitEnemyList();

        InitBattleFace();

        // [FIXME] belongs to battle background controller.
        var bg = BattleBG.GetComponent<BattleBGControl>();
        bgIndex++;
        bg.SetData("00" + bgIndex);
        bgIndex = bgIndex % 3;

        ShowTopData();

        // init character list & attack wait list from team selection controller.
        SyncCharacterList();

        RequestRecords();

        StartCoroutine(InitBattleFighters());
    }

    private void InitTeamController()
    {
        TeamController.Cleanup();
        TeamController.Total = BattleModelLocator.Instance.HeroList.Count;
        TeamController.Row = 3;
        TeamController.Col = 3;
        var characterList = GenerateCharacterList();
        TeamController.CharacterList.AddRange(characterList);
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

        stepController.TotalValue = BattleModelLocator.Instance.EnemyGroup.Count;
    }

    private void ShowTopData()
    {
        stepController.CurrentValue = (currentEnemyGroupIndex + 1);
        stepController.Show();
    }

    private IEnumerable<Character> GenerateCharacterList()
    {
        var characterPoolManager = CharacterPoolManager.Instance;
        var characterList = new List<Character>();
        BattleModelLocator.Instance.HeroList.ForEach(data =>
        {
            var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
            // [FIXME]: Currently we are not binding character id to character in pool.
            var index = (Random.Range(0, characterPoolManager.CharacterPoolList.Count));
            var character = characterPoolManager.CharacterPoolList[index].Take().GetComponent<Character>();
            Utils.AddChild(TeamController.gameObject, character.gameObject);
            character.Data = data;
            character.IDIndex = index;
            characterList.Add(character);
        });
        return characterList;
    }

    private void InitCharacterList()
    {
        for (var i = 0; i < TeamController.Total; i++)
        {
            var obj = TeamController.CharacterList[i];
            var cc = obj.GetComponent<CharacterControl>();
            cc.SetCharacter((i >= TeamController.VisibleCount) ? CharacterType.Friend : CharacterType.Hero);
            cc.SetSelect(false);
        }
    }

    IEnumerator InitBattleFighters()
    {
        yield return StartCoroutine(MakeUpOneByOne(false));
        RunToNextEnemys();
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void InitEnemyList()
    {
        var enemyGroup = BattleModelLocator.Instance.EnemyGroup;
        var enemyList = BattleModelLocator.Instance.EnemyList;

        Logger.Log("Enemy group count: " + enemyGroup.Count + ", current emeny group index: " + currentEnemyGroupIndex +
                   ", total enemy count: " + enemyList.Count);

        EnemyController.Cleanup();
        EnemyController.Total = enemyGroup[currentEnemyGroupIndex];
        EnemyController.Initialize();

        Logger.Log("Current level enemy's count: " + EnemyController.Total);

        for (var i = 0; i < EnemyController.CharacterList.Count; ++i)
        {
            var enemyController = EnemyController.CharacterList[i].gameObject.GetComponent<EnemyControl>();
            var enemyData = enemyList[BattleModelLocator.Instance.MonsterIndex + i];
            var maxValue = enemyData.BattleProperty[RoleAProperty.HP];
            enemyController.Reset();
            enemyController.SetBloodBar(maxValue, maxValue);
            enemyController.CharacterData.Data = enemyData;

            Logger.Log("Init enemy of index: " + (BattleModelLocator.Instance.MonsterIndex + i));
        }

        // set default current select to index of 0.
        EnemyController.SetDefaultCurrentSelect();

        originalEnemyList.Clear();
        originalEnemyList.AddRange(EnemyController.CharacterList);

        BattleModelLocator.Instance.MonsterIndex += EnemyController.Total;
        Logger.Log("Next monster index: " + BattleModelLocator.Instance.MonsterIndex);
    }

    private void SyncCharacterList()
    {
        if (attackWaitList == null)
        {
            attackWaitList = new List<GameObject>();
        }

        attackWaitList.Clear();
        for (var i = 0; i < TeamController.Total; ++i)
        {
            var character = TeamController.CharacterList[i];
            if (i >= TeamController.VisibleCount)
            {
                attackWaitList.Add(character.gameObject);
            }
            else
            {
                charactersLeft[i / TeamController.Row, i % TeamController.Col] = character.gameObject;
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
                var character = charactersLeft[i, j].GetComponent<Character>();
                character.Location = new Position { X = i, Y = j };
                TeamController.CharacterList.Add(character);
            }
        }
        foreach (var character in attackWaitList.Select(wait => wait.GetComponent<Character>()))
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

    public void ResetBattle()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                charactersLeft[i, j] = null;
            }
        }

        while (attackWaitList.Count > 0)
        {
            attackWaitList.RemoveAt(0);
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
                            GameObject obj = charactersLeft[i, j];
                            cc = obj.GetComponent<CharacterControl>();
                            cc.PlayCharacter(Character.State.Run, true);
                            duration = (k - i) * runStepTime;
                            cc.SetCharacterAfter(duration);

                            // Move to target.
                            var target = charactersLeft[k, j];
                            var targetPosition =
                                TeamController.FormationController.LatestPositionList[
                                    TeamController.TwoDimensionToOne(i, j)];
                            iTween.MoveTo(target, targetPosition, duration);

                            charactersLeft[k, j] = null;
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        charactersLeft[i, j] = attackWaitList[0];
                        attackWaitList.RemoveAt(0);

                        charactersLeft[i, j].SetActive(true);
                        cc = charactersLeft[i, j].GetComponent<CharacterControl>();

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
                        iTween.MoveTo(target, targetPosition, duration);
                    }
                    yield return new WaitForSeconds(runWaitTime);
                }
            }
        }

        SyncTeamController();

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
            var cc = characterList[i].GetComponent<CharacterControl>();
            cc.SetFootIndex(nextColorList[i].Color);

            var character = characterList[i];
            character.ColorIndex = colorIndex;
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

    private void OnSelected(GameObject selectedObject)
    {
        var characterControll = selectedObject.GetComponent<CharacterControl>();
        currentFootIndex = characterControll.FootIndex;
        AddAObj(characterControll);
    }

    private void OnDeselected(GameObject selectedObject)
    {
        var characterControll = selectedObject.GetComponent<CharacterControl>();
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

        TeamController.SelectedCharacterList.ForEach(item =>
        {
            var character = item.GetComponent<CharacterControl>();
            character.SetSelect(false);
        });

        if (isAttacked)
        {
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
        if (EnemyController.CharacterList.Count <= 0)
        {
            Logger.Log("Enemy list is empty. Please check it out.");
            return;
        }

        var enemy = EnemyController.CurrentSelect;
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
        iTweenEvent.GetEvent(character.gameObject, "ShakeTween").Play();
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

    void RunToNextEnemys()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Run, true);
                cc.SetCharacterAfter(GameConfig.RunRoNextMonstersTime);
            }
        }

        var bg = BattleBG.GetComponent<BattleBGControl>();
        bg.MoveToNext();

        // [FIXME] This place move enemy when next page.
        foreach (var t in EnemyController.CharacterList)
        {
            var obj = t.gameObject;
            var temptp = obj.AddComponent<TweenPosition>();
            temptp.duration = GameConfig.RunRoNextMonstersTime;
            temptp.@from = new Vector3(obj.transform.localPosition.x + 640, obj.transform.localPosition.y, 0);
            temptp.to = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
            temptp.PlayForward();
        }
    }

    private IEnumerator PlayOneAction(BattleFightRecord record)
    {
        var obj = GetCharacterByAction(record.getAttackAction());
        var cc = obj.GetComponent<CharacterControl>();

        GameObject monster;
        switch (record.getAttackAction().ActType)
        {
            case BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK:
                {
                    monster = GetEnemyByAction(record.ActionList[0]);
                    RunReturn(obj, GameConfig.ShortTime);
                    // [FIXME]: Need new SP effect.
                    yield return new WaitForSeconds(AddMoveCharacter(obj, monster));
                    PlayEnemyBeenAttrack(record.ActionList);
                }
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER:
                isRecover = true;
                cc.PlayCharacter(Character.State.Attack, false);
                yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                cc.PlayCharacter(Character.State.Idle, true);
                CharacterLoseBlood(obj.transform.localPosition,
                    record.getAttackAction().getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED:
                break;
            case BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE:
                break;
            default:
                {
                    monster = GetEnemyByAction(record.ActionList[0]);
                    RunToAttackPlace(obj, monster);
                    yield return new WaitForSeconds(GameConfig.RunToAttrackPosTime);
                    cc.PlayCharacter(Character.State.Attack, false);
                    yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                    PlayEnemyBeenAttrack(record.ActionList);
                    RunReturn(obj, GameConfig.HeroRunReturnTime);
                }
                break;
        }
    }

    private IEnumerator LeftAttackCoroutineHandler()
    {
        if (battleTeamRecord.SkillFighter != null && battleTeamRecord.SkillFighter.Count > 0)
        {
            PlaySpEffect(battleTeamRecord.SkillFighter);
            yield return new WaitForSeconds(0.3f * battleTeamRecord.SkillFighter.Count + 0.5f);
        }

        BattleFightRecord record;
        GameObject obj;
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
            yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime);
            record = battleTeamRecord.RecordList[battleTeamRecord.RecordList.Count - 1];
            var monster = GetEnemyByAction(record.ActionList[0]);
            var ec = monster.GetComponent<EnemyControl>();
            obj = TeamController.SelectedCharacterList[TeamController.SelectedCharacterList.Count - 1].gameObject;
            var cc = obj.GetComponent<CharacterControl>();

            CameraEffect.LookAt = obj.transform;
            CameraEffect.LookAtTime = GameConfig.MoveCameraTime;
            CameraEffect.LookInto();

            cc.CharacterData.StopState();
            yield return new WaitForSeconds(GameConfig.MoveCameraTime);//显示遮罩
            Picture91.SetActive(true);

            //播放角色特效
            EffectManager.PlayEffect(EffectType.NineAttrack, GameConfig.Attrack9PlayEffectTime, -25, 0, obj.transform.position);
            yield return new WaitForSeconds(GameConfig.Attrack9PlayEffectTime);
            //人物大图飞入
            var tt = EffectObject.GetComponent<UITexture>();
            tt.mainTexture = (Texture2D)Resources.Load(EffectType.LeaderTextures[Random.Range(0, 11)], typeof(Texture2D));
            tt.alpha = 1;
            EffectObject.transform.localPosition = new Vector3(Screen.width / 2 + 300, -80, 0);
            EffectObject.transform.localScale = new Vector3(1, 1, 1);
            EffectObject.SetActive(true);

            PlayTweenPosition(EffectObject, GameConfig.Attrack9HeroInTime, new Vector3(Screen.width / 2 + 300, -80, 0), new Vector3(0, -80, 0));
            yield return new WaitForSeconds(GameConfig.Attrack9HeroInTime);
            //显示文字背景
            PlayTweenPosition(EffectObject, 1.2f, new Vector3(0, -80, 0), new Vector3(-25, -80, 0));
            TextBG91.SetActive(true);
            tt = TextBG91.GetComponent<UITexture>();
            tt.alpha = 1;
            yield return new WaitForSeconds(0.1f);
            //文字出现
            Text91.SetActive(true);
            Text91.transform.localScale = new Vector3(5, 5, 1);
            PlayTweenScale(Text91, GameConfig.Attrack9TextInTime, new Vector3(5, 5, 1), new Vector3(1, 1, 1));
            UILabel lb = Text91.GetComponent<UILabel>();
            lb.alpha = 1;
            yield return new WaitForSeconds(GameConfig.Attrack9TextShowTime);
            //文字消失
            PlayTweenScale(Text91, GameConfig.Attrack9TextFadeTime, new Vector3(1, 1, 1), new Vector3(5, 5, 1));
            PlayTweenAlpha(Text91, GameConfig.Attrack9TextFadeTime, 1, 0);
            PlayTweenAlpha(TextBG91, GameConfig.Attrack9TextFadeTime, 1, 0);
            yield return new WaitForSeconds(GameConfig.Attrack9TextFadeTime / 2);
            //遮罩和大图消失
            Picture91.SetActive(false);
            PlayTweenAlpha(EffectObject, GameConfig.Attrack9HeroFadeTime, 1, 0);
            PlayTweenScale(EffectObject, GameConfig.Attrack9HeroFadeTime, new Vector3(1, 1, 1), new Vector3(5, 5, 1));
            yield return new WaitForSeconds(GameConfig.Attrack9HeroFadeTime);
            //镜头拉回
            Text91.SetActive(false);
            TextBG91.SetActive(false);

            yield return new WaitForSeconds(GameConfig.MoveCameraTime);

            CameraEffect.LookOut();

            //攻击动作
            cc.CharacterData.PlayState(Character.State.Attack, false);

            RunToAttackPlace(obj, monster);
            yield return new WaitForSeconds(GameConfig.RunToAttrackPosTime);

            cc.PlayCharacter(Character.State.Attack, false);
            yield return new WaitForSeconds(GameConfig.PlayAttrackTime);

            //9连击播放刀光
            TexSwardBg91.SetActive(true);
            cc.CharacterData.StopState();

            var offset = 0.5f;
            //Vector3 pos = enemy.transform.position;
            var pos = new Vector3(0, 0, monster.transform.position.z);
            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayShake();
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayShake();
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            offset = 0.3f;
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayShake();
            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9TotalSwardTime);
            TexSwardBg91.SetActive(false);
            PlayEnemyBeenAttrack(record.ActionList, true);

            yield return new WaitForSeconds(.4f);

            cc.CharacterData.PlayState(Character.State.Attack, false);
            RunReturn(obj, GameConfig.HeroRunReturnTime);
        }

        if (isRecover)
        {
            yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
            PlayBloodFullEffect();
            yield return new WaitForSeconds(GameConfig.PlayRecoverEffectTime);
            if (battleTeamRecord.RecordList.Count > 0)
            {
                for (var i = 0; i < battleTeamRecord.RecordList.Count; i++)
                {

                    record = battleTeamRecord.RecordList[i];
                    obj = GetCharacterByAction(record.getAttackAction());
                    if (obj != null) RunReturn(obj, GameConfig.HeroRunReturnTime);

                }
            }
        }
        var temp = (battleTeamRecord.SkillFighter != null && battleTeamRecord.SkillFighter.Count > 0) ? 0.3f : 0;
        yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime + temp);
        CheckEnemyDead();

        // Set colors to selected character list, which are appended to attack waiting list already.
        // make color here is because those characters are invisible for now.
        SetColors(TeamController.SelectedCharacterList);

        // Move foot manager one round.
        footManager.Move();

        yield return StartCoroutine(MakeUpOneByOne());

        // show enemy's debuff.
        ShowDebuff(EnemyController.CharacterList);
        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);

        leaderController.TotalLeaderCD = battleTeamRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_MP);
        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);

        ShowTopData();

        recordIndex++;
        DealWithRecord();
    }

    private void CheckEnemyDead()
    {
        var enemyList = BattleModelLocator.Instance.EnemyList;
        var enemyIndexBase = (currentEnemyGroupIndex == 0) ? 0 : BattleModelLocator.Instance.EnemyGroup[currentEnemyGroupIndex - 1];
        var deadList = new List<int>();
        for (var i = 0; i < EnemyController.CharacterList.Count; ++i)
        {
            var character = EnemyController.CharacterList[i];
            var enemyController = character.GetComponent<EnemyControl>();
            if (enemyController.Health <= 0)
            {
                var enemyData = enemyList[enemyIndexBase + character.Index];
                var v = enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_COIN);
                if (v > 0)
                {
                    topController.GoldCount += v;
                    EffectManager.PlayEffect(EffectType.GetMoney, 0.5f, 0, 0, character.transform.position);
                }

                popController.EnergyCount += enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_HERO);
                popController.FPCount += enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_SPRIT);

                topController.BoxCount += enemyData.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_ITEM);
                topController.Show();

                ShowTopData();

                deadList.Add(i);
                Logger.LogWarning("Enemy is dead: " + character);
            }
        }

        // remove enemy from back end order.
        for (var i = deadList.Count - 1; i >= 0; --i)
        {
            // [FIXME]: disable buff bar controller.
            var character = EnemyController.CharacterList[deadList[i]];
            var buffController = character.GetComponent<EnemyControl>().BuffController;
            buffController.gameObject.SetActive(false);

            EnemyController.ReturnAt(deadList[i]);
        }
    }

    private IEnumerator RightAttackCoroutineHandler()
    {
        SetCharacterCanSelect(false);
        if (battleTeamRecord.RecordList.Count > 0)
        {
            foreach (BattleFightRecord record in battleTeamRecord.RecordList)
            {
                var enemy = GetEnemyByAction(record.getAttackAction());
                if (enemy == null) continue;
                var ec = enemy.GetComponent<EnemyControl>();

                switch (record.getAttackAction().ActType)
                {
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK:

                        CameraEffect.LookAt = enemy.transform;
                        CameraEffect.LookAtTime = GameConfig.MoveCameraTime;
                        CameraEffect.LookInto();

                        yield return new WaitForSeconds(GameConfig.MoveCameraTime);
                        EffectManager.PlayEffect(EffectType.EnemySprite, GameConfig.PlayMonsterEffectTime, 0, 0, enemy.transform.position);
                        yield return new WaitForSeconds(GameConfig.PlayMonsterEffectTime);

                        CameraEffect.LookOut();

                        yield return new WaitForSeconds(GameConfig.MoveCameraTime);
                        yield return StartCoroutine(PlayCharacterBeenAttrack(record.ActionList));
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER:
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED:
                        break;
                    case BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE:
                        break;
                    default:
                        yield return new WaitForSeconds(ec.PlayAttrack());
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

    private void ResetMonsterStates(EnemyControl ec, SingleActionRecord monsterRecord)
    {
        ec.SetCdLabel(monsterRecord.getIntProp(BattleRecordConstants.BATTLE_MONSTER_SKILL_ROUND));
    }

    private GameObject GetCharacterByAction(SingleActionRecord action)
    {
        return GetObjectByAction(originalCharacterList, action.FighterInfo);
    }

    /// <summary>
    /// Get monster object according to single action record.
    /// </summary>
    /// <param name="action">Single action record</param>
    /// <returns>The monster</returns>
    private GameObject GetEnemyByAction(SingleActionRecord action)
    {
        return GetObjectByAction(originalEnemyList, action.FighterInfo);
    }

    private GameObject GetObjectByAction(List<Character> characterList, SingleFighterRecord action)
    {
        if (action.Index < 0 || action.Index >= characterList.Count)
        {
            Logger.LogError("[***************] Could not find character with index: " + action.Index + " in side: " + action.Side + ", character list count: " + characterList.Count);
            return null;
        }
        return characterList[action.Index].gameObject;
    }

    private IEnumerator PlayCharacterBeenAttrack(List<SingleActionRecord> actionlist)
    {
        foreach (var action in actionlist)
        {
            var obj = GetCharacterByAction(action);
            if (obj != null)
            {
                EffectManager.PlayEffect(EffectType.Attrack, 0.8f, -20, -20, obj.transform.position);
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Hurt, false);
                CharacterLoseBlood(obj.transform.localPosition, action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                iTweenEvent.GetEvent(obj, "ShakeTween").Play();
            }
        }

        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);
        foreach (var action in actionlist)
        {
            var obj = GetCharacterByAction(action);
            if (obj != null)
            {
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(Character.State.Idle, true);
            }
        }
    }

    private static void RunToAttackPlace(GameObject obj, GameObject enemy)
    {
        var enemyController = enemy.GetComponent<EnemyControl>();
        var duration = GameConfig.RunToAttrackPosTime;
        var increment = enemyController.AttackLocation.transform.position - obj.transform.position;
        iTween.MoveBy(obj, increment, duration);
    }

    private void PlayEnemyBeenAttrack(IEnumerable<SingleActionRecord> actionList, bool showbig = false)
    {
        foreach (var action in actionList)
        {
            var enemy = GetEnemyByAction(action);
            if (enemy == null)
            {
                continue;
            }

            var k = action.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_COUNT);
            var v = action.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_HIT_SINGLE_DAMAGE);
            StartCoroutine(MultPopText(enemy, k, v));
            if (showbig || k > 1)
            {
                EffectManager.PlayEffect(EffectType.SpriteCollection, 0.8f, 0, -20, enemy.transform.position);
            }
            else
            {
                EffectManager.PlayEffect(EffectType.Attrack, 0.8f, 0, -20, enemy.transform.position);
            }

            var enemyController = enemy.GetComponent<EnemyControl>();

            if (showbig)
            {
                CameraEffect.Shake();
            }
            else if (k > 1)
            {
                enemyController.PlayBigShake();
            }
            else
            {
                enemyController.PlayShake();
            }
            if (action.prop.ContainsKey(BattleRecordConstants.SINGLE_ACTION_PROP_HP))
            {
                enemyController.SetHealth(action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
            }
        }
    }

    private static IEnumerator MultPopText(GameObject obj, int count, int value)
    {
        var v = obj.transform.localPosition;
        for (var i = 0; i < count; i++)
        {
            PopTextManager.ShowText("-" + value, 0.6f, 0, 40, 120, v);
            yield return new WaitForSeconds(0.2f);
        }

    }

    private void RunReturn(GameObject obj, float duration)
    {
        var cc = obj.GetComponent<CharacterControl>();
        cc.PlayCharacter(Character.State.Idle, true);

        // Move to target.
        var increment = CharacterWaitingTrans.position - obj.transform.position;
        iTween.MoveBy(obj, increment, duration);

        var character = obj.GetComponent<Character>();
        charactersLeft[character.Location.X, character.Location.Y] = null;
    }

    private void OnActiveLeaderSkill(LeaderData data)
    {
        RequestRecords();
    }

    IEnumerator PlayLeaderEffect()
    {
        BattleModelLocator.Instance.CanSelectHero = false;
        EffectManager.PlayAllEffect(false);
        GameObject effectbg = EffectBg;
        effectbg.SetActive(true);
        var tt = effectbg.GetComponent<UITexture>();
        tt.alpha = 0.9f;

        GameObject effectobj = EffectObject;
        tt = effectobj.GetComponent<UITexture>();
        tt.mainTexture = (Texture2D)Resources.Load(EffectType.LeaderTextures[Random.Range(0, 11)], typeof(Texture2D));
        effectobj.transform.localPosition = new Vector3(0, 0, 0);
        effectobj.transform.localScale = new Vector3(5, 5, 1);
        tt.alpha = 1.0f;
        effectobj.SetActive(true);

        PlayTweenScale(effectobj, 0.2f, new Vector3(5, 5, 1), new Vector3(1, 1, 1));

        yield return new WaitForSeconds(0.2f);
        TextBGObject.SetActive(true);
        tt = TextBGObject.GetComponent<UITexture>();
        tt.alpha = 1;

        PlayTweenScale(effectobj, 1.0f, new Vector3(1, 1, 1), new Vector3(0.9f, 0.9f, 1));


        TextObject.transform.localScale = new Vector3(5, 5, 1);
        var lb = TextObject.GetComponent<UILabel>();
        lb.text = BattleModelLocator.Instance.Skill.BaseTmpl.Name;
        lb.alpha = 1;
        TextObject.SetActive(true);

        PlayTweenScale(TextObject, 0.2f, new Vector3(5, 5, 1), new Vector3(1, 1, 1));
        yield return new WaitForSeconds(0.2f);

        PlayTweenScale(TextObject, 0.8f, new Vector3(1, 1, 1), new Vector3(0.9f, 0.9f, 1));

        yield return new WaitForSeconds(0.8f);

        BreakObject.SetActive(true);
        var tt1 = BreakObject.GetComponent<UITexture>();
        BreakObject.transform.localScale = new Vector3(1, 1, 1);
        tt1.alpha = 0.9f;

        yield return new WaitForSeconds(.1f);
        PlayTweenAlpha(effectbg, 0.3f, 0.9f, 0);

        PlayTweenScale(effectobj, 0.3f, new Vector3(1, 1, 1), new Vector3(5, 5, 1));
        PlayTweenAlpha(effectobj, 0.3f, 1, 0.1f);

        PlayTweenAlpha(BreakObject, 0.3f, 1, 0);
        PlayTweenScale(BreakObject, 0.3f, new Vector3(1, 1, 1), new Vector3(5, 5, 1));

        PlayTweenAlpha(TextBGObject, 0.3f, 1, 0);

        PlayTweenAlpha(TextObject, 0.2f, 1, 0);

        yield return new WaitForSeconds(.4f);
        effectobj.SetActive(false);
        BreakObject.SetActive(false);
        effectbg.SetActive(false);
        TextBGObject.SetActive(false);

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
                    var obj = GetCharacterByAction(t);
                    if (obj == null)
                    {
                        continue;
                    }
                    var cc = obj.GetComponent<CharacterControl>();
                    var k =
                        t.getIntProp(
                            BattleRecordConstants.BATTLE_HERO_PROP_COLOR_CHANGE);
                    cc.SetFootIndex(k);

                    var character = cc.GetComponent<Character>();
                    character.ColorIndex = k;
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
    }

    //播放sp技能特效
    private ArrayList spStartList;
    private void PlaySpEffect(List<int> theList)
    {
        var thecount = (theList.Count > 4) ? 4 : theList.Count;

        if (spStartList == null)
        {
            spStartList = new ArrayList();
        }

        var cantiner = GameObject.Find("Anchor-left");

        var basey = thecount * 160 / 2 - 300;
        const int offsety = -160;
        float delay = 0;
        for (var i = 0; i < thecount; i++)
        {
            GameObject obj = NGUITools.AddChild(cantiner, SpriteStartPrefab);
            obj.transform.localPosition = new Vector3(180, basey + i * offsety + 224, 0);
            SpStartControl sc = obj.GetComponent<SpStartControl>();
            sc.Play(delay);
            delay += 0.5f;
        }
    }

    //播放sp攻击残影拖尾效果
    ArrayList moveCharacters;
    ArrayList moveGameObjects;
    private float AddMoveCharacter(GameObject obj, GameObject enemy)
    {
        if (moveCharacters == null)
        {
            moveCharacters = new ArrayList();
            moveGameObjects = new ArrayList();
        }
        var cc = obj.GetComponent<CharacterControl>();
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

    //播放动画
    void PlayTweenAlpha(GameObject obj, float playtime, float from, float to)
    {
        TweenAlpha ta = obj.AddComponent<TweenAlpha>();
        ta.from = from;
        ta.to = to;
        ta.duration = playtime;
        ta.PlayForward();
        Destroy(ta, playtime);
    }

    void PlayTweenScale(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenScale ts = obj.AddComponent<TweenScale>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }

    void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenPosition ts = obj.AddComponent<TweenPosition>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }

    private IEnumerator GotoNextScene()
    {
        BattleModelLocator.Instance.CanSelectHero = false;
        yield return StartCoroutine(MakeUpOneByOne());

        ResetBuffAll();

        currentEnemyGroupIndex++;
        if (currentEnemyGroupIndex < BattleModelLocator.Instance.EnemyGroup.Count)
        {
            InitEnemyList();

            RunToNextEnemys();
            yield return new WaitForSeconds(GameConfig.RunRoNextMonstersTime);
            if (currentEnemyGroupIndex == BattleModelLocator.Instance.EnemyGroup.Count - 1)
            {
                WarningController.Play();
                yield return new WaitForSeconds(WarningController.Duration);
            }
            ShowTopData();
        }
        BattleModelLocator.Instance.CanSelectHero = true;

#if !UNITY_IPHONE

        // Store info for persistence fight.

        Dictionary<string,float> persistenceInfo = new Dictionary<string, float>();
        persistenceInfo.Clear();
        //TopData
        persistenceInfo.Add("TopData",currentEnemyGroupIndex);
        //hp,mp
        persistenceInfo.Add("Hp",characterValue);
        persistenceInfo.Add("Mp",leaderController.TotalLeaderCD);
        //enemy list
        persistenceInfo.Add("EnemyModelIndex",BattleModelLocator.Instance.MonsterIndex - EnemyController.Total);
        //topinfo
        persistenceInfo.Add("BoxCount",topController.BoxCount);
        persistenceInfo.Add("GoldCount",topController.GoldCount);
        //popinfo
        persistenceInfo.Add("EnergyCount",popController.EnergyCount);
        persistenceInfo.Add("FPCount",popController.FPCount);
        //star
        persistenceInfo.Add("StarCount",starController.CurrentStar);

        BattleWindow.StorePersistence(persistenceInfo);

#endif

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

        if (battleSkillRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_A)
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
        var characterList = (battleTeamFightRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_A)
            ? originalCharacterList
            : originalEnemyList;

        Debug.LogWarning("Character count: ----------------------- " + characterList.Count);

        battleTeamFightRecord.BuffAction.ForEach(action =>
        {
            var characterObject = GetObjectByAction(characterList, action.FighterInfo);
            var character = characterObject.GetComponent<Character>();
            character.BuffController.HurtValueList.Clear();
            character.BuffController.BaseValue = characterValue;
            character.BuffController.HurtValueList.Add(action.ResultHp);
        });

        battleTeamRecord = battleTeamFightRecord;
        if (battleTeamRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_A)
        {
            Logger.LogWarning("Team controller selected to false");

            // Disable team selection.
            TeamController.Enable = false;
            // add selected characters to attack waiting list.
            attackWaitList.AddRange(TeamController.SelectedCharacterList.Select(item => item.gameObject));

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

                if (action.SideIndex == BattleRecordConstants.TARGET_SIDE_A)
                {
                }
                else
                {
                    var monster = GetEnemyByAction(action);
                    if (monster != null)
                    {
                        var ec = monster.GetComponent<EnemyControl>();
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
                attackWaitList.Select(item => item.GetComponent<Character>().ColorIndex).ToList();
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

        var characterList = (battleBuffRecord.SideIndex == BattleRecordConstants.TARGET_SIDE_A)
            ? originalCharacterList
            : originalEnemyList;

        Debug.LogWarning("Character count: ----------------------- " + characterList.Count);

        battleBuffRecord.RecordList.ForEach(record =>
        {
            var characterObject = GetObjectByAction(characterList, record);
            if (characterObject != null)
            {
                var character = characterObject.GetComponent<Character>();
                character.BuffController.Set(record.StateUpdateList);
                ShowBuff(character);
            }
        });

        recordIndex++;
        DealWithRecord();
    }

    private void ResetBuff(List<Character> characterList)
    {
        characterList.ForEach(ResetBuff);
    }

    private void ResetBuffAll()
    {
        var characterList = new List<Character>();
        characterList.AddRange(TeamController.CharacterList);
        characterList.AddRange(EnemyController.CharacterList);
        ResetBuff(characterList);
    }

    private static void ShowBuff(Character character)
    {
        var buffController = (character.GetComponent<CharacterControl>() != null)
            ? character.GetComponent<CharacterControl>().BuffController
            : character.GetComponent<EnemyControl>().BuffController;
        character.ShowBuff();
        character.ShowBuffCD(buffController);
    }

    private static void ResetBuff(Character character)
    {
        var buffController = (character.GetComponent<CharacterControl>() != null)
            ? character.GetComponent<CharacterControl>().BuffController
            : character.GetComponent<EnemyControl>().BuffController;
        buffController.gameObject.SetActive(false);
        character.ResetBuff();
    }

    public void showBattleErrorRecord(BattleErrorRecord battleErrorRecord)
    {
        Logger.LogWarning("I got an error.");
    }

    public void showBattleTeamInfoRecord(BattleTeamInfoRecord battletTeamInfoRecord)
    {
        Logger.Log("[-----RECORD-----] showBattleTeamInfoRecord: " + battletTeamInfoRecord + ", count: " + battletTeamInfoRecord.RecordList.Count);

        if (battletTeamInfoRecord.Side == BattleRecordConstants.TARGET_SIDE_A)
        {
            battletTeamInfoRecord.RecordList.ForEach(record =>
            {
                var characterObject = GetObjectByAction(originalCharacterList, record);
                var characterControll = characterObject.GetComponent<CharacterControl>();
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
                    //BattleWindow.StorePersisitence(this);
                }
                else
                {
                    recordIndex++;
                    DealWithRecord();
                }
                break;
            case BattleRecordConstants.BATTLE_ALL_END:
                {
                    var k = battleEndRecord.getIntProp(BattleRecordConstants.BATTLE_END_WIN_SIDE);

                    var msg = new CSBattlePveFinishMsg
                    {
                        Uuid = BattleModelLocator.Instance.Uuid
                    };

                    if (k == BattleRecordConstants.TARGET_SIDE_A)
                    {
                        msg.BattleResult = 1;
                    }
                    else
                    {
                        WindowManager.Instance.Show(typeof(BattleLostWindow), true);
                        msg.BattleResult = 0;
                    }

                    recordIndex++;
                    DealWithRecord();

                    // reset everything here.
                    ResetAll();

                    MissionModelLocator.Instance.AddStar(starController.CurrentStar);
                    MissionModelLocator.Instance.OldExp = PlayerModelLocator.Instance.Exp;
                    MissionModelLocator.Instance.OldLevel = PlayerModelLocator.Instance.Level;
                    MissionModelLocator.Instance.AddFinishTime(MissionModelLocator.Instance.SelectedStageId);
                    msg.Star = (sbyte)starController.CurrentStar;

#if !UNITY_IPHONE

                    //Store BattleEndMessage and modified modelmissionlocator.
                    Dictionary<string,float> value=new Dictionary<string, float>();
                    value.Add("Uuid", msg.Uuid);
                    value.Add("BattleResult", msg.BattleResult);
                    value.Add("Star",msg.Star);
                    BattleWindow.StoreBattleEndMessage(value);
                    BattleWindow.StoreMissionModelLocator(MissionModelLocator.Instance);

#endif

                    BattleWindow.isRaidReward = false;

                    NetManager.SendMessage(msg);
                    MtaManager.TrackEndPage(MtaType.BattleScreen);

                    //Check battle end succeed.
                    StartCoroutine(BattleWindow.MakeBattleEndSucceed(msg));
                }
                break;
            default:
                recordIndex++;
                DealWithRecord();
                break;
        }
    }




    private void ResetAll()
    {
        BattleModelLocator.Instance.NextList = null;
        EnemyController.OnSelectedChanged = null;

        ResetBuffAll();

        leaderController.LeaderList.ForEach(leader => leader.OnActiveLeaderSkill -= OnActiveLeaderSkill);
    }

    private void OnEnemySelected(GameObject currentSelected, GameObject lastSelected)
    {
        Logger.LogWarning("On enemy select: sender, " + currentSelected.name + ", last selected: " + lastSelected.name);

        var lastEnemy = lastSelected.GetComponent<EnemyControl>();
        lastEnemy.ShowAimTo(false);
        var currentEnemy = currentSelected.GetComponent<EnemyControl>();
        currentEnemy.ShowAimTo(true);
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
        foreach (var characterControl in TeamController.CharacterList.Select(character => character.GetComponent<CharacterControl>()))
        {
            characterControl.SetCanSelect(flag);
        }
    }

    public void PersisitenceSet(Dictionary<string,float> value)
    {
        //Sync TopData
        currentEnemyGroupIndex = (int)value["TopData"];
        ShowTopData();
        //Sync Hp,Mp
        characterValue = value["Hp"];
        leaderController.TotalLeaderCD = (int)value["Mp"];
        hpController.ShowForgroundBar(characterValue);
        mpController.ShowForgroundBar(leaderController.TotalLeaderCD);
        //Sync Enemy Model Index
        BattleModelLocator.Instance.MonsterIndex = (int)value["EnemyModelIndex"];
        InitEnemyList();
        //Sync Topinfo
        topController.BoxCount = value["BoxCount"];
        topController.GoldCount = value["GoldCount"];
        topController.Show();
        //Sync Popinfo
        popController.EnergyCount = value["EnergyCount"];
        popController.FPCount = value["FPCount"];
        popController.Show(false);
        //Sync Star
        starController.CurrentStar = (int)value["StarCount"];
    }

    private void Awake()
    {
        footManager = FaceController.FootManager;
        hpController = FaceController.HPController;
        mpController = FaceController.MPController;
        popController = FaceController.PopController;
        topController = FaceController.TopController;
        stepController = FaceController.StepRecord;
        leaderController = FaceController.LeaderController;
        starController = FaceController.StarController;
    }
}