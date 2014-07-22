using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BattlegroundEditorWindow : EditorWindow
{
    #region Private Fields

    private const int TextureCount = 3;
    private const string JpgExt = ".jpg";
    private const string ResourcePath = "Game/Resources/Textures/Battleground";
    private const string FilePath = "Game/Resources/Config/BattlegroundMap.txt";
    private Dictionary<int, List<string>> battlegroundMap; 

    #endregion

    #region Private Methods

    private void Intialize()
    {
        if (battlegroundMap == null)
        {
            battlegroundMap = new Dictionary<int, List<string>>();
        }

        battlegroundMap.Clear();

        var basePath = string.Format("{0}/{1}", Application.dataPath, ResourcePath);
        Debug.Log("Full path: " + basePath);
        foreach (var folder in Directory.GetDirectories(basePath))
        {
            var folderInfor = new FileInfo(folder);
            var folderID = int.Parse(folderInfor.Name);
            battlegroundMap[folderID] = new List<string>();
            Debug.Log("Find directory: " + folder + " with id: " + folderID);
            foreach (var file in Directory.GetFiles(folder).Where(file => file.EndsWith(JpgExt)))
            {
                battlegroundMap[folderID].Add(file);
                Debug.Log("Find jpg file: " + file);
            }
            if (battlegroundMap[folderID].Count != TextureCount)
            {
                Debug.LogError("Please make sure that you have put only " + TextureCount + " jpg textures to folder " + folder);
            }
        }
    }

    private void WriteToFile()
    {
        var text = new StringBuilder();
        foreach (var pair in battlegroundMap)
        {
            text.Append(pair.Key);
            foreach (var path in pair.Value)
            {
                text.AppendLine(string.Format(" {0}", path));
            }
        }

        var basePath = string.Format("{0}/{1}", Application.dataPath, FilePath);
        File.WriteAllText(basePath, text.ToString());

        Debug.Log("Write to file: " + basePath);

        AssetDatabase.Refresh();
    }

    #endregion

    #region Mono

    private void OnEnable()
    {
        Intialize();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Read battle ground to memory"))
        {
            Intialize();
        }

        if (GUILayout.Button("Write to file"))
        {
            WriteToFile();
        }
    }

    #endregion
}
