namespace InputSystem
{

    using System;
    using System.Collections.Generic;

    using MelonLoader;

    using UnityEngine;

    public sealed class InputSystem : MelonMod
    {

        /// <summary>
        ///     Settings currently in use
        /// </summary>
        public static Settings InputSettings;

        private static readonly Dictionary<string, InputValues<float>> AxisDictionary = new Dictionary<string, InputValues<float>>();

        private static readonly Dictionary<KeyCode, InputValues<bool>> KeyCodeDictionary = new Dictionary<KeyCode, InputValues<bool>>();

        private static MelonPreferences_Category inputSettingsCategory;

        private static MelonPreferences_Entry<float> clickThresholdEntry, doubleClickThresholdEntry, holdTimeThresholdEntry, triggerThresholdEntry;

        public static void RegisterClickAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].ClickActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterClickAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].ClickActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterDoubleClickAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].DoubleClickActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterDoubleClickAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].DoubleClickActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldStartAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].HoldStartActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldStartAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].HoldStartActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldRepeatAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].HoldRepeatActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldRepeatAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].HoldRepeatActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldReleasedAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].HoldReleasedActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldReleasedAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].HoldReleasedActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RemoveAllActions(string id)
        {
            RemoveClickAction(id);
            RemoveDoubleClickAction(id);
            RemoveHoldStartAction(id);
            RemoveHoldReleasedAction(id);
            RemoveHoldRepeatAction(id);
        }

        public static void RemoveClickAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.ClickActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.ClickActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void RemoveDoubleClickAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.DoubleClickActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.DoubleClickActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void RemoveHoldStartAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.HoldStartActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.HoldStartActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void RemoveHoldRepeatAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.HoldRepeatActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.HoldRepeatActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void RemoveHoldReleasedAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.HoldReleasedActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.HoldReleasedActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Checks if a key has been clicked/pressed twice within a specific time threshold.
        /// </summary>
        /// <param name="keyCode">Key to check</param>
        /// <param name="lastTimeClicked">Your own stored reference to last time it was handled</param>
        /// <param name="threshold">Max amount of time between clicks to count as double</param>
        /// <param name="multipleInRow">Whether to keep re-using last click as a double click after eachother or not</param>
        /// <example>
        ///     private float lastTimeForwardClicked = 5f; // has to be set
        ///     if (HasDoubleClicked(KeyCode.W, ref lastTimeForwardClicked)) { DoStuff(); }
        /// </example>
        /// <returns>Whether the key has been double clicked or not</returns>
        public static bool HasDoubleClicked(KeyCode keyCode, ref float lastTimeClicked, float threshold = .5f, bool multipleInRow = false)
        {
            if (!Input.GetKeyDown(keyCode)) return false;
            if (Time.time - lastTimeClicked <= threshold)
            {
                lastTimeClicked = multipleInRow ? Time.time : threshold * 2f;
                return true;
            }

            lastTimeClicked = Time.time;
            return false;
        }

        public override void OnApplicationStart()
        {
            InputSettings.ClickTimeThreshold = 0.3f;
            InputSettings.DoubleClickTimeThreshold = 0.5f;
            InputSettings.HoldTimeThreshold = 1f;
            InputSettings.TriggerThreshold = 0.4f;

            inputSettingsCategory = MelonPreferences.CreateCategory(BuildInfo.Name, "Input System");
            clickThresholdEntry = (MelonPreferences_Entry<float>)inputSettingsCategory.CreateEntry(
                "ClickThreshold",
                InputSettings.ClickTimeThreshold,
                "Click Time-Threshold");

            doubleClickThresholdEntry = (MelonPreferences_Entry<float>)inputSettingsCategory.CreateEntry(
                "DoubleClickThreshold",
                InputSettings.DoubleClickTimeThreshold,
                "Double-Click Time-Threshold");

            holdTimeThresholdEntry = (MelonPreferences_Entry<float>)inputSettingsCategory.CreateEntry(
                "HoldThreshold",
                InputSettings.HoldTimeThreshold,
                "Hold Time-Threshold");

            triggerThresholdEntry = (MelonPreferences_Entry<float>)inputSettingsCategory.CreateEntry(
                "TriggerThreshold",
                InputSettings.TriggerThreshold,
                "Trigger Threshold");

            ApplySettings();
        }

        /// <summary>
        ///     Resets the input settings back to default
        /// </summary>
        public static void ResetSettings()
        {
            clickThresholdEntry.ResetToDefault();
            clickThresholdEntry.Save();

            doubleClickThresholdEntry.ResetToDefault();
            doubleClickThresholdEntry.Save();

            holdTimeThresholdEntry.ResetToDefault();
            holdTimeThresholdEntry.Save();

            triggerThresholdEntry.ResetToDefault();
            triggerThresholdEntry.Save();
        }

        private static void ApplySettings()
        {
            InputSettings.ClickTimeThreshold = clickThresholdEntry.Value;
            InputSettings.DoubleClickTimeThreshold = doubleClickThresholdEntry.Value;
            InputSettings.HoldTimeThreshold = holdTimeThresholdEntry.Value;
            InputSettings.TriggerThreshold = triggerThresholdEntry.Value;
        }

        public override void OnPreferencesSaved()
        {
            ApplySettings();
        }

        public override void OnUpdate()
        {
            // Check Axis
            foreach (KeyValuePair<string, InputValues<float>> pair in AxisDictionary)
                try
                {
                    InputValues<float> value = pair.Value;
                    value.Previous = value.Current;
                    value.Current = Input.GetAxis(pair.Key);

                    // being held down
                    if (Mathf.Abs(value.Current) >= InputSettings.TriggerThreshold)
                    {
                        value.TimeHeld += Time.deltaTime;
                        if (value.TimeHeld >= InputSettings.HoldTimeThreshold)
                            if (!value.HoldTriggered)
                            {
                                value.HoldTriggered = true;
                                foreach (InputAction action in value.HoldStartActions) action.Action?.Invoke();
                            }
                            else
                            {
                                foreach (InputAction holdRepeatAction in value.HoldRepeatActions) holdRepeatAction.Action?.Invoke();
                            }
                    }
                    else
                    {
                        if (value.TimeHeld > 0f
                            && value.TimeHeld <= InputSettings.ClickTimeThreshold)
                        {
                            if (Time.time - value.LastTimeClicked <= InputSettings.DoubleClickTimeThreshold)
                            {
                                value.LastTimeClicked = InputSettings.DoubleClickTimeThreshold * 2;
                                foreach (InputAction action in value.DoubleClickActions) action.Action?.Invoke();
                            }
                            else
                            {
                                value.LastTimeClicked = Time.time;
                                foreach (InputAction action in value.ClickActions) action.Action?.Invoke();
                            }
                        }

                        if (value.HoldTriggered)
                            foreach (InputAction action in value.HoldReleasedActions)
                                action.Action?.Invoke();

                        value.HoldTriggered = false;
                        value.TimeHeld = 0f;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e.Message);
                }

            // Check Keys
            foreach (KeyValuePair<KeyCode, InputValues<bool>> pair in KeyCodeDictionary)
                try
                {
                    InputValues<bool> value = pair.Value;
                    value.Previous = value.Current;
                    value.Current = Input.GetKey(pair.Key);

                    // being held down
                    if (value.Current)
                    {
                        value.TimeHeld += Time.deltaTime;
                        if (value.TimeHeld >= InputSettings.HoldTimeThreshold)
                            if (!value.HoldTriggered)
                            {
                                value.HoldTriggered = true;
                                foreach (InputAction action in value.HoldStartActions) action.Action?.Invoke();
                            }
                            else
                            {
                                foreach (InputAction holdRepeatAction in value.HoldRepeatActions) holdRepeatAction.Action?.Invoke();
                            }
                    }
                    else
                    {
                        if (value.TimeHeld > 0f
                            && value.TimeHeld <= InputSettings.ClickTimeThreshold)
                        {
                            if (Time.time - value.LastTimeClicked <= InputSettings.DoubleClickTimeThreshold)
                            {
                                value.LastTimeClicked = InputSettings.DoubleClickTimeThreshold * 2;
                                foreach (InputAction action in value.DoubleClickActions) action.Action?.Invoke();
                            }
                            else
                            {
                                value.LastTimeClicked = Time.time;
                                foreach (InputAction action in value.ClickActions) action.Action?.Invoke();
                            }
                        }

                        if (value.HoldTriggered)
                            foreach (InputAction action in value.HoldReleasedActions)
                                action.Action?.Invoke();

                        value.HoldTriggered = false;
                        value.TimeHeld = 0f;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e.Message);
                }
        }

        private static void RegisterAxis(string axis)
        {
            if (!AxisDictionary.ContainsKey(axis))
                AxisDictionary.Add(axis, new InputValues<float>());
        }

        private static void RegisterKey(KeyCode keyCode)
        {
            if (!KeyCodeDictionary.ContainsKey(keyCode))
                KeyCodeDictionary.Add(keyCode, new InputValues<bool>());
        }

        /// <summary>
        ///     Settings struct used by the input system for different thresholds
        /// </summary>
        public struct Settings
        {

            private float clickTimeThreshold;

            /// <summary>
            ///     How much time can pass before it stops being a click
            /// </summary>
            public float ClickTimeThreshold
            {
                get => clickTimeThreshold;
                set
                {
                    clickThresholdEntry.Value = value;
                    clickThresholdEntry.Save();
                    clickTimeThreshold = value;
                }
            }

            private float doubleClickTimeThreshold;

            /// <summary>
            ///     How much time between clicks to count as a double click
            /// </summary>
            public float DoubleClickTimeThreshold
            {
                get => doubleClickTimeThreshold;
                set
                {
                    doubleClickThresholdEntry.Value = value;
                    doubleClickThresholdEntry.Save();
                    doubleClickTimeThreshold = value;
                }
            }

            private float holdTimeThreshold;

            /// <summary>
            ///     How long to hold an input until you start the hold action
            /// </summary>
            public float HoldTimeThreshold
            {
                get => holdTimeThreshold;
                set
                {
                    holdTimeThresholdEntry.Value = value;
                    holdTimeThresholdEntry.Save();
                    holdTimeThreshold = value;
                }
            }

            /// <summary>
            ///     How much the axis needs to be pressed until it triggers axis actions
            /// </summary>
            private float triggerThreshold;

            /// <summary>
            ///     How much the axis needs to be pressed until it triggers axis actions
            /// </summary>
            public float TriggerThreshold
            {
                get => triggerThreshold;
                set
                {
                    triggerThresholdEntry.Value = value;
                    triggerThresholdEntry.Save();
                    triggerThreshold = value;
                }
            }

        }

        private class InputAction
        {

            public Action Action { get; set; }

            public string Id { get; set; }

        }

        private class InputValues<T>
        {

            public readonly List<InputAction> ClickActions = new List<InputAction>();

            public readonly List<InputAction> DoubleClickActions = new List<InputAction>();

            public readonly List<InputAction> HoldReleasedActions = new List<InputAction>();

            public readonly List<InputAction> HoldRepeatActions = new List<InputAction>();

            public readonly List<InputAction> HoldStartActions = new List<InputAction>();

            public T Current { get; set; }

            public bool HoldTriggered { get; set; }

            public float LastTimeClicked { get; set; }

            public T Previous { get; set; }

            public float TimeHeld { get; set; }

        }

    }

}