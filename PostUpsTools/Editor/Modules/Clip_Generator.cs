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
using static Parameter_Functions;
using static Layer_Functions;
using static Transition_Functions;
using static Save_Functions;

public class Clip_Generator
{


    public static void AddClipsAndTransition(AnimatorController controller, AnimationClip clip_on, AnimationClip clip_off, string layerName, string parameterName)
    {
        AddLayerIfNotExists(controller, layerName);
        AnimatorControllerLayer layer = controller.layers.FirstOrDefault(l => l.name == layerName);
        AnimatorStateMachine stateMachine = layer.stateMachine;
        layer.defaultWeight = 1.0f;

        AddBoolParameterIfNotExists(controller, parameterName);

        AnimatorControllerLayer newLayer = controller.layers[controller.layers.Length - 1];

        AnimatorState state_on = newLayer.stateMachine.AddState("clip_on");
        state_on.motion = clip_on;

        AnimatorState state_off = newLayer.stateMachine.AddState("clip_off");
        state_off.motion = clip_off;

        AnimatorStateTransition transition1 = CreateTransition(state_on, state_off, controller, parameterName, 0f, AnimatorConditionMode.If, 0f, 0f, false, false, 1f);
        AnimatorStateTransition transition2 = CreateTransition(state_off, state_on, controller, parameterName, 0f, AnimatorConditionMode.IfNot, 0f, 0f, false, false, 1f);

        //check if asset already exists
        if (AssetDatabase.Contains(transition1))
        {
        }
        else
        {
            AssetDatabase.AddObjectToAsset(transition1, controller);
        }

        if (AssetDatabase.Contains(transition2))
        {
        }
        else
        {
            AssetDatabase.AddObjectToAsset(transition2, controller);
        }




        EditorUtility.SetDirty(clip_on);
        EditorUtility.SetDirty(clip_off);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static void CreateAnimatorToggles(GameObject target, Animator animator, AnimatorController controller, AnimationClip clip_on, AnimationClip clip_off)
    {

        AddClipsAndTransition(controller, clip_on, clip_off, target.name + " toggle", target.name + " toggle");

    }

    public static void GenerateToggles(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath)
    {
        Debug.Log("Generating Toggles for " + target.name + " ...");
        AnimationClip clip_on;
        AnimationClip clip_off;

        //Create Clip
        clip_on = new AnimationClip();
        clip_on.name = animationName + "_on";
        AnimationCurve curve_on = new AnimationCurve();
        curve_on.AddKey(0f, 1f);
        clip_on.SetCurve(scenepath, typeof(GameObject), "m_IsActive", curve_on);

        //Add and Save Clip
        controller.AddMotion(clip_on);

        //Create Clip Off
        clip_off = new AnimationClip();
        clip_off.name = animationName + "_off";
        AnimationCurve curve_off = new AnimationCurve();
        curve_off.AddKey(0f, 0f);
        clip_off.SetCurve(scenepath, typeof(GameObject), "m_IsActive", curve_off);
        controller.AddMotion(clip_off);

        //Add and Save Clip
        if ((!File.Exists(usepath + "_on.anim") && !File.Exists(usepath + "_off.anim")))
        {
            AssetDatabase.CreateAsset(clip_on, usepath + "" + animationName + "_on.anim");
            AssetDatabase.CreateAsset(clip_off, usepath + "" + animationName + "_off.anim");
            CreateAnimatorToggles(target, animator, controller, clip_on, clip_off);
            Debug.Log("Generating Done!");
        }
        else
        {
            if (!(File.Exists(usepath + "_on.anim")))
                Debug.LogError("File >" + usepath + animationName + "_on.anim Already Exists");
            if (!(File.Exists(usepath + "_off.anim")))
                Debug.LogError("File >" + usepath + animationName + "_off.anim Already Exists");
        }

    }

    public static void GenerateLinearAudio(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath)
    {
        Debug.Log("Generating Linear Audio for " + target.name + " ...");

        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_linearaudio";
        AnimationCurve curve = AnimationCurve.Linear(0f, 0.0f, 1.0f, 1.0f);
        curve.AddKey(0f, 1f);
        clip.SetCurve(scenepath, typeof(AudioSource), "m_Volume", curve);

        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_linearaudio.anim", controller, animator);

    }

    public static void GenerateAbsoluteAudio(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, float strength)
    {
        Debug.Log("Generating Absolute Audio for " + target.name + " ...");

        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_absoluteaudio";
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, strength);
        clip.SetCurve(scenepath, typeof(AudioSource), "m_Volume", curve);

        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_absoluteaudio.anim", controller, animator);

    }

    public static void GenerateAbsoluteParticleEmission(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, float strength)
    {
        Debug.Log("Generating Absolute ParticleEmission for " + target.name + " ...");

        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_absoluteparticleemission";
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, strength);
        clip.SetCurve(scenepath, typeof(ParticleSystem), "EmissionModule.rateOverTime.scalar", curve);

        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_absoluteparticleemission.anim", controller, animator);

    }

    public static void GenerateHueCurve(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath)
    {
        Debug.Log("Generating Hue Curve for " + target.name + " ...");

        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_huecurve";

        AnimationCurve curveR = new AnimationCurve();
        AnimationCurve curveG = new AnimationCurve();
        AnimationCurve curveB = new AnimationCurve();
        AnimationCurve curveA = new AnimationCurve();

        float[] rValues = new float[] { 1f, 1f, 0f, 0f, 0f, 1f, 1f };
        float[] gValues = new float[] { 0f, 1f, 1f, 1f, 0f, 0f, 0f };
        float[] bValues = new float[] { 0f, 0f, 0f, 1f, 1f, 1f, 0f };

        for (int i = 0; i < 7; i++)
        {
            float time = i / 6f;
            curveR.AddKey(time, rValues[i]);
            curveG.AddKey(time, gValues[i]);
            curveB.AddKey(time, bValues[i]);
        }


        for (int i = 0; i < curveR.length; i++)
        {
            SetLinearTangent(curveR, i);
            SetLinearTangent(curveG, i);
            SetLinearTangent(curveB, i);
        }




        curveA.AddKey(0.0f, 1.0f);
        curveA.AddKey(1.0f, 1.0f);

        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._EmissionColor.r", curveR);
        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._EmissionColor.g", curveG);
        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._EmissionColor.b", curveB);

        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._Color.r", curveR);
        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._Color.g", curveG);
        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._Color.b", curveB);
        clip.SetCurve(scenepath, typeof(MeshRenderer), "material._Color.a", curveA);

        //Add and Save Clip

        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_huecurve.anim", controller, animator);

    }

    public static void GenerateAbsoluteTransform(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, Vector3 Location, Vector3 Rotation, Vector3 Scale)
    {
        Debug.Log("Generating Absolute Transforms for " + target.name + " ...");
        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_transform";

        // Create keyframe for the location
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.x", AnimationCurve.Constant(0f, clip.length, Location.x));
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.y", AnimationCurve.Constant(0f, clip.length, Location.y));
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.z", AnimationCurve.Constant(0f, clip.length, Location.z));

        // Create keyframe for the rotation
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.x", AnimationCurve.Constant(0f, clip.length, Rotation.x));
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.y", AnimationCurve.Constant(0f, clip.length, Rotation.y));
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.z", AnimationCurve.Constant(0f, clip.length, Rotation.z));

        // Create keyframe for the scale
        clip.SetCurve(scenepath, typeof(Transform), "localScale.x", AnimationCurve.Constant(0f, clip.length, Scale.x));
        clip.SetCurve(scenepath, typeof(Transform), "localScale.y", AnimationCurve.Constant(0f, clip.length, Scale.y));
        clip.SetCurve(scenepath, typeof(Transform), "localScale.z", AnimationCurve.Constant(0f, clip.length, Scale.z));

        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_tranform.anim", controller, animator);

    }

    public static void GenerateAbsoluteLocation(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, Vector3 Location)
    {
        Debug.Log("Generating Absolute Locations for " + target.name + " ...");
        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_location";

        // Create keyframe for the location
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.x", AnimationCurve.Constant(0f, clip.length, Location.x));
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.y", AnimationCurve.Constant(0f, clip.length, Location.y));
        clip.SetCurve(scenepath, typeof(Transform), "localPosition.z", AnimationCurve.Constant(0f, clip.length, Location.z));


        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_location.anim", controller, animator);

    }

    public static void GenerateAbsoluteRotation(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, Vector3 Rotation)
    {
        Debug.Log("Generating Absolute Rotations for " + target.name + " ...");
        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_rotation";

        // Create keyframe for the rotation
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.x", AnimationCurve.Constant(0f, clip.length, Rotation.x));
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.y", AnimationCurve.Constant(0f, clip.length, Rotation.y));
        clip.SetCurve(scenepath, typeof(Transform), "localEulerAnglesRaw.z", AnimationCurve.Constant(0f, clip.length, Rotation.z));


        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_rotation.anim", controller, animator);
    }

    public static void GenerateAbsoluteScale(string animationName, GameObject target, Animator animator, AnimatorController controller, string usepath, string scenepath, Vector3 Scale)
    {
        Debug.Log("Generating Absolute Scales for " + target.name + " ...");
        //Create Clip
        AnimationClip clip = new AnimationClip();
        clip.name = animationName + "_scales";


        // Create keyframe for the scale
        clip.SetCurve(scenepath, typeof(Transform), "localScale.x", AnimationCurve.Constant(0f, clip.length, Scale.x));
        clip.SetCurve(scenepath, typeof(Transform), "localScale.y", AnimationCurve.Constant(0f, clip.length, Scale.y));
        clip.SetCurve(scenepath, typeof(Transform), "localScale.z", AnimationCurve.Constant(0f, clip.length, Scale.z));

        //Add and Save Clip
        controller.AddMotion(clip);
        Save(clip, usepath, animationName, "_scales.anim", controller, animator);

    }


    private static void SetLinearTangent(AnimationCurve curve, int index)
    {
        Keyframe keyframe = curve[index];

        if (index == 0)
        {
            // First keyframe
            keyframe.inTangent = 0f;
        }
        else
        {
            // Calculate the tangent based on the previous keyframe
            Keyframe prevKeyframe = curve[index - 1];
            float deltaX = keyframe.time - prevKeyframe.time;
            float deltaY = keyframe.value - prevKeyframe.value;
            keyframe.inTangent = deltaY / deltaX;
        }

        if (index == curve.length - 1)
        {
            // Last keyframe
            keyframe.outTangent = 0f;
        }
        else
        {
            // Calculate the tangent based on the next keyframe
            Keyframe nextKeyframe = curve[index + 1];
            float deltaX = nextKeyframe.time - keyframe.time;
            float deltaY = nextKeyframe.value - keyframe.value;
            keyframe.outTangent = deltaY / deltaX;
        }

        curve.MoveKey(index, keyframe);
    }

}
