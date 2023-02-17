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
public class Quick_Animations
{

    public Quick_Animations()
    {
    }

    bool quickgenerators;
    public void Menu(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath)
    {


        if (quickgenerators)
            quickgenerators = false;
        {

            EditorGUILayout.LabelField("Quickanimations:", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Toggle Animations"))
            {
                controller = LoadController();
                GenerateToggles(animationName, target, animator, controller, usepath, scenepath);
            }

            if (GUILayout.Button("Create Linear Audio Volume"))
            {
                controller = LoadController();
                GenerateLinearAudio(animationName, target, animator, controller, usepath, scenepath);
            }

            if (GUILayout.Button("Create Hue Curve (Color+Emission)"))
            {
                controller = LoadController();
                GenerateHueCurve(animationName, target, animator, controller, usepath, scenepath);
            }
        }



    }

}
