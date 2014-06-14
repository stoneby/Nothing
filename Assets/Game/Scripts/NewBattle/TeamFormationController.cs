using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class TeamFormationController : MonoBehaviour
{
    public List<TeamFormation> FormationList;

    public TextAsset FormationText;

    public GameObject FormationPrefab;

    public string XmlName;

    public int Index;

    public List<GameObject> SpawnList;

    public string Description;

    public List<Vector3> LatestPositionList
    {
        get
        {
            return (FormationList != null && FormationList.Count != 0)
                       ? FormationList[FormationList.Count - 1].PositionList
                       : null;
        }
    }

    private const string RelatedPath = "Game/Resources/TeamFormation";
    private string persistentPath;

#if UNITY_EDITOR
    private int counter;
    public int SpawnCount;
#endif

    #region Private Fields

    public void ReadXml()
    {
        FormationList.Clear();

        Clean();

        var persistentFile = string.Format("{0}/{1}.xml", Application.persistentDataPath, XmlName);
        var hasPersistentFile = File.Exists(persistentFile);
        var document = new XmlDocument();
        if (hasPersistentFile)
        {
            document.Load(persistentFile);
        }
        else
        {
            document.LoadXml(FormationText.text);            
        }
        var rootNode = document.SelectSingleNode("/Root");
        var formationNodeList = rootNode.SelectNodes("TeamFormation");
        foreach (XmlNode formationNode in formationNodeList)
        {
            var formation = new TeamFormation();
            formation.ReadXmlNode(formationNode);
            FormationList.Add(formation);
        }

        if (Index < 0 || Index >= FormationList.Count)
        {
            Logger.LogWarning("Formation index should be in range (0 - " + FormationList.Count + "), which is: " + Index + ", we will get 0 as default.");
            Index = 0;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            RefreshView();
        }
#endif
        var path = (hasPersistentFile) ? persistentFile : string.Format("{0}/{1}.xml", RelatedPath, XmlName);
        Logger.Log("Load xml from file: " + path + " succeed.");
    }

#if UNITY_EDITOR

    private void RefreshView()
    {
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
    
#endif

    public void Clean()
    {
        SpawnList.ForEach(DestroyImmediate);
        SpawnList.Clear();
    }

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

#if UNITY_EDITOR
        var path = string.Format("{0}/{1}/{2}.xml", Application.dataPath, RelatedPath, XmlName);
#else
        var path = string.Format("{0}/{1}.xml", Application.persistentDataPath, XmlName);
#endif
        document.Save(path);

        Logger.Log("Save xml to file: " + path + " succeed.");
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

    #endregion

    #region Mono

    private void Awake()
    {
        if(FormationText == null)
        {
            Logger.LogError("Formation text should no be null, please make one from editor first.");
            return;
        }

        if (string.IsNullOrEmpty(XmlName))
        {
            Logger.LogError("Xml name should not be null.");
            return;
        }

        // Note: Remember that Awake is not garanteed to be called before caller like TeamSimpleController or TeamSelectController.
        ReadXml();
    }

    #endregion
}
