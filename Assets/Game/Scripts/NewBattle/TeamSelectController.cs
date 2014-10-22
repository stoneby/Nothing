using System.Collections.Generic;
using System.Resources;
using System.Runtime.Remoting;
using System.Text;
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

    public CharacterGroupController GroupController;

    public AttackSimulator AttackSimulator;

    public MyPoolManager DragBarPool;

    public int Row;
    public int Col;
    public int Total;

    public bool EditMode;

    public delegate void CharacterHandler(Character attackedObject);
    public delegate void BoolHandler(bool isAttacked);
    public delegate void VoidHandler();

    public VoidHandler OnStart;
    public BoolHandler OnStop;
    public CharacterHandler OnSelect;
    public CharacterHandler OnDeselect;
    public CharacterHandler OnAttack;

    /// <summary>
    /// Flag indicates if enable / disable the whole controller.
    /// </summary>
    public bool Enable;

    /// <summary>
    /// Flag indicates if auto compute wait stack enabled.
    /// </summary>
    public bool AutoWaitEnabled;

    /// <summary>
    /// Waiting stack that hold waiting hero's position.
    /// </summary>
    public List<GameObject> WaitingStackList;

    /// <summary>
    /// Team Formation controller.
    /// </summary>
    public TeamFormationController FormationController;

    /// <summary>
    /// Collider factor that make drag more comfortable and better user experience.
    /// </summary>
    [Range(0f, 1f)]
    public float ColliderFactor = 1f;

    /// <summary>
    /// Visible number of characters.
    /// </summary>
    public int VisibleCount
    {
        get { return Row * Col; }
    }

    /// <summary>
    /// Character list.
    /// </summary>
    public List<Character> CharacterList
    {
        get { return GroupController.CharacterList; }
    }

    #endregion

    #region Private Fields

    private bool initialized;

    /// <summary>
    /// Normal dragbar depth.
    /// </summary>
    /// <remarks>Color index range [1, 5].</remarks>
    private int normalDepth;
    /// <summary>
    /// Lower dragbar depth.
    /// </summary>
    /// <remarks>Rotating dragbar, gray color in our case.</remarks>
    private int lowerDepth;

    /// <summary>
    /// Flag indicates if drag process is started.
    /// </summary>
    private bool dragStart;

    private GameObject targetObject;

    private const string AudioPathPrefix = "Sounds/Attack_";

    private readonly List<AudioClip> selectAudios = new List<AudioClip>();

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
        return new Position { X = i / Row, Y = i % Row };
    }

    public void Cleanup()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        // call this line before character list got clear in group controller.
        // unregister events to all characters.
        UnregisterEventHandlers();

        GroupController.Cleanup();
    }

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        ////couldn't select as default.
        //Enable = true;

        GroupController.Initialize();

        if (CharacterList.Count < VisibleCount)
        {
            Logger.LogError("Please make sure character list count - " + CharacterList.Count +
                            " is more than visiable count - " +
                            VisibleCount);
            return;
        }

        InitWaitingStackPosition();
        InitOnStagePosition();

        // generate bounds according to its children.
        var boundGenerator = GetComponent<BoundsGenerator>() ?? gameObject.AddComponent<BoundsGenerator>();
        boundGenerator.Generate();

        SelectedCharacterList = new List<Character>(CharacterList.Count);

        normalDepth = DragBarPool.SpawnObject.GetComponent<AbstractDragBarController>().GetDepth();
        lowerDepth = normalDepth - 1;

        RegisterEventHandlers();

        LoadAudios();
    }

    public override string ToString()
    {
        var result = new StringBuilder('{');
        CharacterList.ForEach(
            item => result.Append("(name: " + item.name + ", color: " + item.ColorIndex + ")\n"));
        result.Append('}');
        return result.ToString();
    }

    public void Reset()
    {
        targetObject = null;

        dragStart = false;
        DragBarPool.ObjectList.ForEach(bar => DragBarPool.Return(bar));

        AdjustColliders(1 / ColliderFactor);
    }

    public void OnDragOverAnotherTeamHandler(Character target)
    {
        Logger.Log("Ready to attack target: " + target.name);
        targetObject = target.gameObject;
    }

    #endregion

    #region Private Methods

    private void OnCharacterClick(GameObject sender)
    {
        if (EditMode || !Enable)
        {
            return;
        }

        var currentCharacter = sender.GetComponent<Character>();
        Logger.LogWarning("On character click: " + sender.name + ", can selected: " + currentCharacter.CanSelected);
        if (!currentCharacter.CanSelected)
        {
            return;
        }

        SelectedCharacterList.Clear();
        SelectedCharacterList.Add(currentCharacter);

        if (OnStop != null)
        {
            OnStop(true);
        }
    }

    private void OnCharacterDrag(GameObject sender, Vector2 delta)
    {
        if (EditMode)
        {
            var target = UICamera.currentTouch.pos;
            var targetWorld = UICamera.mainCamera.ScreenToWorldPoint(target);
            sender.transform.position = targetWorld;

            return;
        }

        if (!Enable || !dragStart)
        {
            return;
        }

        //Logger.Log("On character drag: " + sender.name + " with delta: " + delta);

        var currentDrag = DragBarPool.CurrentObject;
        if (currentDrag == null)
        {
            Logger.LogWarning("Current drag is null in DragBarPool.");
            return;
        }
        var sourcePosition = UICamera.mainCamera.WorldToScreenPoint(LastCharacter.transform.position);
        var targetPosition = UICamera.currentTouch.pos;
        var dragbarController = currentDrag.GetComponent<AbstractDragBarController>();
        dragbarController.SetRotate(new Vector2(sourcePosition.x, sourcePosition.y), targetPosition);
        dragbarController.SetWidth(sourcePosition, targetPosition);
        dragbarController.SetSprite("new_drag_normal");
        dragbarController.SetDepth(lowerDepth);
    }

    private void OnCharacterDragStart(GameObject sender)
    {
        if (EditMode || !Enable)
        {
            return;
        }

        var currentCharacter = sender.GetComponent<Character>();
        if (!currentCharacter.CanSelected)
        {
            return;
        }

        Logger.Log("On character drag start: " + sender.name);

        dragStart = true;
        SelectedCharacterList.Clear();
        AdjustColliders(ColliderFactor);

        if (OnStart != null)
        {
            OnStart();
        }
    }

    private void OnCharacterDragOver(GameObject sender, GameObject draggedObject)
    {
        if (EditMode || !Enable)
        {
            return;
        }

        Logger.Log("On character drag over: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);

        var currentCharacter = sender.GetComponent<Character>();

        if (!dragStart)
        {
            Logger.LogWarning("Drag over but drag is not started at current TeamSelectController with name: " + name);

            targetObject = sender;
            if (OnAttack != null)
            {
                OnAttack(currentCharacter);
            }
            return;
        }

        var firstTime = (LastCharacter == null);
        if (!firstTime && !LastCharacter.IsNeighborhood(currentCharacter))
        {
            Logger.LogWarning("Current character: " + currentCharacter + " is not my neighbor: " + LastCharacter + ", with can selected: " + currentCharacter.CanSelected);
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
                    OnDeselect(deselectedObject);
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

            // play sound only when select counter bigger than 1.
            var selectCount = SelectedCharacterList.Count;
            audio.PlayOneShot(selectAudios[selectCount - 1]);

            if (OnSelect != null)
            {
                OnSelect(currentCharacter);
            }

            Logger.Log("Add dragged character, which is neighbor - " + currentCharacter +
                      ", to selected character list - " + SelectedCharacterList.Count);
        }
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        if (EditMode || !Enable || !dragStart)
        {
            return;
        }

        Logger.Log("On character drag out: " + sender.name + ", dragged started game ojbect: " + draggedObject.name);
    }

    private void OnCharacterDragEnd(GameObject sender)
    {
        if (EditMode || !Enable || !dragStart)
        {
            return;
        }

        Logger.Log("On character drop end: " + sender.name + ", name:" + name);

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

    private void AdjustColliders(float factor)
    {
        CharacterList.ForEach(character =>
        {
            var boxCollider = character.GetComponent<BoxCollider>();
            boxCollider.size *= factor;
        });
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
            dragbarController.SetDepth(normalDepth);

            Logger.LogWarning("Source position: " + sourcePosition + ", target position: " + targetPosition + ", rotation: " + DragBarPool.CurrentObject.transform.rotation);
        }

        var dragBar = DragBarPool.Take();
        Utils.AddChild(sender, dragBar);

        Logger.LogWarning("Added drag bar to parent: " + sender.name);
    }

    private void RegisterEventHandlers()
    {
        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDragStart = OnCharacterDragStart;
            listener.onDragOver = OnCharacterDragOver;
            listener.onDragEnd = OnCharacterDragEnd;
            listener.onDragOut = OnCharacterDragOut;
            listener.onDrag = OnCharacterDrag;
            listener.onClick = OnCharacterClick;
        });
    }

    private void UnregisterEventHandlers()
    {
        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDragStart = null;
            listener.onDragOver = null;
            listener.onDragEnd = null;
            listener.onDragOut = null;
            listener.onDrag = null;
            listener.onClick = null;
        });
    }

    private void InitOnStagePosition()
    {
        // get latest formation list as default.
        var positionList = FormationController.LatestPositionList;
        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            if (i < VisibleCount)
            {
                // logic location.
                character.Location = OneDimensionToTwo(i);

                // world position.
                character.transform.position = positionList[i];
            }
            else
            {
                var stackCount = WaitingStackList.Count;
                var index = i % stackCount;
                character.transform.position = WaitingStackList[index].transform.position;
            }
            character.Index = i;
            character.name += "_" + i;
        }
    }

    private void InitWaitingStackPosition()
    {
        if (!AutoWaitEnabled)
        {
            return;
        }

        for (var i = 0; i < Row; ++i)
        {
            var index = (Col - 1) * Row + i;
            var targetPosition = FormationController.LatestPositionList[index];

            var sourcePosition = targetPosition + targetPosition -
                                 FormationController.LatestPositionList[index - Col];

            Logger.LogWarning("Waiting stack index of: " + index + ", right index: " + (index - Col));
            WaitingStackList[i].transform.position = sourcePosition;
        }
    }

    private void LoadAudios()
    {
        for (var i = 0; i < VisibleCount; ++i)
        {
            selectAudios.Add(ResoucesManager.Instance.Load<AudioClip>(AudioPathPrefix + (i + 1)));
        }

        Logger.LogWarning("Loading audio resouces count: " + selectAudios.Count);
    }

    #endregion

    #region Mono

    void Start()
    {
        Initialize();
    }

    #endregion
}
