using System.Collections.Generic;
using com.kx.sglm.gs.battle.share.data;
using UnityEngine;

/// <summary>
/// Team selection controller, which could select team memebers from styles.
/// Currently we support matrix style only.
/// Support editor mode during runtime.
/// Support dynamic loading team members together with static binding.
/// Support team position loading from formatter.
/// </summary>
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
    public GameObjectHandler OnDeselect;
    public GameObjectHandler OnAttack;

    #endregion

    #region Private Fields

    private bool initialized;

    /// <summary>
    /// Flag indicates if drag process is started.
    /// </summary>
    private bool dragStart;

    private GameObject targetObject;

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

    #region Public Methods

    public int TwoDimensionToOne(int x, int y)
    {
        return x * Row + y;
    }

    public int TwoDimensionToOne(Position p)
    {
        return p.X * Row + p.Y;
    }

    public Position OneDimensionToTwo(int i)
    {
        return new Position {X = i / Row, Y = i % Row};
    }

    public void Cleanup()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        // return back to pool.
        CharacterList.ForEach(character => CharacterPool.Return(character.gameObject));

        // unregister events to all characters.
        UnregisterEventHandlers();

        CharacterList.Clear();
    }

    public void RegisterEventHandlers()
    {
        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDragStart += OnCharacterDragStart;
            listener.onDragOver += OnCharacterDragOver;
            listener.onDragEnd += OnCharacterDragEnd;
            listener.onDragOut += OnCharacterDragOut;
            listener.onDrag += OnCharacterDrag;
        });
    }

    public void UnregisterEventHandlers()
    {
        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDragStart -= OnCharacterDragStart;
            listener.onDragOver -= OnCharacterDragOver;
            listener.onDragEnd -= OnCharacterDragEnd;
            listener.onDragOut -= OnCharacterDragOut;
            listener.onDrag -= OnCharacterDrag;
        });
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
            Logger.LogError("Please make sure character list count - " + CharacterList.Count +
                            " is more than visiable count - " +
                            visableTotal);
            return;
        }

        var boxCollider = gameObject.GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

        // get latest formation list as default.
        var positionList = FormationController.LatestPositionList;
        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            if (i < visableTotal)
            {
                // logic location.
                character.Location = OneDimensionToTwo(i);

                // world position.
                character.transform.position = positionList[i];
            }
            else
            {
                character.transform.position = Vector3.zero;
                character.gameObject.SetActive(false);
            }
            character.Index = i;
            character.name += "_" + i;
        }

        // generate bounds according to its children.
        var boundGenerator = GetComponent<BoundsGenerator>() ?? gameObject.AddComponent<BoundsGenerator>();
        boundGenerator.Generate();

        SelectedCharacterList = new List<Character>(CharacterList.Count);

        RegisterEventHandlers();
    }

    public void Print()
    {
        CharacterList.ForEach(
            item => Logger.LogWarning("Character: " + item.name + ", position: " + item.transform.position));
    }

    public void Reset()
    {
        targetObject = null;

        dragStart = false;
        DragBarPool.ObjectList.ForEach(bar => DragBarPool.Return(bar));
    }

    public void OnDragOverAnotherTeamHandler(GameObject target)
    {
        Logger.Log("Ready to attack target: " + target.name);
        targetObject = target;
    }

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
        dragbarController.SetSprite("new_drag_normal");
    }

    private void OnCharacterDragStart(GameObject sender)
    {
        Logger.Log("On character drag start: " + sender.name);

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
                var deselectedObject = SelectedCharacterList[SelectedCharacterList.Count - 1];
                if (OnDeselect != null)
                {
                    OnDeselect(deselectedObject.gameObject);
                }

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
            AttackSimulator.TeamController = this;
            AttackSimulator.Attack(targetObject);
        }

        var isAttacking = CheckAttack();
        if (OnStop != null)
        {
            OnStop(isAttacking);
        }

        Logger.LogWarning("Selected character list: " + SelectedCharacterList.Count);
        Reset();
    }

    private bool CheckAttack()
    {
        var pos = UICamera.currentTouch.pos;
        var worldPos = Camera.main.ScreenToWorldPoint(pos);
        var bounds = collider.bounds;
        return bounds.Contains(worldPos);
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        Logger.Log("On character drag out: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);
    }

    private void DrawDragBar(GameObject sender)
    {
        // fix last drag bar's width and rotation.
        if (LastCharacter != null)
        {
            var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(LastCharacter.transform.position);
            var targetPosition = UICamera.mainCamera.WorldToScreenPoint(sender.transform.position);

            var dragbarController = DragBarPool.CurrentObject.GetComponent<AbstractDragBarController>();
            dragbarController.SetRotate(sourcePosition, targetPosition);
            dragbarController.SetWidth(sourcePosition, targetPosition);
            var color = (sender.GetComponent<Character>()).ColorIndex;
            var spriteName = string.Format("new_drag_{0}", color);
            dragbarController.SetSprite(spriteName);

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

    #endregion

    #region Mono

    void Start()
    {
        Initialize();
    }

    #endregion
}
