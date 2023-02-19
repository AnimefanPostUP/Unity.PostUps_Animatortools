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
using static AbsoluteAnimations;
using static Controller_Functions;
public class AnimatorInspector
{


    //Trigger

    public AnimatorInspector()
    {

    }

    bool copytool;

    //array for transitions
    AnimatorStateTransition[] transitions;
    ToolTransition[] cachedTransitions;

    private int columns = 3;
    private bool deletetranstiontrigger;
    private int deleteindex;
    private bool pasteTrigger;
    private int pasteIndex;

    //Array of operators


    public void Menu(AnimatorController controller)
    {
        
        copytool = EditorGUILayout.Foldout(copytool, "Inspector (Dummy)");

        if (copytool)
        {
            // Begin vertical
            EditorGUILayout.BeginVertical();

            //Experimental
            if (cachedTransitions != null)
            {
                
                  



            }


            GUILayout.Space(30);
            if (Selection.activeObject != null)
                if (Selection.activeObject is AnimatorStateTransition)
                {



                    //Get all transitions with the same source and destination
                    AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;
                    transitions = GetTransitions(controller, GetSourceState(controller, selectiontransition), selectiontransition.destinationState);

                    //get name of sourcestate
                    string sourcename = GetSourceState(controller, selectiontransition).name;
                    string destiname = selectiontransition.destinationState.name;

                    //transitionlist to array

                    if (cachedTransitions != null)
                    {


                    }
                    else EditorGUILayout.LabelField("No Condition to Paste", EditorStyles.boldLabel);


                    if (GUILayout.Button("Copy All Transitions", GUILayout.Width(Screen.width)))
                    {
                        //initiate transition array
                        cachedTransitions = new ToolTransition[transitions.Length];


                        for (int i = 0; i < transitions.Length; i++)
                        {

                            //Debug transition name
                            //Debug.Log("Transition " + i + " copied");

                            cachedTransitions[i] = new ToolTransition(true);

                            //initiate ToolCondition array
                            cachedTransitions[i].conditions = new ToolCondition[transitions[i].conditions.Length];

                            for (int j = 0; j < transitions[i].conditions.Length; j++)
                            {

                                //Debug condition parameter, mode and threshold
                                //Debug.Log(transitions[i].conditions[j].parameter + " " + transitions[i].conditions[j].mode + " " + transitions[i].conditions[j].threshold);
                                cachedTransitions[i].conditions[j] = new ToolCondition(transitions[i].conditions[j].parameter, transitions[i].conditions[j].mode, transitions[i].conditions[j].threshold);
                            }
                        }

                    }



                    /*
                    //show conditions of transition array 
                    EditorGUILayout.LabelField("Current Transitions of Selection:", EditorStyles.boldLabel);
                    for (int i = 0; i < transitions.Length; i++)
                    {
                        EditorGUILayout.LabelField("Transition " + i);
                        for (int j = 0; j < transitions[i].conditions.Length; j++)
                        {
                            //read out array
                            EditorGUILayout.LabelField(transitions[i].conditions[j].parameter + " " + transitions[i].conditions[j].mode + " " + transitions[i].conditions[j].threshold);
                        }
                    }
                    */

                    if (Selection.activeObject is AnimatorStateTransition && cachedTransitions != null)
                    {

                        bool checkanycopy = false;
                        if (cachedTransitions != null && cachedTransitions.Length > 0)
                            if (cachedTransitions != null)
                            {
                                for (int i = 0; i < cachedTransitions.Length; i++)
                                {
                                    if (cachedTransitions[i].copy)
                                    {
                                        checkanycopy = true;
                                    }
                                }
                            }

                        if (checkanycopy == false) EditorGUILayout.LabelField("No Transitions Selected", EditorStyles.boldLabel);




                        if (cachedTransitions != null && cachedTransitions.Length > 0)
                            if (checkanycopy && GUILayout.Button("Paste Selected Transitions", GUILayout.Width(Screen.width)))
                            {



                                AnimatorState source = GetSourceState(controller, selectiontransition);
                                AnimatorState desti = selectiontransition.destinationState;


                                //iterate through all cachedtransitions
                                for (int i = 0; i < cachedTransitions.Length; i++)
                                {
                                    if (cachedTransitions[i].copy)
                                    {


                                        //create new transition object
                                        AnimatorStateTransition newtransition = source.AddTransition(desti);

                                        //iterate through all cachedconditions
                                        for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                        {
                                            //add condition to transition object
                                            newtransition.AddCondition(cachedTransitions[i].conditions[j].mode, cachedTransitions[i].conditions[j].threshold, cachedTransitions[i].conditions[j].parameter);
                                            //add transtion to controller
                                        }
                                    }
                                }

                                EditorUtility.SetDirty(controller);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();

                            }



                        if (cachedTransitions != null && cachedTransitions.Length > 0)
                            if (checkanycopy && GUILayout.Button("Overwrite All Transitions (Reload!)", GUILayout.Width(Screen.width)))
                            {


                                AnimatorState source = GetSourceState(controller, selectiontransition);
                                AnimatorState desti = selectiontransition.destinationState;


                                DeleteTransitions(controller, source, desti);

                                //iterate through all cachedtransitions
                                for (int i = 0; i < cachedTransitions.Length; i++)
                                {
                                    if (cachedTransitions[i].copy)
                                    {


                                        //create new transition object
                                        AnimatorStateTransition newtransition = source.AddTransition(desti);

                                        //iterate through all cachedconditions
                                        for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                        {
                                            //add condition to transition object
                                            newtransition.AddCondition(cachedTransitions[i].conditions[j].mode, cachedTransitions[i].conditions[j].threshold, cachedTransitions[i].conditions[j].parameter);
                                        }
                                    }
                                }

                                //save changes
                                EditorUtility.SetDirty(controller);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();


                            }



                        if (pasteTrigger)
                        {
                            pasteTrigger = false;

                            AnimatorState source = GetSourceState(controller, selectiontransition);
                            AnimatorState desti = selectiontransition.destinationState;


                            //iterate through all cachedtransitions
                            for (int i = 0; i < cachedTransitions.Length; i++)
                            {
                                if (i == pasteIndex)
                                {


                                    //create new transition object
                                    AnimatorStateTransition newtransition = source.AddTransition(desti);

                                    //iterate through all cachedconditions
                                    for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                    {
                                        //add condition to transition object

                                        newtransition.AddCondition(cachedTransitions[i].conditions[j].mode, cachedTransitions[i].conditions[j].threshold, cachedTransitions[i].conditions[j].parameter);
                                        //add transtion to controller
                                    }
                                }
                            }

                            EditorUtility.SetDirty(controller);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();




                        }
                    }
                }

            //end vertical
            EditorGUILayout.EndVertical();
        }
        else
        {

            reload();
        }

    }

    public void reload()
    {
        //reset cache
        cachedTransitions = null;

        //reset transition array
        transitions = null;

        //reset paste trigger
        pasteTrigger = false;

        //reset paste index
        pasteIndex = 0;
    }


}













