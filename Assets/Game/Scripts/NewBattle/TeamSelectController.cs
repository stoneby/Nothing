using System.Collections.Generic;
using UnityEngine;

public class TeamSelectController : MonoBehaviour
{
    #region Public Fields

    public TeamFormationController FormationController;
    public AttackSimulator AttackSimulator;

    public List<Character> CharacterList;
    public MyPoolManager DragBarPool;
    public MyPoolManager CharacterPool;

    public int Row;
    public int Col;
    public int Total;

    public bool EditMode;

    public delegate void GameObjectHandler(GameObject attackedObject);
    public delegate void BoolHandler(bool isAttacked);
    public delegate void VoidHandler();

    public VoidHandler OnStart;
    public BoolHandler OnStop;
    public GameObjectHandler OnSelect;
    public GameObjectHandler OnAttack;

    #endregion

    #region Private Fields

    private bool initialized;

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
        var dragbarController = DragBarPool.CurrentObject.GetComponent<AbstractDragBarController>();
        dragbarController.SetRotate(new Vector2(sourcePosition.x, sourcePosition.y), targetPosition);
        dragbarController.SetWidth(sourcePosition, targetPosition);
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

        if (OnStart != null)
        {
            OnStart();
        }
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
                var dragName = DragBarPool.SpawnObject.name;
                DragBarPool.CurrentObject = t.FindChild(string.Format("{0}(Clone)", dragName)).gameObject;
                Logger.LogWarning("Current drag bar to parent is: " + selectedObject.name);
            }

            Logger.LogWarning("Current character: " + currentCharacter + " is already selected.");
        }
        else
        {
            DrawDragBar(sender);

            SelectedCharacterList.Add(currentCharacter);

            if (OnSelect != null)
            {
                OnSelect(currentCharacter.gameObject);
            }

            Logger.Log("Add dragged character, which is neighbor - " + currentCharacter +
                      ", to selected character list - " + SelectedCharacterList.Count);
        }
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
        }

        if (OnStop != null)
        {
            OnStop(false);
        }

        Logger.LogWarning("Selected character list: " + SelectedCharacterList.Count);
        Reset();
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        Logger.Log("On character drag out: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);
    }

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        if (CharacterList.Count == 0)
        {
            Logger.LogWarning("Dynamic binding mode, take character from pool of number: " + Total);

            for (var i = 0; i < Total; ++i)
            {
                var character = CharacterPool.Take().GetComponent<Character>();
                CharacterList.Add(character);
                AddChild(gameObject, character.gameObject);
            }
        }

        Total = CharacterList.Count;

        var visableTotal = Row * Col;
        if (CharacterList.Count < visableTotal)
        {
            Logger.LogError("Please make sure character list count - " + CharacterList.Count + " is more than Row * Col - " +
                            Row * Col);
            return;
        }

        
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        Logger.LogWarning("Add box collider: " + boxCollider.name);
        var positionList = FormationController.FormationList[FormationController.Index].PositionList;
        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            if (i < visableTotal)
            {
                // logic location.
                character.Index = i;
                character.Location.X = i / Col;
                character.Location.Y = i % Row;

                // world position.
                character.name += character.Index;
                character.transform.position = positionList[i];

                // update grouped box collider.
                boxCollider.bounds.Encapsulate(character.collider.bounds);

                Logger.LogWarning("Collider: " + character.name + ", " + character.collider.bounds);
            }
            else
            {
                character.transform.position = Vector3.zero;
                character.gameObject.SetActive(false);
            }
        }

        Logger.LogWarning("Total collider bounds: " + boxCollider.bounds);

        SelectedCharacterList = new List<Character>(CharacterList.Count);

        CharacterList.ForEach(character =>
        {
            if (UIEventListener.Get(character.gameObject).onDrag == null)
            {
                UIEventListener.Get(character.gameObject).onDrag += OnCharacterDrag;
            }
            var eventListenere = UIEventListener.Get(character.gameObject);
            eventListenere.onDragStart += OnCharacterDragStart;
            eventListenere.onDragOver += OnCharacterDragOver;
            eventListenere.onDragEnd += OnCharacterDragEnd;
            eventListenere.onDragOut += OnCharacterDragOut;
        });
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

            var dragbarController = DragBarPool.CurrentObject.GetComponent<AbstractDragBarController>();
            dragbarController.SetWidth(sourcePosition, targetPosition);

            Logger.LogWarning("Source position: " + sourcePosition + ", target position: " + targetPosition + ", rotation: " + DragBarPool.CurrentObject.transform.rotation);
        }

        var dragBar = DragBarPool.Take();
        AddChild(sender, dragBar);

        Logger.LogWarning("Added drag bar to parent: " + sender.name);
    }

    private void AddChild(GameObject sender, GameObject childObject)
    {
        var t = childObject.transform;
        t.parent = sender.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        childObject.SetActive(true);
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
        Initialize();
    }

    #endregion
}
