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

using static Boxdrawer;


public class Copy_Tools
{


    //Trigger

    public Copy_Tools()
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



    bool batchconnectfan;
    bool batchconnectstrip;
    AnimatorStateTransition lastes;
    AnimatorState lastesstate;
    AnimatorState lastesfanstate;

    bool reversebatch = false;

    //Array of operators


    public void Menu(AnimatorController controller)
    {
        //if (GUILayout.Button("MENU Transition Copytool")) copytool = !copytool;
        copytool = EditorGUILayout.Foldout(copytool, "Copytool");

        if (copytool)
        {
            // Begin vertical
            EditorGUILayout.BeginVertical();

            //Experimental
            if (cachedTransitions != null)
            {
                GUILayout.Space(25);
                //Loop for transitions array
                for (int i = 0; i < cachedTransitions.Length; i++)
                {


                    GUIStyle transitionBackground = new GUIStyle(GUI.skin.box);

                    if (cachedTransitions[i].copy)
                    {
                        transitionBackground.normal.background = MakeRoundRectangle((int)100, (int)100, new Color(0.16f, 0.16f, 0.16f), 2f);
                    }
                    else
                    {
                        transitionBackground.normal.background = MakeRoundRectangle((int)100, (int)100, new Color(0.26f, 0.26f, 0.26f), 2f);
                    }

                    GUIStyle conditionBackground = new GUIStyle(GUI.skin.box);
                    conditionBackground.normal.background = MakeRoundRectangle((int)100, (int)100, new Color(0.2f, 0.2f, 0.2f), 2f);

                    GUILayout.BeginVertical(transitionBackground);

                    EditorGUILayout.BeginHorizontal();

                    //Button to delete transition
                    cachedTransitions[i].copy = EditorGUILayout.Toggle("", cachedTransitions[i].copy, GUILayout.Width(30));

                    //button to deselect copy exept for the current transition
                    if (GUILayout.Button("O", GUILayout.Width(30)))
                    {
                        for (int j = 0; j < cachedTransitions.Length; j++)
                        {
                            if (i != j)
                            {
                                cachedTransitions[j].copy = false;
                            }
                            else cachedTransitions[j].copy = true;
                        }
                    }

                    if (GUILayout.Button("Paste", GUILayout.Width(1f * Screen.width / 8f)))
                    {   //Paste transition to array: will be pasted in the end of the code

                        pasteIndex = i;
                        pasteTrigger = true;
                    }


                    if (GUILayout.Button("  X  ", GUILayout.Width(55)))
                    {
                        //Remove transition from array: will be pasted in the end of the code
                        deletetranstiontrigger = true;
                    }



                    EditorGUILayout.LabelField("Transition" + i, EditorStyles.boldLabel, GUILayout.Width(1f * Screen.width / 8f));

                    /*
                            duration = 0.0f;
                            offset = 0.0f;
                            isExit = false;
                            hasExitTime = true;
                            exitTime = 0.01f;*
                    */
                    cachedTransitions[i].expandmenu = EditorGUILayout.Foldout(cachedTransitions[i].expandmenu, " settings ");

                    EditorGUILayout.EndHorizontal();

                    //foldout 


                    if (cachedTransitions[i].expandmenu)
                    {
                        GUILayout.Space(8f);
                        EditorGUILayout.BeginHorizontal();

                        cachedTransitions[i].duration = EditorGUILayout.FloatField("Duration", cachedTransitions[i].duration, GUILayout.Width(3f * Screen.width / 8f));
                        GUILayout.Space(1f * Screen.width / 8f);
                        cachedTransitions[i].offset = EditorGUILayout.FloatField("Offset", cachedTransitions[i].offset, GUILayout.Width(3f * Screen.width / 8f));

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();

                        cachedTransitions[i].hasExitTime = EditorGUILayout.Toggle("HasExitTime", cachedTransitions[i].hasExitTime, GUILayout.Width(3f * Screen.width / 8f));
                        GUILayout.Space(1f * Screen.width / 8f);
                        cachedTransitions[i].exitTime = EditorGUILayout.FloatField("ExitTime", cachedTransitions[i].exitTime, GUILayout.Width(3f * Screen.width / 8f));

                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.BeginVertical();

                    //read all Parameters
                    string[] parameters = new string[controller.parameters.Length];
                    for (int j = 0; j < controller.parameters.Length; j++)
                    {
                        parameters[j] = controller.parameters[j].name;
                    }

                    //Loop for conditions
                    for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                    {


                        //get Parameter Index and Type----------------------------------------------------
                        int param = GetParameterIndex(controller, cachedTransitions[i].conditions[j].parameter.ToString());
                        AnimatorControllerParameterType type = GetParameterType(controller, cachedTransitions[i].conditions[j].parameter);

                        //Save Parameter------------------------------------------------------

                        //Exit selection mode
                        if (cachedTransitions[i].conditions[j].parameterselection)
                        {
                            if (GUILayout.Button("" + parameters[param] + " > exit selection mode"))
                            {
                                cachedTransitions[i].conditions[j].parameterselection = !cachedTransitions[i].conditions[j].parameterselection;
                            }
                        }

                        if (cachedTransitions[i].conditions[j].parameterselection)
                        {
                            GUILayout.Space(18);

                            //get controller parameters names
                            string[] listparameters = new string[controller.parameters.Length];

                            //get controller parameters names
                            for (int m = 0; m < controller.parameters.Length; m++)
                            {
                                listparameters[m] = controller.parameters[m].name;
                            }

                            //calculate rows and columns
                            int rows = Mathf.CeilToInt(listparameters.Length / (float)columns);

                            for (int k = 0; k < rows; k++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                for (int m = 0; m < columns; m++)
                                {
                                    int index = k * columns + m;
                                    if (index >= listparameters.Length)
                                    {
                                        break;
                                    }

                                    //Buttons to select parameter
                                    if (GUILayout.Button(listparameters[index], GUILayout.Width(Screen.width / columns)))

                                    {
                                        cachedTransitions[i].conditions[j].parameter = listparameters[index];

                                        //Set mode and other depending on Type
                                        if (controller.parameters[index].type == AnimatorControllerParameterType.Bool)
                                        {
                                            cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.If;
                                            cachedTransitions[i].conditions[j].threshold = 22;
                                        }
                                        else if (controller.parameters[index].type == AnimatorControllerParameterType.Float)
                                        {
                                            cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.Greater;
                                            cachedTransitions[i].conditions[j].threshold = 0.0f;
                                        }
                                        else if (controller.parameters[index].type == AnimatorControllerParameterType.Int)
                                        {
                                            cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.Equals;
                                            cachedTransitions[i].conditions[j].threshold = 0;
                                        }
                                        else if (controller.parameters[index].type == AnimatorControllerParameterType.Trigger)
                                        {
                                            cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.If;
                                            cachedTransitions[i].conditions[j].threshold = 0;
                                        }


                                        cachedTransitions[i].conditions[j].parameterselection = false;
                                    }

                                }
                                EditorGUILayout.EndHorizontal();

                            }
                            GUILayout.Space(25);
                        }
                        EditorGUILayout.BeginHorizontal(conditionBackground);

                        //Menu is parameterselection is false
                        if (!cachedTransitions[i].conditions[j].parameterselection)
                            if (GUILayout.Button(parameters[param], GUILayout.Width(3 * Screen.width / 10f)))
                            {
                                cachedTransitions[i].conditions[j].parameterselection = !cachedTransitions[i].conditions[j].parameterselection;
                            }


                        //Save Operator------------------------------------------------------

                        //Get the selected Type
                        int selectedIndex = 0;
                        switch (cachedTransitions[i].conditions[j].mode.ToString())
                        {
                            case "Equals":
                                selectedIndex = 0;
                                break;
                            case "NotEqual":
                                selectedIndex = 1;
                                break;
                            case "Greater":
                                selectedIndex = 2;
                                break;
                            case "Less":
                                selectedIndex = 3;
                                break;

                        }

                        //Parameter Type
                        EditorGUILayout.LabelField("" + type.ToString(), GUILayout.Width(1 * Screen.width / 10f));

                        //Transitions Mode Popup Text
                        string[] options = new string[4];

                        //set Text for Popup based on Type
                        if (type == AnimatorControllerParameterType.Float)
                        {

                            options = new string[2] { "Greater", "Less" };
                            selectedIndex = selectedIndex - 2;
                            if (selectedIndex > 2 || selectedIndex < 0) { selectedIndex = 0; } //errocorection for unknow bug that doesnt initialize selected float parameters properly
                        }
                        else if (type == AnimatorControllerParameterType.Bool) //Special Treatment for bool due to the fact that is uses 2 operators as threshold
                        {

                            if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.If) { selectedIndex = 0; } else { selectedIndex = 1; }

                            EditorGUILayout.Popup("", 0, new string[] { "==" }, GUILayout.Width(2 * Screen.width / 10f));

                            options = new string[2] { "True", "False" };

                        }
                        else
                        if (type == AnimatorControllerParameterType.Trigger) //Trigger uses only one operator
                        {
                            options = new string[1] { "==" };
                            selectedIndex = 0;
                        }
                        else if (type == AnimatorControllerParameterType.Int) //Int uses 4 operators
                        {
                            options = new string[4] { "Equals", "NotEqual", "Greater", "Less" };
                        }
                        else //Error
                        {
                            Debug.LogError("Error: Parameter Type not found");
                            options = new string[1] { "ERROR" };
                            selectedIndex = -1;

                        }

                        //Popup
                        selectedIndex = EditorGUILayout.Popup("", selectedIndex, options, GUILayout.Width(2 * Screen.width / 10f));


                        //Convert Popup Index
                        if (type == AnimatorControllerParameterType.Float) selectedIndex = selectedIndex + 2;

                        // If not Bool or Trigger, set threshold
                        if (type != AnimatorControllerParameterType.Bool && type != AnimatorControllerParameterType.Trigger)
                        {
                            //switchcase for operator
                            switch (selectedIndex)
                            {
                                case 0:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.Equals;
                                    break;
                                case 1:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.NotEqual;
                                    break;
                                case 2:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.Greater;
                                    break;
                                case 3:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.Less;
                                    break;
                            }
                        }

                        //set bool
                        if (type == AnimatorControllerParameterType.Bool)
                        {
                            switch (selectedIndex)
                            {
                                case 0:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.If;
                                    break;
                                case 1:
                                    cachedTransitions[i].conditions[j].mode = AnimatorConditionMode.IfNot;
                                    break;
                            }
                        }
                        //Threshold Values----------------------------------------------------


                        //if type is float
                        else if (type == AnimatorControllerParameterType.Float)
                        {

                            float newThreshold;
                            string threshold = EditorGUILayout.TextField("", cachedTransitions[i].conditions[j].threshold.ToString(), GUILayout.Width(2 * Screen.width / 10f));
                            //convert dots to commas
                            threshold = threshold.Replace('.', ',');

                            if (float.TryParse(threshold, out newThreshold))
                            {
                                cachedTransitions[i].conditions[j].threshold = newThreshold;
                            }
                        }
                        else if (type == AnimatorControllerParameterType.Int)
                        {
                            //handle integer
                            int newThreshold2;
                            if (int.TryParse(EditorGUILayout.TextField("", cachedTransitions[i].conditions[j].threshold.ToString(), GUILayout.Width(2 * Screen.width / 10f)), out newThreshold2))
                            {
                                cachedTransitions[i].conditions[j].threshold = newThreshold2;
                            }
                        }


                        //Delete Condition Button if not Batch connecting
                        if (!batchconnectfan && !batchconnectstrip)
                        {
                            if (GUILayout.Button("  X  ", GUILayout.Width(35)))
                            {
                                //Remove condition from array
                                cachedTransitions[i].conditions = cachedTransitions[i].conditions.Where((source, index) => index != j).ToArray();
                            }
                        }
                        else
                        {

                            //switchcase by type empty for setting iterator values
                            switch (type)
                            {
                                case AnimatorControllerParameterType.Bool:
                                    //checkbox for condition.flip
                                    //label
                                    EditorGUILayout.LabelField("Invert: ", GUILayout.Width(1 * Screen.width / 16f));
                                    cachedTransitions[i].conditions[j].flip = EditorGUILayout.Toggle(cachedTransitions[i].conditions[j].flip, GUILayout.Width(15));
                                    break;
                                case AnimatorControllerParameterType.Float:
                                    //Textinput for condition.iteratorfloat including parsing
                                    float newIteratorFloat;
                                    EditorGUILayout.LabelField("Iterate: ", GUILayout.Width(1 * Screen.width / 16f));
                                    string iteratorFloat = EditorGUILayout.TextField("", cachedTransitions[i].conditions[j].iteratorfloat.ToString(), GUILayout.Width(1 * Screen.width / 16f));
                                    //convert dots to commas
                                    iteratorFloat = iteratorFloat.Replace('.', ',');
                                    if (float.TryParse(iteratorFloat, out newIteratorFloat))
                                    {
                                        cachedTransitions[i].conditions[j].iteratorfloat = newIteratorFloat;
                                    }
                                    break;
                                case AnimatorControllerParameterType.Int:
                                    //Textinput for condition.iterator including parsing
                                    int newIterator;
                                    EditorGUILayout.LabelField("Iterate: ", GUILayout.Width(1 * Screen.width / 16f));
                                    if (int.TryParse(EditorGUILayout.TextField("", cachedTransitions[i].conditions[j].iterator.ToString(), GUILayout.Width(1 * Screen.width / 16f)), out newIterator))
                                    {
                                        cachedTransitions[i].conditions[j].iterator = newIterator;
                                    }
                                    break;
                                case AnimatorControllerParameterType.Trigger:
                                    //nothing to do here
                                    break;
                            }
                        }


                        EditorGUILayout.EndHorizontal();
                    }


                    //Add new condition
                    if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                    {
                        //Add new condition to array
                        ToolCondition[] newConditions = new ToolCondition[cachedTransitions[i].conditions.Length + 1];
                        for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                        {
                            newConditions[j] = cachedTransitions[i].conditions[j];
                        }


                        //add dummy parameter if empty
                        if (controller.parameters.Length == 0)
                        {
                            //add dumy parameter to controller
                            controller.AddParameter("DummyParameter", AnimatorControllerParameterType.Bool);
                        }

                        //get parameter names
                        string[] parameters2 = new string[controller.parameters.Length];

                        for (int j = 0; j < controller.parameters.Length; j++)
                        {
                            parameters2[j] = controller.parameters[j].name;
                        }


                        //get parameter type
                        AnimatorControllerParameterType type = GetParameterType(controller, parameters2[0]);

                        //set new condition
                        newConditions[newConditions.Length - 1] = new ToolCondition(parameters2[0], AnimatorConditionMode.Equals, 0);
                        cachedTransitions[i].conditions = newConditions;



                    }

                    GUILayout.EndVertical(); //End Graphic Box for Transitions


                    GUILayout.EndVertical();

                    GUILayout.Space(25);
                }

                //Add   new Transition  to cache

                if (GUILayout.Button("Add Transition", GUILayout.ExpandWidth(false)))
                {
                    //Add new transition to array
                    ToolTransition newTransition = new ToolTransition(false);

                    //initialize conditions
                    newTransition.conditions = new ToolCondition[1];

                    // add parameter to controller if empty
                    if (controller.parameters.Length == 0)
                    {
                        controller.AddParameter("NewParameter", AnimatorControllerParameterType.Bool);
                    }

                    newTransition.conditions[0] = new ToolCondition(controller.parameters[0].name, AnimatorConditionMode.Equals, 0);


                    //add newTranstion to array
                    ToolTransition[] newTransitions = new ToolTransition[cachedTransitions.Length + 1];
                    for (int j = 0; j < cachedTransitions.Length; j++)
                    {
                        newTransitions[j] = cachedTransitions[j];
                    }
                    newTransitions[newTransitions.Length - 1] = newTransition;
                    cachedTransitions = newTransitions;

                }

                if (deletetranstiontrigger)
                {
                    cachedTransitions = cachedTransitions.Where((source, index) => index != deleteindex).ToArray();
                    deletetranstiontrigger = false;

                }

            }
            else
            {
                EditorGUILayout.LabelField("No Condition in Cache", EditorStyles.boldLabel);
                //space
                GUILayout.Space(30);

                if (!(Selection.activeObject is AnimatorStateTransition))
                {

                    EditorGUILayout.LabelField("Select a Transition for more Options", EditorStyles.boldLabel);
                }

                //Add transition to cache button
                if (GUILayout.Button("Add Transition", GUILayout.ExpandWidth(false)))
                {
                    //Add new transition to array
                    ToolTransition newTransition = new ToolTransition(false);

                    //initialize conditions
                    newTransition.conditions = new ToolCondition[1];

                    // add parameter to controller if empty
                    if (controller.parameters.Length == 0)
                    {
                        controller.AddParameter("NewParameter", AnimatorControllerParameterType.Bool);
                    }

                    newTransition.conditions[0] = new ToolCondition(controller.parameters[0].name, AnimatorConditionMode.Equals, 0);

                    //add newTranstion to cachedTransitions
                    cachedTransitions = new ToolTransition[1];
                    cachedTransitions[0] = newTransition;

                }



            }


            GUILayout.Space(30);


            if (Selection.activeObject != null)
                if (Selection.activeObject is AnimatorStateTransition)
                {



                    //Get all transitions with the same source and destination
                    AnimatorStateTransition selectiontransition = Selection.activeObject as AnimatorStateTransition;
                    transitions = GetTransitions(controller, GetSourceState(controller, selectiontransition), selectiontransition.destinationState);


                    //transitionlist to array

                    if (cachedTransitions != null)
                    {


                    }
                    else EditorGUILayout.LabelField("No Condition to Paste", EditorStyles.boldLabel);


                    //add reversed transitions to array
                    //button
                    if (GUILayout.Button("Add Selected as Reversed Transitions"))
                    {

                        bool anyselected = false;
                        int oldlength = cachedTransitions.Length;
                        int copylength = oldlength;

                        //iterate cachedTransitions and set abyselected to true if any transition is selected
                        for (int i = 0; i < cachedTransitions.Length; i++)
                        {
                            if (cachedTransitions[i].copy)
                            {
                                anyselected = true;
                            }
                        }




                        if (anyselected)
                        {
                            //iterate Transitions with .copy==true 
                            int conditioncount = 0;

                            for (int i = 0; i < cachedTransitions.Length; i++)
                            {
                                if (cachedTransitions[i].copy)
                                {
                                    //count how many conditions are in the transitions

                                    for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                    {

                                        conditioncount++;

                                    }
                                }
                            }

                            //create new array with oldlength + conditioncount
                            ToolTransition[] newTransitions = new ToolTransition[oldlength + conditioncount];

                            //copy old transitions to new array
                            for (int i = 0; i < oldlength; i++)
                            {
                                newTransitions[i] = cachedTransitions[i];
                            }

                            //iterate Transitions with .copy==true
                            for (int i = 0; i < cachedTransitions.Length; i++)
                            {
                                if (cachedTransitions[i].copy)
                                {
                                    //iterate conditions with .copy==true
                                    for (int j = 0; j < cachedTransitions[i].conditions.Length; j++) //i+oldlength
                                    {
                                        //add every condition as single transition to the new array

                                            newTransitions[copylength] = new ToolTransition(true);

                                            //Copy Transition properties
                                            newTransitions[copylength].duration = cachedTransitions[i].duration;
                                            newTransitions[copylength].exitTime = cachedTransitions[i].exitTime;
                                            newTransitions[copylength].hasExitTime = cachedTransitions[i].hasExitTime;
                                            newTransitions[copylength].offset = cachedTransitions[i].offset;
                                            //other copy options can be added here

                                            newTransitions[copylength].conditions = new ToolCondition[1];
                                            newTransitions[copylength].conditions[0] = cachedTransitions[i].conditions[j];
                                            newTransitions[copylength].conditions[0].threshold = cachedTransitions[i].conditions[j].threshold;
                                            newTransitions[copylength].conditions[0].parameter = cachedTransitions[i].conditions[j].parameter;


                                            //newTransitions[oldlength+i].conditions[0].mode = transitions[i].conditions[j].mode;
                                            //invert the mode If to IfNot and vice versa
                                            if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.If)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.IfNot;
                                            }
                                            else if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.IfNot)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.If;
                                            }

                                            //invert greater to less and vice versa
                                            else if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.Greater)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.Less;
                                            }
                                            else if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.Less)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.Greater;
                                            }

                                            //invert greater or equal to less or equal and vice versa
                                            else if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.Equals)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.NotEqual;
                                            }
                                            else if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.NotEqual)
                                            {
                                                newTransitions[copylength].conditions[0].mode = AnimatorConditionMode.Equals;
                                            }
                                            else
                                            {
                                                newTransitions[copylength].conditions[0].mode = cachedTransitions[i].conditions[j].mode;
                                            }


                                            copylength++;
                                    }
                                }
                            }

                            cachedTransitions = newTransitions;


                            for (int i = 0; i < oldlength; i++)
                            {
                                cachedTransitions[i].copy = false;
                            }
                        }

                        //save
                       




                    }








                    //Horizontal Box for Copy,Paste,Overwrite Buttons
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Copy All", GUILayout.Width(Screen.width / 4)))
                    {
                        //initiate transition array
                        cachedTransitions = new ToolTransition[transitions.Length];


                        for (int i = 0; i < transitions.Length; i++)
                        {

                            //Debug transition name
                            //Debug.Log("Transition " + i + " copied");

                            cachedTransitions[i] = new ToolTransition(true);

                            //copy transitions properties to cache
                            //cachedTransitions[i].isExit = transitions[i].isExit;
                            cachedTransitions[i].hasExitTime = transitions[i].hasExitTime;
                            //cachedTransitions[i].hasFixedDuration = transitions[i].hasFixedDuration;
                            cachedTransitions[i].exitTime = transitions[i].exitTime;
                            cachedTransitions[i].duration = transitions[i].duration;
                            cachedTransitions[i].offset = transitions[i].offset;
                            //cachedTransitions[i].mute = transitions[i].mute;
                            //cachedTransitions[i].solo = transitions[i].solo;
                            //cachedTransitions[i].canTransitionToSelf = transitions[i].canTransitionToSelf;
                            //cachedTransitions[i].interruptionSource = transitions[i].interruptionSource;
                            //cachedTransitions[i].orderedInterruption = transitions[i].orderedInterruption;
                            //cachedTransitions[i].additive = transitions[i].additive;
                            //cachedTransitions[i].removeStartOffset = transitions[i].removeStartOffset;
                            //cachedTransitions[i].mirror = transitions[i].mirror;
                            //cachedTransitions[i].keepAnimatorControllerStateOnDisable = transitions[i].keepAnimatorControllerStateOnDisable;


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

                        if (checkanycopy == false) EditorGUILayout.LabelField("No Transition Selected", EditorStyles.boldLabel);




                        if (cachedTransitions != null && cachedTransitions.Length > 0)
                            if (checkanycopy && GUILayout.Button("Paste Selected", GUILayout.Width(Screen.width / 4)))
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
                            if (checkanycopy && GUILayout.Button("Overwrite All (Reload!)", GUILayout.Width(Screen.width / 3)))
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
                                        AnimatorStateTransition newtransition = CreateEmptyTransition(
                                        source,
                                        desti,
                                        cachedTransitions[i].duration,
                                        cachedTransitions[i].offset,
                                        cachedTransitions[i].isExit,
                                        cachedTransitions[i].hasExitTime,
                                        cachedTransitions[i].exitTime);

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

                        GUILayout.EndHorizontal();


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
                                    AnimatorStateTransition newtransition = CreateEmptyTransition(
                                    source,
                                     desti,
                                     cachedTransitions[i].duration,
                                     cachedTransitions[i].offset,
                                     cachedTransitions[i].isExit,
                                     cachedTransitions[i].hasExitTime,
                                     cachedTransitions[i].exitTime);



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
                    else
                    {
                        //Begin Vertical
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(1f));
                        EditorGUILayout.LabelField("Select States to Batchconnect (Placeholder)", EditorStyles.boldLabel, GUILayout.Width(Screen.width / 1.5f));
                        EditorGUILayout.EndVertical();
                    }




                }




            if (Selection.activeObject is AnimatorState && cachedTransitions != null) //lastes = selectiontransition;
            {
                AnimatorState selectionstate = Selection.activeObject as AnimatorState;

                EditorGUILayout.BeginHorizontal();

                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Batchconnect Fan", GUILayout.Width(Screen.width / 2)))
                    {

                        if (selectionstate != null)
                        {
                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectfan = true;
                            reversebatch = false;
                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }



                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Reverse Fan", GUILayout.Width(Screen.width / 2)))
                    {

                        if (selectionstate != null)
                        {
                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectfan = true;
                            reversebatch = true;
                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }


                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();


                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Batchconnect Strip", GUILayout.Width(Screen.width / 2)))
                    {

                        if (selectionstate != null)
                        {
                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectstrip = true;
                            reversebatch = false;
                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }

                if (!batchconnectfan && !batchconnectstrip)
                    if (GUILayout.Button("Reverse Strip", GUILayout.Width(Screen.width / 2)))
                    {

                        if (selectionstate != null)
                        {
                            lastesstate = selectionstate;
                            lastesfanstate = selectionstate;
                            batchconnectstrip = true;
                            reversebatch = true;
                        }
                        //AddCondition(selectiontransition, "param", 0.0f, AnimatorConditionMode.If);
                    }

                EditorGUILayout.EndHorizontal();

                if (batchconnectfan || batchconnectstrip)
                {


                    if (GUILayout.Button("Cancel Batchconnect"))
                    {
                        batchconnectfan = false;
                        batchconnectstrip = false;
                        lastesstate = null;
                        reversebatch = false;
                    }

                    if (lastesstate == null || selectionstate == null) { batchconnectfan = false; batchconnectstrip = false; }
                    else
                    if (batchconnectfan && lastesstate != selectionstate && selectionstate != lastesfanstate)
                    {
                        //find first Parameter
                        //iterate through all cachedtransitions
                        for (int i = 0; i < cachedTransitions.Length; i++)
                        {
                            if (cachedTransitions[i].copy)
                            {

                                AnimatorStateTransition newtransition;

                                if (reversebatch)
                                {
                                    newtransition = CreateEmptyTransition(
                                        selectionstate,
                                        lastesstate,
                                        cachedTransitions[i].duration,
                                        cachedTransitions[i].offset,
                                        cachedTransitions[i].isExit,
                                        cachedTransitions[i].hasExitTime,
                                        cachedTransitions[i].exitTime);
                                }
                                else
                                {

                                    newtransition = CreateEmptyTransition(
                                        lastesstate,
                                        selectionstate,
                                        cachedTransitions[i].duration,
                                        cachedTransitions[i].offset,
                                        cachedTransitions[i].isExit,
                                        cachedTransitions[i].hasExitTime,
                                        cachedTransitions[i].exitTime);
                                }

                                //iterate through all cachedconditions
                                for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                {
                                    //add condition to transition object
                                    newtransition.AddCondition(cachedTransitions[i].conditions[j].mode, cachedTransitions[i].conditions[j].threshold, cachedTransitions[i].conditions[j].parameter);

                                    //get parameter type:
                                    AnimatorControllerParameterType paramtype = GetParameterType(controller, cachedTransitions[i].conditions[j].parameter);

                                    //switch parameter type
                                    switch (paramtype)
                                    {
                                        case AnimatorControllerParameterType.Bool:
                                            //get mode of current condition
                                            AnimatorConditionMode mode = cachedTransitions[i].conditions[j].mode;

                                            if (cachedTransitions[i].conditions[j].flip)
                                            {
                                                if (mode == AnimatorConditionMode.If)
                                                    mode = AnimatorConditionMode.IfNot;
                                                else if (mode == AnimatorConditionMode.IfNot)
                                                    mode = AnimatorConditionMode.If;
                                            }

                                            break;
                                        case AnimatorControllerParameterType.Float:
                                            //add condition.iteratorfloat to threshold if condition.flip is true
                                            cachedTransitions[i].conditions[j].threshold = cachedTransitions[i].conditions[j].threshold + cachedTransitions[i].conditions[j].iteratorfloat;
                                            break;
                                        case AnimatorControllerParameterType.Int:
                                            //add condition.iterator to threshold if condition.flip is true
                                            cachedTransitions[i].conditions[j].threshold = cachedTransitions[i].conditions[j].threshold + cachedTransitions[i].conditions[j].iterator;
                                            break;
                                        case AnimatorControllerParameterType.Trigger:
                                            //nothing to do
                                            break;
                                    }
                                }
                            }
                        }
                        lastesfanstate = selectionstate;
                        //save changes
                        EditorUtility.SetDirty(controller);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();


                    }

                    if (batchconnectstrip && lastesstate != selectionstate && selectionstate != lastesfanstate && lastesstate != null)
                    {
                        //find first Parameter
                        //iterate through all cachedtransitions
                        for (int i = 0; i < cachedTransitions.Length; i++)
                        {
                            if (cachedTransitions[i].copy)
                            {
                                //(AnimatorState sourceState, AnimatorState destinationState, float duration, float offset, bool isExit, bool hasExitTime, float exitTime)

                                AnimatorStateTransition newtransition;

                                if (reversebatch)
                                {
                                    newtransition = CreateEmptyTransition(
                                        selectionstate,
                                        lastesstate,
                                        cachedTransitions[i].duration,
                                        cachedTransitions[i].offset,
                                        cachedTransitions[i].isExit,
                                        cachedTransitions[i].hasExitTime,
                                        cachedTransitions[i].exitTime);
                                }
                                else
                                {

                                    newtransition = CreateEmptyTransition(
                                        lastesstate,
                                        selectionstate,
                                        cachedTransitions[i].duration,
                                        cachedTransitions[i].offset,
                                        cachedTransitions[i].isExit,
                                        cachedTransitions[i].hasExitTime,
                                        cachedTransitions[i].exitTime);
                                }

                                //iterate through all cachedconditions
                                for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                                {
                                    //add condition to transition object
                                    newtransition.AddCondition(cachedTransitions[i].conditions[j].mode, cachedTransitions[i].conditions[j].threshold, cachedTransitions[i].conditions[j].parameter);

                                    //get parameter type:
                                    AnimatorControllerParameterType paramtype = GetParameterType(controller, cachedTransitions[i].conditions[j].parameter);

                                    //switch parameter type
                                    switch (paramtype)
                                    {
                                        case AnimatorControllerParameterType.Bool:
                                            //get mode of current condition
                                            AnimatorConditionMode mode = cachedTransitions[i].conditions[j].mode;

                                            if (cachedTransitions[i].conditions[j].flip)
                                            {
                                                if (mode == AnimatorConditionMode.If)
                                                    mode = AnimatorConditionMode.IfNot;
                                                else if (mode == AnimatorConditionMode.IfNot)
                                                    mode = AnimatorConditionMode.If;
                                            }

                                            break;
                                        case AnimatorControllerParameterType.Float:
                                            //add condition.iteratorfloat to threshold if condition.flip is true
                                            cachedTransitions[i].conditions[j].threshold = cachedTransitions[i].conditions[j].threshold + cachedTransitions[i].conditions[j].iteratorfloat;
                                            break;
                                        case AnimatorControllerParameterType.Int:
                                            //add condition.iterator to threshold if condition.flip is true
                                            cachedTransitions[i].conditions[j].threshold = cachedTransitions[i].conditions[j].threshold + cachedTransitions[i].conditions[j].iterator;
                                            break;
                                        case AnimatorControllerParameterType.Trigger:
                                            //nothing to do
                                            break;
                                    }
                                }
                            }
                        }

                        lastesstate = selectionstate;
                        lastesfanstate = selectionstate;

                        //save changes
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

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


}

//Create Transistion class for easier handling
public class ToolTransition
{
    //(AnimatorState sourceState, AnimatorState destinationState, float duration, float offset, bool isExit, bool hasExitTime, float exitTime)

    public ToolCondition[] conditions;

    public bool copy;

    public float duration;

    public float offset;

    public bool isExit;

    public bool hasExitTime;

    public float exitTime;

    public bool expandmenu;

    public ToolTransition(bool copy)
    {
        this.copy = copy;

        duration = 0.0f;
        offset = 0.0f;
        isExit = false;
        hasExitTime = true;
        exitTime = 0.01f;
        expandmenu = false;

    }
}

//create condition class for easier handling
public class ToolCondition
{
    public string parameter;
    public AnimatorConditionMode mode;
    public float threshold;
    public bool parameterselection;
    public float iteratorfloat;
    public int iterator;
    public bool flip;

    public ToolCondition(string parameter, AnimatorConditionMode mode, float threshold)
    {
        this.parameter = parameter;
        this.mode = mode;
        this.threshold = threshold;
        parameterselection = false;
        flip = false;
        iterator = 0;
        iteratorfloat = 0.0f;
    }
}












