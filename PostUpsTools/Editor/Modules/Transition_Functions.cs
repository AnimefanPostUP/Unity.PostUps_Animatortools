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


public class Transition_Functions : MonoBehaviour
{


    //Method get gets all transition with the same source and destination state
    public static AnimatorStateTransition[] GetTransitions(AnimatorController controller, AnimatorState sourceState, AnimatorState destinationState)
    {
        List<AnimatorStateTransition> transitions = new List<AnimatorStateTransition>();

        //get layer count
        int layerCount = controller.layers.Length;

        //find the layer that contains the source state
        for (int i = 0; i < layerCount; i++)
        {
            AnimatorStateMachine stateMachine = controller.layers[i].stateMachine;

            foreach (ChildAnimatorState childState in stateMachine.states)
            {
                if (childState.state == sourceState)
                {
                    foreach (AnimatorStateTransition transition in childState.state.transitions)
                    {

                        //Debug to see if the transition is the one we want

                        if (transition.destinationState == destinationState)
                        {
                            transitions.Add(transition);
                        }
                    }
                }
            }
        }





        return transitions.ToArray();
    }


    //Delete all transitions with the same source and destination state
    public static void DeleteTransitions(AnimatorController controller, AnimatorState sourceState, AnimatorState destinationState)
    {
        AnimatorStateTransition[] transitions = GetTransitions(controller, sourceState, destinationState);

        foreach (AnimatorStateTransition transition in transitions)
        {
            AnimatorState parent = GetParentState(controller, transition);

            //check if transition has same source and destination state
            parent.RemoveTransition(transition);
        }
    }


    public static AnimatorStateTransition AddDuplicate(AnimatorStateTransition originalTransition, AnimatorController controller)
    {

        //AnimatorState source = GetSourceState(controller, originalTransition);
        AnimatorState source = originalTransition.destinationState;
        AnimatorState parent = GetParentState(controller, originalTransition);

        AnimatorStateTransition newTransition = parent.AddTransition(source);
        newTransition.duration = originalTransition.duration;
        newTransition.offset = originalTransition.offset;
        newTransition.exitTime = originalTransition.exitTime;
        newTransition.hasExitTime = originalTransition.hasExitTime;
        newTransition.interruptionSource = originalTransition.interruptionSource;
        newTransition.orderedInterruption = originalTransition.orderedInterruption;
        newTransition.mute = originalTransition.mute;
        newTransition.solo = originalTransition.solo;
        newTransition.canTransitionToSelf = originalTransition.canTransitionToSelf;
        return newTransition;
    }

    public static AnimatorStateTransition AddDuplicateOn(AnimatorStateTransition originalTransition, AnimatorState source, AnimatorState destination, AnimatorController controller)
    {

        AnimatorStateTransition newTransition = source.AddTransition(destination);

        newTransition.duration = originalTransition.duration;
        newTransition.offset = originalTransition.offset;
        newTransition.exitTime = originalTransition.exitTime;
        newTransition.hasExitTime = originalTransition.hasExitTime;
        newTransition.interruptionSource = originalTransition.interruptionSource;
        newTransition.orderedInterruption = originalTransition.orderedInterruption;
        newTransition.mute = originalTransition.mute;
        newTransition.solo = originalTransition.solo;
        newTransition.canTransitionToSelf = originalTransition.canTransitionToSelf;
 
        

        return newTransition;
    }



    public static AnimatorStateTransition AddDuplicateReversed(AnimatorStateTransition originalTransition, AnimatorController controller)
    {

        //AnimatorState source = GetSourceState(controller, originalTransition);
        AnimatorState source = originalTransition.destinationState;
        AnimatorState parent = GetParentState(controller, originalTransition);

        AnimatorStateTransition newTransition = source.AddTransition(parent);
        newTransition.duration = originalTransition.duration;
        newTransition.offset = originalTransition.offset;
        newTransition.exitTime = originalTransition.exitTime;
        newTransition.hasExitTime = originalTransition.hasExitTime;
        newTransition.interruptionSource = originalTransition.interruptionSource;
        newTransition.orderedInterruption = originalTransition.orderedInterruption;
        newTransition.mute = originalTransition.mute;
        newTransition.solo = originalTransition.solo;
        newTransition.canTransitionToSelf = originalTransition.canTransitionToSelf;
        return newTransition;
    }

    public static AnimatorStateTransition CreateTransition(AnimatorState sourceState, AnimatorState destinationState, string parameter, float threshold, AnimatorConditionMode conditionType, float duration, float offset, bool isExit, bool hasExitTime, float exitTime)
    {
        AnimatorStateTransition transition = new AnimatorStateTransition();

        transition.destinationState = destinationState;
        transition.isExit = isExit;
        transition.hasExitTime = hasExitTime;
        transition.duration = duration;
        transition.offset = offset;
        transition.AddCondition(conditionType, threshold, parameter);
        transition.exitTime = exitTime;
        sourceState.AddTransition(transition);

        return transition;
    }


    public static void AddCondition(AnimatorStateTransition transition, string parameter, float threshold, AnimatorConditionMode conditionType)
    {
        AnimatorCondition[] conditions = transition.conditions;
        AnimatorCondition newCondition = new AnimatorCondition();
        //newCondition.parameter = parameter;
        //newCondition.threshold = threshold;
        //newCondition.mode = conditionType;
        AnimatorCondition[] newConditions = new AnimatorCondition[conditions.Length + 1];
        for (int i = 0; i < conditions.Length; i++)
        {
            newConditions[i] = conditions[i];
        }
        newConditions[newConditions.Length - 1] = newCondition;
        transition.conditions = newConditions;

    }

    public static void ChangeTransitionDestination(AnimatorStateTransition transition, AnimatorState destination, AnimatorController controller)
    {
        // Change the destination state of the transition to the new state
        transition.destinationState = destination;
    }

    public static AnimatorStateTransition ChangeTransitionSource(AnimatorStateTransition transition, AnimatorState newsource, AnimatorController controller)
    {
        AnimatorState parent = GetParentState(controller, transition);
        AnimatorStateTransition newtransition=AddDuplicateOn(transition, newsource , transition.destinationState , controller);
        parent.RemoveTransition(transition);

        return newtransition;

    }

    
    public static AnimatorStateTransition SwapDestinationSource(AnimatorStateTransition transition, AnimatorController controller)
    {
        AnimatorState parent = GetParentState(controller, transition);
        AnimatorStateTransition newtransition=AddDuplicateOn(transition , transition.destinationState , parent, controller);
        parent.RemoveTransition(transition);

        return newtransition;

    }

    //Remove Transition from a state
    public static void RemoveTransitionfromState(AnimatorStateTransition transition, AnimatorController controller, AnimatorState sourceState)
    {

        //deleting transition only from state   
        AnimatorStateTransition[] transitions = sourceState.transitions;
        List<AnimatorStateTransition> newTransitions = new List<AnimatorStateTransition>();
        foreach (AnimatorStateTransition t in transitions)
        {
            if (t != transition)
            {
                newTransitions.Add(t);
            }
        }
        sourceState.transitions = newTransitions.ToArray();



    }

}
