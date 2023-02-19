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
    bool binddestination;
    bool bindsource;

    bool batchconnectfan;
    bool batchconnectstrip;
    AnimatorStateTransition lastes;
    AnimatorState lastesstate;
    AnimatorState lastesfanstate;
    AnimatorState tempState;


    public Animator_Utils()
    {
    }
    public void Menu(AnimatorController controller)
    {
        if (GUILayout.Button("MENU Animator "))
        {
            animatorutility = !animatorutility;
            binddestination = false;
            bindsource = false;
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



            if (Selection.activeObject is AnimatorState) //lastes = selectiontransition;
            {
                AnimatorState selectionstate = Selection.activeObject as AnimatorState;

                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Batchconnect Fan"))
                    {

                        if (selectionstate != null)
                        {
                            /*
                            foreach (AnimatorState state in Selection.objects)
                            {
                                if (state != selectionstate)
                                {
                                                       Debug.Log("" + state.name);
                                }
                            }
                            */
                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectfan = true;


                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }

                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Batchconnect Strip"))
                    {

                        if (selectionstate != null)
                        {

                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectstrip = true;


                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }

                if (batchconnectfan || batchconnectstrip)
                {
                    if (batchconnectfan && lastesstate != selectionstate && selectionstate != lastesfanstate && lastesstate != null)
                    {
                        //find first Parameter
                        CreateEmptyTransition(lastesstate, selectionstate, 0f, 0f, false, false, 0.01f);
                        lastesfanstate = selectionstate;

                        //tempState = AddTemptstateByState(controller, lastesstate);
                        //EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);
                    }

                    if (batchconnectstrip && lastesstate != selectionstate && selectionstate != lastesfanstate && lastesstate != null)
                    {
                        //find first Parameter
                        CreateEmptyTransition(lastesstate, selectionstate, 0f, 0f, true, false, 0.01f);
                        lastesstate = selectionstate;
                        lastesfanstate = selectionstate;

                        //tempState = AddTemptstateByState(controller, lastesstate);
                        //EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);
                    }


                    if (GUILayout.Button("Cancel Batchconnect"))
                    {
                        batchconnectfan = false;
                        batchconnectstrip = false;
                        lastesstate = null;
                    }
                }
            }
            else { EditorGUILayout.LabelField("Select States to Batchconnect (Placeholder)", EditorStyles.boldLabel); }



            GUILayout.Space(30);
            if (Selection.activeObject is AnimatorStateTransition)
            {
                AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;
                if (selectiontransition != null)
                {

                    EditorGUILayout.LabelField("Condition Tools:         " + selectiontransition.conditions, EditorStyles.boldLabel);

                    /*
                        if (GUILayout.Button("Add Condition (Ignore)"))
                        {
                            AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                        }
                    */


                    if (!binddestination && !bindsource && !batchconnectfan && !batchconnectstrip)
                        if (GUILayout.Button("Bind Destination"))
                        {
                            lastes = selectiontransition;
                            binddestination = true;
                        }


                    if (!bindsource && !binddestination)
                        if (GUILayout.Button("Bind Source"))
                        {
                            lastes = selectiontransition;
                            bindsource = true;
                        }

                    if (!bindsource && !binddestination)
                        if (GUILayout.Button("Swap Source and Destination"))
                        {
                            AnimatorStateTransition newtransition = SwapDestinationSource(selectiontransition, controller);
                            tempState = AddTemptstate(controller, newtransition);
                            EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);
                        }
                }
            }
            else if (!binddestination && !bindsource) EditorGUILayout.LabelField("No Transistion Selected", EditorStyles.boldLabel);


            if (binddestination || bindsource)
            {
                EditorGUILayout.LabelField("Select Animator State to Rebind", EditorStyles.boldLabel);
                if (GUILayout.Button("Cancel Rebind"))
                {
                    binddestination = false;
                    bindsource = false;
                }
            }

            if (Selection.activeObject != null)
                if (Selection.activeObject.GetType() == typeof(AnimatorState))
                {
                    if (binddestination)
                    {
                        Debug.LogWarning("Rebinding Destination...");
                        ChangeTransitionDestination(lastes, Selection.activeObject as AnimatorState, controller);
                        binddestination = false;
                        bindsource = false;
                        //Update Window
                        tempState = AddTemptstate(controller, lastes);
                        EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);
                    }
                    else if (bindsource)
                    {
                        Debug.LogWarning("Rebinding Source...");
                        AnimatorStateTransition newtransition = ChangeTransitionSource(lastes, Selection.activeObject as AnimatorState, controller);
                        binddestination = false;
                        bindsource = false;
                        tempState = AddTemptstate(controller, newtransition);
                        EditorCoroutineUtility.StartCoroutine(DelayedFunction(controller, tempState), this);

                    }
                    GUILayout.Space(30);
                }
        }
    }

}
