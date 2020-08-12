namespace InputSystem
{

    using System;

    using MelonLoader;

    using UnityEngine;

    public class InputExample
    {

        // if you want unique per instance
        // = Guid.NewGuid().ToString();
        private readonly string inputExampleID = "InputExampleActions";

        // not needed at exit but if you want to de-register
        public void OnApplicationQuit()
        {
            InputSystem.RemoveAllActions(inputExampleID);
        }

        // can also register separate action id's if you want to
        public void OnApplicationStart()
        {
            InputSystem.RegisterClickAction(inputExampleID, () => MelonLogger.Log("F Key Clicked"), KeyCode.F);
            InputSystem.RegisterDoubleClickAction(inputExampleID, () => MelonLogger.Log("F Key Double Clicked"), KeyCode.F);
            InputSystem.RegisterHoldAction(inputExampleID, () => MelonLogger.Log("Holding Down F"), KeyCode.F);
            InputSystem.RegisterHoldReleasedAction(inputExampleID, () => MelonLogger.Log("F Key Hold Released"), KeyCode.F);

            InputSystem.RegisterClickAction(inputExampleID, () => MelonLogger.Log("Left Trigger Clicked"), InputAxes.LeftTrigger);
            InputSystem.RegisterDoubleClickAction(inputExampleID, () => MelonLogger.Log("Left Trigger  Double Clicked"), InputAxes.LeftTrigger);
            InputSystem.RegisterHoldAction(inputExampleID, () => MelonLogger.Log("Holding Down Left Trigger "), InputAxes.LeftTrigger);
            InputSystem.RegisterHoldReleasedAction(inputExampleID, () => MelonLogger.Log("Left Trigger Hold Released"), InputAxes.LeftTrigger);
        }

    }

}