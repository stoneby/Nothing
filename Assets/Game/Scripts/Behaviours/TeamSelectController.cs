using System.Collections.Generic;
using UnityEngine;

public class TeamSelectController : MonoBehaviour
{
    #region Public Fields

    public List<Character> CharacterList;
    public PoolManager DragBarPool;
    public Camera CurrentCamera;

    public bool AutoAdjustPosition;
    public int Row;
    public int Col;

    #endregion

    #region Public Properties

    /// <summary>
    /// Selected character list.
    /// </summary>
    public List<Character> SelectedCharacterList { get; set; }

    /// <summary>
    /// Last character index which is always the last in SelectedCharacterList
    /// Return null when the list is empty.
    /// </summary>
    public Character LastCharacter
    {
        get
        {
            return SelectedCharacterList.Count == 0
                ? null
                : SelectedCharacterList[SelectedCharacterList.Count - 1];
        }
    }

    #endregion

    #region Private Fields

    /// <summary>
    /// Flag indicates if drag process is started.
    /// </summary>
    private bool dragStart;

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void OnCharacterClick(GameObject sender)
    {
        Debug.LogWarning("On character click: " + sender.name);
    }

    private void OnCharacterPress(GameObject sender, bool isPressed)
    {
        Debug.Log("On character press: " + sender.name + ", is pressed: " + isPressed);
    }
    private void OnCharacterDrag(GameObject sender, Vector2 delta)
    {
        Debug.Log("On character drag: " + sender.name + " with delta: " + delta);

        Debug.LogWarning("Last touch positon: " + UICamera.lastTouchPosition);
        Debug.LogWarning("Current touch: " + UICamera.currentTouch);

        var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(sender.transform.position);
        var targetPosition = UICamera.currentTouch.pos;
        DragBarPool.CurrentObject.transform.rotation = Utils.GetRotation(
            new Vector2(sourcePosition.x, sourcePosition.y), targetPosition);
        //var targetPosition = UICamera.mainCamera.ScreenToWorldPoint(UICamera.currentTouch.pos);
        //DragBarPool.CurrentObject.transform.rotation = Utils.GetRotation(sender.transform.position, targetPosition);

        Debug.LogWarning("Souce positioN: " + sourcePosition + ", target position: " + targetPosition);

        var dragbarSprite = DragBarPool.CurrentObject.GetComponent<DragBarController>().Sprite;
        Debug.Log("drag bar sprite before width: " + dragbarSprite.width + ", height: " + dragbarSprite.height);
        var distance = (int)Mathf.Abs(Vector2.Distance(sourcePosition, targetPosition));
        var newWidth = dragbarSprite.minWidth*1.0/2 + distance + Mathf.Abs(dragbarSprite.transform.localPosition.x);
        Debug.Log("Distance : " + distance + ", minWidth: " + dragbarSprite.minWidth + ", new width: " + newWidth);
        dragbarSprite.width = (int)newWidth;
        Debug.Log("drag bar sprite after width: " + dragbarSprite.width + ", height: " + dragbarSprite.height);
    }

    private void OnCharacterDragStart(GameObject sender)
    {
        Debug.Log("On character drop start: " + sender.name);

        dragStart = true;
        SelectedCharacterList.Clear();
    }

    private void OnCharacterDragOver(GameObject sender, GameObject draggedObject)
    {
        Debug.Log("On character drop over: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);

        if (!dragStart)
        {
            Debug.LogWarning("Drag over but drag is not started at one character.");
            return;
        }

        var currentCharacter = sender.GetComponent<Character>();
        var firstTime = (LastCharacter == null);
        if (!firstTime && !LastCharacter.IsNeighborhood(currentCharacter))
        {
            Debug.LogWarning("Current character: " + currentCharacter + " is eithor not my neighbor: " + LastCharacter);
            return;
        }

        var selectedObject = SelectedCharacterList.Find(character => character.Index == currentCharacter.Index);
        if (selectedObject != null)
        {
            Debug.LogWarning("Current character: " + currentCharacter + " is already selected.");
            return;
        }

        SelectedCharacterList.Add(currentCharacter);

        DrawDragBar(sender);

        Debug.Log("Add dragged character, which is neighbor - " + currentCharacter + ", to selected character list - " + SelectedCharacterList.Count);
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        //Debug.Log("On character drop out: " + sender.name + ", dragged game ojbect: " + draggedObject.name);
    }

    private void OnCharacterDragEnd(GameObject sender)
    {
        Debug.Log("On character drop end: " + sender.name);

        Debug.LogWarning("Selected character list: " + SelectedCharacterList.Count);

        dragStart = false;
        SelectedCharacterList.Clear();
    }

    private void DrawDragBar(GameObject sender)
    {
        var dragBar = DragBarPool.Take();
        dragBar.transform.position = sender.transform.position;
        dragBar.transform.localScale = new Vector3(1f, 1f, 1f);
        dragBar.SetActive(true);
    }

    #endregion

    #region Mono

    void Start()
    {
        if (CharacterList.Count != Row*Col)
        {
            Debug.LogError("Please make sure character list count - " + CharacterList.Count + " is the same as Row * Col - " + Row * Col);
            return;
        }

        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            character.Index = i;
            if (AutoAdjustPosition)
            {
                character.Location.X = i/Col;
                character.Location.Y = i%Row;
            }
        }

        SelectedCharacterList = new List<Character>(CharacterList.Count);

        CharacterList.ForEach(character =>
        {
            UIEventListener.Get(character.gameObject).onClick += OnCharacterClick;
            UIEventListener.Get(character.gameObject).onPress += OnCharacterPress;
            UIEventListener.Get(character.gameObject).onDrag += OnCharacterDrag;
            UIEventListener.Get(character.gameObject).onDragStart += OnCharacterDragStart;
            UIEventListener.Get(character.gameObject).onDragOver += OnCharacterDragOver;
            UIEventListener.Get(character.gameObject).onDragOut += OnCharacterDragOut;
            UIEventListener.Get(character.gameObject).onDragEnd += OnCharacterDragEnd;
        });
    }

    #endregion
}
