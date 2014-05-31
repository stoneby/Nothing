using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.battle.share.input;
using KXSGCodec;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private int characterAttrackValue;
    private GameObject leftContainerObj;
    private float characterMaxValue;
    private float characterValue;

    private const int HorgapL = -170;
    private const int HorgapR = 170;
    private const int Vergap = 160;
    private const int BaseLx = -150;
    private const int BaseRx = 150;
    private const int Basey = 240 + 40 - 60;

    private bool isDraging;
    private bool isPlaying;
    private bool isBattling;
    private NextFootManager footManager;
    private GameObject lineObj;

    //开始一场战斗,attracks攻击的12个武将的数组,enemys敌人方n波敌人的数组
    private List<GameObject> attrackWaitList;
    private readonly GameObject[,] charactersLeft = new GameObject[3, 3];
    private GameObject[] enemyList;	//当前敌方数组
    private int currEnemyGroupIndex;	//当前是第几波敌人;

    private float realTime;

    private int oldI = -1;
    private int oldJ = -1;

    private ArrayList lineList;

    private const float Tolerance = 0.1f;

    public void Init()
    {
        var containerobj = GameObject.Find("BattleFieldPanel");

        lineObj = NGUITools.AddChild(containerobj, DragBarPrefab);
        var sp = lineObj.GetComponent<UISprite>();
        sp.spriteName = "drag_normal";

        lineObj.SetActive(false);

        var obj = GameObject.Find("BattleUIPanel");
        footManager = obj.GetComponent<NextFootManager>();

        centerX = Screen.width / 2;
        centerY = Screen.height / 2;
        BaseX = BaseLx + 30;
        BaseY = Basey + 55;
        minX = BaseX + 3 * OffsetX;
        minY = BaseY + 3 * OffsetY;

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
        if (attrackWaitList == null)
        {
            attrackWaitList = new List<GameObject>();
        }

        leftContainerObj = GameObject.Find("BattleFieldWidgetLeft");

        for (int i = 0; i < BattleModelLocator.Instance.HeroList.Count; i++)
        {
            var t = BattleModelLocator.Instance.HeroList[i];
            var obj = NGUITools.AddChild(leftContainerObj, CharacterPrefab);
            attrackWaitList.Add(obj);
            obj.SetActive(false);
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

        var bg = BattleBG.GetComponent<BattleBGControl>();
        bgIndex++;
        bg.SetData("00" + bgIndex);
        bgIndex = bgIndex%3;
        //PopTextManager.PopTip("Bg Index = " + bgIndex);

        characterValue = 0;
        characterMaxValue = 0;
        
        lineList = new ArrayList();
        isBattling = true;
        LeaderCD = 0;
        LeaderCDMax = 50;
        ResetLeaderCd();

        BoxCount = 0;
        FPCount = 0;
        EnergyCount = 0;
        GoldCount = 0;

        ShowTopData();
        SelectedMonsterIndex = -1;
        EventManager.Instance.AddListener<LeaderUseEvent>(OnLeaderUseHandler);
        currEnemyGroupIndex = 0;
        CreateCurrentEnemys();
        RequestRecords();
        StartCoroutine(InitBattleFighters());
    }

    IEnumerator InitBattleFighters()
    {
        yield return StartCoroutine(MakeUpOneByOne(false));
        RunToNextEnemys();
    }
    
    //创建本关的怪
    void CreateCurrentEnemys()
    {
        Logger.Log("怪物关卡数：" + BattleModelLocator.Instance.MonsterGroup.Count + ",当前关卡索引：" + currEnemyGroupIndex + ",总怪物数：" + BattleModelLocator.Instance.MonsterList.Count);
        var rightcontainerobj = GameObject.Find("BattleFieldWidgetRight");

        var enemycount = BattleModelLocator.Instance.MonsterGroup[currEnemyGroupIndex];// enemys[currEnemyGroupIndex];
        Logger.Log("当前关卡怪物数：" + enemycount);
        enemyList = new GameObject[enemycount];
        float xx = (enemycount > 2) ? BaseRx + HorgapR - 70 : BaseRx + HorgapR;
        float y1 = Basey - Vergap - 40;
        float y2 = Basey - 50;
        float tempgap = Vergap * 1.6f;

        var offsetx = 640;//(currEnemyGroupIndex == 0) ? 0 : 640;
        for (var i = 0; i < enemycount; i++)
        {
            var obj = NGUITools.AddChild(rightcontainerobj, EnemyPrefab);
            
            if (currEnemyGroupIndex == BattleModelLocator.Instance.MonsterGroup.Count - 1)obj.SetActive(false);
            var ec = obj.GetComponent<EnemyControl>();
            ec.Init(OnClickMonsterHanlder, BattleModelLocator.Instance.MonsterList[BattleModelLocator.Instance.MonsterIndex + i]);
            //ec.SetValue();//3000, 3000, 2 + i);
            enemyList[i] = obj;
            var k = (int) Math.Floor((decimal) (i / 2));
            if (i == enemycount - 1 && i % 2 == 0)
            {
                obj.transform.localPosition = new Vector3(xx + HorgapR * k + offsetx, y1, 0);
            }
            else
            {
                obj.transform.localPosition = new Vector3(xx + offsetx + HorgapR * k, y2 - tempgap * (i % 2), 0);
            }
        }
        BattleModelLocator.Instance.MonsterIndex += enemycount;
    }

    private int SelectedMonsterIndex = -1;
    private void OnClickMonsterHanlder(FighterInfo monster)
    {
        SelectedMonsterIndex = monster.Index;
    }

    //战斗结束后销毁
    public void DestroyBattle()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (charactersLeft[i, j] != null)
                {
                    Destroy(charactersLeft[i, j]);
                    charactersLeft[i, j] = null;
                }
            }
        }

        while (attrackWaitList.Count > 0)
        {
            Destroy(attrackWaitList[0] as GameObject);
            attrackWaitList.RemoveAt(0);
        }

        for (var i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i] != null)
            {
                Destroy(enemyList[i]);
                enemyList[i] = null;
            }
        }

        while (pointList != null && pointList.Count > 0)
        {
            if (pointList[0] != null)
            {
                Destroy(pointList[0] as GameObject);
                pointList.RemoveAt(0);
            }
        }

        EventManager.Instance.RemoveListener<LeaderUseEvent>(OnLeaderUseHandler);
    }

    //后面的武将补位
    IEnumerator MakeUpOneByOne(bool needresetcharacter = true)
    {
        TeamController.CharacterList.Clear();

        float runStepTime = (needresetcharacter) ? GameConfig.RunStepNeedTime : GameConfig.ShortTime;
        float runWaitTime = (needresetcharacter) ? GameConfig.NextRunWaitTime : GameConfig.ShortTime;
        GameObject obj;
        CharacterControl cc;
        TweenPosition tp;
        float t = GameConfig.ShortTime;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (charactersLeft[i, j] == null)
                {
                    var flag = true;
                    for (int k = i + 1; k < 3; k++)
                    {
                        if (charactersLeft[k, j] != null)
                        {
                            charactersLeft[i, j] = charactersLeft[k, j];
                            obj = charactersLeft[i, j];
                            cc = obj.GetComponent<CharacterControl>();
                            cc.PlayCharacter(CharacterType.ActionRun);
                            cc.SetIndex(i, j);
                            t = (k - i) * runStepTime;
                            cc.SetCharacterAfter(t);
                            tp = obj.GetComponent<TweenPosition>();
                            tp.ResetToBeginning();
                            tp.from = new Vector3(BaseLx + HorgapL * k, obj.transform.localPosition.y, 0);
                            tp.to = new Vector3(BaseLx + HorgapL * i, obj.transform.localPosition.y, 0);
                            tp.duration = t;
                            tp.PlayForward();
                            charactersLeft[k, j] = null;
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        charactersLeft[i, j] = attrackWaitList[0] as GameObject;
                        attrackWaitList.RemoveAt(0);
                        charactersLeft[i, j].transform.localPosition =
                            new Vector3(BaseLx + HorgapL * i - 300, Basey - Vergap * j, 0);
                        charactersLeft[i, j].SetActive(true);
                        cc = charactersLeft[i, j].GetComponent<CharacterControl>();

                        cc.SetIndex(i, j);
                        cc.SetFootIndex(footManager.GetNext());
                        cc.PlayCharacter(CharacterType.ActionRun);
                        t = (2 - i) * GameConfig.RunStepNeedTime + GameConfig.RunStepNeedTime;
//                        if (i == tempi && j == tempj)
//                        {
//                            cc.ShowSpEffect(true);
//                        }
                        if (needresetcharacter) cc.SetCharacterAfter(t);

                        tp = charactersLeft[i, j].GetComponent<TweenPosition>();
                        tp.ResetToBeginning();
                        tp.from = new Vector3(BaseLx + HorgapL * i - 300, Basey - Vergap * j, 0);
                        tp.to = new Vector3(BaseLx + HorgapL * i, Basey - Vergap * j, 0);
                        tp.duration = t;
                        tp.PlayForward();
                    }
                    yield return new WaitForSeconds(runWaitTime);
                }
            }
        }

        if (needresetcharacter) yield return new WaitForSeconds(t); ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - realTime > 0.1)
        {
            ResetCharacters();
        }
        realTime = Time.realtimeSinceStartup;

        //TouchHandler();

        MoveCharacterUpdate();
    }

    //画一条连接两个武将的线 
    void DrawLine(float oldi, float oldj, float newi, float newj)
    {
        GameObject containerobj = GameObject.Find("BattleFieldPanel");

        GameObject obj = NGUITools.AddChild(containerobj, DragBarPrefab);
        UISprite sp = obj.GetComponent<UISprite>();
        sp.spriteName = BattleTypeConstant.SelectLines[currentFootIndex];
        //NGUITools.AddSprite (containerobj, atlas, "drag_" + currentFootIndex.ToString ());

        Transform oldtrans = charactersLeft[Mathf.CeilToInt(oldi), Mathf.CeilToInt(oldj)].transform;
        Transform newtrans = charactersLeft[Mathf.CeilToInt(newi), Mathf.CeilToInt(newj)].transform;

        float xx = newtrans.localPosition.x - oldtrans.localPosition.x;
        float yy = newtrans.localPosition.y - oldtrans.localPosition.y;

        obj.transform.localPosition =
            new Vector3(oldtrans.localPosition.x + xx / 2 - 25, oldtrans.localPosition.y + yy / 2 - 10, 0);

        //UISprite sp1 = obj.GetComponent<UISprite>();
        float vv = Mathf.Sqrt(xx * xx + yy * yy) + 86;
        sp.width = Mathf.FloorToInt(vv);
        //lineObj.transform.localScale = new Vector3(Mathf.Sqrt (xx * xx + yy * yy) / 160, 1, 1);
        float arrow = Mathf.Atan2(yy, xx);
        arrow = arrow * 18000;
        arrow = arrow / 314;
        obj.transform.localRotation = Quaternion.Euler(0, 0, arrow);
        lineList.Add(obj);
    }

    //draw a line from current item to mouse
    void DrawLine(float xx, float yy)
    {
        xx /= CameraAdjuster.CameraScale;
        yy /= CameraAdjuster.CameraScale;
        Transform oldtrans = charactersLeft[oldI, oldJ].transform;
        xx -= oldtrans.localPosition.x;
        xx += 25;
        yy += 15;
        yy -= oldtrans.localPosition.y;
        lineObj.transform.localPosition =
            new Vector3(oldtrans.localPosition.x + xx / 2 - 25, oldtrans.localPosition.y + yy / 2 - 10, 0);

        var sp = lineObj.GetComponent<UISprite>();
        var vv = Mathf.Sqrt(xx * xx + yy * yy) + 86;
        sp.width = Mathf.FloorToInt(vv);
        var arrow = Mathf.Atan2(yy, xx);
        arrow = arrow * 18000;
        arrow = arrow / 314;
        lineObj.transform.localRotation = Quaternion.Euler(0, 0, arrow);
    }

    //处理鼠标选取
    private float centerX;
    private float centerY;
    private float BaseX;
    private float BaseY;
    private const float OffsetX = -170;
    private const float OffsetY = -160;
    private float minX;
    private float minY;
    private Vector2 currentPoint;
    private Vector2 prePoint;
    private List<GameObject> pointList;
    private ArrayList selectEffectList;
    private int currentFootIndex;

    private void TouchHandler()
    {
        if (isPlaying) return;
        if (!isBattling) return;

        var mx = Input.mousePosition.x;
        var my = Input.mousePosition.y;
        var xx = mx - centerX;
        var yy = my - centerY;

        if (Input.GetMouseButtonDown(0))
        {
            Logger.Log("Mouse Value (" + mx + ", " + my + ")");
            prePoint = GetIndexByPlace(xx, yy);
            if (prePoint.x >= 0 && prePoint.y >= 0 && prePoint.x < 3 && prePoint.y < 3)
            {
                Reset();
            }
        }

        if (Input.GetMouseButtonUp(0) && pointList != null)
        {
            isDraging = false;
            lineObj.SetActive(false);
            while (lineList != null && lineList.Count > 0)
            {
                GameObject temp = lineList[0] as GameObject;
                Destroy(temp);
                lineList.RemoveAt(0);
            }
            UILabel uilb = CharacterAttrackValueLabel.GetComponent<UILabel>();
            uilb.text = "";

            while (selectEffectList != null && selectEffectList.Count > 0)
            {
                Destroy(selectEffectList[0] as GameObject);
                selectEffectList.RemoveAt(0);
            }

            xx = xx / CameraAdjuster.CameraScale;
            yy = yy / CameraAdjuster.CameraScale;

            var _indexArr = new int[pointList.Count];
            for (int k = 0; k < pointList.Count; k++)
            {
                var tempcc = pointList[k].GetComponent<CharacterControl>();
                tempcc.SetSelect(false);
                _indexArr[k] = tempcc.XIndex * 3 + tempcc.YIndex;
            }

            if (xx > minX - 50 && xx < BaseX + 50 && yy > minY - 50 && yy < BaseY + 50)
            {
                DoAttack(_indexArr);
            }
            else
            {
                if (pointList != null)
                {
                    pointList.Clear();
                    ShowHp();
                    ShowMp();
                }
                //PopTextManager.PopTip("取消攻击");
            }
            Logger.Log("Mouse Up ------------------------------");
        }

        if (isDraging)
        {
            DoDrag(xx, yy);
        }
    }

    private void Reset()
    {
        isDraging = true;
        Logger.Log("Nouse Down ------------------------------");
        if (pointList == null) pointList = new List<GameObject>();
        if (selectEffectList == null) selectEffectList = new ArrayList();
        selectEffectList.Clear();
        pointList.Clear();
        while (lineList.Count > 0)
        {
            GameObject temp = lineList[0] as GameObject;
            Destroy(temp);
            lineList.RemoveAt(0);
        }
        characterAttrackValue = 0;

        GameObject obj = charactersLeft[Mathf.CeilToInt(prePoint.x), Mathf.CeilToInt(prePoint.y)];
        CharacterControl cc = obj.GetComponent<CharacterControl>();
        currentFootIndex = cc.FootIndex;

        AddAObj(obj);

        oldI = Mathf.CeilToInt(prePoint.x);
        oldJ = Mathf.CeilToInt(prePoint.y);
        lineObj.SetActive(true);
    }

    private void DoAttack(int[] _indexArr)
    {
        //调取服务器战斗
        var _action = new ProduceFighterIndexAction();
        _action.HeroIndex = _indexArr;
        var enemyindex = GetCurrentEnemyIndex();
        if (enemyindex >= 0)
        {
            var enemy = enemyList[enemyindex];
            var ec = enemy.GetComponent<EnemyControl>();

            _action.TargetIndex = ec.Data.Index;
            BattleModelLocator.Instance.MainBattle.handleBattleEvent(_action);
            RequestRecords();
        }
    }

    //增加一个选择的武将
    void AddAObj(GameObject obj, bool isadd = true)
    {
        var tempcc1 = obj.GetComponent<CharacterControl>();

        if (isadd)
        {
            pointList.Add(obj);
            tempcc1.SetSelect(true, pointList.Count - 1);
            selectEffectList.Add(EffectManager.ShowEffect(EffectType.SelectEffects[currentFootIndex], -25, -30, obj.transform.position));
            //PopTextManager.ShowText(tempcc1.AttrackValue.ToString(), 0.6f, -25, 60, 50, obj.transform.localPosition);
        }
        else
        {
            pointList.RemoveAt(pointList.Count - 1);
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
        ShowTempMp(pointList.Count);
    }

    //handle sth when mouse is picking characters
    void DoDrag(float xx, float yy)
    {
        var currpoint = GetIndexByPlace(xx, yy);
        var flag = false;
        if (currpoint.x >= 0 && currpoint.y >= 0 && currpoint.x < 3 && currpoint.y < 3 &&
            (Math.Abs(currpoint.x - prePoint.x) > Tolerance || Math.Abs(currpoint.y - prePoint.y) > Tolerance) &&
            Mathf.Abs(currpoint.x - prePoint.x) < 2 && Mathf.Abs(currpoint.y - prePoint.y) < 2)
        {

            GameObject obj = charactersLeft[Mathf.CeilToInt(currpoint.x), Mathf.CeilToInt(currpoint.y)];
            var cc = obj.GetComponent<CharacterControl>();
            if (currentFootIndex == cc.FootIndex)
            {
                flag = true;
                if (pointList.Count > 1)
                {
                    var tempobj = pointList[pointList.Count - 2] as GameObject;
                    var tempcc = tempobj.GetComponent<CharacterControl>();
                    if (Math.Abs(tempcc.XIndex - currpoint.x) < Tolerance && Math.Abs(tempcc.YIndex - currpoint.y) < Tolerance)
                    {
                        AddAObj(pointList[pointList.Count - 1] as GameObject, false);

                        var temp = lineList[lineList.Count - 1] as GameObject;

                        lineList.RemoveAt(lineList.Count - 1);
                        Destroy(temp);
                        prePoint = currpoint;
                        oldI = Mathf.CeilToInt(prePoint.x);
                        oldJ = Mathf.CeilToInt(prePoint.y);
                        flag = false;
                    }
                    else
                    {
                        foreach (var t in pointList)
                        {
                            tempobj = t as GameObject;
                            tempcc = tempobj.GetComponent<CharacterControl>();
                            if (Math.Abs(tempcc.XIndex - currpoint.x) < Tolerance && Math.Abs(tempcc.YIndex - currpoint.y) < Tolerance)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                AddAObj(charactersLeft[Mathf.CeilToInt(currpoint.x), Mathf.CeilToInt(currpoint.y)]);
                DrawLine(prePoint.x, prePoint.y, currpoint.x, currpoint.y);
                prePoint = currpoint;
                oldI = Mathf.CeilToInt(prePoint.x);
                oldJ = Mathf.CeilToInt(prePoint.y);
            }
        }
        DrawLine(xx, yy);
    }

    //左侧部队攻击
    void DoAttrackLeft()
    {
        if (pointList != null && pointList.Count > 0)
        {
            isPlaying = true;
            StartCoroutine(LeftAttrackCoroutineHandler());
        }
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
            if (losevalue > 0) PopTextManager.ShowText("-" + losevalue, 0.6f, -25, 60, 50, pos);
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

    //获取当前怪的索引
    int GetCurrentEnemyIndex()
    {
        if (SelectedMonsterIndex >= 0 && enemyList[SelectedMonsterIndex] != null)
        {
            return SelectedMonsterIndex;
        }
        var m = -1;
        for (var k = 0; k < enemyList.Length; k++)
        {
            if (enemyList[k] != null)
            {
                m = k;
                break;
            }
        }
        return m;
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
        
        for (int i = 0; i < enemyList.Length; i++)
        {
            var obj = enemyList[i];
            var temptp = obj.AddComponent<TweenPosition>();
            temptp.duration = GameConfig.RunRoNextMonstersTime;
            temptp.from = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
            temptp.to = new Vector3(obj.transform.localPosition.x - 640, obj.transform.localPosition.y, 0);
            temptp.PlayForward();
        }
    }

    //获取该action的怪
    private GameObject GetMonsterObject(SingleActionRecord record)
    {
        for (int i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i] == null)continue;
            var ec = enemyList[i].GetComponent<EnemyControl>();
            if (ec.Data.Index == record.Index)
            {
                return enemyList[i];
            }
        }
        return null;
    }

    //private const float AttrackTime = 0.3f;
    //播放武将的一次出手动作
    private IEnumerator PlayOneAction(BattleFightRecord record)
    {
        GameObject obj;
        GameObject monster;
        TweenPosition tp;
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
        TweenPosition tp;
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
                for (var i = 0; i < enemyList.Length; i++)
                {
                    if (enemyList[i] == null)continue;
                    var ec = enemyList[i].GetComponent<EnemyControl>();
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
            obj = pointList[pointList.Count - 1] as GameObject;
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
        yield return new WaitForSeconds(GameConfig.TotalHeroAttrackTime);
        CheckMonsterDead();
        yield return StartCoroutine(MakeUpOneByOne());
        pointList.Clear();

        for (var i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i] == null) continue;
            var ec = enemyList[i].GetComponent<EnemyControl>();
            ec.ShowBlood(true);
        }
        ShowTopData();
        isPlaying = false;
        isPlayingRecord = false;
        recordIndex++;
        dealWithRecord();
    }

    //判断怪是否死亡
    private void CheckMonsterDead()
    {
        for (int i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i] == null) continue;
            var ec = enemyList[i].GetComponent<EnemyControl>();
            if (ec.HP <= 0)
            {
                var v = ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_COIN);
                if (v > 0)
                {
                    GoldCount += v;
                    EffectManager.PlayEffect(EffectType.GetMoney, 0.5f, 0, 0, enemyList[i].transform.position);
                }
                
                EnergyCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_HERO);
                BoxCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_ITEM);
                FPCount += ec.Data.getIntProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DROP_SPRIT);
                if (ec.Data.Index == SelectedMonsterIndex)
                {
                    SelectedMonsterIndex = -1;
                }
                ec.OnDestory();
                Destroy(enemyList[i]);
                enemyList[i] = null;
                ShowTopData();
            }
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
        var tp = obj.GetComponent<TweenPosition>();
        var cc = obj.GetComponent<CharacterControl>();
        tp.ResetToBeginning();
        tp.from = new Vector3(BaseLx + HorgapL * cc.XIndex, obj.transform.localPosition.y, 0);
        float toy = Random.Range(-70, 70);
        float tox = Random.Range(-80, 0);
        tp.to = new Vector3(enemy.transform.localPosition.x - 80 + tox, enemy.transform.localPosition.y + toy, 0);

        tp.duration = GameConfig.RunToAttrackPosTime;
        tp.PlayForward();
    }

    //播放怪物的受击
    private void PlayEnemyBeenAttrack(CharacterControl cc, List<SingleActionRecord> enemylist, bool showbig = false)
    {
        for (int i = 0; i < enemylist.Count; i++)
        {
            var action = enemylist[i];
            var enemy = GetMonsterObject(action);
            if (enemy == null) continue;
            if (showbig)
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
//            else if (cc.HaveSp || showbig)
//            {
//                ec.PlayBigBeen();
//            }
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

    //武将攻击后返回等待队列
    private void RunReturn(GameObject obj, float runtime)
    {
        var cc = obj.GetComponent<CharacterControl>();
        cc.PlayCharacter(0);
        var tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = IsRecover ? new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0) : 
            new Vector3(50, obj.transform.localPosition.y, 0);
        tp.to = new Vector3(-Screen.width / 2 - 300, obj.transform.localPosition.y, 0);
        tp.duration = runtime;
        tp.PlayForward();
        charactersLeft[cc.XIndex, cc.YIndex] = null;
        attrackWaitList.Add(obj);
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

    //get character indexplace by the mouse place
    private Vector2 GetIndexByPlace(float xx, float yy)
    {
        Logger.Log("1. xx:" + xx.ToString() + ",yy: " + yy.ToString());
        xx = xx / CameraAdjuster.CameraScale;
        yy = yy / CameraAdjuster.CameraScale;
        Logger.Log("2. xx:" + xx.ToString() + ",yy: " + yy.ToString());
        Vector2 v2 = new Vector2(-1, -1);
        if (xx > minX && xx < BaseX && yy > minY && yy < BaseY)
        {
            for (var i = 0; i < 3; i++)
            {
                var v = BaseX + OffsetX * i - 55;
                if (xx > v + OffsetX / 2 && xx <= v - OffsetX / 2)
                {
                    v2.x = i;
                    break;
                }
            }

            for (var j = 0; j < 3; j++)
            {
                float v = BaseY + OffsetY * j;
                if (yy > v + OffsetY && yy <= v + OffsetY / 3)
                {
                    v2.y = j;
                    break;
                }
            }
        }
        Logger.Log("1. i:" + v2.x.ToString() + ",j: " + v2.y.ToString());
        return v2;
    }

    //下面的函数用来处理队长
    private void OnLeaderUseHandler(LeaderUseEvent e)
    {
        LeaderCD -= e.CDCount;
        if (LeaderCD >= 0)
        {
            InvokeLeaderSkill(e.SkillIndex);
        }
        else
        {
            LeaderCD = 0;
        }
        
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
        lc.Init(0, 0, 0);
        leaders.Add(obj);
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

    public void InvokeLeaderSkill(int theindex)
    {
        if (!isBattling) return;
        StartCoroutine(PlayLeaderEffect());

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                if (obj != null)
                {
                    var cc = obj.GetComponent<CharacterControl>();
                    cc.SetFootIndex(theindex);
                }
            }
        }
    }

    IEnumerator PlayLeaderEffect()
    {
        EffectManager.PlayAllEffect(false);
        GameObject effectbg = EffectBg;
        effectbg.SetActive(true);
        var tt = effectbg.GetComponent<UITexture>();
        tt.alpha = 0.9f;

		GameObject effectobj = EffectObject;
		tt = effectobj.GetComponent<UITexture>();
		tt.mainTexture = (Texture2D)Resources.Load(EffectType.LeaderTextures[Random.Range(0, 11)], typeof(Texture2D));
		effectobj.transform.localPosition = new Vector3 (0,0,0);
		effectobj.transform.localScale = new Vector3 (5,5,1);
		tt.alpha = 0.1f;
		effectobj.SetActive (true);

		PlayTweenScale (effectobj, 0.2f, new Vector3 (5, 5, 1), new Vector3 (1,1,1));
		PlayTweenAlpha (effectobj, 0.2f, 0.1f, 1);

		yield return new WaitForSeconds (0.2f);
		TextBGObject.SetActive (true);
		tt = TextBGObject.GetComponent<UITexture>();
		tt.alpha = 1;

		PlayTweenScale (effectobj, 1.0f, new Vector3 (1, 1, 1), new Vector3 (0.9f, 0.9f, 1));


		TextObject.transform.localScale = new Vector3 (5,5,1);
		UILabel lb = TextObject.GetComponent<UILabel>();
		lb.alpha = 1;
		TextObject.SetActive (true);

		PlayTweenScale (TextObject, 0.2f, new Vector3 (5,5,1), new Vector3 (1,1,1));
		yield return new WaitForSeconds (0.2f);

		PlayTweenScale (TextObject, 0.8f, new Vector3 (1,1,1), new Vector3 (0.9f, 0.9f, 1));

		yield return new WaitForSeconds (0.8f);

		BreakObject.SetActive (true);
		UITexture tt1 = BreakObject.GetComponent<UITexture>();
		BreakObject.transform.localScale = new Vector3 (1,1,1);
		tt1.alpha = 0.9f;

		yield return new WaitForSeconds (.1f);
		PlayTweenAlpha (effectbg, 0.3f, 0.9f, 0);

		PlayTweenScale (effectobj, 0.3f, new Vector3 (1,1,1), new Vector3 (5,5,1));
		PlayTweenAlpha (effectobj, 0.3f, 1, 0.1f);

		PlayTweenAlpha (BreakObject, 0.3f, 1, 0);
		PlayTweenScale (BreakObject, 0.3f, new Vector3 (1,1,1), new Vector3(5, 5, 1));

		PlayTweenAlpha (TextBGObject, 0.3f, 1, 0);

		PlayTweenAlpha (TextObject, 0.2f, 1, 0);

		yield return new WaitForSeconds (.4f);
		effectobj.SetActive (false);
		BreakObject.SetActive(false);
		effectbg.SetActive (false);
		TextBGObject.SetActive (false);
        ResetLeaderCd();
        ShowMp();
        EffectManager.PlayAllEffect(true);
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
        if (charactersLeft == null || attrackWaitList == null) return;

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
        foreach (var t in attrackWaitList)
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
        vo.Init(cc.GetNamePrefix(), new Vector3(BaseLx + HorgapL * cc.XIndex, obj.transform.localPosition.y, 0),
                 new Vector3(enemy.transform.localPosition.x, enemy.transform.localPosition.y, 0));
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
		ta.PlayForward ();
		Destroy (ta, playtime);
	}

	void PlayTweenScale(GameObject obj, float playtime, Vector3 from, Vector3 to)
	{
		TweenScale ts = obj.AddComponent<TweenScale>();
		ts.from = from;
		ts.to = to;
		ts.duration = playtime;
		ts.PlayForward ();
		Destroy (ts, playtime);
	}

	void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
	{
		TweenPosition ts = obj.AddComponent<TweenPosition>();
		ts.from = from;
		ts.to = to;
		ts.duration = playtime;
		ts.PlayForward ();
		Destroy (ts, playtime);
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
        SelectedMonsterIndex = -1;
        yield return StartCoroutine(MakeUpOneByOne());
        pointList.Clear();

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
                for (int i = 0; i < enemyList.Length; i++)
                {
                    var obj = enemyList[i] as GameObject;
                    obj.SetActive(true);
                }
                warningObj = EffectManager.ShowEffect(EffectType.Warning, 0, 0, new Vector3(0, 0, 0));
                yield return new WaitForSeconds(1.8f);
                PlayWarningEnd();
                yield return new WaitForSeconds(.4f);
                HideWaring();
            }
            ShowTopData();
        }
        recordIndex++;
        dealWithRecord();
    }

    //下面是处理服务器返回数据用的接口
    //技能
    public void showBattleSkillRecord(BattleSkillRecord battleSkillRecord)
    {
        //throw new NotImplementedException();
        return;
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
        //throw new NotImplementedException();
        isPlayingRecord = false;
        if (BattleModelLocator.Instance.NextList == null)
        {
            BattleModelLocator.Instance.NextList = battleIndexRecord.FillPointList;
        }
        else
        {
            BattleModelLocator.Instance.NextList = battleIndexRecord.FillPointList;
            BattleModelLocator.Instance.NextList.RemoveAt(0);
            BattleModelLocator.Instance.NextList.RemoveAt(0);
            BattleModelLocator.Instance.NextList.RemoveAt(0);
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
            LeaderCD = battleIndexRecord.getIntProp(BattleRecordConstants.BATTLE_HERO_TOTAL_MP);
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
            Logger.Log("战斗结束 =推推推推推推推平 ");

        }
        else if (battleEndRecord.EndType == BattleRecordConstants.BATTLE_ALL_END)
        {
            int k = battleEndRecord.getIntProp(BattleRecordConstants.BATTLE_END_WIN_SIDE);

            var msg = new CSBattlePveFinishMsg();
            msg.Uuid = BattleModelLocator.Instance.Uuid;

            if (k == BattleRecordConstants.TARGET_SIDE_A)
            {
                WindowManager.Instance.Show(typeof(BattleWinWindow), true);
                msg.BattleResult = 1;
            }
            else
            {
                WindowManager.Instance.Show(typeof(BattleLostWindow), true);
                msg.BattleResult = 0;
            }

            isBattling = false;
            recordIndex++;
            dealWithRecord();
            BattleModelLocator.Instance.NextList = null;
            Logger.Log("战斗结束 = 结结结结结结结结结结结束 ");

            msg.Star = 2;
            NetManager.SendMessage(msg);
        }
        else
        {
            recordIndex++;
            dealWithRecord();
        }
        
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
        for (var i = 0; i < attrackWaitList.Count; i++)
        {
            var obj = attrackWaitList[i];
            if (obj != null)
            {
                var cc = obj.GetComponent<CharacterControl>();
                cc.SetCanSelect(flag);
            }
        }
    }
}