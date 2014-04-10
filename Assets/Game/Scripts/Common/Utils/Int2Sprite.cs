﻿using System;
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
        while (value != 0)
        {
            var charToAdd = (char)(value % 10 + '0');
            result.Add(charToAdd);
            value /= 10;
        }
        result.Reverse(0, result.Count);
        return result;
    }

    public static void Show(Transform parent, UISprite template, int value)
    {
        var charList = I2CharList(value);
        var spriteWidth = template.width;
        var spriteHeight = template.height;
        int index = 1;
        foreach (var charItem in charList)
        {
            var obj = Object.Instantiate(template.gameObject) as GameObject;
            if (obj == null)
            {
                return;
            }
            obj.SetActive(true);
            var sprite = obj.GetComponent<UISprite>();
            sprite.spriteName = "" + charItem;
            sprite.width = spriteWidth;
            sprite.height = spriteHeight;
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Vector3.right * spriteWidth * index;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            index++;
        }
    }
}