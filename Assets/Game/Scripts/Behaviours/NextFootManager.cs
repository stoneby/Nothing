using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Next character color manager.
/// </summary>
/// <remarks>Display as colored circles on the left part of screen</remarks>
public class NextFootManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Next color prefab.
    /// </summary>
    public GameObject FootPrefab;

    /// <summary>
    /// Foot list that hold all foot colors.
    /// </summary>
    /// <remarks>
    /// The count should be 2 more greater than Size.
    /// One is the left buffer, the other one is the right buffer.
    /// </remarks>
    public List<GameObject> FootList;

    /// <summary>
    /// Duration of step move from one foot to another.
    /// </summary>
    public float StepMoveDuration;

    /// <summary>
    /// Scale vector of the last game object in foot list.
    /// </summary>
    public Vector3 ScaleVector;

    /// <summary>
    /// Fade value of the last game object in foot list.
    /// </summary>
    public float FadeValue;

    /// <summary>
    /// On stage color list.
    /// </summary>
    /// <remarks>
    /// Should be greater than onStageSize.
    /// Setup at initialize state, on stage but will just hold if there is no enough room for others.
    /// </remarks>
    public List<int> OnStageColorList;

    /// <summary>
    /// Waiting color list that we could not see which behind the stage.
    /// </summary>
    public List<int> WaitingColorList; 

    #endregion

    #region Private Fields

    private bool initialized;

    /// <summary>
    /// Size of foot all we have.
    /// </summary>
    /// <remarks>
    /// On stage size equals to exactly visible foot symbols.
    /// Equals to FootList's size - BufferCount.
    /// </remarks>
    private int onStageSize;

    private List<int> candidateColorList;

    private List<Vector3> positionList;

    private Color defaultColor;
    private Color fadeColor;

    /// <summary>
    /// Buffer count including left and right buffer.
    /// </summary>
    private const int BufferCount = 2;

    #endregion

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize everything well.
    /// </summary>
    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (candidateColorList == null)
        {
            candidateColorList = new List<int>();
        }
        candidateColorList.Clear();

        if (OnStageColorList == null || OnStageColorList.Count == 0)
        {
            Logger.LogError("On stage color list is null or empty, there is nothing foot prefab to spawn with.");
            return;
        }

        onStageSize = FootList.Count - BufferCount;

        if (OnStageColorList.Count < onStageSize)
        {
            Logger.LogError("OnStageColorList size: " + OnStageColorList.Count + " should be greater or equals to onStageSize: " + onStageSize);
            return;
        }

        positionList = new List<Vector3>();
        positionList.AddRange(FootList.Select(item => item.transform.position));

        // get the 1st visible color as default color.
        // since the actual 1st color is fade out to zero.
        defaultColor = FootList[1].GetComponent<UIWidget>().color;
        fadeColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, FadeValue);

        // last one is always inactive, which is only used for stack holder.
        FootList[FootList.Count - 1].SetActive(false);

        // Initialize on stage color.
        SetOnStageSprite();
    }

    /// <summary>
    /// Reset everything to inital status.
    /// </summary>
    public void Reset()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;
        OnStageColorList.Clear();
        WaitingColorList.Clear();
    }

    /// <summary>
    /// Move through all waiting color list.
    /// </summary>
    public void Move()
    {
        candidateColorList.AddRange(WaitingColorList);

        Logger.LogWarning("Ready candidate color list:" + candidateColorList.Count);

        StartCoroutine(DoMove());
    }

    private IEnumerator DoMove()
    {
        var moveCount = WaitingColorList.Count;
        for (var i = 0; i < moveCount; ++i)
        {
            // Set first sprite in hiding list.
            SetSprite(FootList[0], candidateColorList[0]);
            // update candidate color list.
            candidateColorList.RemoveAt(0);

            MoveOneRound();
            yield return new WaitForSeconds(StepMoveDuration);
            ResetOneRound();
        }
    }

    /// <summary>
    /// Move one round in waiting color list.
    /// </summary>
    public void MoveOneRound()
    {
        // iterate all but not including the last one.
        for (var i = 0; i <= onStageSize; ++i)
        {
            var fromObject = FootList[i];
            var toLocation = positionList[i + 1];

            // moving forward.
            iTween.MoveTo(fromObject, toLocation, StepMoveDuration);

            if (i == 0)
            {
                ColorTo(fromObject, defaultColor);
            }

            // the last object should scale and fade out.
            if (i == onStageSize)
            {
                ScaleTo(fromObject, ScaleVector);
                ColorTo(fromObject, fadeColor);
            }
        }
    }

    private void SetSprite(GameObject fromObject, float color)
    {
        var sprite = fromObject.GetComponent<UISprite>();
        sprite.spriteName = "nextfoot_" + color;
    }

    /// <summary>
    /// Setup on stage sprite.
    /// </summary>
    /// <remarks>Wait list first color should be located at right most to the GUI.</remarks>
    public void SetOnStageSprite()
    {
        // set up visiable sprites.
        // set foot list index in between 0.. [1, the one before last]... last one (in foot list game objects).
        for (var i = onStageSize; i >= 1; --i)
        {
            SetSprite(FootList[i], OnStageColorList[onStageSize - i]);
        }

        // holding down colors to candidate color list, which will not show on stage but we hold them.
        for (var i = 0; i < OnStageColorList.Count; ++i)
        {
            if (i >= onStageSize)
            {
                candidateColorList.Add(OnStageColorList[i]);
            }
        }

        Logger.LogWarning("Initial candidate color list:" + candidateColorList.Count);
    }

    private void ScaleTo(GameObject fromObject, Vector3 newScale)
    {
        var scaleTween = fromObject.GetComponent<TweenScale>() ?? fromObject.AddComponent<TweenScale>();
        scaleTween.ResetToBeginning();
        scaleTween.from = fromObject.transform.localScale;
        scaleTween.to = newScale;
        scaleTween.duration = StepMoveDuration;
        scaleTween.PlayForward();
    }

    private void ColorTo(GameObject fromObject, Color newColor)
    {
        var color = fromObject.GetComponent<UIWidget>().color;
        var colorTween = fromObject.GetComponent<TweenColor>() ?? fromObject.AddComponent<TweenColor>();
        colorTween.ResetToBeginning();
        colorTween.from = color;
        colorTween.to = newColor;
        colorTween.duration = StepMoveDuration;
        colorTween.PlayForward();
    }

    /// <summary>
    /// Reset one round move.
    /// </summary>
    public void ResetOneRound()
    {
        // reset last object normalized.
        var lastFoot = FootList[onStageSize];
        lastFoot.transform.position = positionList[0];
        lastFoot.transform.localScale = Vector3.one;

        // circle reuse of foot object.
        FootList.RemoveAt(onStageSize);
        FootList.Insert(0, lastFoot);
    }
}
