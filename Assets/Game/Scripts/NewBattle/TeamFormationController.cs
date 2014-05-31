using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TeamFormationController : MonoBehaviour
{
    public List<TeamFormation> FormationList;

    public TextAsset FormationText;

    public GameObject FormationPrefab;

    public int Index;

#if UNITY_EDITOR
    private const string XmlPath = "Game/Resources/TeamFormation/TeamFormation.xml";
    private int counter;

    [HideInInspector]
    public List<GameObject> SpawnList;

    public string Description;
    public int SpawnCount;
#endif

    #region Private Fields

    public void ReadXml()
    {
        FormationList.Clear();
        Clean();

        var document = new XmlDocument();
        document.LoadXml(FormationText.text);
        var rootNode = document.SelectSingleNode("/Root");
        var formationNodeList = rootNode.SelectNodes("TeamFormation");
        foreach (XmlNode formationNode in formationNodeList)
        {
            var formation = new TeamFormation();
            formation.ReadXmlNode(formationNode);
            FormationList.Add(formation);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            RefreshView();
        }
#endif

        Logger.Log("Load xml from file: " + XmlPath + " succeed.");
    }

    private void RefreshView()
    {
        if (Index < 0 || Index >= FormationList.Count)
        {
            Logger.LogWarning("Formation index should be in range (0 - " + FormationList.Count + "), which is: " + Index + ", we will get 0 as default.");
            Index = 0;
        }

        // spawn game object if needed.
        Spawn();

        var currentFormation = FormationList[Index];
        if (SpawnCount != currentFormation.PositionList.Count)
        {
            Logger.LogWarning("Please make sure that spawn count: " + SpawnCount + " equals to formation list count: " + currentFormation.PositionList.Count);
            return;
        }

        for (var i = 0; i < SpawnCount; i++)
        {
            SpawnList[i].transform.position = currentFormation.PositionList[i];
        }
    }

#if UNITY_EDITOR
    public void WriteXml()
    {
        InjectData();

        var document = new XmlDocument();
        var rootElement = document.CreateElement("Root");
        FormationList.ForEach(formation =>
        {
            var formationElement = formation.WriteXmlElement(document);
            rootElement.AppendChild(formationElement);
        });
        document.AppendChild(rootElement);
        var path = string.Format("{0}/{1}", Application.dataPath, XmlPath);
        document.Save(path);

        Logger.Log("Save xml to file: " + XmlPath + " succeed.");
    }

    public void InjectData()
    {
        var teamFormation = new TeamFormation
        {
            Description = Description,
        };
        SpawnList.ForEach(item =>
        {
            teamFormation.PositionList.Add(item.transform.position);
        });
        FormationList.Add(teamFormation);
    }

    public void Spawn()
    {
        if (SpawnList == null)
        {
            SpawnList = new List<GameObject>();
        }
        SpawnList.Clear();
        counter = 0;

        for (var i = 0; i < SpawnCount; ++i)
        {
            var spawnObject = NGUITools.AddChild(gameObject, FormationPrefab);
            spawnObject.name = string.Format("{0}_{1}", FormationPrefab.name, counter++);
            SpawnList.Add(spawnObject);
            Debug.Log("Add spawn object: " + spawnObject.name);
        }
    }

    public void Clean()
    {
        SpawnList.ForEach(DestroyImmediate);
        SpawnList.Clear();
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
