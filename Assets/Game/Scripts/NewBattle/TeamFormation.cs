using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Team formation class.
/// </summary>
[Serializable]
public class TeamFormation
{
    /// <summary>
    /// Formation description.
    /// </summary>
    [SerializeField]
    public string Description;

    /// <summary>
    /// Position list.
    /// </summary>
    [SerializeField]
    public List<Vector3> PositionList;

    public void ReadXmlNode(XmlNode element)
    {
        var descriptionNode = element.SelectSingleNode("Description");
        Description = descriptionNode.Value;
        var positionNodeList = element.SelectNodes("Position");
        PositionList.Clear();
        foreach (XmlNode positionNode in positionNodeList)
        {
            var positionStr = positionNode.Value;
            positionStr = positionStr.Substring(1, positionStr.Length - 1);
            var positionList = positionStr.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (positionList.Length != 3)
            {
                Logger.LogError("Current position is not valid, which is: " + positionStr + ", we want a vector3f like string.");
                continue;
            }
            var position = new Vector3(float.Parse(positionList[0]), float.Parse(positionList[1]), float.Parse(positionList[2]));
            PositionList.Add(position);
        }
    }

    public XmlNode WriteXmlElement(XmlDocument document)
    {
        var node = document.CreateElement("TeamFormation");
        var descriptionNode = document.CreateElement("Description");
        descriptionNode.InnerText = Description;
        PositionList.ForEach(position =>
        {
            var positionNode = document.CreateElement("Position");
            positionNode.InnerText = string.Format("({0},{1},{2})", position.x, position.y, position.z);


        });
        return node;
    }
}
