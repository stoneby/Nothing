using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Common.Model;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class InitBattleField : MonoBehaviour
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
//    public GameObject PopTextPrefab;

	public GameObject BreakObject;
	public GameObject TextBGObject;
	public GameObject TextObject;
	public GameObject Picture91;
	public GameObject TextBG91;
    public GameObject TexSwardBg91;
	public GameObject Text91;
//    public GameObject[] heads;

    
    public GameObject CharacterAttrackValueLabel;

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
    private bool isInited;
    //private int[] attracks;
    //private int[] enemys;
    private ArrayList attrackWaitList;
    private readonly GameObject[,] charactersLeft = new GameObject[3, 3];
    private GameObject[] enemyList;	//当前敌方数组
    private int currEnemyGroupIndex;	//当前是第几波敌人;
    private bool objHaveBeenStart;
    private bool needCallStartBattle;

    private float realTime;

    private int oldI = -1;
    private int oldJ = -1;

    private ArrayList lineList;

    private const float Tolerance = 0.1f;

    // Use this for initialization
    void Start()
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
		BreakObject.SetActive (false);
		TextBGObject.SetActive (false);
		TextObject.SetActive (false);
		Picture91.SetActive (false);
		TextBG91.SetActive (false);
        Text91.SetActive(false);
        TexSwardBg91.SetActive(false);

        //PopTextManager.Init(GameObject.Find("EffectPanel"), PopTextPrefab);
        InitHpBar();
        InitLeaders();
        InitTopDataBar();
        
        Debug.Log(Screen.height);
    }

    public void StartBattle()
    {
        //attracks = attrackArray;
        //enemys = enemyArray;
        if (!objHaveBeenStart)
        {
            needCallStartBattle = true;
            return;
        }

        if (!isInited)
        {
            isInited = true;
            attrackWaitList = new ArrayList();
        }

        currEnemyGroupIndex = 0;

        leftContainerObj = GameObject.Find("BattleFieldWidgetLeft");

        foreach (var t in BattleModelLocator.Instance.FighterList)
        {
            var obj = NGUITools.AddChild(leftContainerObj, CharacterPrefab);
            attrackWaitList.Add(obj);
            obj.SetActive(false);
            var cc = obj.GetComponent<CharacterControl>();

            cc.SetCharacter(t);
            cc.SetSelect(false);
        }

        var bgObj = GameObject.Find("BackgroundTexture");
        var tp = bgObj.GetComponent<TweenPosition>();
        var tempv = 900 - 640;//(1800 - (Screen.width / + 500)) / 2;
        tp.from = new Vector3(tempv, 0, 0);
        tp.to = new Vector3(tempv, 0, 0);
        tp.PlayForward();

        CreateCurrentEnemys();

       

        characterValue = 15000;
        characterMaxValue = 15000;

        StartCoroutine(MakeUpOneByOne());
        lineList = new ArrayList();
        isBattling = true;
        LeaderCD = 0;
        LeaderCDMax = 50;
        ResetLeaderCd();

        BoxCount = 0;
        FPCount = 0;
        EnergyCount = 0;
        GoldCount = 0;

        ShowHp();
        ShowMp();
        ShowTopData();

        EventManager.Instance.AddListener<LeaderUseEvent>(OnLeaderUseHandler);
    }

    void CreateCurrentEnemys()
    {
        var rightcontainerobj = GameObject.Find("BattleFieldWidgetRight");
        var enemycount = BattleModelLocator.Instance.MonsterGroup[currEnemyGroupIndex];// enemys[currEnemyGroupIndex];
        enemyList = new GameObject[enemycount];
        float xx = BaseRx + HorgapR;
        float yy;
        float tempgap = Vergap;
        switch (enemycount)
        {
            case 1:
                yy = Basey - Vergap - 50;
                break;
            case 2:
                yy = Basey - 50;
                tempgap += Vergap / 2;
                break;
            default:
                yy = Basey;
                tempgap = 3 * Vergap / 4;
                break;
        }
        var offsetx = (currEnemyGroupIndex == 0) ? 0 : 500;
        for (var i = 0; i < enemycount; i++)
        {
            var obj = NGUITools.AddChild(rightcontainerobj, EnemyPrefab);
            var ec = obj.GetComponent<EnemyControl>();
            ec.SetValue(3000, 3000, 2 + i);
            enemyList[i] = obj;
            if (enemycount == 3 && i == 1)
            {
                obj.transform.localPosition =
                    new Vector3(xx + HorgapR + offsetx, yy - tempgap * i, 0);
            }
            else
            {
                obj.transform.localPosition =
                    new Vector3(xx + offsetx, yy - tempgap * i, 0);
            }
        }
    }

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
    IEnumerator MakeUpOneByOne()
    {
        var tempi = Random.Range(0, 3);
        int tempj = Random.Range(0, 3);
        GameObject obj;
        CharacterControl cc;
        TweenPosition tp;
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
                            var t = (k - i) * 0.2f;
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
                        cc.PlayCharacter(2);
                        var t = (2 - i) * 0.2f + 0.2f;
                        if (i == tempi && j == tempj)
                        {
                            cc.ShowSpEffect(true);
                        }
                        cc.SetCharacterAfter(t);

                        tp = charactersLeft[i, j].GetComponent<TweenPosition>();
                        tp.ResetToBeginning();
                        tp.from = new Vector3(BaseLx + HorgapL * i - 300, Basey - Vergap * j, 0);
                        tp.to = new Vector3(BaseLx + HorgapL * i, Basey - Vergap * j, 0);
                        tp.duration = t;
                        tp.PlayForward();
                    }
                    yield return new WaitForSeconds(.15f);
                }
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - realTime > 0.1)
        {
            ResetCharacters();
        }
        realTime = Time.realtimeSinceStartup;

        if (!objHaveBeenStart)
        {
            objHaveBeenStart = true;
            if (needCallStartBattle)
            {
                needCallStartBattle = false;
                StartBattle();
            }
        }

        TouchHandler();

        MoveCharacterUpdate();
    }

    void DrawLine(float oldi, float oldj, float newi, float newj)
    {
        GameObject containerobj = GameObject.Find("BattleFieldPanel");

        GameObject obj = NGUITools.AddChild(containerobj, DragBarPrefab);
        UISprite sp = obj.GetComponent<UISprite>();
        sp.spriteName = BattleType.SelectLines[currentFootIndex];
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
    private ArrayList pointList;
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
            Debug.Log("Mouse Value (" + mx + ", " + my + ")");
            prePoint = GetIndexByPlace(xx, yy);
            if (prePoint.x >= 0 && prePoint.y >= 0 && prePoint.x < 3 && prePoint.y < 3)
            {
                isDraging = true;
                Debug.Log("Nouse Down ------------------------------");
                if (pointList == null) pointList = new ArrayList();
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
        }

        if (Input.GetMouseButtonUp(0))
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

            playSpCount = 0;
            if (pointList != null)
            {
                foreach (var cc in pointList.OfType<GameObject>().Select(obj => obj.GetComponent<CharacterControl>()))
                {
                    cc.SetSelect(false);
                    if (cc.HaveSp)
                    {
                        playSpCount++;
                    }
                }
            }

            xx = xx / CameraAdjuster.CameraScale;
            yy = yy / CameraAdjuster.CameraScale;

            if (xx > minX - 50 && xx < BaseX + 50 && yy > minY - 50 && yy < BaseY + 50)
            {
                DoAttrack();
            }
            else
            {
                if (pointList != null)
                {
                    pointList.Clear();
                    ShowHp();
                    ShowMp();
                }
            }
            Debug.Log("Nouse Up ------------------------------");
        }

        if (isDraging)
        {
            DoDrag(xx, yy);
        }
    }

    void AddAObj(GameObject obj, bool isadd = true)
    {
        var tempcc1 = obj.GetComponent<CharacterControl>();

        if (isadd)
        {
            pointList.Add(obj);
            tempcc1.SetSelect(true, pointList.Count - 1);
            selectEffectList.Add(EffectManager.ShowEffect(EffectType.SelectEffects[currentFootIndex], -25, -30, obj.transform.position));
            PopTextManager.ShowText(tempcc1.AttrackValue.ToString(), 0.6f, -25, 60, 50, obj.transform.localPosition);
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
        if (currentFootIndex == BattleType.FootPink)
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

    //play a attracking actions
    void DoAttrack()
    {
        if (pointList != null && pointList.Count > 0)
        {
            isPlaying = true;
            StartCoroutine(AttrackCoroutineHandler());
        }
    }

    void CharacterLoseBlood(Vector3 pos, bool isnotadd = true)
    {
        if (isnotadd)
        {
            var losevalue = characterValue > 600 ? Random.Range(400, 600) : characterValue;
            characterValue -= losevalue;
            PopTextManager.ShowText("-" + losevalue, 0.6f, -25, 60, 50, pos);
        }
        else
        {
            PopTextManager.ShowText("+" + characterAttrackValue, 0.6f, 80, 100, 50, pos);
        }
        ShowHp();
    }

    void PlayBloodFullEffect()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                EffectManager.PlayEffect(EffectType.BloodFull, .6f, -20, 0, obj.transform.position);
            }
        }
    }

    int GetCurrentEnemyIndex()
    {
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

    void RunToNextEnemys()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                var cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionRun);
                cc.SetCharacterAfter(0.8f);
            }
        }
        GameObject bgObj = GameObject.Find("BackgroundTexture");
        var tp = bgObj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.duration = 0.8f;
        //float tempv = 900 - 640;
        var tempv1 = 900 - 640 - 500 * (currEnemyGroupIndex - 1);//900 - (Screen.width / 2 + 500 * (_currEnemyGroupIndex - 1)) / MainViewController.cameraScale;
        var tempv2 = 900 - 640 - 500 * currEnemyGroupIndex;//900 - (Screen.width / 2 + 500 * _currEnemyGroupIndex) / MainViewController.cameraScale;
        tp.from = new Vector3(tempv1, 0, 0);
        tp.to = new Vector3(tempv2, 0, 0);
        tp.PlayForward();
        for (int i = 0; i < enemyList.Length; i++)
        {
            var obj = enemyList[i];
            var temptp = obj.AddComponent<TweenPosition>();
            temptp.duration = 0.8f;
            temptp.from = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
            temptp.to = new Vector3(obj.transform.localPosition.x - 500, obj.transform.localPosition.y, 0);
            temptp.PlayForward();
        }
    }

    private int playSpCount;

    IEnumerator AttrackCoroutineHandler()
    {
        GameObject obj;
        TweenPosition tp;
        CharacterControl cc;
        int k;

        if (currentFootIndex == BattleType.FootPink)
        {
            characterValue = (characterValue + characterAttrackValue < characterMaxValue) ? characterValue + characterAttrackValue : characterMaxValue;
            CharacterLoseBlood(new Vector3(CharacterHPLabel.transform.localPosition.x - Screen.width / 2,
                                            CharacterHPLabel.transform.localPosition.y - Screen.height / 2, CharacterHPLabel.transform.localPosition.z), false);
            foreach (var t in pointList)
            {
                obj = t as GameObject;
                cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionAttrack);
            }
            yield return new WaitForSeconds(.4f);
            foreach (var t in pointList)
            {
                obj = t as GameObject;
                cc = obj.GetComponent<CharacterControl>();
                cc.PlayCharacter(CharacterType.ActionWait);
            }
            PlayBloodFullEffect();
            yield return new WaitForSeconds(.6f);
            foreach (var t in pointList)
            {
                obj = t as GameObject;

                tp = obj.GetComponent<TweenPosition>();
                cc = obj.GetComponent<CharacterControl>();
                cc.IsActive = false;
                tp.ResetToBeginning();
                tp.@from = new Vector3(BaseLx + HorgapL * cc.XIndex, obj.transform.localPosition.y, 0);
                tp.to = new Vector3(-Screen.width / 2 - 300, obj.transform.localPosition.y, 0);
                tp.duration = .5f;
                tp.PlayForward();
                charactersLeft[cc.XIndex, cc.YIndex] = null;
                attrackWaitList.Add(obj);
                yield return new WaitForSeconds(.1f);
            }
        }
        else
        {
            k = GetCurrentEnemyIndex();
            if (k >= 0)
            {
                var enemy = enemyList[k];
                var ec = enemy.GetComponent<EnemyControl>();
                if (playSpCount > 0)
                {
                    PlaySpEffect(playSpCount);
                    yield return new WaitForSeconds(0.3f * playSpCount + 0.5f);
                }
                const float attracktime = 0.3f;

                if (pointList.Count > 0)
                {
                    obj = pointList[0] as GameObject;
                    cc = obj.GetComponent<CharacterControl>();

                    if (cc.HaveSp)
                    {
                        RunReturn(obj, 0.01f);
                        yield return new WaitForSeconds(AddMoveCharacter(obj, enemy));
                        PlayBeenAttrack(cc, enemy);
                    }
                    else
                    {
                        RunToAttrackPlace(obj, enemy);
                        yield return new WaitForSeconds(attracktime);
                        cc.PlayCharacter(3);
                        yield return new WaitForSeconds(.4f);
                        PlayBeenAttrack(cc, enemy);
                        RunReturn(obj);
                    }
                }

                if (pointList.Count > 1)
                {
                    var thecount = (pointList.Count != 9) ? pointList.Count : pointList.Count - 1;
                    for (var i = 1; i < thecount + 2; i++)
                    {
                        if (i <= thecount - 1)
                        {
                            obj = pointList[i] as GameObject;
                            cc = obj.GetComponent<CharacterControl>();
                            if (cc.HaveSp)
                            {
                                //runReturn(obj, 0.01f);
                                obj.SetActive(false);
                                AddMoveCharacter(obj, enemy);
                                //pointList[i] = null;
                            }
                            else
                            {
                                RunToAttrackPlace(obj, enemy);
                            }
                        }

                        if (i >= 2 && i <= thecount)
                        {
                            obj = pointList[i - 1] as GameObject;
                            if (obj != null)
                            {
                                cc = obj.GetComponent<CharacterControl>();
                                if (!cc.HaveSp) cc.PlayCharacter(3);
                            }
                        }

                        if (i >= 3 && i <= thecount + 1)
                        {
                            obj = pointList[i - 2] as GameObject;
                            if (obj != null)
                            {
                                cc = obj.GetComponent<CharacterControl>();
                                PlayBeenAttrack(cc, enemy);
                                if (cc.HaveSp)
                                {
                                    RunReturn(obj, 0.01f);
                                    obj.SetActive(true);
                                }
                                else
                                {
                                    RunReturn(obj);
                                }
                            }
                        }

                        yield return new WaitForSeconds(0.3f);
                    }
                }

                if (pointList.Count == 9)
                {
                    obj = pointList[pointList.Count - 1] as GameObject;
                    cc = obj.GetComponent<CharacterControl>();
					//yield return new WaitForSeconds (0.5f);//镜头拉近
					PlayMoveCamera(obj);
					cc.Stop();
					yield return new WaitForSeconds (0.2f);//显示遮罩
					Picture91.SetActive (true);
					yield return new WaitForSeconds (0.1f);//播放角色特效
					EffectManager.PlayEffect(EffectType.NineAttrack, 0.9f, -25, 0, obj.transform.position);
					yield return new WaitForSeconds (0.9f);//人物大图飞入
					UITexture tt = EffectObject.GetComponent<UITexture>();
					tt.mainTexture = (Texture2D)Resources.Load(EffectType.LeaderTextures[Random.Range(0, 11)], typeof(Texture2D));
					tt.alpha = 1;
					EffectObject.transform.localPosition = new Vector3(Screen.width / 2 + 300, -80, 0);
					EffectObject.transform.localScale = new Vector3(1, 1, 1);
					EffectObject.SetActive (true);
					
					PlayTweenPosition (EffectObject, 0.3f, new Vector3 (Screen.width / 2 + 300, -80, 0), new Vector3 (0,-80,0));
					yield return new WaitForSeconds (0.3f);//显示文字背景
					PlayTweenPosition(EffectObject, 1.2f, new Vector3 (0,-80,0), new Vector3 (-25,-80,0));
					TextBG91.SetActive(true);
					tt = TextBG91.GetComponent<UITexture>();
					tt.alpha = 1;
					yield return new WaitForSeconds (0.1f);//文字一出现
					Text91.SetActive(true);
					Text91.transform.localScale = new Vector3(5,5,1);
					PlayTweenScale(Text91, 0.2f, new Vector3(5,5,1), new Vector3(1,1,1));
					UILabel lb = Text91.GetComponent<UILabel>();
					lb.alpha = 1;
					yield return new WaitForSeconds (1.0f);//文字消失
					PlayTweenScale(Text91, 0.2f, new Vector3(1,1,1), new Vector3(5,5,1));
					PlayTweenAlpha(Text91, 0.2f, 1, 0);
					PlayTweenAlpha(TextBG91, 0.2f, 1, 0);
					yield return new WaitForSeconds (0.1f);//遮罩和大图消失
					Picture91.SetActive (false);
					PlayTweenAlpha(EffectObject, 0.2f, 1, 0);
					PlayTweenScale(EffectObject, 0.2f, new Vector3(1,1,1), new Vector3(5,5,1));
					yield return new WaitForSeconds (0.2f);//镜头拉回
					Text91.SetActive(false);
					TextBG91.SetActive(false);
					PlayMoveCameraEnd();
					yield return new WaitForSeconds (0.3f);//攻击动作
                    cc.Play();

                    RunToAttrackPlace(obj, enemy);
                    yield return new WaitForSeconds(attracktime);

                    cc.PlayCharacter(3);
                    yield return new WaitForSeconds(attracktime);

                    //9连击播放刀光
                    TexSwardBg91.SetActive(true);
                    //var eftt = EffectBg.GetComponent<UITexture>();
                    //eftt.alpha = 0.1f;

                    cc.Stop();
                    var offset = 0.5f;
                    //Vector3 pos = enemy.transform.position;
                    var pos = new Vector3(0, 0, enemy.transform.position.z);
                    EffectManager.PlayEffect(EffectType.SwordEffect3, 0.5f, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
                    yield return new WaitForSeconds(.1f);
                    
                    ec.PlayBeen();
                    EffectManager.PlayEffect(EffectType.SwordEffect2, 0.5f, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
                    yield return new WaitForSeconds(.1f);
       
                    EffectManager.PlayEffect(EffectType.SwordEffect3, 0.5f, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
                    yield return new WaitForSeconds(.1f);
                    
                    ec.PlayBeen();
                    EffectManager.PlayEffect(EffectType.SwordEffect2, 0.5f, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
                    yield return new WaitForSeconds(.2f);
                   
                    offset = 0.3f;
                    EffectManager.PlayEffect(EffectType.SwordEffect2, 0.5f, 0, 0, new Vector3(pos.x + offset, pos.y - offset, pos.z), 0, true);
                    yield return new WaitForSeconds(.1f);
                   
                    ec.PlayBeen();
                    EffectManager.PlayEffect(EffectType.SwordEffect3, 0.5f, 0, 0, new Vector3(pos.x - offset, pos.y + offset, pos.z), 0, true);
                    yield return new WaitForSeconds(.1f);
                    
                    EffectManager.PlayEffect(EffectType.SwordEffect3, 0.5f, 0, 0, new Vector3(pos.x + offset, pos.y + offset, pos.z), 90, true);
                    yield return new WaitForSeconds(.1f);
                    
                    EffectManager.PlayEffect(EffectType.SwordEffect2, 0.5f, 0, 0, new Vector3(pos.x - offset, pos.y - offset, pos.z), 90, true);
                    yield return new WaitForSeconds(1.0f);
                    TexSwardBg91.SetActive(false);
                   // PlayTweenAlpha(EffectBg, 0.2f, 0.8f, 0);
                    PlayBeenAttrack(cc, enemy, true);

                    yield return new WaitForSeconds(.4f);
                    
                    cc.Play();
                    RunReturn(obj);
                }


                if (ec.LoseBlood(0))
                {
                    Destroy(enemy, 1);
                    enemyList[k] = null;
                    BoxCount++;
                    GoldCount += Random.Range(5, 11);
                }
            }
        }

        k = GetCurrentEnemyIndex();
        LeaderCD += pointList.Count;
        if (LeaderCD > LeaderCDMax) LeaderCD = LeaderCDMax;
        ShowMp();
        if (k < 0)
        {
            //本回合战斗结束,如果有下一回合，准备下一回合，否则关卡战斗结束
            if (currEnemyGroupIndex < BattleModelLocator.Instance.MonsterGroup.Count - 1)
            {
                yield return StartCoroutine(MakeUpOneByOne());
                pointList.Clear();

                currEnemyGroupIndex++;

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

                CreateCurrentEnemys();

                RunToNextEnemys();
                yield return new WaitForSeconds(.8f);

            }
            else
            {
                yield return new WaitForSeconds(.8f);
                //ResetLeaderCd();
                //SubWindowManager.CurrentWindow = SubWindowType.BattleEnd;

                //WindowManager.Instance.Show(WindowType.BattleEnd, true);
                WindowManager.Instance.Show(typeof(BattleWinWindow), true);

                isBattling = false;
            }
        }
        else
        {
            yield return StartCoroutine(MakeUpOneByOne());
            pointList.Clear();
            //回击
            foreach (var t in enemyList)
            {
                var enemy = t;
                if (enemy == null) continue;
                var ec = enemy.GetComponent<EnemyControl>();
                if (ec.CanSpAttrack())
                {
                    PlayMoveCamera(enemy);

                    yield return new WaitForSeconds(0.25f);

                    EffectManager.PlayEffect(EffectType.EnemySprite, 0.7f, 0, 0, enemy.transform.position);

                    yield return new WaitForSeconds(0.9f);
                    PlayMoveCameraEnd();

                    yield return new WaitForSeconds(0.4f);
                    var m = Random.Range(0, 3);
                    for (var j = 0; j < 3; j++)
                    {
                        obj = charactersLeft[j, m];
                        EffectManager.PlayEffect(EffectType.Attrack, 0.8f, -20, -20, obj.transform.position);
                        cc = obj.GetComponent<CharacterControl>();
                        cc.PlayCharacter(1);
                        CharacterLoseBlood(obj.transform.localPosition);
                        iTweenEvent.GetEvent(obj, "ShakeTween").Play();
                    }

                    yield return new WaitForSeconds(.8f);
                    for (var j = 0; j < 3; j++)
                    {
                        obj = charactersLeft[j, m];
                        cc = obj.GetComponent<CharacterControl>();
                        cc.PlayCharacter(0);
                    }
                }
                else if (ec.CanAttrack())
                {
                    yield return new WaitForSeconds(ec.PlayAttrack());
                    obj = charactersLeft[GetRendom(0, 3), GetRendom(0, 3)];
                    EffectManager.PlayEffect(EffectType.Attrack, 0.8f, -20, -20, obj.transform.position);
                    cc = obj.GetComponent<CharacterControl>();
                    cc.PlayCharacter(1);
                    CharacterLoseBlood(obj.transform.localPosition);
                    iTweenEvent.GetEvent(obj, "ShakeTween").Play();
                    yield return new WaitForSeconds(.8f);
                    cc.PlayCharacter(0);
                }
            }
        }

        ResetLeaderCd();
        ShowTopData();
        isPlaying = false;
        if (characterValue <= 0)
        {
            //ResetLeaderCd();

            //SubWindowManager.CurrentWindow = SubWindowType.BattleEnd;

            //WindowManager.Instance.Show(WindowType.BattleEnd, true);
            WindowManager.Instance.Show(typeof(BattleWinWindow), true);

            isBattling = false;
        }
    }

    private static int GetRendom(int min, int max)
    {
        var k = Random.Range(min, max);
        if (k < min) k = min;
        if (k > max - 1) k = max - 1;
        Debug.Log(k);
        return k;
    }

    private static void RunToAttrackPlace(GameObject obj, GameObject enemy)
    {
        var tp = obj.GetComponent<TweenPosition>();
        var cc = obj.GetComponent<CharacterControl>();
        tp.ResetToBeginning();
        tp.from = new Vector3(BaseLx + HorgapL * cc.XIndex, obj.transform.localPosition.y, 0);
        float toy = Random.Range(-70, 70);
        float tox = Random.Range(-80, 0);
        tp.to = new Vector3(enemy.transform.localPosition.x - 80 + tox, enemy.transform.localPosition.y + toy, 0);

        tp.duration = .3f;
        tp.PlayForward();
    }

    private static void PlayBeenAttrack(CharacterControl cc, GameObject enemy, bool showbig = false)
    {
        if (cc.HaveSp || showbig)
        {
            EffectManager.PlayEffect(EffectType.SpriteCollection, 0.8f, 0, -20, enemy.transform.position);
        }
        else
        {
            EffectManager.PlayEffect(EffectType.Attrack, 0.8f, 0, -20, enemy.transform.position);
        }

        var ec = enemy.GetComponent<EnemyControl>();
        if (ec.LoseBlood(cc.AttrackValue) && showbig)
        {
            var obj = GameObject.Find("BattleFieldPanel");
            iTweenEvent.GetEvent(obj, "ShakeTweener").Play();
        }
        else if (cc.HaveSp || showbig)
        {
            ec.PlayBigBeen();
        }
        else
        {
            ec.PlayBeen();
        }
    }

    private void RunReturn(GameObject obj, float runtime = 0.5f)
    {
        CharacterControl cc = obj.GetComponent<CharacterControl>();
        cc.PlayCharacter(0);
        var tp = obj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(50, obj.transform.localPosition.y, 0);
        tp.to = new Vector3(-Screen.width / 2 - 300, obj.transform.localPosition.y, 0);
        tp.duration = runtime;
        tp.PlayForward();
        charactersLeft[cc.XIndex, cc.YIndex] = null;
        attrackWaitList.Add(obj);
    }


    private GameObject tempGameObj;

    private void PlayMoveCamera(GameObject obj)
    {
        tempGameObj = obj;
        var containerobj = GameObject.Find("BattleFieldPanel");
        var tp = containerobj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(0, 0, 0);
        tp.to = new Vector3(-tempGameObj.transform.localPosition.x * 2.5f, -tempGameObj.transform.localPosition.y * 2.5f, 0);
        tp.duration = 0.25f;
        tp.PlayForward();
        var ts = containerobj.GetComponent<TweenScale>();
        ts.ResetToBeginning();
        ts.from = new Vector3(1, 1, 1);
        ts.to = new Vector3(2.5f, 2.5f, 1);
        ts.duration = 0.25f;
        ts.PlayForward();
    }

    private void PlayMoveCameraEnd()
    {
        GameObject containerobj = GameObject.Find("BattleFieldPanel");
        var tp = containerobj.GetComponent<TweenPosition>();
        tp.ResetToBeginning();
        tp.from = new Vector3(-tempGameObj.transform.localPosition.x * 2.5f, -tempGameObj.transform.localPosition.y * 2.5f, 0);
        tp.to = new Vector3(0, 0, 0);
        tp.duration = 0.1f;
        tp.PlayForward();
        var ts = containerobj.GetComponent<TweenScale>();
        while (ts.onFinished.Count > 0) ts.onFinished.RemoveAt(0);
        ts.ResetToBeginning();
        ts.from = new Vector3(2.5f, 2.5f, 1);
        ts.to = new Vector3(1, 1, 1);
        ts.duration = 0.1f;
        ts.PlayForward();
    }

    //get character indexplace by the mouse place
    private Vector2 GetIndexByPlace(float xx, float yy)
    {
        Debug.Log("1. xx:" + xx.ToString() + ",yy: " + yy.ToString());
        xx = xx / CameraAdjuster.CameraScale;
        yy = yy / CameraAdjuster.CameraScale;
        Debug.Log("2. xx:" + xx.ToString() + ",yy: " + yy.ToString());
        Vector2 v2 = new Vector2(-1, -1);
        if (xx > minX && xx < BaseX && yy > minY && yy < BaseY)
        {
            for (var i = 0; i < 3; i++)
            {
                var v = BaseX + OffsetX * i - 55;
//                if (xx > v + OffsetX + 20 && xx <= v - 10)
//                {
//                    v2.x = i;
//                    break;
//                }
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
        Debug.Log("1. i:" + v2.x.ToString() + ",j: " + v2.y.ToString());
        return v2;
    }

    private void StopAll()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                var cc = obj.GetComponent<CharacterControl>();
                cc.Stop();
            }
        }
    }

    private void PlayAll()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var obj = charactersLeft[i, j];
                var cc = obj.GetComponent<CharacterControl>();
                cc.Play();
            }
        }
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
    private void PlaySpEffect(int thecount)
    {
        if (thecount > 4)
        {
            thecount = 4;
        }

        if (spStartList == null)
        {
            spStartList = new ArrayList();
        }

        var cantiner = GameObject.Find("Anchor-left");

        var basey = thecount * 160 / 2 - 300;
        Debug.Log(basey);
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
        float xx = BattleType.PosHPMin + (BattleType.PosHPMax - BattleType.PosHPMin)*characterValue/characterMaxValue;
        SpriteHP1.transform.localPosition = new Vector3(xx, BattleType.PosHPY, 0);
        SpriteHP2.transform.localPosition = new Vector3(xx, BattleType.PosHPY, 0);
        var lb = CharacterHPLabel.GetComponent<UILabel>();
        lb.text = characterValue + "/" + characterMaxValue;
    }

    private void ShowTempHp(float offset)
    {
        if (offset < 0) return;
        float value = characterValue + offset;
        if (value > characterMaxValue) value = characterMaxValue;
        float xx = BattleType.PosHPMin + (BattleType.PosHPMax - BattleType.PosHPMin) * value / characterMaxValue;
        SpriteHP2.transform.localPosition = new Vector3(xx, BattleType.PosHPY, 0);
    }

    private void ShowMp()
    {
        float xx = BattleType.PosMPMin + (BattleType.PosMPMax - BattleType.PosMPMin) * LeaderCD / LeaderCDMax;
        SpriteMP1.transform.localPosition = new Vector3(xx, BattleType.PosMPY, 0);
        SpriteMP2.transform.localPosition = new Vector3(xx, BattleType.PosMPY, 0);
        var lb = CharacterMPLabel.GetComponent<UILabel>();
        lb.text = LeaderCD + "/" + LeaderCDMax;
    }

    private void ShowTempMp(int offset)
    {
        if (offset < 0) return;
        int value = LeaderCD + offset;
        if (value > LeaderCDMax) value = LeaderCDMax;
        float xx = BattleType.PosMPMin + (BattleType.PosMPMax - BattleType.PosMPMin) * value / LeaderCDMax;
        SpriteMP2.transform.localPosition = new Vector3(xx, BattleType.PosMPY, 0);
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
}