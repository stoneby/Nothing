using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEngine;

public class ZipTest : MonoBehaviour
{
    public string UrlPath;

    private WWW www;
    private bool isUnzipped;

    // Use this for initialization
    void Start()
    {
        www = new WWW(UrlPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (www.isDone && !isUnzipped)
        {
            Debug.Log("Load of test.zip complete");
            byte[] data = www.bytes;

            string docPath = Application.dataPath;
            docPath = docPath.Substring(0, docPath.Length - 5);
            docPath = docPath.Substring(0, docPath.LastIndexOf("/"));
            docPath += "/test.zip";
            Debug.Log("docPath=" + docPath);
            System.IO.File.WriteAllBytes(docPath, data);

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(docPath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        string filename = docPath.Substring(0, docPath.Length - 8);
                        filename += theEntry.Name;
                        Debug.Log("Unzipping: " + filename);
                        using (FileStream streamWriter = File.Create(filename))
                        {
                            int size = 2048;
                            byte[] fdata = new byte[2048];
                            while (true)
                            {
                                size = s.Read(fdata, 0, fdata.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(fdata, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                isUnzipped = true;
            }
        }
    }
}