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


    public static AnimatorConditionMode InvertMode(AnimatorConditionMode mode)
    {
        if (mode == AnimatorConditionMode.If) //bool
            return AnimatorConditionMode.IfNot;
        else if (mode == AnimatorConditionMode.IfNot)
            return AnimatorConditionMode.If;
        else if (mode == AnimatorConditionMode.Equals) //int
            return AnimatorConditionMode.NotEqual;
        else if (mode == AnimatorConditionMode.NotEqual)
            return AnimatorConditionMode.Equals;
        else if (mode == AnimatorConditionMode.Greater) //float
            return AnimatorConditionMode.Less;
        else if (mode == AnimatorConditionMode.Less)
            return AnimatorConditionMode.Greater;

        return mode;
    }

    public static int modeToInt(AnimatorConditionMode mode)
    {

        switch (mode.ToString())
        {
            case "Equals":
                return 0;
            case "NotEqual":
                return 1;
            case "Greater":
                return 2;
            case "Less":
                return 3;
        }

        return -1;

    }



    public static AnimatorConditionMode intToMode(int mode)
    {
        switch (mode)
        {
            case 0:
                return AnimatorConditionMode.Equals;
            case 1:
                return AnimatorConditionMode.NotEqual;
            case 2:
                return AnimatorConditionMode.Greater;
            case 3:
                return AnimatorConditionMode.Less;
        }

        return AnimatorConditionMode.Equals;
    }



    public static AnimatorConditionMode intToBoolMode(int mode)
    {
        switch (mode)
        {
            case 0:
                return AnimatorConditionMode.If;
            case 1:
                return AnimatorConditionMode.IfNot;
        }

        return AnimatorConditionMode.If;
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

    public int iteratorvalue;
    public float iteratorvaluefloat;

    public bool iteratorvaluebool;

    public ToolCondition(string parameter, AnimatorConditionMode mode, float threshold)
    {
        this.parameter = parameter;
        this.mode = mode;
        this.threshold = threshold;
        parameterselection = false;
        flip = false;
        iterator = 0;
        iteratorfloat = 0.0f;

        iteratorvalue = 0;
        iteratorvaluefloat = 0.0f;
        iteratorvaluebool = false;
    }

    public void setFloatTresholdByString(string threshold)
    {
        float newThreshold = 0.0f;
        threshold = threshold.Replace('.', ',');

        if (float.TryParse(threshold, out newThreshold))
        {
            this.threshold = newThreshold;
        }

        this.threshold = newThreshold;
    }


    public void setIntTresholdByString(string threshold)
    {

        int newThreshold = 0;

        if (int.TryParse(threshold, out newThreshold))
        {
            this.threshold = newThreshold;
        }

        this.threshold = newThreshold;
    }

    public void iterateValues(AnimatorControllerParameterType type)
    {

        if (type == AnimatorControllerParameterType.Float)
        {
            iteratorvaluefloat += iteratorfloat;
        }
        else if (type == AnimatorControllerParameterType.Int)
        {
            iteratorvalue += iterator;
        }
        else if (type == AnimatorControllerParameterType.Bool && flip)
        {
            iteratorvaluebool = !iteratorvaluebool;
        }

    }

    public void resetPropertiesByType(AnimatorControllerParameterType type)
    {
    
            if (type == AnimatorControllerParameterType.Bool)
            {
                mode = AnimatorConditionMode.If;
                threshold = 22;
            }
            else if (type == AnimatorControllerParameterType.Float)
            {
                mode = AnimatorConditionMode.Greater;
                threshold = 0.0f;
            }
            else if (type == AnimatorControllerParameterType.Int)
            {
                mode = AnimatorConditionMode.Equals;
                threshold = 0;
            }
            else if (type == AnimatorControllerParameterType.Trigger)
            {
                mode = AnimatorConditionMode.If;
                threshold = 0;
            }
    }

    public void resetIteratorValues()
    {
        iteratorvalue = 0;
        iteratorvaluefloat = 0.0f;
        iteratorvaluebool = false;
    }


}
