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
    public List<Vector3> PositionList = new List<Vector3>();

    /// <summary>
    /// Read xml from node.
    /// </summary>
    /// <param name="element">Xml node</param>
    public void ReadXmlNode(XmlNode element)
    {
        var descriptionNode = element.SelectSingleNode("Description");
        Description = descriptionNode.InnerText;
        Logger.Log("Description: " + Description);
        var positionNodeList = element.SelectNodes("Position");
        PositionList.Clear();
        foreach (XmlNode positionNode in positionNodeList)
        {
            var positionStr = positionNode.InnerText;
            // remove the beginning and end of source string.
            positionStr = positionStr.Trim().Substring(1, positionStr.Length - 2);
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

    /// <summary>
    /// Write xml to node.
    /// </summary>
    /// <param name="document">Xml document</param>
    /// <returns>Xml node</returns>
    public XmlNode WriteXmlElement(XmlDocument document)
    {
        var node = document.CreateElement("TeamFormation");
        var descriptionNode = document.CreateElement("Description");
        descriptionNode.InnerText = Description;
        node.AppendChild(descriptionNode);
        PositionList.ForEach(position =>
        {
            var positionNode = document.CreateElement("Position");
            positionNode.InnerText = string.Format("({0},{1},{2})", position.x, position.y, position.z);
            node.AppendChild(positionNode);
        });
        return node;
    }
}
