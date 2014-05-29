using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TeamFormationController : MonoBehaviour
{
    public List<TeamFormation> FormationList;

    public TextAsset FormationText;

    public GameObject FormationPrefab;

#if UNITY_EDITOR
    private const string XmlPath = "Game/Resources/TeamFormation/TeamFormation.xml";
#endif

    #region Private Fields

    public void ReadXml()
    {
        var document = new XmlDocument();
        document.LoadXml(FormationText.text);
        var rootNode = document.SelectSingleNode("/Root");
        var formationNodeList = rootNode.SelectNodes("TeamFormation");
        foreach (XmlNode formationNode in formationNodeList)
        {
            var formation = new TeamFormation();
            formation.ReadXmlNode(formationNode);
        }
    }

#if UNITY_EDITOR
    public void WriteXml()
    {
        var document = new XmlDocument();
        var rootElement = document.CreateElement("Root");
        FormationList.ForEach(formation =>
        {
            var formationElement = formation.WriteXmlElement(document);
            rootElement.AppendChild(formationElement);
        });
        document.AppendChild(rootElement);
        document.Save(string.Format("{0}/{1}", Application.dataPath, XmlPath));
    }
#endif

    #endregion

    #region Mono

    private void Awake()
    {
        if(FormationText == null)
        {
            Logger.LogError("Formation text should no be null, please make one from editor first.");
            return;
        }

        ReadXml();
    }

    #endregion
}
