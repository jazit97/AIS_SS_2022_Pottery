using System;
using System.IO;
using Parabox.Stl;
using UnityEngine;

/*
 * Provides static methods for exporting a provided mesh to different file types.
 */
public static class Export 
{

    /// <summary>
    /// Exports the mesh the mesh to the exports folder. If the folder does not exist it will be generated.
    /// If the filename is empty a GUID will be generated as the filename.
    /// </summary>
    /// <param name="mesh">Mesh to be exported</param>
    /// <param name="filename">Optional filename</param>
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

        string completePath = Path.Combine(path, filename + ".stl");

        Debug.Log(completePath);
        Exporter.WriteFile(completePath, mesh, FileType.Binary);
       
    }
}
