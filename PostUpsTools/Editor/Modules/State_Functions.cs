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

using static Clip_Generator;
using static Parameter_Functions;
using static Layer_Functions;

public class State_Functions
{


    public bool HasState(AnimatorController controller, string stateName)
    {
        int stateCount = controller.layers[0].stateMachine.states.Length;
        for (int i = 0; i < stateCount; i++)
        {
            if (controller.layers[0].stateMachine.states[i].state.name == stateName)
            {
                return true;
            }
        }
        return false;
    }


    public AnimatorState GetState(AnimatorController controller, string stateName)
    {
        int stateCount = controller.layers[0].stateMachine.states.Length;
        for (int i = 0; i < stateCount; i++)
        {
            if (controller.layers[0].stateMachine.states[i].state.name == stateName)
            {
                return controller.layers[0].stateMachine.states[i].state;
            }
        }
        return null;
    }

    public int GetStateIndex(AnimatorController controller, string stateName)
    {
        int stateCount = controller.layers[0].stateMachine.states.Length;
        for (int i = 0; i < stateCount; i++)
        {
            if (controller.layers[0].stateMachine.states[i].state.name == stateName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetLayerIndex(AnimatorController animatorController, string layerName)
    {
        int layerCount = animatorController.layers.Length;
        for (int i = 0; i < layerCount; i++)
        {
            if (animatorController.layers[i].name == layerName)
            {
                return i;
            }
        }
        return -1;
    }




    public static AnimatorState GetParentState(AnimatorController controller, AnimatorStateTransition transition)
    {
        AnimatorControllerLayer[] layers = new AnimatorControllerLayer[controller.layers.Length];
        for (int i = 0; i < controller.layers.Length; i++)
        {
            layers[i] = controller.layers[i];
            var layer = layers[i];
            var stateMachine = layer.stateMachine;
            for (int j = 0; j < stateMachine.states.Length; j++)
            {
                var childState = stateMachine.states[j];
                for (int k = 0; k < childState.state.transitions.Length; k++)
                {
                    var stateTransition = childState.state.transitions[k];
                    if (stateTransition == transition)
                    {
                        return childState.state;
                    }
                }
            }
        }
        return null;
    }

    public static AnimatorState GetSourceState(AnimatorController controller, AnimatorStateTransition transition)
    {

        AnimatorControllerLayer[] layers = new AnimatorControllerLayer[controller.layers.Length];
        for (int i = 0; i < controller.layers.Length; i++)
        {
            layers[i] = controller.layers[i];
            var layer = layers[i];
            var stateMachine = layer.stateMachine;
            for (int j = 0; j < stateMachine.states.Length; j++)
            {
                var childState = stateMachine.states[j];
                for (int k = 0; k < childState.state.transitions.Length; k++)
                {
                    var stateTransition = childState.state.transitions[k];
                    if (stateTransition == transition)
                    {
                        return childState.state;
                    }
                }
            }
        }
        return null;
    }


    public static bool HasTransition(AnimatorController controller, string sourceStateName, string destinationStateName)
    {
        int stateCount = controller.layers[0].stateMachine.states.Length;
        for (int i = 0; i < stateCount; i++)
        {
            if (controller.layers[0].stateMachine.states[i].state.name == sourceStateName)
            {
                int transitionCount = controller.layers[0].stateMachine.states[i].state.transitions.Length;
                for (int j = 0; j < transitionCount; j++)
                {
                    if (controller.layers[0].stateMachine.states[i].state.transitions[j].destinationState.name == destinationStateName)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }





//Debug State--------------------------------------------------------------------

public static  AnimatorState AddTemptstate(AnimatorController controller, AnimatorStateTransition transition)
    {

        int activelayer = 0;

        //Find the Layer of the Animator State Transition without using parentStateMaschine
        for (int i = 0; i < controller.layers.Length; i++)
        {
            for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
            {
                if (controller.layers[i].stateMachine.states[j].state == transition.destinationState)
                {
                    activelayer = i;
                }
            }
        }

        //adding the state to the current active layer
        AnimatorState state = controller.layers[activelayer].stateMachine.AddState("Delete if not removed after 10s");
        return state;
    }

    public static  AnimatorState AddTemptstateByState(AnimatorController controller, AnimatorState sourcestate)
    {

        int activelayer = 0;

        //Find the Layer of the Animator State Transition without using parentStateMaschine
        for (int i = 0; i < controller.layers.Length; i++)
        {
            for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
            {
                if (controller.layers[i].stateMachine.states[j].state == sourcestate)
                {
                    activelayer = i;
                }
            }
        }

        //adding the state to the current active layer
        AnimatorState state = controller.layers[activelayer].stateMachine.AddState("Delete if not removed after 10s");
        return state;
    }


    public static void RemoveTempstate(AnimatorController controller, AnimatorState tempState)
    {

        if (tempState == null)
        {
            Debug.LogWarning("Cannot remove null state.");
            return;
        }



        //Find the layer of the Animator State Transition without using parentStateMaschine if not found Debug Log
        int activelayer = 0;
        for (int i = 0; i < controller.layers.Length; i++)
        {
            for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
            {
                if (controller.layers[i].stateMachine.states[j].state == tempState)
                {
                    activelayer = i;
                }
            }
        }

        // Remove the state from the controller
        controller.layers[activelayer].stateMachine.RemoveState(tempState);

        // Log a message to indicate that the state has been removed
        Debug.Log("Temporary state removed from animator controller.");
    }


}
