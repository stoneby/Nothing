using System.Collections.Generic;
using UnityEngine;

public class TeamSimpleController : MonoBehaviour
{
    #region Public Fields

    public TeamFormationController FormationController;

    public List<Character> CharacterList;
    public MyPoolManager CharacterPool;

    public int Total;

    public bool EditMode;

    public delegate void SelectedChanged(GameObject currentObject, GameObject lastObject);

    [HideInInspector]
    public SelectedChanged OnSelectedChanged;

    [HideInInspector]
    public Character CurrentSelect;

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

        CharacterList.RemoveAt(index);
        CharacterPool.Return(character.gameObject);

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

        // return back to pool.
        CharacterList.ForEach(character => CharacterPool.Return(character.gameObject));

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

        if (CharacterList == null)
        {
            CharacterList = new List<Character>();
        }

        if (CharacterList.Count == 0)
        {
            Logger.LogWarning("Dynamic binding mode, take character from pool of number: " + Total, gameObject);

            for (var i = 0; i < Total; ++i)
            {
                var character = CharacterPool.Take().GetComponent<Character>();
                CharacterList.Add(character);
                AddChild(gameObject, character.gameObject);
            }
        }

        Total = CharacterList.Count;
        if (Total <= 0)
        {
            Logger.LogError("Total is: " + Total + ", which means there is nothing to spawn with.", gameObject);
            return;
        }

        if (FormationController.FormationList.Count < Total)
        {
            Logger.LogError(
                "Formation list does not contain position list count: " + Total + ", which formation list total count: " +
                FormationController.FormationList.Count, gameObject);
            return;
        }

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

    private void Start()
    {
        Initialize();
    }

    #endregion
}
