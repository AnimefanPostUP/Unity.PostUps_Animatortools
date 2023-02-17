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

    //Array of operators


    public void Menu(AnimatorController controller)
    {
        if (GUILayout.Button("MENU Transition Copytool")) copytool = !copytool;

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

                    //get conditions from transition
                    //AnimatorCondition[] conditions = cachedTransitions[i].conditions;
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
                    {

                        pasteIndex = i;
                        pasteTrigger = true;
                    }


                    if (GUILayout.Button("  X  ", GUILayout.Width(55)))
                    {
                        //Remove transition from array
                        deletetranstiontrigger = true;

                    }
                    EditorGUILayout.LabelField("Transition: " + i, EditorStyles.boldLabel, GUILayout.Width(3f * Screen.width / 8f));


                    EditorGUILayout.EndHorizontal();


                    string[] parameters = new string[controller.parameters.Length];
                    for (int j = 0; j < controller.parameters.Length; j++)
                    {
                        parameters[j] = controller.parameters[j].name;
                    }

                    for (int j = 0; j < cachedTransitions[i].conditions.Length; j++)
                    {


                        //Parameter Values----------------------------------------------------
                        int param = GetParameterIndex(controller, cachedTransitions[i].conditions[j].parameter.ToString());
                        AnimatorControllerParameterType type = GetParameterType(controller, cachedTransitions[i].conditions[j].parameter);

                        //Save Parameter------------------------------------------------------

                        if (cachedTransitions[i].conditions[j].parameterselection)
                            if (GUILayout.Button("" + parameters[param] + " > exit selection mode"))
                            {
                                cachedTransitions[i].conditions[j].parameterselection = !cachedTransitions[i].conditions[j].parameterselection;
                            }


                        if (cachedTransitions[i].conditions[j].parameterselection)
                        {
                            //space
                            GUILayout.Space(18);
                            //get controller parameters names
                            string[] listparameters = new string[controller.parameters.Length];

                            for (int m = 0; m < controller.parameters.Length; m++)
                            {
                                listparameters[m] = controller.parameters[m].name;
                            }

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

                                    if (GUILayout.Button(listparameters[index], GUILayout.Width(Screen.width / columns)))

                                    {
                                        cachedTransitions[i].conditions[j].parameter = listparameters[index];
                                        cachedTransitions[i].conditions[j].parameterselection = false;
                                    }

                                }
                                EditorGUILayout.EndHorizontal();

                            }
                            GUILayout.Space(25);
                        }

                        EditorGUILayout.BeginHorizontal();

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


                        //debug parameter type
                        EditorGUILayout.LabelField(">" + type.ToString(), GUILayout.Width(1 * Screen.width / 10f));

                        //Transitions Mode Popup Text
                        string[] options = new string[4];

                        //set Text for Popup
                        if (type == AnimatorControllerParameterType.Float)
                        {
                            options = new string[2] { "Greater", "Less" };
                            selectedIndex = selectedIndex - 2;
                        }
                        else if (type == AnimatorControllerParameterType.Bool) //Special Treatment for bool due to the fact that is uses 2 operators as threshold
                        {

                            if (cachedTransitions[i].conditions[j].mode == AnimatorConditionMode.If)
                            {
                                selectedIndex = 0;
                            }
                            else
                            {
                                selectedIndex = 1;
                            }

                            EditorGUILayout.Popup("", 0, new string[] { "==" }, GUILayout.Width(2 * Screen.width / 10f));

                            options = new string[2] { "True", "False" };

                        }
                        else
                        if (type == AnimatorControllerParameterType.Trigger)
                        {
                            options = new string[1] { "==" };
                            selectedIndex = 0;
                        }
                        else if (type == AnimatorControllerParameterType.Int)
                        {
                            options = new string[4] { "Equals", "NotEqual", "Greater", "Less" };
                        }
                        else
                        {
                            options = new string[1] { "ERROR" };

                        }

                        //Popup
                        selectedIndex = EditorGUILayout.Popup("", selectedIndex, options, GUILayout.Width(2 * Screen.width / 10f));


                        //Add errorcorrecting here later
                        if (type == AnimatorControllerParameterType.Float) selectedIndex = selectedIndex + 2;

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
                            // make  AnimatorConditionMode.Equals if is float
                        }

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

                        if (type == AnimatorControllerParameterType.Trigger)
                        {


                        }

                        //if type is float
                        else if (type == AnimatorControllerParameterType.Float)
                        {

                            float newThreshold;
                            if (float.TryParse(EditorGUILayout.TextField("", cachedTransitions[i].conditions[j].threshold.ToString(), GUILayout.Width(2 * Screen.width / 10f)), out newThreshold))
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


                        //Delete Condition
                        if (GUILayout.Button("  X  ", GUILayout.Width(35)))
                        {
                            //Remove condition from array
                            cachedTransitions[i].conditions = cachedTransitions[i].conditions.Where((source, index) => index != j).ToArray();
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
                            Debug.Log("Transition " + i + " copied");

                            cachedTransitions[i] = new ToolTransition(true);

                            //initiate ToolCondition array
                            cachedTransitions[i].conditions = new ToolCondition[transitions[i].conditions.Length];

                            for (int j = 0; j < transitions[i].conditions.Length; j++)
                            {

                                //Debug condition parameter, mode and threshold
                                Debug.Log(transitions[i].conditions[j].parameter + " " + transitions[i].conditions[j].mode + " " + transitions[i].conditions[j].threshold);
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

//Create Transistion class for easier handling
public class ToolTransition
{
    public ToolCondition[] conditions;

    public bool copy;

    public ToolTransition(bool copy)
    {
        this.copy = copy;
    }
}

//create condition class for easier handling
public class ToolCondition
{
    public string parameter;
    public AnimatorConditionMode mode;
    public float threshold;

    public bool parameterselection;

    public ToolCondition(string parameter, AnimatorConditionMode mode, float threshold)
    {
        this.parameter = parameter;
        this.mode = mode;
        this.threshold = threshold;
        parameterselection = false;
    }
}












