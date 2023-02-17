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

public class Animator_Utils
{

    bool animatorutility;
    bool bindertrigger;
    AnimatorStateTransition lastes;
    AnimatorState tempState;


    public Animator_Utils()
    {
    }
    public void Menu(AnimatorController controller)
    {
        if (GUILayout.Button("MENU Animator "))
        {
            animatorutility = !animatorutility;
            bindertrigger = false;

        }

        if (animatorutility)
        {
            GUILayout.Space(30);

            if (false)
            { //Debuggin Method add toggle later
                if (GUILayout.Button("Clearnup  Parameters (Can Break this Tool))"))
                {
                    CleanAnimatorParameters(controller);
                }
                //Get All Selected States in the Unity Editor
            }


            
            if (Selection.activeObject is AnimatorState)
            {
                if (GUILayout.Button("Batchconnect States"))
                {
                    AnimatorState selectionstate = Selection.activeObject as AnimatorState;
                    if (selectionstate != null)
                    {
                        foreach (AnimatorState state in Selection.objects)
                        {
                            if (state != selectionstate)
                            {
                                //CreateTransition(lastesstate, state_off, parameterName, 0f, AnimatorConditionMode.If, 0f, 0f, false, false, 1f);
                                Debug.Log("" + state.name);
                            }
                        }
                    }


                }
                //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
            }
            else { EditorGUILayout.LabelField("Select States to Batchconnect (Placeholder)", EditorStyles.boldLabel); }

            GUILayout.Space(30);
            if (Selection.activeObject is AnimatorStateTransition)
            {
                AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;
                if (selectiontransition != null)
                {

                    EditorGUILayout.LabelField("Condition Tools:         " + selectiontransition.conditions, EditorStyles.boldLabel);

                    if (GUILayout.Button("Add Condition (Ignore)"))
                    {
                        AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }

                    if (!bindertrigger)
                        if (GUILayout.Button("Rebind"))
                        {
                            lastes = selectiontransition;
                            bindertrigger = true;
                        }
                }
            }
            else if (!bindertrigger) EditorGUILayout.LabelField("No Transistion Selected", EditorStyles.boldLabel);


            if (bindertrigger)
            {
                EditorGUILayout.LabelField("Select Animator State to Rebind", EditorStyles.boldLabel);
                if (GUILayout.Button("Cancel Rebind"))
                {

                }
            }

            if (Selection.activeObject != null)
                if (Selection.activeObject.GetType() == typeof(AnimatorState) && bindertrigger)
                {
                    Debug.LogWarning("Rebinding...");
                    ChangeTransitionDestination(lastes, Selection.activeObject as AnimatorState, controller);
                    bindertrigger = false;

                    //Update Window
                    tempState = AddTemptstate(controller, lastes);
                    EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);
                }
            GUILayout.Space(30);
        }
    }

}
