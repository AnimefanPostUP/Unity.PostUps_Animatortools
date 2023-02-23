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




public class Recently_Used
{

    public Recently_Used()
    {
    }

    List<AnimatorController> controllers = new List<AnimatorController>();
    bool menutoggle;

    public void Menu(AnimatorController controller)
    {

        menutoggle = EditorGUILayout.Foldout(menutoggle, "Recently Used Animators");

        AddToList(controller);

        if (menutoggle)
        {

            GUILayout.Space(10);

            if (controller != null)
            {
                string controllerpath = AssetDatabase.GetAssetPath(controller);
                //label with path
                EditorGUILayout.LabelField("Selected Controller:" + controllerpath, EditorStyles.boldLabel);
            }

            //spacer
            GUILayout.Space(10);


            //for loop to display the list
            for (int i = 0; i < controllers.Count; i++)
            {

                //horizontal layout
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Select" + controllers[i].name, GUILayout.Width(12 * Screen.width / 16f)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(controllers[i]), typeof(AnimatorController));
                }

                //get screen width




                if (GUILayout.Button("X", GUILayout.Width(1 * Screen.width / 16f)))
                {
                    controllers.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }
        }



    }

    private void AddToList(AnimatorController controller)
    {
        if (controllers.Contains(controller))
        {
            controllers.Remove(controller);
        }
        controllers.Add(controller);

        //remove the oldest item if the list is longer than 5
        if (controllers.Count > 5)
        {
            controllers.RemoveAt(0);
        }
    }


    // https://gist.github.com/forestrf/29c391c7cae0202ea18392b03bb70ac1

}
