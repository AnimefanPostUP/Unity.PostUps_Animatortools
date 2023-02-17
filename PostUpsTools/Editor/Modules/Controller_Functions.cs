using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using Unity.EditorCoroutines.Editor;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Controller_Functions
{
    // Start is called before the first frame update
    public static AnimatorController LoadController()
    {
        string controllerPath = "Assets/PostUpsTools/Generator.controller";
        AnimatorController existingController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (existingController == null)
        {
            return AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        }
        else
        {
            return existingController;
        }
    }

}
