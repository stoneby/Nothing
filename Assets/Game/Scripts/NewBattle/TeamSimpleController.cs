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
        }
    }


    #endregion

    #region Mono

    private void Start()
    {
        // generate bounds according to its children.
        var boundGenerator = GetComponent<BoundsGenerator>() ?? gameObject.AddComponent<BoundsGenerator>();
        boundGenerator.Generate();

        CharacterList.ForEach(character =>
        {
            var listener = UIEventListener.Get(character.gameObject);
            listener.onDrag += OnCharacterDrag;
        });
    }

    #endregion
}
