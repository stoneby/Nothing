using System.Collections.Generic;
using UnityEngine;

public class TeamSimpleController : MonoBehaviour
{
    #region Public Fields

    public CharacterGroupController GroupController;

    public bool EditMode;

    /// <summary>
    /// Team Formation controller.
    /// </summary>
    public TeamFormationController FormationController;

    public delegate void SelectedChanged(GameObject currentObject, GameObject lastObject);

    [HideInInspector]
    public SelectedChanged OnSelectedChanged;

    [HideInInspector]
    public Character CurrentSelect;

    /// <summary>
    /// Character list.
    /// </summary>
    public List<Character> CharacterList
    {
        get { return GroupController.CharacterList; }
    }

    public int Total
    {
        get { return GroupController.Total; }
        set { GroupController.Total = value; }
    }

    #endregion

    #region Private Fields

    private bool initialized;

    #endregion

    #region Public Methods

    /// <summary>
    /// Return index of character list back.
    /// </summary>
    /// <param name="index">Index</param>
    public void ReturnAt(int index)
    {
        if (index < 0 || index >= CharacterList.Count)
        {
            Logger.LogError("Index: " + index + " is out of range of character list count: " + CharacterList.Count);
            return;
        }
        // [NOTE:] enemy controller will take care of object reuse itself, just deactive it is okay.
        var character = CharacterList[index];
        character.ResetBuff();

        CharacterList.RemoveAt(index);
        GroupController.Generator.Return(character.gameObject);

        if (CurrentSelect == character)
        {
            // update current select in case current select has been removed.
            SetDefaultCurrentSelect();
        }
    }

    /// <summary>
    /// Set current select default value.
    /// </summary>
    /// <remarks>Always set to index of 0 if any.</remarks>
    public void SetDefaultCurrentSelect()
    {
        CurrentSelect = (CharacterList.Count > 0 ? CharacterList[0] : null);

        if (CurrentSelect != null)
        {
            OnSelectedChanged(CurrentSelect.gameObject, CurrentSelect.gameObject);
        }
    }

    public void Cleanup()
    {
        if (!initialized)
        {
            return;
        }

        initialized = false;

        GroupController.Cleanup();

        // unregister events to all characters.
        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDrag = null;
            listener.onSelect = null;
        });

        CharacterList.Clear();

        CurrentSelect = null;
    }

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        GroupController.Initialize();

        if (FormationController.FormationList.Count < Total)
        {
            Logger.LogError(
                "Formation list does not contain position list count: " + Total + ", which formation list total count: " +
                FormationController.FormationList.Count, gameObject);
            return;
        }

        InitOnStagePosition();

        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDrag = OnCharacterDrag;
            listener.onSelect = OnCharacterSeleced;
        });
    }

    #endregion

    #region Private Methods

    private void OnCharacterSeleced(GameObject sender, bool selected)
    {
        Logger.LogWarning("On Character Selected: " + sender.name + ", selected: " + selected);

        var character = sender.GetComponent<Character>();
        if (selected)
        {
            if (OnSelectedChanged != null)
            {
                OnSelectedChanged(character.gameObject, CurrentSelect.gameObject);
            }
            CurrentSelect = character;
        }
    }

    private void OnCharacterDrag(GameObject sender, Vector2 delta)
    {
        //Logger.Log("On character drag: " + sender.name + " with delta: " + delta);

        if (EditMode)
        {
            var target = UICamera.currentTouch.pos;
            var targetWorld = UICamera.mainCamera.ScreenToWorldPoint(target);
            sender.transform.position = targetWorld;
        }
    }

    private void InitOnStagePosition()
    {
        // get formation with position list count with total.
        var positionList = FormationController.FormationList[Total - 1].PositionList;
        if (positionList.Count != Total)
        {
            Logger.LogError("Total: " + Total + " should be equal to position list count: " + positionList.Count +
                            " of index: " + (Total - 1) + " from formation list.", gameObject);
            return;
        } 
        
        for (var i = 0; i < CharacterList.Count; ++i)
        {
            var character = CharacterList[i];
            // logic location.
            character.Index = i;
            character.Location = new Position { X = 0, Y = i };

            // world position.
            character.name += "_" + character.Index;
            character.transform.position = positionList[i];
        }

    }


    #endregion

    #region Mono

    private void Start()
    {
        Initialize();
    }

    #endregion
}
