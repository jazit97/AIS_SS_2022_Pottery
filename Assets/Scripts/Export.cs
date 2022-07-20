using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Parabox.Stl;
using UnityEngine;

public static class Export 
{

    public static void ExportMeshToSTL(Mesh mesh, string filename = "")
    {
        if (filename == "")
        {
            filename = Guid.NewGuid().ToString();
        }

        string path = Path.Combine(Application.dataPath, "Export");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string completePath = Path.Combine(path, filename);

        Debug.Log(completePath);
        Exporter.WriteFile(completePath, mesh);
        //pb_Stl.WriteFile(path, mesh, FileType.Ascii);
        ;
        //OR
        //pb_Stl_Exporter.Export(path, new GameObject[] { objMeshToExport }, FileType.Ascii);
    }
}
