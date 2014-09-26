using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Control object's movement with packaging iTween.
/// Modify MonsterFace object's script if you want to modify lock movement.
/// Modify GreenHandGuide object 's script if you want to modify direct and shake movement.
/// </summary>
public class ObjectMoveController : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The object to move.
    /// </summary>
    public GameObject CurrentObject;

    /// <summary>
    /// Flag to loop move or not.
    /// </summary>
    public bool IsLoop;

    /// <summary>
    /// The time to move after every movement.
    /// </summary>
    public float WaitTime;

    /// <summary>
    /// Speed for direct move.
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// Speed for shake move.
    /// </summary>
    public float ShakeSpeed;

    /// <summary>
    /// Shake movement range.
    /// </summary>
    public Vector3 MoveGap;

    /// <summary>
    /// Time for rotate move 120 degrees.
    /// </summary>
    public float RotateTime;

    /// <summary>
    /// Time for zoom change a period of scale.
    /// </summary>
    public float ZoomTime;

    /// <summary>
    /// Zoom movement scale.
    /// </summary>
    public float ZoomScale;

    #endregion

    #region Private Fields

    /// <summary>
    /// Direct move track.
    /// </summary>
    private List<Vector3> moveTraceList = new List<Vector3>();

    /// <summary>
    /// Used to play particle effect.
    /// </summary>
    private EffectController effectController;

    #endregion

    #region Public Methods

    /// <summary>
    /// Move object with direct trace, execute a coroutine.
    /// </summary>
    /// <param name="loop">flag to loop move or not</param>
    /// <param name="moveTrace">direct move track</param>
    public void DirectMove(bool loop, List<Vector3> moveTrace)
    {
        if (moveTrace.Count == 0)
        {
            Logger.LogError("Not correct para nums in ObjectMoveController.DirectMove");
            return;
        }
        IsLoop = loop;
        moveTraceList = moveTrace;
        CurrentObject.SetActive(true);
        StartCoroutine("DoDirectMove");
    }

    /// <summary>
    /// This is a animation or particle system play.
    /// </summary>
    /// <param name="loop">flag to loop move or not</param>
    /// <param name="pos">effect play position</param>
    public void EffectPlay(bool loop, Vector3 pos)
    {
        IsLoop = loop;
        moveTraceList.Clear();
        moveTraceList.Add(pos);
        CurrentObject.SetActive(true);
        CurrentObject.transform.position = moveTraceList[0];

        ////Set render queue for display correct.
        //var setRenderQueue = CurrentObject.GetComponent<SetRenderQueue>() ??
        //                     CurrentObject.AddComponent<SetRenderQueue>();
        //setRenderQueue.RenderQueue = RenderQueue.ToppestEffect;

        //Play animation or particle system.
        effectController = CurrentObject.GetComponent<EffectController>();
        effectController.Play(true);
    }

    /// <summary>
    /// Shake object with moveGap variable, execute a coroutine.
    /// </summary>
    /// <param name="loop">flag to loop move or not</param>
    /// <param name="pos">object shake start position</param>
    public void ShakeMove(bool loop, Vector3 pos)
    {
        IsLoop = loop;
        moveTraceList.Clear();
        moveTraceList.Add(pos);
        CurrentObject.SetActive(true);
        StartCoroutine("DoShakeMove");
    }

    /// <summary>
    /// Lock movement consisted by rotation and zooming, execute 2 coroutines.
    /// </summary>
    /// <param name="loop">flag to loop move or not</param>
    /// <param name="pos">object lock move start position</param>
    public void LockMove(bool loop, Vector3 pos)
    {
        IsLoop = loop;
        CurrentObject.transform.position = pos;
        CurrentObject.SetActive(true);
        StartCoroutine("DoRotateMove");
        StartCoroutine("DoZoomMove");
    }

    /// <summary>
    /// Stop all move by stopping all coroutines and deactive object.
    /// </summary>
    public void StopMove()
    {
        IsLoop = false;
        StopCoroutine("DoDirectMove");
        if (effectController)
        {
            effectController.Stop();
        }
        StopCoroutine("DoShakeMove");
        StopCoroutine("DoRotateMove");
        StopCoroutine("DoZoomMove");
        if (CurrentObject)
        {
            CurrentObject.SetActive(false);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Do direct move coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoDirectMove()
    {
        do
        {
            CurrentObject.transform.position = moveTraceList[0];

            for (int i = 0; i < moveTraceList.Count - 1; i++)
            {
                var distance = Vector3.Distance(moveTraceList[i], moveTraceList[i + 1]);
                iTween.MoveTo(CurrentObject, iTween.Hash("position", moveTraceList[i + 1], "time", distance / MoveSpeed, "easetype", iTween.EaseType.linear));
                yield return new WaitForSeconds(distance / MoveSpeed);
            }

            yield return new WaitForSeconds(WaitTime);
        } while (IsLoop);
    }

    /// <summary>
    /// Do shake move coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoShakeMove()
    {
        do
        {
            CurrentObject.transform.position = moveTraceList[0];

            float distance = Vector3.Distance(CurrentObject.transform.position, moveTraceList[0] + MoveGap);
            iTween.MoveTo(CurrentObject, moveTraceList[0] + MoveGap, distance / ShakeSpeed);
            yield return new WaitForSeconds(distance / ShakeSpeed);

            distance = Vector3.Distance(CurrentObject.transform.position, moveTraceList[0] + MoveGap);
            iTween.MoveTo(CurrentObject, moveTraceList[0] - MoveGap, distance / ShakeSpeed);
            yield return new WaitForSeconds(distance / ShakeSpeed);
        } while (IsLoop);
    }

    /// <summary>
    /// Do rotate move coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoRotateMove()
    {
        do
        {
            iTween.RotateTo(CurrentObject, iTween.Hash("rotation", new Vector3(0, 0, 120), "time", RotateTime, "easetype", iTween.EaseType.linear));
            yield return new WaitForSeconds(RotateTime);

            iTween.RotateTo(CurrentObject, iTween.Hash("rotation", new Vector3(0, 0, 240), "time", RotateTime, "easetype", iTween.EaseType.linear));
            yield return new WaitForSeconds(RotateTime);

            iTween.RotateTo(CurrentObject, iTween.Hash("rotation", new Vector3(0, 0, 360), "time", RotateTime, "easetype", iTween.EaseType.linear));
            yield return new WaitForSeconds(RotateTime);
        } while (IsLoop);
    }

    /// <summary>
    /// Do zoom move coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoZoomMove()
    {
        do
        {
            iTween.ScaleTo(CurrentObject, iTween.Hash("scale", new Vector3(ZoomScale, ZoomScale, 0), "time", ZoomTime, "easetype", iTween.EaseType.linear));
            yield return new WaitForSeconds(ZoomTime);

            iTween.ScaleTo(CurrentObject, iTween.Hash("scale", new Vector3(1, 1, 0), "time", ZoomTime, "easetype", iTween.EaseType.linear));
            yield return new WaitForSeconds(ZoomTime);
        } while (IsLoop);
    }

    #endregion
}
