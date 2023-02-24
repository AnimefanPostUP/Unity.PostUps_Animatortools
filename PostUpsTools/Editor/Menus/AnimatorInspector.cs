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
    List<SerializedProperty> serializedTransitions = new List<SerializedProperty>();
    AnimatorStateTransition[] selectedTransitionsarray;
    List<AnimatorStateTransition> selectedTransitions;

    private bool pasteTrigger;
    private int pasteIndex;


    public void Menu(AnimatorController controller)
    {

        copytool = EditorGUILayout.Foldout(copytool, "Inspector (Dummy)");

        if (copytool)
        {

            serializedTransitions.Clear();

            if (Selection.activeObject is AnimatorStateTransition)
            {
                AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;
                //initialise the selected transitions          
                selectedTransitionsarray = GetTransitions(controller, GetSourceState(controller, selectiontransition), selectiontransition.destinationState);

                selectedTransitions = new List<AnimatorStateTransition>(selectedTransitionsarray);

                SerializedObject serializedController = new SerializedObject(controller);

                foreach (AnimatorState state in controller.layers.SelectMany(layer => layer.stateMachine.states).Select(state => state.state))
                {
                    // Iterate over each transition in the state
                    foreach (AnimatorStateTransition transition in state.transitions)
                    {
                        // Check if the transition is in the selectedTransitions list
                        if (selectedTransitions.Contains(transition))
                        {
                            // Get the SerializedObject for the AnimatorStateTransition
                            SerializedObject transitionSerializedObject = new SerializedObject(transition);

                            // Add the SerializedObject to the list of serialized transitions
                            serializedTransitions.Add(transitionSerializedObject.FindProperty("m_Solo"));
                            serializedTransitions.Add(transitionSerializedObject.FindProperty("m_Mute"));
                            // Display the transition in the inspector
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_Solo"), false);
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_Mute"), false);
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_HasExitTime"), new GUIContent("Has Exit Time"));
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_ExitTime"), true);
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_HasFixedDuration"), true);
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_TransitionDuration"), new GUIContent("Duration"));
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_TransitionOffset"), new GUIContent("Offset"));
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_InterruptionSource"), new GUIContent("Interruption Source"));
                            EditorGUILayout.PropertyField(transitionSerializedObject.FindProperty("m_OrderedInterruption"), new GUIContent("Ordered Interruption"));

                            EditorGUILayout.EndVertical();
                            transitionSerializedObject.ApplyModifiedProperties();


                            // Add SerializedProperties for the conditions
                            SerializedProperty conditions = transitionSerializedObject.FindProperty("m_Conditions");
                            for (int i = 0; i < conditions.arraySize; i++)
                            {

                                SerializedProperty condition = conditions.GetArrayElementAtIndex(i);

                                if (conditions != null && conditions.arraySize > 0 && condition != null)
                                {

                                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                                    //Property Fields for the conditions
                                    EditorGUILayout.PropertyField(condition.FindPropertyRelative("m_ConditionMode"), new GUIContent("Condition Mode"));
                                    EditorGUILayout.PropertyField(condition.FindPropertyRelative("m_ConditionEvent"), new GUIContent("Event"));
                                    EditorGUILayout.PropertyField(condition.FindPropertyRelative("m_EventTreshold"), new GUIContent("Threshhold"));

                                    EditorGUILayout.EndVertical();

                                }

                            }

                            transitionSerializedObject.ApplyModifiedProperties();




                        }

                    }

                }

            }



        }
    }

    public void reload()
    {

    }


}













