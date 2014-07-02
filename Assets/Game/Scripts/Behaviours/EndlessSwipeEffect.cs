using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
[RequireComponent(typeof(ScreenRaycaster))]
[RequireComponent(typeof(DragRecognizer))]
public class EndlessSwipeEffect : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The objects which will trigger swipe effect.
    /// </summary>
    public List<GameObject> DragObjects;

    /// <summary>
    /// The time of bouce animation costs.
    /// </summary>
    public float BounceTime = 0.2f;

    /// <summary>
    /// The time of move per item.
    /// </summary>
    public float MoveTime = 0.8f;    

    public delegate void OnUpdateData();

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    public OnUpdateData UpdateData;

    /// <summary>
    /// The transfrom of current item.
    /// </summary>
    public Transform CurrentItem
    {
        get
        {
            if(curCustomIndex == 0)
            {
                return itemList[0];
            }
            if(curCustomIndex == customDataCount -1)
            {
                return itemList[2];
            }

            return itemList[1];
        }
    }

    #endregion

    #region Private Fileds

    private readonly List<Transform> itemList = new List<Transform>();
    private int cellCount;
    private UIGrid cachedGrid;
    private int curCustomIndex = -1;
    private int customDataCount = -1;
    int dragFingerIndex = -1;
    private Vector3 cachedLocalPosition;
    private Vector3 cachedDragPosition;
    private GameObject selection;
    private float cellWidth;
    private bool isMoving = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// The index of current custom data in all custom data list.
    /// </summary>
    public int CurCustomIndex
    {
        get { return curCustomIndex; }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Awake()
    {
        GetComponent<ScreenRaycaster>().Cameras[0] = UICamera.currentCamera;
        GetComponent<DragRecognizer>().EventMessageName = "OnGestureDrag";
    }

    /// <summary>
    /// Move the game object to target local position.
    /// </summary>
    /// <param name="localPosition">The target local position.</param>
    /// <param name="left">If true, indicates it moves from right to left.</param>
    private void MoveTo(Vector3 localPosition, bool left)
    {
        isMoving = true;
        if (left)
        {
            iTween.MoveTo(gameObject,
                          iTween.Hash("position", localPosition,
                                      "islocal", true,
                                      "time", MoveTime,
                                      "oncomplete", "OnComplete"));
        }
        else
        {
            iTween.MoveTo(gameObject,
                          iTween.Hash("position", localPosition,
                                      "islocal", true,
                                      "time", MoveTime,
                                      "oncomplete", "OnComplete"));
        }
    }

    /// <summary>
    /// Reset the isMoving varible.
    /// </summary>
    private void OnComplete()
    {
        isMoving = false;
    }

    /// <summary>
    /// Start to play the bounce effect.
    /// </summary>
    /// <param name="position">Bouce to the target position.</param>
    private void BeginBounce(Vector3 position)
    {
        isMoving = true;
        iTween.MoveTo(gameObject,
                      iTween.Hash(
                          "position", position,
                          "islocal", true,
                          "easetype", iTween.EaseType.spring,
                          "time", BounceTime,
                          "oncomplete", "OnComplete"));
    }

    /// <summary>
    /// Adjust postion of all items.
    /// </summary>
    /// <param name="isLeft">If true, indicates it moves from right to left.</param>
    private void AdjustPostion(bool isLeft)
    {
        var itemIndex = isLeft ? 0 : cellCount - 1;
        var itemToMove = itemList[itemIndex];
        var offset = cellWidth * cellCount * (isLeft ? Vector3.right : Vector3.left);
        itemToMove.localPosition += offset;
        itemList.RemoveAt(itemIndex);
        itemList.Insert(cellCount - 1 - itemIndex, itemToMove);
    }

    /// <summary>
    /// The call back of drag gesture.
    /// </summary>
    /// <param name="gesture">The drag gesture.</param>
    void OnGestureDrag(DragGesture gesture)
    {
        //If the last swipe move is not finished, we don't reponse to the gesture.
        if(isMoving)
        {
            return;
        }
        // first finger
        FingerGestures.Finger finger = gesture.Fingers[0];
        if(gesture.Phase == ContinuousGesturePhase.Started)
        {
            selection = gesture.Selection;
            // dismiss this event if we're not interacting with our drag object
            if(!DragObjects.Contains(selection))
            {
                return;
            }
            // remember which finger is dragging dragObject
            dragFingerIndex = finger.Index;
            cachedLocalPosition = transform.localPosition;
            cachedDragPosition = Utils.GetWorldPos(UICamera.currentCamera, gesture.Position);
        }
        // gesture in progress, make sure that this event comes from the finger that is dragging our dragObject
        else if(finger.Index == dragFingerIndex)
        {
            if(gesture.Phase == ContinuousGesturePhase.Updated)
            {
                // update the position by converting the current screen position of the finger to a world position on the Z = 0 plane
                var pos = Utils.GetWorldPos(UICamera.currentCamera, gesture.Position);
                var offset = new Vector3(pos.x - cachedDragPosition.x, 0, 0);
                transform.position = transform.position + offset;
                cachedDragPosition = pos;
            }
            else
            {
                // reset our drag finger index
                dragFingerIndex = -1;
                selection = null;
                //Swipe right
                if(cachedLocalPosition.x + 0.1f < transform.localPosition.x)
                {
                    if (curCustomIndex == 0)
                    {
                        BeginBounce(cachedLocalPosition);
                    }
                    else if (curCustomIndex > 0)
                    {
                        if (curCustomIndex > 1 && curCustomIndex < customDataCount - 1)
                        {
                            AdjustPostion(false);
                        }
                        MoveTo(cachedLocalPosition + Vector3.right * cellWidth, true);
                        curCustomIndex--;
                        if (UpdateData != null)
                        {
                            UpdateData();
                        }
                    }
                }
                //Swipe left
                else if(cachedLocalPosition.x > transform.localPosition.x + 0.1f)
                {
                    if (curCustomIndex == customDataCount - 1)
                    {
                        BeginBounce(cachedLocalPosition);
                    }
                    else if (curCustomIndex < customDataCount - 1)
                    {
                        if (curCustomIndex < customDataCount - 2 && curCustomIndex > 0)
                        {
                            AdjustPostion(true);
                        }
                        MoveTo(cachedLocalPosition + Vector3.left * cellWidth, true);
                        curCustomIndex++;
                        if (UpdateData != null)
                        {
                            UpdateData();
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Init custom data, include the index of curent custom and total custom data.
    /// </summary>
    /// <param name="index">The index of current custom data.</param>
    /// <param name="count">The total count of custom data.</param>
    public void InitCustomData(int index, int count)
    {
        if (cachedGrid == null)
        {
            cachedGrid = GetComponent<UIGrid>();
            cellWidth = cachedGrid.cellWidth;
        }
        cellCount = cachedGrid.transform.childCount;
        itemList.Clear();
        for (var i = 0; i < cellCount; ++i)
        {
            var t = cachedGrid.transform.GetChild(i);
            itemList.Add(t);
        }
        curCustomIndex = index;
        customDataCount = count;
        var cachedActive = cachedGrid.gameObject.activeSelf;
        cachedGrid.gameObject.SetActive(false);
        if (curCustomIndex == 0)
        {
            transform.localPosition = transform.localPosition + Vector3.right * cellWidth;
        }
        else if (curCustomIndex == customDataCount - 1)
        {
            transform.localPosition = transform.localPosition + Vector3.left * cellWidth;
        }
        cachedGrid.gameObject.SetActive(cachedActive);
    }

    /// <summary>
    /// Interface for user to trigger swipe effect.
    /// </summary>
    /// <param name="isLeft">If true, it trigger swipe left effect.</param>
    public void ExcueSwipe(bool isLeft)
    {
        if (isLeft)
        {
            if (curCustomIndex <customDataCount - 1)
            {
                if (curCustomIndex < customDataCount - 2 && curCustomIndex > 0)
                {
                    AdjustPostion(true);
                }
                MoveTo(transform.localPosition + Vector3.left * cellWidth, true);
                curCustomIndex++;
                if (UpdateData != null)
                {
                    UpdateData();
                }
            }
        }
        else
        {
            if (curCustomIndex > 0)
            {
                if (curCustomIndex > 1 && curCustomIndex < customDataCount - 1)
                {
                    AdjustPostion(false);
                }
                MoveTo(transform.localPosition + Vector3.right * cellWidth, true);
                curCustomIndex--;
                if (UpdateData != null)
                {
                    UpdateData();
                }
            }
        }
    }

    #endregion
}
