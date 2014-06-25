using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// The class is to use number sprite to show number text instead of number text.
/// </summary>
public class Int2Sprite
{
    /// <summary>
    /// Convert integer to char list.
    /// </summary>
    /// <param name="value">The integer to be converted.</param>
    /// <returns>The char list which is converted from the integer value.</returns>
    /// <exception cref="Exception"></exception>
    private static IEnumerable<char> I2CharList(int value)
    {
        if (value < 0)
        {
            throw new Exception("The integer value is not valid to convert.");
        }
        var result = new List<char>();
        if (value == 0)
        {
            result.Add('0');
        }
        else
        {
            while (value != 0)
            {
                var charToAdd = (char)(value % 10 + '0');
                result.Add(charToAdd);
                value /= 10;
            }
        }
        
        result.Reverse(0, result.Count);
        return result;
    }

    public static List<GameObject> Show(Transform parent, UISprite template, int value, List<GameObject> objlist = null)
    {
        var charList = I2CharList(value) as List<char>;
        var spriteWidth = template.width;
        var spriteHeight = template.height;
        if (objlist == null) objlist = new List<GameObject>();
        for (var i = 0; i < objlist.Count; i++)
        {
            objlist[i].SetActive(false);
        }

        for (var i = objlist.Count; i < charList.Count; i++)
        {
            var obj = Object.Instantiate(template.gameObject) as GameObject;
            if (obj == null)
            {
                return null;
            }
            obj.SetActive(true);
            var sprite = obj.GetComponent<UISprite>();
            sprite.width = spriteWidth;
            sprite.height = spriteHeight;
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Vector3.right * spriteWidth * i;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            objlist.Add(obj);
        }

        for (var i = 0; i < charList.Count; i++)
        {
            objlist[i].SetActive(true);
            var sprite = objlist[i].GetComponent<UISprite>();
            sprite.spriteName = charList[i].ToString();

        }
        return objlist;
    }
}
