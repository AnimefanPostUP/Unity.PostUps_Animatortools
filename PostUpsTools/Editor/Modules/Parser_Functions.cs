using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using Unity.EditorCoroutines.Editor;


using static State_Functions;
using static Clip_Generator;
using static Parameter_Functions;
using static Layer_Functions;
using static Transition_Functions;
using static AbsoluteAnimations;

public class Parser_Functions
{

    public static string ReverseConditionString(string input)
    {
        string output = "";
        int i = 0;
        while (i < input.Length)
        {
            if (i < input.Length - 1 && input[i] == '&' && input[i + 1] == '&')
            {
                output += "||";
                i += 2;
            }
            else if (i < input.Length - 1 && input[i] == '|' && input[i + 1] == '|')
            {
                output += "&&";
                i += 2;
            }
            else if (input[i] == '>' && (i == input.Length - 1 || input[i + 1] != '='))
            {
                output += "<";
                i += 1;
            }
            else if (input[i] == '<' && (i == input.Length - 1 || input[i + 1] != '='))
            {
                output += ">";
                i += 1;
            }
            else if (i < input.Length - 1 && input[i] == '>' && input[i + 1] == '=')
            {
                output += "<=";
                i += 2;
            }
            else if (i < input.Length - 1 && input[i] == '<' && input[i + 1] == '=')
            {
                output += ">=";
                i += 2;
            }
            else if (i < input.Length - 1 && input[i] == '=' && input[i + 1] == '=')
            {
                output += "!=";
                i += 2;
            }
            else if (i < input.Length - 1 && input[i] == '!' && input[i + 1] == '=')
            {
                output += "==";
                i += 2;
            }
            else
            {
                output += input[i];
                i += 1;
            }
        }

        return output;
    }


    public static string RemoveBrackets(string input)
    {
        // Define a regular expression to match round and square brackets
        Regex regex = new Regex(@"[\[\]]|\(|\)");

        // Replace all instances of brackets with an empty string
        string output = regex.Replace(input, "");

        return output;
    }


    public static List<string> Tokenize(string input)
    {
        char[] separators = new char[] { '(', ')', ' ', '&', '|', '=', '!', '>', '<' };
        List<string> tokens = new List<string>();
        int start = 0;
        int end = 0;

        while (end < input.Length)
        {
            while (end < input.Length && Array.IndexOf(separators, input[end]) == -1)
            {
                end++;
            }

            tokens.Add(input.Substring(start, end - start));
            end++;
            start = end;
        }

        return tokens;
    }



    public static void ParseAndAddCondition(AnimatorStateTransition transition, Dictionary<string, string> dict, AnimatorController controller)
    {
        string parameter = dict["parameter"];
        string operatorStr = dict["operator"];
        string secondParameterStr = dict["secondParameter"];

        //add parameter if it doesn't exist
        if (!ParameterExists(controller, parameter))
        {
            //get the parameter type
            AnimatorControllerParameterType type = AnimatorControllerParameterType.Float;
            if (secondParameterStr == "true" || secondParameterStr == "false")
            {
                type = AnimatorControllerParameterType.Bool;
            }
            //if is equals or not equals, set int
            else if (operatorStr == "==" || operatorStr == "!=")
            {
                type = AnimatorControllerParameterType.Int;
            }
            //if not containing commas or dots
            else if (!secondParameterStr.Contains(",") && !secondParameterStr.Contains("."))
            {
                type = AnimatorControllerParameterType.Int;
            }

            Debug.Log("Adding by Type:" + type);
            CreateParameter(controller, parameter, type);


        }

        if (secondParameterStr == "true")
        {
            secondParameterStr = "1";
        }
        else if (secondParameterStr == "false")
        {
            secondParameterStr = "0";
        }

        // Parse the operator
        AnimatorConditionMode conditionType = AnimatorConditionMode.Equals;
        switch (operatorStr)
        {
            case ">":
                conditionType = AnimatorConditionMode.Greater;
                break;
            case "<":
                conditionType = AnimatorConditionMode.Less;
                break;
            case ">=":
                dict["operator"] = ">";
                ParseAndAddCondition(transition, dict, controller);
                conditionType = AnimatorConditionMode.Equals;
                break;
            case "<=":
                dict["operator"] = "<";
                ParseAndAddCondition(transition, dict, controller);
                conditionType = AnimatorConditionMode.Equals;
                break;
            case "==":
                conditionType = AnimatorConditionMode.Equals;
                break;
            case "=":
                conditionType = AnimatorConditionMode.Equals;
                break;
            case "!=":
                conditionType = AnimatorConditionMode.NotEqual;
                break;
            default:
                // Invalid operator, do something
                break;
        }

        // Parse the threshold as a float
        //replace secondParameterStr . with , for float parsing
        secondParameterStr = secondParameterStr.Replace(".", ",");
        float threshold;
        if (!float.TryParse(secondParameterStr, out threshold))
        {
            // Invalid threshold, do something
            return;
        }

        // Add the condition to the transition
        transition.AddCondition(conditionType, threshold, RemoveBrackets(parameter));
    }

    /*
    private static List<Dictionary<string, string>> ParseCondition(string condition)
    {
        List<Dictionary<string, string>> objectList = new List<Dictionary<string, string>>();
        string[] subConditions = SplitTopLevel(condition, "||");
        foreach (string subCondition in subConditions)
        {
            string[] parts = SplitTopLevel(subCondition, "&&");
            foreach (string part in parts)
            {
                Match match = Regex.Match(part, @"^(.*?)([=!><]+)(.*?)$");
                if (match.Success)
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>
                    {
                        { "parameter", match.Groups[1].Value.Trim() },
                        { "operator", match.Groups[2].Value.Trim() },
                        { "secondParameter", match.Groups[3].Value.Trim() }
                    };
                    objectList.Add(dict);
                }
            }
        }

        // Output the list of dictionaries to the console
        Console.WriteLine("List of dictionaries:");
        foreach (Dictionary<string, string> dict in objectList)
        {
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine();
        }

        return objectList;
    }

    private static string[] SplitTopLevel(string str, string op)
    {
        int level = 0;
        List<string> parts = new List<string>();
        int startIndex = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '(')
            {
                level++;
            }
            else if (str[i] == ')')
            {
                level--;
            }
            else if (level == 0 && str.Substring(i, op.Length) == op)
            {
                parts.Add(str.Substring(startIndex, i - startIndex));
                startIndex = i + op.Length;
            }
        }
        parts.Add(str.Substring(startIndex));
        return parts.ToArray();
    }





    */
    /* better deprecated*/
    public static List<List<Dictionary<string, string>>> ParseString(string str)
    {
        List<List<Dictionary<string, string>>> result = new List<List<Dictionary<string, string>>>();

        int startIndex = str.IndexOf("if(");
        int endIndex = str.LastIndexOf(")");

        if (startIndex != -1 && endIndex != -1)
        {
            string condition = str.Substring(startIndex + 3, endIndex - startIndex - 3);
            string[] subConditions = condition.Split(new string[] { "||" }, System.StringSplitOptions.None);
            foreach (string subCondition in subConditions)
            {
                string[] parts = subCondition.Split(new string[] { "&&" }, System.StringSplitOptions.None);
                List<Dictionary<string, string>> objectList = new List<Dictionary<string, string>>();
                foreach (string part in parts)
                {
                    Match match = Regex.Match(part, @"^(.*?)([=!><]+)(.*?)$");
                    if (match.Success)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>
                    {
                        { "parameter", match.Groups[1].Value.Trim('(', ')', ' ') },
                        { "operator", match.Groups[2].Value.Trim() },
                        { "secondParameter", match.Groups[3].Value.Trim('(', ')', ' ') }
                    };
                        objectList.Add(dict);
                    }
                }
                if (objectList.Count > 0)
                {
                    result.Add(objectList);
                }
            }
        }

        return result;
    }

    /**/
    /* deprecated code
        public static List<List<Dictionary<string, string>>> ParseString(string str)
    {
        List<List<Dictionary<string, string>>> result = new List<List<Dictionary<string, string>>>();

        int startIndex = str.IndexOf("if(");
        int endIndex = str.IndexOf(")");

        if (startIndex != -1 && endIndex != -1)
        {
            string condition = str.Substring(startIndex + 3, endIndex - startIndex - 3);
            str = str.Substring(endIndex + 1);
            string[] subConditions = condition.Split(new string[] { " || " }, System.StringSplitOptions.None);
            List<Dictionary<string, string>> currentConditions = new List<Dictionary<string, string>>();
            foreach (string subCondition in subConditions)
            {
                string[] parts = subCondition.Split(new string[] { " && " }, System.StringSplitOptions.None);
                List<Dictionary<string, string>> objectList = new List<Dictionary<string, string>>();
                if (currentConditions.Count > 0)
                {
                    objectList.AddRange(currentConditions.GetRange(0, currentConditions.Count - 1));
                }
                foreach (string part in parts)
                {
                    Match match = Regex.Match(part, @"^(.*?)([=!><]+)(.*?)$");
                    if (match.Success)
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>
                        {
                            { "parameter", match.Groups[1].Value.Trim() },
                            { "operator", match.Groups[2].Value.Trim() },
                            { "secondParameter", match.Groups[3].Value.Trim() }
                        };
                        objectList.Add(dict);
                    }
                }
                if (objectList.Count > 0)
                {
                    result.Add(objectList);
                    currentConditions = objectList;
                }
            }
        }

        return result;
    }
    */

}
