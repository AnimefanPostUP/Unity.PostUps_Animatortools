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

using static State_Functions;
using static Clip_Generator;
using static Parameter_Functions;
using static Layer_Functions;
using static Transition_Functions;
using static Parser_Functions;
using static Save_Functions;

public class Parser_Menu
{

    public Parser_Menu()
    {
    }

    bool animatorparser;
    bool bindertrigger;
    AnimatorStateTransition lastes;
    AnimatorState tempState;
    string inputText = "";






    //empty void method for calling the menu
    public void Menu(AnimatorController controller)
    {

        animatorparser = EditorGUILayout.Foldout(animatorparser, "Parser (experimental!)");

        if (!animatorparser)
        {
            bindertrigger = false;
        }

        if (animatorparser)
        {
            GUILayout.Space(15);
            inputText = EditorGUILayout.TextField("Parser (experimental!):", inputText);

            if (Selection.activeObject is AnimatorStateTransition)
            {



                GUILayout.Space(30);
                AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;

                if (GUILayout.Button("Generate Transitions"))
                {
                    List<List<Dictionary<string, string>>> parsed = ParseString(inputText);

                    //Debug.Log( parsed.FindLastIndex);
                    //Debug parsed
                    AnimatorStateTransition tmp = selectiontransition;
                    int step = 1;
                    foreach (List<Dictionary<string, string>> objectList in parsed)
                    {

                        Debug.LogWarning("Creating Transition->");

                        foreach (Dictionary<string, string> dict in objectList)
                        {

                            ParseAndAddCondition(tmp, dict, controller);
                            Debug.Log(dict["parameter"] + " " + dict["operator"] + " " + dict["secondParameter"]);
                        }
                        //gettng for each step the count of the parsed list
                        step++;
                        if (step > 1 && step <= parsed.Count)
                            tmp = AddDuplicate(selectiontransition, controller);

                    }

                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    DeselectAll(controller);
                }

                if (GUILayout.Button("Generate Inverted Transitions"))
                {
                    List<List<Dictionary<string, string>>> parsed = ParseString(ReverseConditionString(inputText));
                    Debug.LogWarning("This Function is Unfinished");

                    AnimatorStateTransition tmp = AddDuplicateReversed(selectiontransition, controller);
                    int step = 1;
                    foreach (List<Dictionary<string, string>> objectList in parsed)
                    {

                        Debug.LogWarning("Creating Transition->");

                        foreach (Dictionary<string, string> dict in objectList)
                        {

                            ParseAndAddCondition(tmp, dict, controller);
                            Debug.Log(dict["parameter"] + " " + dict["operator"] + " " + dict["secondParameter"]);
                        }

                        step++;
                        if (step > 1 && step <= parsed.Count)
                            tmp = AddDuplicateReversed(selectiontransition, controller);
                    }

                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    DeselectAll(controller);
                }

            }
            else if (!bindertrigger) EditorGUILayout.LabelField("No Transistion Selected", EditorStyles.boldLabel);
            GUILayout.Space(30);
        }

    }

    private void DeselectAll(AnimatorController controller)
    {
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            state.state.motion = null;
        }
    }

}
