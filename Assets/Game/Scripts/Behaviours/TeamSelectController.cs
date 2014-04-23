using System.Collections.Generic;
using UnityEngine;

public class TeamSelectController : MonoBehaviour
{
    #region Public Fields

    public List<Character> CharacterList;
    public PoolManager DragBarPool;
    public Camera CurrentCamera;

    #endregion

    #region Public Properties

    /// <summary>
    /// Selected character list.
    /// </summary>
    public List<int> SelectedCharacterList { get; set; }

    /// <summary>
    /// Last character index which is always the last in SelectedCharacterList
    /// -1 when the list is empty.
    /// </summary>
    public int LastCharacterIndex
    {
        get
        {
            return SelectedCharacterList.Count == 0
                ? Utils.Invalid
                : SelectedCharacterList[SelectedCharacterList.Count - 1];
        }
    }

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
        //Debug.Log("On character drag: " + sender.name + ", delata vector: " + delta);
    }

    private void OnCharacterDrop(GameObject sender, GameObject draggedObject)
    {
        Debug.Log("On character drop: " + sender.name + ", dragged game ojbect: " + draggedObject.name);
    }

    private void OnCharacterHover(GameObject sender, bool isHover)
    {
        //Debug.Log("On character hover: " + sender.name + ", is hover: " + isHover);
    }

    private void OnCharacterSelect(GameObject sender, bool isSelected)
    {
        Debug.Log("On character selected: " + sender.name + ", is selected: " + isSelected);   
    }

    private void OnCharacterDragStart(GameObject sender)
    {
        Debug.Log("On character drop start: " + sender.name);
    }

    private void OnCharacterDragOver(GameObject sender, GameObject draggedObject)
    {
        Debug.Log("On character drop over: " + sender.name + ", dragged game ojbect: " + draggedObject.name);
    }

    private void OnCharacterDragOut(GameObject sender, GameObject draggedObject)
    {
        Debug.Log("On character drop out: " + sender.name + ", dragged game ojbect: " + draggedObject.name);
    }

    private void OnCharacterDragEnd(GameObject sender)
    {
        Debug.Log("On character drop end: " + sender.name);
    }

    #endregion

    #region Mono

    void Start()
    {
        SelectedCharacterList = new List<int>(CharacterList.Count);

        CharacterList.ForEach(character =>
        {
            UIEventListener.Get(character.gameObject).onClick += OnCharacterClick;
            UIEventListener.Get(character.gameObject).onPress += OnCharacterPress;
            UIEventListener.Get(character.gameObject).onDrag += OnCharacterDrag;
            UIEventListener.Get(character.gameObject).onDragStart += OnCharacterDragStart;
            UIEventListener.Get(character.gameObject).onDragOver += OnCharacterDragOver;
            UIEventListener.Get(character.gameObject).onDragOut += OnCharacterDragOut;
            UIEventListener.Get(character.gameObject).onDragEnd += OnCharacterDragEnd;
            UIEventListener.Get(character.gameObject).onDrop += OnCharacterDrop;
            UIEventListener.Get(character.gameObject).onHover += OnCharacterHover;
            UIEventListener.Get(character.gameObject).onSelect += OnCharacterSelect;
        });
    }

    #endregion
}
