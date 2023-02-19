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
using static Parameter_Functions;
using static Layer_Functions;
using static Transition_Functions;
using static Controller_Functions;
using static PostUP_UI;
public class AbsoluteAnimations
{

    private bool isActive = true;
    Vector3 location;
    Vector3 rotation;
    Vector3 scale;

    private float volume = 1.0f;
    private float particleEmission = 1.0f;

    private bool customName = false;

    private AnimatorController controller;


    private AnimationType selectedOption;

    private bool quickgenerators = false;

    public AbsoluteAnimations()
    {

    }

    public enum AnimationType
    {
        Active,
        Transform,
        Location,
        Rotation,
        Scale,
        AudioSourceVolume,
        ParticleEmission
    }

    public void GenerateSelected(string  animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath)
    {

        quickgenerators = EditorGUILayout.Foldout(quickgenerators, "Absolute Animations");

        if (quickgenerators)
        {
            GUILayout.Space(30);
            GUILayout.BeginVertical();


            //Options
            selectedOption = (AnimationType)EditorGUILayout.EnumPopup("Select Option:", selectedOption);


            //Animation Data
            switch (selectedOption)
            {

                case AnimationType.Active:
                    isActive = EditorGUILayout.Toggle("Active", isActive);
                    break;

                case AnimationType.Transform:
                    location = EditorGUILayout.Vector3Field("Location", location);
                    rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
                    scale = EditorGUILayout.Vector3Field("Scale", scale);
                    break;

                case AnimationType.Location:
                    location = EditorGUILayout.Vector3Field("Location", location);
                    break;

                case AnimationType.Rotation:
                    rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
                    break;

                case AnimationType.Scale:
                    scale = EditorGUILayout.Vector3Field("Scale", scale);
                    break;

                case AnimationType.AudioSourceVolume:
                    volume = EditorGUILayout.Slider("Volume", volume, 0.0f, 1.0f);
                    break;

                case AnimationType.ParticleEmission:
                    particleEmission = EditorGUILayout.Slider("Volume", particleEmission, 0.0f, 300.0f);
                    break;
            }




            if (GUILayout.Button("Generate Selected"))
            {
                if (!customName) animationName = target.name;
                controller = LoadController();



                switch (selectedOption)
                {

                    case AnimationType.Active:

                        break;

                    case AnimationType.Transform:
                        GenerateAbsoluteTransform(animationName, target, animator, controller, usepath, scenepath, location, rotation, scale);
                        break;

                    case AnimationType.Location:
                        GenerateAbsoluteLocation(animationName, target, animator, controller, usepath, scenepath, location);
                        break;

                    case AnimationType.Rotation:
                        GenerateAbsoluteRotation(animationName, target, animator, controller, usepath, scenepath, rotation);
                        break;

                    case AnimationType.Scale:
                        GenerateAbsoluteScale(animationName, target, animator, controller, usepath, scenepath, scale);
                        break;

                    case AnimationType.AudioSourceVolume:
                        GenerateAbsoluteAudio(animationName, target, animator, controller, usepath, scenepath, volume);
                        break;

                    case AnimationType.ParticleEmission:
                        GenerateAbsoluteParticleEmission(animationName, target, animator, controller, usepath, scenepath, particleEmission);
                        break;
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(30);
        }




    }

}