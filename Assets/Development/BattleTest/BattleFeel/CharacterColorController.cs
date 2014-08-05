using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColorController : MonoBehaviour
{
    public TeamSelectController TeamController;

    public List<int> ColorList;

    private void FillColor()
    {
        if (ColorList.Count != TeamController.CharacterList.Count)
        {
            Debug.LogError("ColorList count should be equals to CharacterList of teamcontroller.");
        }

        for (var i = 0; i < ColorList.Count; ++i)
        {
            var character = TeamController.CharacterList[i];
            var color = ColorList[i];
            character.ColorIndex = color;
        }
    }

    private IEnumerator NextFrame()
    {
        yield return null;
        FillColor();
    }


    void Start()
    {
        StartCoroutine(NextFrame());
    }
}
