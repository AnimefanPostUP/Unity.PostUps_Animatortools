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
using static Layer_Functions;

public class Parameter_Functions
{


    //Method to get Parameters Index
    public static int GetParameterIndex(AnimatorController controller, string name)
    {
        AnimatorControllerParameter[] parameters = controller.parameters;
        AnimatorControllerParameter param = parameters.FirstOrDefault(p => p.name == name);

        if (param != null)
        {
            return Array.IndexOf(parameters, param);
        }
        else
        {
            return -1;
        }
    }

    public static void CleanAnimatorParameters(AnimatorController controller)
    {
        //get existing parameters
        AnimatorControllerParameter[] parameters = controller.parameters;

        //get all states
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState[] states = stateMachine.states.Select(s => s.state).ToArray();

        //get all transitions
        AnimatorStateTransition[] transitions = states.SelectMany(s => s.transitions).ToArray();

        //get all conditions
        AnimatorCondition[] conditions = transitions.SelectMany(t => t.conditions).ToArray();

        //get all parameters used in conditions
        string[] usedParameters = conditions.Select(c => c.parameter).ToArray();

        //remove all parameters that are not used
        /*
        AnimatorControllerParameter[] newParameters = parameters.Where(p => usedParameters.Contains(p.name)).ToArray();
        controller.parameters = newParameters;
        */

        //remove parameters that is not in usedParameters
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (!usedParameters.Contains(parameter.name))
            {
                Debug.Log("Removing: " + parameter.name);
                controller.RemoveParameter(parameter);
            }
        }

    }

    public static void AddBoolParameterIfNotExists(AnimatorController controller, string parameterName)
    {
        AnimatorControllerParameter[] parameters = controller.parameters;
        AnimatorControllerParameter leverParam = parameters.FirstOrDefault(p => p.name == parameterName);

        if (leverParam == null)
        {
            leverParam = new AnimatorControllerParameter();
            leverParam.name = parameterName;
            leverParam.type = AnimatorControllerParameterType.Bool;

            int paramCount = parameters.Length;
            AnimatorControllerParameter[] newParams = new AnimatorControllerParameter[paramCount + 1];

            for (int i = 0; i < paramCount; i++)
            {
                newParams[i] = parameters[i];
            }

            newParams[paramCount] = leverParam;
            controller.parameters = newParams;
        }
    }

    public static bool ParameterExists(AnimatorController controller, string name)
    {
        AnimatorControllerParameter param = controller.parameters.FirstOrDefault(p => p.name == name);
        return (param != null);
    }
    public static AnimatorControllerParameterType GetParameterType(AnimatorController controller, string name)
    {
        AnimatorControllerParameter[] parameters = controller.parameters;
        AnimatorControllerParameter param = parameters.FirstOrDefault(p => p.name == name);

        return param.type;
    }

    public static void CreateParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        AnimatorControllerParameter[] parameters = controller.parameters;
        AnimatorControllerParameter param = parameters.FirstOrDefault(p => p.name == name);

        if (param == null)
        {
            param = new AnimatorControllerParameter();
            param.name = name;
            param.type = type;

            int paramCount = parameters.Length;
            AnimatorControllerParameter[] newParams = new AnimatorControllerParameter[paramCount + 1];

            for (int i = 0; i < paramCount; i++)
            {
                newParams[i] = parameters[i];
            }

            newParams[paramCount] = param;
            controller.parameters = newParams;
        }
    }
}
