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


public class Recent_Objects
{

    public Recent_Objects()
    {
    }

    List<GameObject> gameobjects = new List<GameObject>();
    bool menutoggle;

    public void Menu(AnimatorController controller)
    {

        menutoggle = EditorGUILayout.Foldout(menutoggle, "Recently Used Objects");

        //is active selection is gameobject
        if (Selection.activeObject is GameObject)
        {
            AddToList(Selection.activeObject as GameObject);
        }

        if (menutoggle)
        {



            //for loop to display the list
            for (int i = 0; i < gameobjects.Count; i++)
            {

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Select " + gameobjects[i].name, GUILayout.Width(12 * Screen.width / 16f)))
                {
                    Selection.activeObject = gameobjects[i];
                }


                if (GUILayout.Button("X", GUILayout.Width(1 * Screen.width / 16f)))
                {
                    gameobjects.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }
        }

    }

    private void AddToList(GameObject gameobject)
    {
        if (gameobjects.Contains(gameobject))
        {
            gameobjects.Remove(gameobject);
        }
        gameobjects.Add(gameobject);

        //remove the oldest item if the list is longer than 5
        if (gameobjects.Count > 15)
        {
            gameobjects.RemoveAt(0);
        }
    }

}
