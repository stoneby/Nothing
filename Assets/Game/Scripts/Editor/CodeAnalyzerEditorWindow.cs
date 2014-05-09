using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CodeAnalyzerEditorWindow : EditorWindow
{
    private const string Script = ".cs";
    private Dictionary<string, List<KeyValuePair<string, int>>> codeDict = new Dictionary<string, List<KeyValuePair<string, int>>>();
    private bool show;

    private void OnGUI()
    {
        if (GUILayout.Button("Analyze"))
        {
            show = true;
            Analyze();
    
            ShowResult();
        }
    }

    private void Analyze()
    {
        codeDict.Clear();

        var root = Application.dataPath + "/Game";
        Logger.Log("Root: " + root);
        foreach (var file in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
        {
            var fileInfor = new FileInfo(file);
            if (!codeDict.ContainsKey(fileInfor.Extension))
            {
                codeDict[fileInfor.Extension] = new List<KeyValuePair<string, int>>();
            }

            var count = -1;
            if (fileInfor.Extension == Script)
            {
                count = File.ReadAllLines(file).Length;
                codeDict[fileInfor.Extension].Add(new KeyValuePair<string, int>(fileInfor.Name, count));
            }
        }
    }

    private void ShowResult()
    {
        int totalCount = 0;
        int fileCount = 0;
        var text = new StringBuilder();
        foreach (var pair in codeDict)
        {
            text.AppendLine(pair.Key);
            foreach (var keyValue in pair.Value)
            {
                text.AppendLine(keyValue.Key);
                text.AppendLine(keyValue.Value.ToString());

                totalCount += keyValue.Value;
                fileCount++;
            }
        }
        Logger.Log(text);
        Logger.Log("Total file count: " + fileCount);
        Logger.Log("Total code lines: " + totalCount);
        //GUILayout.TextArea(text.ToString(), 100);
    }
}
