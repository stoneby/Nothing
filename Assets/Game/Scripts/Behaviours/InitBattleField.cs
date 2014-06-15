using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.input;
using KXSGCodec;
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
    public GameObject WarningBg1;
    public GameObject WarningBg2;
    public GameObject WarningText;

    public GameObject BattleBG;

    public GameObject BreakObject;
    public GameObject TextBGObject;
    public GameObject TextObject;
    public GameObject Picture91;
    public GameObject TextBG91;
    public GameObject TexSwardBg91;
    public GameObject Text91;

    public GameObject CharacterAttrackValueLabel;

    public TeamSelectController TeamController;
    public TeamSimpleController EnemyController;

    public Transform CharacterWaitingTrans;

    private List<GameObject> waitingStackList = new List<GameObject>();

    private int characterAttrackValue;
    private GameObject leftContainerObj;
    private float characterMaxValue;
    private float characterValue;

    private bool isPlaying;
    private bool isBattling;
    private NextFootManager footManager;
    private GameObject lineObj;

    //开始一场战斗,attracks攻击的12个武将的数组,enemys敌人方n波敌人的数组
    private List<GameObject> attackWaitList;
    private readonly GameObject[,] charactersLeft = new GameObject[3, 3];
    //private GameObject[] enemyList;
    private int currEnemyGroupIndex;	//当前是第几波敌人;

    private float realTime;

    private int Star;
    private GameObject Star1;
    private GameObject Star2;
    private GameObject Star3;

    public void Init()
    {
        var containerobj = GameObject.Find("BattleFieldPanel");

        Star1 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite star 1").gameObject;
        Star2 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite star 2").gameObject;
        Star3 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite star 3").gameObject;

        lineObj = NGUITools.AddChild(containerobj, DragBarPrefab);
        var sp = lineObj.GetComponent<UISprite>();
        sp.spriteName = "drag_normal";

        lineObj.SetActive(false);

        var obj = GameObject.Find("BattleUIPanel");
        footManager = obj.GetComponent<NextFootManager>();

        EffectBg.SetActive(false);
        EffectObject.SetActive(false);
        WarningBg1.SetActive(false);
        WarningBg2.SetActive(false);
        WarningText.SetActive(false);
        BreakObject.SetActive(false);
        TextBGObject.SetActive(false);
        TextObject.SetActive(false);
        Picture91.SetActive(false);
        TextBG91.SetActive(false);
        Text91.SetActive(false);
        TexSwardBg91.SetActive(false);

        InitHpBar();
        InitLeaders();
        InitTopDataBar();
    }

    private static int bgIndex = 0;
    //开始战斗
    public void StartBattle()
    {
        leftContainerObj = GameObject.Find("BattleFieldWidgetLeft");

        TeamController.Total = BattleModelLocator.Instance.HeroList.Count;
        TeamController.Row = 3;
        TeamController.Col = 3;
        TeamController.Initialize();

        if (attackWaitList == null)
        {
            attackWaitList = new List<GameObject>();
        }

        InitCharacterList();

        var bg = BattleBG.GetComponent<BattleBGControl>();
        bgIndex++;
        bg.SetData("00" + bgIndex);
        bgIndex = bgIndex % 3;
        //PopTextManager.PopTip("Bg Index = " + bgIndex);

        characterValue = 0;
        characterMaxValue = 0;

        isBattling = true;
        LeaderCD = 0;
        LeaderCDMax = 50;
        ResetLeaderCd();

        BoxCount = 0;
        FPCount = 0;
        EnergyCount = 0;
        GoldCount = 0;

        Star = 3;
        Star1.SetActive(true);
        Star2.SetActive(true);
        Star3.SetActive(true);

        ShowTopData();
        EventManager.Instance.AddListener<LeaderUseEvent>(OnLeaderUseHandler);
        currEnemyGroupIndex = 0;
        ResetLeaderData();
        CreateCurrentEnemys();
        RequestRecords();

        TeamController.Print();

        InitWaitingStackList();
        AdjustCharacterList();
        StartCoroutine(InitBattleFighters());
    }

    private void InitWaitingStackList()
    {
        if (waitingStackList == null)
        {
            waitingStackList = new List<GameObject>();
        }
        waitingStackList.Clear();


        for (var i = 0; i < TeamController.Row; ++i)
        {
            var index = (TeamController.Col - 1) * TeamController.Row + i;
            var targetPosition = TeamController.FormationController.LatestPositionList[index];

            var sourcePosition = targetPosition + targetPosition -
                TeamController.FormationController.LatestPositionList[index - TeamController.Col];

            Logger.LogWarning("Waiting stack index of: " + index + ", right index: " + (index - TeamController.Col));
            var game = new GameObject("WaitingStack_" + i);
            game.transform.parent = gameObject.transform;
            game.transform.position = sourcePosition;
            waitingStackList.Add(game);
        }
    }

    private void AdjustCharacterList()
    {
        attackWaitList.Clear();
        var visibleCount = TeamController.Row * TeamController.Col;
        for (int i = 0; i < TeamController.Total; ++i)
        {
            var character = TeamController.CharacterList[i];
            if (i >= visibleCount)
            {
                attackWaitList.Add(character.gameObject);
            }
            else
            {
                charactersLeft[i / TeamController.Row, i % TeamController.Col] = character.gameObject;
            }

            Logger.LogWarning("Adjust character: " + i);
        }
    }

    private void AdjustTeamController()
    {
        TeamController.CharacterList.Clear();
        for (var i = 0; i < TeamController.Col; i++)
        {
            for (var j = 0; j < TeamController.Row; j++)
            {
                var character = charactersLeft[i, j].GetComponent<Character>();
                character.Location = new Position { X = i, Y = j };
                character.Index = TeamController.CharacterList.Count;
                TeamController.CharacterList.Add(character);
            }
        }
        foreach (var character in attackWaitList.Select(wait => wait.GetComponent<Character>()))
        {
            character.Index = TeamController.CharacterList.Count;
            TeamController.CharacterList.Add(character);
        }

        // name change accordingly for debugging.
        foreach (var character in TeamController.CharacterList)
        {
            var index = character.name.LastIndexOf('_');
            character.name = character.name.Remove(index + 1) + character.Index;
        }
    }

    private void InitializeEnemyList()
    {
        for (var i = 0; i < EnemyController.CharacterList.Count; ++i)
        {
            var ec = EnemyController.CharacterList[i].gameObject.GetComponent<EnemyControl>();
            ec.Init(null, BattleModelLocator.Instance.MonsterList[BattleModelLocator.Instance.MonsterIndex + i]);

            Logger.Log("Init enemy of index: " + (BattleModelLocator.Instance.MonsterIndex + i));
        }
    }

    private void InitCharacterList()
    {
        for (int i = 0; i < TeamController.Total; i++)
        {
            var t = BattleModelLocator.Instance.HeroList[i];
            var obj = TeamController.CharacterList[i];
            var cc = obj.GetComponent<CharacterControl>();

            if (i > 8)
            {
                cc.SetCharacter(t, BattleTypeConstant.IsFriend);
            }
            else
            {
                cc.SetCharacter(t);
            }
            cc.SetSelect(false);
        }
    }

    IEnumerator InitBattleFighters()
    {
        yield return StartCoroutine(MakeUpOneByOne(false));
        RunToNextEnemys();
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    //创建本关的怪
    void CreateCurrentEnemys()
    {
        Logger.Log("怪物关卡数：" + BattleModelLocator.Instance.MonsterGroup.Count + ",当前关卡索引：" + currEnemyGroupIndex + ",总怪物数：" + BattleModelLocator.Instance.MonsterList.Count);

        EnemyController.Cleanup();
        EnemyController.Total = BattleModelLocator.Instance.MonsterGroup[currEnemyGroupIndex];
        EnemyController.Initialize();

        Logger.Log("当前关卡怪物数：" + EnemyController.Total);

        InitializeEnemyList();

        BattleModelLocator.Instance.MonsterIndex += EnemyController.Total;
        Logger.Log("Next monster index: " + BattleModelLocator.Instance.MonsterIndex);
    }

    //战斗结束后销毁
    public void DestroyBattle()
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

        EventManager.Instance.RemoveListener<LeaderUseEvent>(OnLeaderUseHandler);
    }

    //后面的武将补位
    IEnumerator MakeUpOneByOne(bool needresetcharacter = true)
    {
        float runStepTime = (needresetcharacter) ? GameConfig.RunStepNeedTime : GameConfig.ShortTime;
        float runWaitTime = (needresetcharacter) ? GameConfig.NextRunWaitTime : GameConfig.ShortTime;
        float duration = GameConfig.ShortTime;
        for (var i = 0; i < TeamController.Col; i++)
        {
            for (var j = 0; j < TeamController.Row; j++)
            {
                CharacterControl cc;
                if (charactersLeft[i, j] == null)
                {
                    var flag = true;
                    for (int k = i + 1; k < 3; k++)
                    {
                        if (charactersLeft[k, j] != null)
                        {
                            charactersLeft[i, j] = charactersLeft[k, j];
                            GameObject obj = charactersLeft[i, j];
                            cc = obj.GetComponent<CharacterControl>();
                            cc.PlayCharacter(CharacterType.ActionRun);
                            duration = (k - i) * runStepTime;
                            cc.SetCharacterAfter(duration);

                            // Move to target.
                            var target = charactersLeft[k, j];
                            var targetPosition =
                                TeamController.FormationController.LatestPositionList[
                                    TeamController.TwoDimensionToOne(i, j)];
                            iTween.MoveTo(target, targetPosition, duration);

                            Logger.LogWarning("Character: " + target.GetComponent<Character>() + "Move to position: " +
                                              targetPosition + ", duration: " + duration);

                            charactersLeft[k, j] = null;
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        charactersLeft[i, j] = attackWaitList[0] as GameObject;
                        attackWaitList.RemoveAt(0);

                        charactersLeft[i, j].SetActive(true);
                        cc = charactersLeft[i, j].GetComponent<CharacterControl>();

                        //cc.SetFootIndex(footManager.GetNext());
                        cc.PlayCharacter(CharacterType.ActionRun);

                        //var character = cc.GetComponent<Character>();
                        //character.ColorIndex = cc.FootIndex;

                        duration = (2 - i) * GameConfig.RunStepNeedTime + GameConfig.RunStepNeedTime;

                        if (needresetcharacter)
                        {
                            cc.SetCharacterAfter(duration);
                        }


                        // Move to target from delta left.
                        var index = TeamController.TwoDimensionToOne(i, j);
                        var targetPosition = TeamController.FormationController.LatestPositionList[index];
                        var sourcePosition = waitingStackList[j].transform.position;
                        charactersLeft[i, j].transform.position = sourcePosition;

                        var target = charactersLeft[i, j];
                        iTween.MoveTo(target, targetPosition, duration);

                        Logger.LogWarning("Character: " + target.GetComponent<Character>() + "Move to position: " +
                                          targetPosition + ", duration: " + duration);
                    }
                    yield return new WaitForSeconds(runWaitTime);
                }
            }
        }

        AdjustTeamController();

        if (needresetcharacter)
        {
            yield return new WaitForSeconds(duration);
        }

        TeamController.Print();
    }

    private void SetColor()
    {
        foreach (var character in TeamController.CharacterList)
        {
            SetColor(character.gameObject);
        }
    }

    private void SetColor(GameObject target)
    {
        var cc = target.GetComponent<CharacterControl>();
        cc.SetFootIndex(footManager.GetNext());

        var character = target.GetComponent<Character>();
        character.ColorIndex = cc.FootIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - realTime > 0.1)
        {
            ResetCharacters();
        }
        realTime = Time.realtimeSinceStartup;

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
        Logger.Log("-------------- on selected: " + selectedObject.name);

        var cc = selectedObject.GetComponent<CharacterControl>();
        currentFootIndex = cc.FootIndex;
        AddAObj(selectedObject);
    }

    private void OnDeselected(GameObject selectedObject)
    {
        Logger.Log("-------------- on deselected: " + selectedObject.name);
        AddAObj(selectedObject, false);
    }

    private void OnSelectedStart()
    {
        Reset();
    }

    private void OnSelectedStop(bool isAttacked)
    {
        CleanAttackValue();
        CleanEffect();

        // Disable team selection.
        TeamController.UnregisterEventHandlers();
        TeamController.SelectedCharacterList.ForEach(item =>
        {
            var character = item.GetComponent<CharacterControl>();
            character.SetSelect(false);
        });

        if (isAttacked)
        {
            var selectedList = TeamController.SelectedCharacterList.Select(item => item.Index);
            DoAttack(selectedList.ToArray());
        }
        else
        {
            ShowHp();
            ShowMp();
        }
    }

    private ArrayList selectEffectList;
    private int currentFootIndex;

    private void CleanAttackValue()
    {
        UILabel uilb = CharacterAttrackValueLabel.GetComponent<UILabel>();
        uilb.text = "";
    }

    private void CleanEffect()
    {
        while (selectEffectList != null && selectEffectList.Count > 0)
        {
            Destroy(selectEffectList[0] as GameObject);
            selectEffectList.RemoveAt(0);
        }
    }

    private void Reset()
    {
        if (selectEffectList == null)
        {
            selectEffectList = new ArrayList();
        }
        selectEffectList.Clear();

        characterAttrackValue = 0;
    }

    private void DoAttack(int[] indexArr)
    {
        if (EnemyController.CharacterList.Count <= 0)
        {
            Logger.Log("Enemy list is empty. Please check it out.");
            return;
        }

        //调取服务器战斗
        var enemy = EnemyController.CharacterList[0];
        var ec = enemy.GetComponent<EnemyControl>();
        var action = new ProduceFighterIndexAction
        {
            HeroIndex = indexArr,
            TargetIndex = ec.Data.Index
        };
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(action);
        RequestRecords();
    }

    //增加一个选择的武将
    void AddAObj(GameObject obj, bool isadd = true)
    {
        var tempcc1 = obj.GetComponent<CharacterControl>();

        if (isadd)
        {
            tempcc1.SetSelect(true, TeamController.SelectedCharacterList.Count - 1);
            selectEffectList.Add(EffectManager.ShowEffect(EffectType.SelectEffects[currentFootIndex], 0, 0, obj.transform.position));
        }
        else
        {
            tempcc1.SetSelect(false);
            Destroy(selectEffectList[selectEffectList.Count - 1] as GameObject);
            selectEffectList.RemoveAt(selectEffectList.Count - 1);
        }

        characterAttrackValue = isadd ? characterAttrackValue + tempcc1.AttrackValue : characterAttrackValue - tempcc1.AttrackValue;
        var uilb = CharacterAttrackValueLabel.GetComponent<UILabel>();
        uilb.text = characterAttrackValue.ToString();
        if (currentFootIndex == BattleTypeConstant.FootPink)
        {
            ShowTempHp(characterAttrackValue);
        }
        ShowTempMp(TeamController.SelectedCharacterList.Count);
    }

    //左侧部队攻击
    void DoAttrackLeft()
    {
        isPlaying = true;
        StartCoroutine(LeftAttrackCoroutineHandler());
    }

    //右侧部队攻击
    void DoAttrackRight()
    {
        isPlaying = true;
        StartCoroutine(RightAttrackCoroutineHandler());
    }

    //显示武将掉血
    void CharacterLoseBlood(Vector3 pos, float newvalue, bool isnotadd = true)
    {
        if (isnotadd)
        {
            var losevalue = characterValue - newvalue;

            characterValue = newvalue;
            if (losevalue > 0)
            {
                PopTextManager.ShowText("-" + losevalue, 0.6f, -25, 60, 50, pos);
                var v = characterMaxValue - characterValue;
                if (v > (characterMaxValue / 2) && Star > 1)
                {
                    Star = 1;
                    Star2.SetActive(false);
                }
                else if (v > (characterMaxValue / 4) && Star > 2)
                {
                    Star = 2;
                    Star3.SetActive(false);
                }
            }
        }
        else
        {
            PopTextManager.ShowText("+" + characterAttrackValue, 0.6f, 80, 100, 50, pos);
        }
        ShowHp();
    }

    //播放回血特效
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

    //跑到下一波怪
    void RunToNextEnemys()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionRun);
                cc.SetCharacterAfter(GameConfig.RunRoNextMonstersTime);
            }
        }
        var bg = BattleBG.GetComponent<BattleBGControl>();
        bg.MoveToNext();

        // [NOTE:] This place move enemy when next page.
        //for (int i = 0; i < enemyList.Length; i++)
        //{
        //    var obj = enemyList[i];
        //    var temptp = obj.AddComponent<TweenPosition>();
        //    temptp.duration = GameConfig.RunRoNextMonstersTime;
        //    temptp.from = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
        //    temptp.to = new Vector3(obj.transform.localPosition.x - 640, obj.transform.localPosition.y, 0);
        //    temptp.PlayForward();
        //}
    }

    //获取该action的怪
    private GameObject GetMonsterObject(SingleActionRecord record)
    {
        var enemy = EnemyController.CharacterList.Find(character =>
        {
            var ec = character.GetComponent<EnemyControl>();
            return (ec.Data.index == record.Index);
        });
        return (enemy != null) ? enemy.gameObject : null;
    }

    //private const float AttrackTime = 0.3f;
    //播放武将的一次出手动作
    private IEnumerator PlayOneAction(BattleFightRecord record)
    {
        GameObject obj;
        GameObject monster;
        CharacterControl cc;
        obj = GetCharacterByAction(record.getAttackAction());
        if (obj == null)
        {
            Logger.Log("没有获取到武将，传入的index=" + record.getAttackAction().Index);
        }
        else
        {
            cc = obj.GetComponent<CharacterControl>();

            if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK)
            {
                monster = GetMonsterObject(record.ActionList[0]);
                var ec = monster.GetComponent<EnemyControl>();
                ec.ShowBlood(true);
                RunReturn(obj, GameConfig.ShortTime);
                yield return new WaitForSeconds(AddMoveCharacter(obj, monster));
                PlayEnemyBeenAttrack(cc, record.ActionList);
            }
            else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
            {
                IsRecover = true;
                cc.PlayCharacter(CharacterType.ActionAttrack);
                yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                cc.PlayCharacter(CharacterType.ActionWait);
                CharacterLoseBlood(obj.transform.localPosition, record.getAttackAction().getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                //RunReturn(obj);
            }
            else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED)
            {
            }
            else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE)
            {
            }
            else
            {
                monster = GetMonsterObject(record.ActionList[0]);
                var ec = monster.GetComponent<EnemyControl>();
                ec.ShowBlood(true);
                RunToAttrackPlace(obj, monster);
                yield return new WaitForSeconds(GameConfig.RunToAttrackPosTime);
                cc.PlayCharacter(CharacterType.ActionAttrack);
                yield return new WaitForSeconds(GameConfig.PlayAttrackTime);
                PlayEnemyBeenAttrack(cc, record.ActionList);
                RunReturn(obj, GameConfig.HeroRunReturnTime);
            }
        }
    }

    //播放左边武将攻击动作
    private bool IsRecover;
    private IEnumerator LeftAttrackCoroutineHandler()
    {
        if (battleTeamRecord.SkillFighter != null && battleTeamRecord.SkillFighter.Count > 0)
        {
            PlaySpEffect(battleTeamRecord.SkillFighter);
            yield return new WaitForSeconds(0.3f * battleTeamRecord.SkillFighter.Count + 0.5f);
        }

        BattleFightRecord record;
        GameObject obj;
        GameObject monster;
        CharacterControl cc;
        IsRecover = false;

        if (battleTeamRecord.RecordList.Count > 0)
        {
            record = battleTeamRecord.RecordList[0];
            if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
            {
                StartCoroutine(PlayOneAction(record));
            }
            else
            {
                foreach (var ec in EnemyController.CharacterList.Select(character => character.GetComponent<EnemyControl>()))
                {
                    ec.ShowBlood(false);
                }
                yield return StartCoroutine(PlayOneAction(record));
            }
        }

        if (battleTeamRecord.RecordList.Count > 1)
        {
            var thecount = (battleTeamRecord.RecordList.Count != 9 || IsRecover) ? battleTeamRecord.RecordList.Count : battleTeamRecord.RecordList.Count - 1;
            for (var i = 1; i < thecount; i++)
            {
                record = battleTeamRecord.RecordList[i];
                StartCoroutine(PlayOneAction(record));
                if (!IsRecover) yield return new WaitForSeconds(GameConfig.NextAttrackWaitTime);
            }
        }

        if (battleTeamRecord.RecordList.Count == 9 && !IsRecover)
        {
            yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime);
            record = battleTeamRecord.RecordList[battleTeamRecord.RecordList.Count - 1];
            monster = GetMonsterObject(record.ActionList[0]);
            var ec = monster.GetComponent<EnemyControl>();
            //obj = pointList[pointList.Count - 1] as GameObject;
            obj = TeamController.SelectedCharacterList[TeamController.SelectedCharacterList.Count - 1].gameObject;
            cc = obj.GetComponent<CharacterControl>();
            //yield return new WaitForSeconds (0.5f);//镜头拉近
            PlayMoveCamera(obj);
            cc.Stop();
            yield return new WaitForSeconds(GameConfig.MoveCameraTime);//显示遮罩
            Picture91.SetActive(true);
            //yield return new WaitForSeconds(0.1f);
            //播放角色特效
            EffectManager.PlayEffect(EffectType.NineAttrack, GameConfig.Attrack9PlayEffectTime, -25, 0, obj.transform.position);
            yield return new WaitForSeconds(GameConfig.Attrack9PlayEffectTime);
            //人物大图飞入
            UITexture tt = EffectObject.GetComponent<UITexture>();
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
            PlayMoveCameraEnd();
            yield return new WaitForSeconds(GameConfig.MoveCameraTime);
            //攻击动作
            cc.Play();

            RunToAttrackPlace(obj, monster);
            yield return new WaitForSeconds(GameConfig.RunToAttrackPosTime);

            cc.PlayCharacter(3);
            yield return new WaitForSeconds(GameConfig.PlayAttrackTime);

            //9连击播放刀光
            TexSwardBg91.SetActive(true);
            cc.Stop();
            var offset = 0.5f;
            //Vector3 pos = enemy.transform.position;
            var pos = new Vector3(0, 0, monster.transform.position.z);
            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayBeen();
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayBeen();
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            offset = 0.3f;
            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            ec.PlayBeen();
            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect3, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9SwardWaitTime);

            EffectManager.PlayEffect(EffectType.SwordEffect2, GameConfig.Attrack9SwardEffectTime, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
            yield return new WaitForSeconds(GameConfig.Attrack9TotalSwardTime);
            TexSwardBg91.SetActive(false);
            PlayEnemyBeenAttrack(cc, record.ActionList, true);

            yield return new WaitForSeconds(.4f);

            cc.Play();
            RunReturn(obj, GameConfig.HeroRunReturnTime);
        }

        if (IsRecover)
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
        CheckMonsterDead();
        yield return StartCoroutine(MakeUpOneByOne());

        // Enable team selection.
        TeamController.RegisterEventHandlers();

        LeaderCD = battleTeamRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_MP);
        ShowMp();
        ShowTopData();
        isPlaying = false;
        isPlayingRecord = false;
        recordIndex++;
        dealWithRecord();

    }

    //判断怪是否死亡
    private void CheckMonsterDead()
    {
        var dead = false;
        foreach (var character in EnemyController.CharacterList)
        {
            var ec = character.GetComponent<EnemyControl>();
            if (ec.HP <= 0)
            {
                var v = ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_COIN);
                if (v > 0)
                {
                    GoldCount += v;
                    EffectManager.PlayEffect(EffectType.GetMoney, 0.5f, 0, 0, character.transform.position);
                }

                EnergyCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_HERO);
                BoxCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_ITEM);
                FPCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_SPRIT);
                ec.OnDestory();

                ShowTopData();

                EnemyController.CharacterPool.Return(character.gameObject);
                dead = true;
            }
        }

        if (dead)
        {
            EnemyController.CharacterList.RemoveAt(0);
        }
    }

    //播放右边怪物攻击动作
    private IEnumerator RightAttrackCoroutineHandler()
    {
        SetCharacterCanSelect(false);
        if (battleTeamRecord.RecordList.Count > 0)
        {
            BattleFightRecord record;
            for (int i = 0; i < battleTeamRecord.RecordList.Count; i++)
            {
                record = battleTeamRecord.RecordList[i];
                var enemy = GetMonsterObject(record.getAttackAction());
                if (enemy == null) continue;
                var ec = enemy.GetComponent<EnemyControl>();

                if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_SP_ATTACK)
                {
                    PlayMoveCamera(enemy);

                    yield return new WaitForSeconds(GameConfig.MoveCameraTime);

                    EffectManager.PlayEffect(EffectType.EnemySprite, GameConfig.PlayMonsterEffectTime, 0, 0, enemy.transform.position);

                    yield return new WaitForSeconds(GameConfig.PlayMonsterEffectTime);
                    PlayMoveCameraEnd();

                    yield return new WaitForSeconds(GameConfig.MoveCameraTime);
                    yield return StartCoroutine(PlayCharacterBeenAttrack(record.ActionList));
                }
                else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
                {
                }
                else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVERED)
                {
                }
                else if (record.getAttackAction().ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_DEFENCE)
                {
                }
                else
                {
                    yield return new WaitForSeconds(ec.PlayAttrack());
                    yield return StartCoroutine(PlayCharacterBeenAttrack(record.ActionList));
                }

                //yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime);
                ResetMonsterStates(ec, record.getAttackAction().StateUpdateList);
            }
        }
        isPlaying = false;
        isPlayingRecord = false;
        recordIndex++;
        dealWithRecord();
        SetCharacterCanSelect(true);
    }

    //设置怪的状态参数显示
    private void ResetMonsterStates(EnemyControl ec, List<FighterStateRecord> statelist)
    {
        for (int j = 0; j < statelist.Count; j++)
        {
            var state = statelist[j];
            if (state.State == BattleKeyConstants.BATTLE_STATE_MONSTER_SKILL_ROUND)
            {
                ec.SetRoundCount(state.LeftRound);
            }
        }
    }

    //获取该action的武将对象
    private GameObject GetCharacterByAction(SingleActionRecord action)
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (charactersLeft[i, j] != null)
                {
                    var cc = charactersLeft[i, j].GetComponent<CharacterControl>();
                    if (cc.Data.Index == action.Index)
                    {
                        return charactersLeft[i, j];
                    }
                }
            }
        }
        return null;
    }

    //播放武将的受击
    private IEnumerator PlayCharacterBeenAttrack(List<SingleActionRecord> actionlist)
    {
        for (var i = 0; i < actionlist.Count; i++)
        {
            var action = actionlist[i];
            var obj = GetCharacterByAction(action);
            if (obj != null)
            {
                EffectManager.PlayEffect(EffectType.Attrack, 0.8f, -20, -20, obj.transform.position);
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionBeaten);
                CharacterLoseBlood(obj.transform.localPosition, action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
                iTweenEvent.GetEvent(obj, "ShakeTween").Play();
            }
        }

        yield return new WaitForSeconds(GameConfig.HeroBeenAttrackTime);
        for (var j = 0; j < actionlist.Count; j++)
        {
            var action = actionlist[j];
            var obj = GetCharacterByAction(action);
            if (obj != null)
            {
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionWait);
            }
        }
    }

    //跑到攻击位
    private static void RunToAttrackPlace(GameObject obj, GameObject enemy)
    {
        var duration = GameConfig.RunToAttrackPosTime;
        iTween.MoveTo(obj, enemy.transform.position, duration);
    }

    //播放怪物的受击
    private void PlayEnemyBeenAttrack(CharacterControl cc, List<SingleActionRecord> enemylist, bool showbig = false)
    {
        for (int i = 0; i < enemylist.Count; i++)
        {
            var action = enemylist[i];
            var enemy = GetMonsterObject(action);
            if (enemy == null) continue;

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

            var ec = enemy.GetComponent<EnemyControl>();

            if (showbig)
            {
                var obj = GameObject.Find("BattleFieldPanel");
                iTweenEvent.GetEvent(obj, "ShakeTweener").Play();
            }
            else if (k > 1)
            {
                ec.PlayBigBeen();
            }
            else
            {
                ec.PlayBeen();
            }
            if (action.prop.ContainsKey(BattleRecordConstants.SINGLE_ACTION_PROP_HP))
            {
                ec.SetHP(action.getIntProp(BattleRecordConstants.SINGLE_ACTION_PROP_HP));
            }
        }
    }

    private IEnumerator MultPopText(GameObject obj, int count, int value)
    {
        var v = obj.transform.localPosition;
        for (int i = 0; i < count; i++)
        {
            PopTextManager.ShowText("-" + value, 0.6f, 0, 40, 120, v);
            yield return new WaitForSeconds(0.2f);
        }

    }

    //武将攻击后返回等待队列
    private void RunReturn(GameObject obj, float duration)
    {
        var cc = obj.GetComponent<CharacterControl>();
        cc.PlayCharacter(0);

        // Move to target.
        //iTween.MoveTo(obj, CharacterWaitingTrans.position, duration);
        iTween.MoveTo(obj,
            iTween.Hash("position", CharacterWaitingTrans.position, "duration", duration, "oncomplete",
                "OnRunReturnComplete", "oncompletetarget", gameObject, "oncompleteparams", obj));

        var character = obj.GetComponent<Character>();
        charactersLeft[character.Location.X, character.Location.Y] = null;
        attackWaitList.Add(obj);

        Debug.LogWarning("Return object: " + obj.name + ", location: " + character.Location);
    }

    private void OnRunReturnComplete(GameObject target)
    {
        SetColor(target);
    }

    private GameObject tempGameObj;
    //播放摄像机拉镜头
    private void PlayMoveCamera(GameObject obj)
    {
        tempGameObj = obj;
        var containerobj = GameObject.Find("BattleFieldPanel");
        var tp = containerobj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(0, 0, 0);
        tp.to = new Vector3(-tempGameObj.transform.localPosition.x * 2.5f, -tempGameObj.transform.localPosition.y * 2.5f, 0);
        tp.duration = GameConfig.MoveCameraTime;
        tp.PlayForward();
        var ts = containerobj.GetComponent<TweenScale>();
        ts.ResetToBeginning();
        ts.from = new Vector3(1, 1, 1);
        ts.to = new Vector3(2.5f, 2.5f, 1);
        ts.duration = GameConfig.MoveCameraTime;
        ts.PlayForward();
    }

    private void PlayMoveCameraEnd()
    {
        GameObject containerobj = GameObject.Find("BattleFieldPanel");
        var tp = containerobj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(-tempGameObj.transform.localPosition.x * 2.5f, -tempGameObj.transform.localPosition.y * 2.5f, 0);
        tp.to = new Vector3(0, 0, 0);
        tp.duration = GameConfig.MoveCameraTime / 2.5f;
        tp.PlayForward();
        var ts = containerobj.GetComponent<TweenScale>();
        while (ts.onFinished.Count > 0) ts.onFinished.RemoveAt(0);
        ts.ResetToBeginning();
        ts.from = new Vector3(2.5f, 2.5f, 1);
        ts.to = new Vector3(1, 1, 1);
        ts.duration = GameConfig.MoveCameraTime / 2.5f;
        ts.PlayForward();
    }

    //下面的函数用来处理队长
    private void OnLeaderUseHandler(LeaderUseEvent e)
    {
        RequestRecords();
    }

    private List<GameObject> leaders;
    private int LeaderCD;
    private int LeaderCDMax = 50;

    private void InitLeaders()
    {
        LeaderCD = 0;
        GameObject ldContainer = GameObject.Find("Anchor-bottomleft");
        const int basex = -100;
        const int basey = 40;
        const int offsetx = 125;
        var i = 0;
        leaders = new List<GameObject>();
        var obj = NGUITools.AddChild(ldContainer, LeaderPrefab);
        obj.transform.localPosition = new Vector3(basex + i * offsetx, basey, 0);
        var lc = obj.GetComponent<LeaderControl>();
        lc.Init(1, 9, i + 1);
        leaders.Add(obj);

        i = 1;
        obj = NGUITools.AddChild(ldContainer, LeaderPrefab);
        obj.transform.localPosition = new Vector3(basex + i * offsetx, basey, 0);
        lc = obj.GetComponent<LeaderControl>();
        lc.Init(2, 15, i + 1);
        leaders.Add(obj);

        i = 2;
        obj = NGUITools.AddChild(ldContainer, LeaderPrefab);
        obj.transform.localPosition = new Vector3(basex + i * offsetx, basey, 0);
        lc = obj.GetComponent<LeaderControl>();
        lc.Init(3, 25, i + 1);
        leaders.Add(obj);

        i = 3;
        obj = NGUITools.AddChild(ldContainer, LeaderPrefab);
        obj.transform.localPosition = new Vector3(basex + i * offsetx, basey, 0);
        lc = obj.GetComponent<LeaderControl>();
        lc.Init(2, 25, i + 1);
        leaders.Add(obj);
    }

    private void ResetLeaderData()
    {
        if (BattleModelLocator.Instance.HeroList == null) return;
        if (BattleModelLocator.Instance.HeroList.Count >= 10)
        {
            var leader = leaders[0].GetComponent<LeaderControl>();
            leader.SetData(BattleModelLocator.Instance.HeroList[0], 0);
            leader = leaders[1].GetComponent<LeaderControl>();
            leader.SetData(BattleModelLocator.Instance.HeroList[1], 1);
            leader = leaders[2].GetComponent<LeaderControl>();
            leader.SetData(BattleModelLocator.Instance.HeroList[2], 2);
            leader = leaders[3].GetComponent<LeaderControl>();
            leader.SetData(BattleModelLocator.Instance.HeroList[9], 9);
        }
    }

    private void ResetLeaderCd()
    {
        if (leaders == null) return;
        foreach (var t in leaders)
        {
            var lc = t.GetComponent<LeaderControl>();
            lc.Reset(LeaderCD);
        }
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
        UILabel lb = TextObject.GetComponent<UILabel>();
        lb.text = BattleModelLocator.Instance.Skill.Name;
        lb.alpha = 1;
        TextObject.SetActive(true);

        PlayTweenScale(TextObject, 0.2f, new Vector3(5, 5, 1), new Vector3(1, 1, 1));
        yield return new WaitForSeconds(0.2f);

        PlayTweenScale(TextObject, 0.8f, new Vector3(1, 1, 1), new Vector3(0.9f, 0.9f, 1));

        yield return new WaitForSeconds(0.8f);

        BreakObject.SetActive(true);
        UITexture tt1 = BreakObject.GetComponent<UITexture>();
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
        //        ResetLeaderCd();
        var attrack = LeaderSkillRecord.OrCreateFightRecord.getAttackAction();
        LeaderCD = LeaderSkillRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_PROP_MP);
        ShowMp();
        if (attrack.ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_CHANGE_COLOR)
        {
            for (int i = 0; i < LeaderSkillRecord.OrCreateFightRecord.ActionList.Count; i++)
            {
                var obj = GetCharacterByAction(LeaderSkillRecord.OrCreateFightRecord.ActionList[i]);
                if (obj != null)
                {
                    var cc = obj.GetComponent<CharacterControl>();
                    var k =
                        LeaderSkillRecord.OrCreateFightRecord.ActionList[i].getIntProp(
                            BattleRecordConstants.BATTLE_HERO_PROP_COLOR_CHANGE);
                    cc.SetFootIndex(k);
                }
            }
        }
        else if (attrack.ActType == BattleRecordConstants.SINGLE_ACTION_TYPE_RECOVER)
        {
            PlayBloodFullEffect();
            var obj = GetCharacterByAction(attrack);
            var k =
                attrack.getIntProp(
                    BattleRecordConstants.SINGLE_ACTION_PROP_HP);
            if (obj != null)
            {
                CharacterLoseBlood(obj.transform.localPosition, k);
            }
            else
            {
                CharacterLoseBlood(new Vector3(0, 0, 0), k);
            }
        }
        ShowMp();
        EffectManager.PlayAllEffect(true);
        BattleModelLocator.Instance.CanSelectHero = true;
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
        Logger.Log(basey);
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

    //播放警告全屏效果
    GameObject warningObj;

    private void PlayWarning(float playtime)
    {
        EffectManager.PlayAllEffect(false);
        GameObject effectbg1 = WarningBg1;
        GameObject effectbg2 = WarningBg2;
        effectbg1.SetActive(true);
        effectbg2.SetActive(true);

        TweenPosition tp1 = effectbg1.GetComponent<TweenPosition>();
        tp1.duration = playtime;
        tp1.PlayForward();

        var tp2 = effectbg2.GetComponent<TweenPosition>();
        tp2.duration = playtime;
        tp2.PlayForward();

        var effecttext = WarningText;
        effecttext.SetActive(true);

        var ts = effecttext.GetComponent<TweenScale>();
        var tex = effecttext.GetComponent<UITexture>();
        tex.alpha = 0;
        ts.delay = playtime;
        ts.duration = playtime;
        ts.PlayForward();

        var ta = effecttext.GetComponent<TweenAlpha>();
        ta.delay = playtime;
        ta.PlayForward();
    }

    private void PlayWarningEnd()
    {
        Destroy(warningObj);
        GameObject effecttext = WarningText;
        var ts = effecttext.GetComponent<TweenScale>();
        ts.delay = 0;
        ts.PlayReverse();
        var ta = effecttext.GetComponent<TweenAlpha>();
        ta.delay = 0;
        ta.PlayReverse();

        var effectbg1 = WarningBg1;
        var effectbg2 = WarningBg2;

        var tp1 = effectbg1.GetComponent<TweenPosition>();
        tp1.PlayReverse();

        var tp2 = effectbg2.GetComponent<TweenPosition>();
        tp2.PlayReverse();
    }

    private void HideWaring()
    {
        EffectManager.PlayAllEffect(true);
        WarningBg1.SetActive(false);
        WarningBg2.SetActive(false);
        WarningText.SetActive(false);
    }

    //处理游戏重新激活时动画播放太快
    private void ResetCharacters()
    {
        if (charactersLeft == null || attackWaitList == null) return;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (charactersLeft[i, j] != null)
                {
                    ResetCharacter(charactersLeft[i, j] as GameObject);
                }
            }
        }
        foreach (var t in attackWaitList)
        {
            ResetCharacter(t as GameObject);
        }
    }

    private void ResetCharacter(GameObject obj)
    {
        var cc = obj.GetComponent<CharacterControl>();
        cc.ResetCharacter();
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

    //处理progressbar
    private GameObject CharacterMPLabel;
    private GameObject CharacterHPLabel;
    private GameObject SpriteHP1;
    private GameObject SpriteHP2;
    private GameObject SpriteMP1;
    private GameObject SpriteMP2;

    private void InitHpBar()
    {
        CharacterHPLabel = transform.FindChild("BattleUIPanel/Anchor-bottomleft/CharacterBloodLabel").gameObject;
        CharacterMPLabel = transform.FindChild("BattleUIPanel/Anchor-bottomleft/CharacterCDLabel").gameObject;
        SpriteHP1 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite - hp1").gameObject;
        SpriteHP2 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite - hp2").gameObject;
        SpriteMP1 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite - mp1").gameObject;
        SpriteMP2 = transform.FindChild("BattleUIPanel/Anchor-bottomleft/Sprite - mp2").gameObject;

    }

    private void ShowHp()
    {
        if (characterMaxValue < characterValue) characterMaxValue = characterValue;
        float xx = BattleTypeConstant.PosHPMin + (BattleTypeConstant.PosHPMax - BattleTypeConstant.PosHPMin) * characterValue / characterMaxValue;
        SpriteHP1.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosHPY, 0);
        SpriteHP2.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosHPY, 0);
        var lb = CharacterHPLabel.GetComponent<UILabel>();
        lb.text = characterValue + "/" + characterMaxValue;
    }

    private void ShowTempHp(float offset)
    {
        if (offset < 0) return;
        float value = characterValue + offset;
        if (value > characterMaxValue) value = characterMaxValue;
        float xx = BattleTypeConstant.PosHPMin + (BattleTypeConstant.PosHPMax - BattleTypeConstant.PosHPMin) * value / characterMaxValue;
        SpriteHP2.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosHPY, 0);
    }

    private void ShowMp()
    {
        ResetLeaderCd();
        if (LeaderCDMax < LeaderCD) LeaderCDMax = LeaderCD;
        float xx = BattleTypeConstant.PosMPMin + (BattleTypeConstant.PosMPMax - BattleTypeConstant.PosMPMin) * LeaderCD / LeaderCDMax;
        SpriteMP1.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosMPY, 0);
        SpriteMP2.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosMPY, 0);
        var lb = CharacterMPLabel.GetComponent<UILabel>();
        lb.text = LeaderCD + "/" + LeaderCDMax;
    }

    private void ShowTempMp(int offset)
    {
        if (offset < 0) return;
        int value = LeaderCD + offset;
        if (value > LeaderCDMax) value = LeaderCDMax;
        float xx = BattleTypeConstant.PosMPMin + (BattleTypeConstant.PosMPMax - BattleTypeConstant.PosMPMin) * value / LeaderCDMax;
        SpriteMP2.transform.localPosition = new Vector3(xx, BattleTypeConstant.PosMPY, 0);
    }

    //显示上方获得数据
    private GameObject BoxLabel;
    private GameObject FPLabel;
    private GameObject StepLabel;
    private GameObject EnergyLabel;
    private GameObject GoldLabel;
    private int BoxCount;
    private int FPCount;
    private int EnergyCount;
    private int GoldCount;
    private void InitTopDataBar()
    {
        BoxLabel = transform.FindChild("BattleUIPanel/Anchor-topright/Sprite - topbar/Label - value1").gameObject;
        FPLabel = transform.FindChild("BattleUIPanel/Anchor-topright/Sprite - topbar/Label - value2").gameObject;
        StepLabel = transform.FindChild("BattleUIPanel/Anchor-topright/Sprite - topbar/Label - value3").gameObject;
        EnergyLabel = transform.FindChild("BattleUIPanel/Anchor-topright/Sprite - topbar/Label - value4").gameObject;
        GoldLabel = transform.FindChild("BattleUIPanel/Anchor-topright/Sprite - topbar/Label - value5").gameObject;
        BoxCount = 0;
        FPCount = 0;
        EnergyCount = 0;
        GoldCount = 0;
    }

    private void ShowTopData()
    {
        var lb = BoxLabel.GetComponent<UILabel>();
        lb.text = BoxCount.ToString();

        lb = FPLabel.GetComponent<UILabel>();
        lb.text = FPCount.ToString();

        lb = EnergyLabel.GetComponent<UILabel>();
        lb.text = EnergyCount.ToString();

        lb = GoldLabel.GetComponent<UILabel>();
        lb.text = GoldCount.ToString();

        lb = StepLabel.GetComponent<UILabel>();
        lb.text = (currEnemyGroupIndex + 1).ToString() + "/" + BattleModelLocator.Instance.MonsterGroup.Count.ToString();
    }

    private IEnumerator GotoNextScene()
    {
        BattleModelLocator.Instance.CanSelectHero = false;
        yield return StartCoroutine(MakeUpOneByOne());

        currEnemyGroupIndex++;
        if (currEnemyGroupIndex < BattleModelLocator.Instance.MonsterGroup.Count)
        {
            CreateCurrentEnemys();

            RunToNextEnemys();
            yield return new WaitForSeconds(GameConfig.RunRoNextMonstersTime);
            if (currEnemyGroupIndex == BattleModelLocator.Instance.MonsterGroup.Count - 1)
            {
                yield return new WaitForSeconds(.4f);
                PlayWarning(0.2f);
                yield return new WaitForSeconds(0.6f);

                warningObj = EffectManager.ShowEffect(EffectType.Warning, 0, 0, new Vector3(0, 0, 0));
                yield return new WaitForSeconds(1.8f);
                PlayWarningEnd();
                yield return new WaitForSeconds(.4f);
                HideWaring();
            }
            ShowTopData();
        }
        BattleModelLocator.Instance.CanSelectHero = true;
        recordIndex++;
        dealWithRecord();
    }

    //下面是处理服务器返回数据用的接口
    //技能
    private BattleSkillRecord LeaderSkillRecord;
    public void showBattleSkillRecord(BattleSkillRecord battleSkillRecord)
    {

        if (battleSkillRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_A)
        {
            LeaderSkillRecord = battleSkillRecord;

            StartCoroutine(PlayLeaderEffect());

        }
    }

    private BattleTeamFightRecord battleTeamRecord;
    //一边队伍出手的所有信息
    public void showBattleTeamFightRecord(BattleTeamFightRecord battleTeamFightRecord)
    {
        //throw new NotImplementedException();
        Logger.Log("战斗过程 = " + battleTeamFightRecord.RecordList.Count);
        isPlayingRecord = true;
        battleTeamRecord = battleTeamFightRecord;
        if (battleTeamRecord.TeamSide == BattleRecordConstants.TARGET_SIDE_A)
        {
            DoAttrackLeft();
        }
        else
        {
            DoAttrackRight();
        }
    }

    //回合变化。怪物（cd）+武将（buff）
    public void showBattleRoundCountRecord(BattleRoundCountRecord roundCountRecord)
    {
        //throw new NotImplementedException();
        isPlayingRecord = false;
        Logger.Log("CD等状态 = " + roundCountRecord.RecordList.Count);
        if (roundCountRecord != null && roundCountRecord.RecordList != null && roundCountRecord.RecordList.Count > 0)
        {
            for (int i = 0; i < roundCountRecord.RecordList.Count; i++)
            {
                SingleActionRecord action = roundCountRecord.RecordList[i];

                if (action.SideIndex == BattleRecordConstants.TARGET_SIDE_A)
                {
                    var obj = GetCharacterByAction(action);
                    var cc = obj.GetComponent<CharacterControl>();
                    for (int j = 0; j < action.StateUpdateList.Count; j++)
                    {
                        var state = action.StateUpdateList[j];
                        switch (state.State)
                        {
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    var monster = GetMonsterObject(action);
                    if (monster != null)
                    {
                        var ec = monster.GetComponent<EnemyControl>();
                        ResetMonsterStates(ec, action.StateUpdateList);
                    }
                }
            }
        }
        recordIndex++;
        dealWithRecord();
    }

    //武将颜色和位置变化
    public void showBattleIndexRecord(BattleIndexRecord battleIndexRecord)
    {
        isPlayingRecord = false;
        if (BattleModelLocator.Instance.NextList == null)
        {
            BattleModelLocator.Instance.NextList = battleIndexRecord.FillPointList;
            SetColor();
        }
        else
        {
            BattleModelLocator.Instance.NextList = battleIndexRecord.FillPointList;
            // Remove redurant colors that already taken from attack waiting list.
            // [NOTE] New colors left for selected characters.
            foreach (var wait in attackWaitList)
            {
                BattleModelLocator.Instance.NextList.RemoveAt(0);
            }
        }

        string str = "";
        for (int i = 0; i < BattleModelLocator.Instance.NextList.Count; i++)
        {
            var obj = BattleModelLocator.Instance.NextList[i];
            str += "(" + i.ToString() + ":index=" + obj.Index.ToString() + ",color=" + obj.Color.ToString() + ")";
        }

        //battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_HP)
        Logger.Log("脚下标志 = " + str);
        if (battleIndexRecord.prop.ContainsKey(BattleRecordConstants.BATTLE_HERO_TOTAL_HP))
        {
            characterValue = battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_HP);
        }
        if (battleIndexRecord.prop.ContainsKey(BattleRecordConstants.BATTLE_HERO_TOTAL_MP))
        {
            LeaderCDMax = battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_MP);
            LeaderCD = 0;
        }
        ShowHp();
        ShowMp();
        recordIndex++;
        dealWithRecord();
    }

    //推屏和战斗结束
    public void showBattleEndRecord(BattleEndRecord battleEndRecord)
    {
        //throw new NotImplementedException();
        Logger.Log("战斗结束 = " + battleEndRecord.EndType);
        if (battleEndRecord.EndType == BattleRecordConstants.BATTLE_SCENE_END)
        {
            if (characterValue > 0)
            {
                StartCoroutine(GotoNextScene());
            }
            else
            {
                recordIndex++;
                dealWithRecord();
            }
        }
        else if (battleEndRecord.EndType == BattleRecordConstants.BATTLE_ALL_END)
        {
            int k = battleEndRecord.getIntProp(BattleRecordConstants.BATTLE_END_WIN_SIDE);

            var msg = new CSBattlePveFinishMsg();
            msg.Uuid = BattleModelLocator.Instance.Uuid;

            if (k == BattleRecordConstants.TARGET_SIDE_A)
            {
                msg.BattleResult = 1;
            }
            else
            {
                Star = 0;
                WindowManager.Instance.Show(typeof(BattleLostWindow), true);
                msg.BattleResult = 0;
            }

            isBattling = false;
            recordIndex++;
            dealWithRecord();
            BattleModelLocator.Instance.NextList = null;
            footManager.Clear();
            msg.Star = (sbyte) Star;
            MissionModelLocator.Instance.AddStar(Star);
            MissionModelLocator.Instance.OldExp = PlayerModelLocator.Instance.Exp;
            MissionModelLocator.Instance.OldLevel = PlayerModelLocator.Instance.Level;
            MissionModelLocator.Instance.AddFinishTime(MissionModelLocator.Instance.SelectedStageId);
            NetManager.SendMessage(msg);
        }
        else
        {
            recordIndex++;
            dealWithRecord();
        }
    }

    private void OnBattleResult(bool win)
    {
        var msg = new CSBattlePveFinishMsg();
        msg.Uuid = BattleModelLocator.Instance.Uuid;
        msg.BattleResult = (win) ? 1 : 0;

        isBattling = false;
        recordIndex++;
        dealWithRecord();
        BattleModelLocator.Instance.NextList = null;
        footManager.Clear();
        msg.Star = (sbyte)Star;
        MissionModelLocator.Instance.AddStar(Star);
        MissionModelLocator.Instance.OldExp = PlayerModelLocator.Instance.Exp;
        MissionModelLocator.Instance.OldLevel = PlayerModelLocator.Instance.Level;
        MissionModelLocator.Instance.AddFinishTime(MissionModelLocator.Instance.SelectedStageId);
        NetManager.SendMessage(msg);
    }

    public void showBattleErrorRecord(BattleErrorRecord battleErrorRecord)
    {
        //throw new NotImplementedException();
    }

    private List<IBattleViewRecord> recordList;
    private int recordIndex;
    private bool isPlayingRecord = false;
    private void RequestRecords()
    {
        //Logger.Log("Call RequestRecords");
        recordList = (List<IBattleViewRecord>)BattleModelLocator.Instance.MainBattle.Record.reportRecordListAndClear();
        recordIndex = 0;
        //Logger.Log("Return Count = " + recordList.Count);
        dealWithRecord();
    }

    private void dealWithRecord()
    {
        if (recordIndex < recordList.Count)
        {
            var record = recordList[recordIndex];
            record.show(this);
        }
    }

    private void SetCharacterCanSelect(bool flag)
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                if (obj != null)
                {
                    var cc = obj.GetComponent<CharacterControl>();
                    cc.SetCanSelect(flag);
                }
            }
        }
        for (var i = 0; i < attackWaitList.Count; i++)
        {
            var obj = attackWaitList[i];
            if (obj != null)
            {
                var cc = obj.GetComponent<CharacterControl>();
                cc.SetCanSelect(flag);
            }
        }
    }
}