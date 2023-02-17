using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using UnityEngine.Animations;
using Unity.EditorCoroutines.Editor;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class Folder_Dialog
{
    private string path;
    private string usepath;
    private string foldername;

    private string folderpath;
    public Folder_Dialog()
    {
        path = "Assets/PostUpsTools/";
        usepath = path + "Assets/";
        foldername = "PostUpsTools";
        folderpath = usepath + foldername;
    }



    public string FolderDialogGUI()
    {
        //Custom File Path
        if (GUILayout.Button("Select File"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder Path", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                string projectPath = Application.dataPath;
                int assetsIndex = projectPath.LastIndexOf("Assets");
                projectPath = projectPath.Substring(0, assetsIndex);
                string relativePath = selectedPath.Replace(projectPath, "");
                path = relativePath;
                usepath = path + "/";

            }
            else { usepath = "Assets/PostUpsTools/"; }


        }
        return usepath;
    }


}
