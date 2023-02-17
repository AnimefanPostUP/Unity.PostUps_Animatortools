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

using static State_Functions;
using static Clip_Generator;
using static Parameter_Functions;
using static Layer_Functions;
using static Transition_Functions;


public class Save_Functions
{

    //May moved to a seperate class later
    public static IEnumerator DelayedFunction(AnimatorController controller, AnimatorState tempState)
    {
        Debug.Log("Function started");

        yield return new WaitForSeconds(1.0f);
        RemoveTempstate(controller, tempState);
        Debug.Log("Function finished");
    }


    public static  string GetRelativePathFromTo(GameObject from, GameObject to)
    {
        Transform fromTransform = from.transform;
        Transform toTransform = to.transform;

        string path = "";
        Transform currentTransform = toTransform;

        while (currentTransform != fromTransform)
        {
            path = currentTransform.name + "/" + path;
            currentTransform = currentTransform.parent;

            if (currentTransform == null)
            {
                Debug.LogError("The 'to' GameObject is not a child of the 'from' GameObject.");
                return null;
            }
        }
        return path;
    }
    public static void Save(AnimationClip clip, string filepath, string filename, string fileaddition, AnimatorController controller, Animator animator)
    {
        if (!File.Exists(filepath + filename + fileaddition))
        {
            AssetDatabase.CreateAsset(clip, filepath + filename + fileaddition);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Generating Done!");
        }
        else
        {
            Debug.LogError("File  " + filepath + filename + fileaddition + " Already Exists");
        }
    }
}
