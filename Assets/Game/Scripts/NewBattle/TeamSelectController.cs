using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectController : MonoBehaviour
{
    #region Public Fields

    public AttackSimulator AttackSimulator;

    public List<Character> CharacterList;
    public MyPoolManager DragBarPool;
    public Camera CurrentCamera;

    public bool AutoAdjustPosition;
    public int Row;
    public int Col;

    public Vector3 OffSet;

    public bool EditMode;

    public delegate void AttackHandler(GameObject attackedObject);
    public AttackHandler OnAttack;

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

    private GameObject targetObject;

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void OnCharacterClick(GameObject sender)
    {
        Logger.LogWarning("On character click: " + sender.name);
    }

    private void OnCharacterPress(GameObject sender, bool isPressed)
    {
        Logger.Log("On character press: " + sender.name + ", is pressed: " + isPressed);
    }
    private void OnCharacterDrag(GameObject sender, Vector2 delta)
    {
        //Logger.Log("On character drag: " + sender.name + " with delta: " + delta);

        if (EditMode)
        {
            var target = UICamera.currentTouch.pos;
            var targetWorld = UICamera.mainCamera.ScreenToWorldPoint(target);
            sender.transform.position = targetWorld;

            return;
        }

        var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(LastCharacter.transform.position);
        var targetPosition = UICamera.currentTouch.pos;

        DragBarPool.CurrentObject.transform.localRotation = Utils.GetRotation(
                new Vector2(sourcePosition.x, sourcePosition.y), targetPosition);

        //Logger.LogWarning("current drag bar to parent: " + DragBarPool.CurrentObject.transform.parent.name);

        SetDragbarWidth(sourcePosition, targetPosition);
    }

    private void SetDragbarWidth(Vector3 sourcePosition, Vector2 targetPosition)
    {
        var barSprite = DragBarPool.CurrentObject.GetComponent<DragBarController>().BarSprite;
        var distance = Mathf.Abs(Vector2.Distance(sourcePosition, targetPosition));
        barSprite.width = (int)(distance * UICamera.mainCamera.orthographicSize);
    }

    private void OnCharacterDragStart(GameObject sender)
    {
        Logger.Log("On character drop start: " + sender.name);

        if (EditMode)
        {
            return;
        }

        dragStart = true;
        SelectedCharacterList.Clear();
    }

    private void OnCharacterDragOver(GameObject sender, GameObject draggedObject)
    {
        Logger.Log("On character drag over: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);

        if (EditMode)
        {
            return;
        }

        if (!dragStart)
        {
            Logger.LogWarning("Drag over but drag is not started at current TeamSelectController with name: " + name);

            targetObject = sender;
            if (OnAttack != null)
            {
                OnAttack(targetObject);
            } 
            return;
        }

        var currentCharacter = sender.GetComponent<Character>();
        var firstTime = (LastCharacter == null);
        if (!firstTime && !LastCharacter.IsNeighborhood(currentCharacter))
        {
            Logger.LogWarning("Current character: " + currentCharacter + " is not my neighbor: " + LastCharacter);
            return;
        }

        var selectedObject = SelectedCharacterList.Find(character => character.Index == currentCharacter.Index);
        if (selectedObject != null)
        {
            var oneBeforeLastIndex = SelectedCharacterList.Count - 2;
            // condition of cancel last select character.
            if (oneBeforeLastIndex >= 0 && SelectedCharacterList[oneBeforeLastIndex] == selectedObject)
            {
                SelectedCharacterList.RemoveAt(SelectedCharacterList.Count - 1);

                Logger.LogWarning("Return drag bar to parent: " + DragBarPool.CurrentObject.transform.parent.name);

                DragBarPool.Return(DragBarPool.CurrentObject);

                var t = selectedObject.transform;
                DragBarPool.CurrentObject = t.FindChild("DragBar(Clone)").gameObject;

                Logger.LogWarning("Current drag bar to parent is: " + selectedObject.name);
            }

            Logger.LogWarning("Current character: " + currentCharacter + " is already selected.");
        }
        else
        {
            DrawDragBar(sender);

            SelectedCharacterList.Add(currentCharacter);

            Logger.Log("Add dragged character, which is neighbor - " + currentCharacter +
                      ", to selected character list - " + SelectedCharacterList.Count);
        }
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        //Logger.Log("On character drop out: " + sender.name + ", dragged game ojbect: " + draggedObject.name);
    }

    private void OnCharacterDragEnd(GameObject sender)
    {
        Logger.Log("On character drop end: " + sender.name + ", name:" + name);

        if (EditMode)
        {
            return;
        }

        // PVP attach is on going.
        if (targetObject != null)
        {
            DragBarPool.ObjectList.ForEach(bar => DragBarPool.Return(bar));

            AttackSimulator.TeamController = this;
            AttackSimulator.Attack(targetObject);
            return;
        }

        Logger.LogWarning("Selected character list: " + SelectedCharacterList.Count);
        Reset();
    }

    public void Reset()
    {
        targetObject = null;

        dragStart = false;
        SelectedCharacterList.Clear();
        DragBarPool.ObjectList.ForEach(bar => DragBarPool.Return(bar));
    }

    private void DrawDragBar(GameObject sender)
    {
        // fix last drag bar's width and rotation.
        if (LastCharacter != null)
        {
            var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(LastCharacter.transform.position);
            var targetPosition = UICamera.mainCamera.WorldToScreenPoint(sender.transform.position);
            DragBarPool.CurrentObject.transform.localRotation = Utils.GetRotation(sourcePosition, targetPosition);

            SetDragbarWidth(sourcePosition, targetPosition);

            Logger.LogWarning("Source position: " + sourcePosition + ", target position: " + targetPosition + ", rotation: " + DragBarPool.CurrentObject.transform.rotation);
        }

        var dragBar = DragBarPool.Take();
        AddChild(sender, dragBar);

        Logger.LogWarning("Added drag bar to parent: " + sender.name);
    }

    private void AddChild(GameObject sender, GameObject dragBar)
    {
        var t = dragBar.transform;
        t.parent = sender.transform;
        t.localPosition = Vector3.zero + OffSet;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        dragBar.SetActive(true);
    }

    public void OnDragOverAnotherTeamHandler(GameObject target)
    {
        Logger.Log("Ready to attack target: " + target.name);
        targetObject = target;
    }

    #endregion

    #region Mono

    void Start()
    {
        if (CharacterList.Count != Row * Col)
        {
            Logger.LogError("Please make sure character list count - " + CharacterList.Count + " is the same as Row * Col - " + Row * Col);
            return;
        }

        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            character.Index = i;
            if (AutoAdjustPosition)
            {
                character.Location.X = i / Col;
                character.Location.Y = i % Row;
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
