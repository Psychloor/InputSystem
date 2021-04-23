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

        internal static MelonPreferences_Entry<float> ClickThresholdEntry;

        internal static MelonPreferences_Entry<float> DoubleClickThresholdEntry;

        internal static MelonPreferences_Entry<float> HoldTimeThresholdEntry;

        internal static MelonPreferences_Entry<float> TriggerThresholdEntry;

        internal static MelonPreferences_Entry<bool> FixedUpdateEntry;

        private static bool useFixedUpdate;

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

        [Obsolete("This utility method has been removed, since it's more of an manual method which could just be copy pasted from V1.1.0", true)]
        public static bool HasDoubleClicked(KeyCode keyCode, ref float lastTimeClicked, float threshold)
        {
            return false;
        }

        public override void OnApplicationStart()
        {
            InputSettings.ClickTimeThreshold = 0.3f;
            InputSettings.DoubleClickTimeThreshold = 0.5f;
            InputSettings.HoldTimeThreshold = 1f;
            InputSettings.TriggerThreshold = 0.4f;

            inputSettingsCategory = MelonPreferences.CreateCategory(BuildInfo.Name, "Input System");
            ClickThresholdEntry =
                inputSettingsCategory.CreateEntry("ClickThreshold", InputSettings.ClickTimeThreshold, "Click Time-Threshold") as MelonPreferences_Entry<float>;

            DoubleClickThresholdEntry = inputSettingsCategory.CreateEntry(
                                            "DoubleClickThreshold",
                                            InputSettings.DoubleClickTimeThreshold,
                                            "Double-Click Time-Threshold") as MelonPreferences_Entry<float>;

            HoldTimeThresholdEntry =
                inputSettingsCategory.CreateEntry("HoldThreshold", InputSettings.HoldTimeThreshold, "Hold Time-Threshold") as MelonPreferences_Entry<float>;

            TriggerThresholdEntry =
                inputSettingsCategory.CreateEntry("TriggerThreshold", InputSettings.TriggerThreshold, "Trigger Threshold") as MelonPreferences_Entry<float>;

            FixedUpdateEntry = inputSettingsCategory.CreateEntry("FixedUpdate", false, "Use Fixed Update") as MelonPreferences_Entry<bool>;

            ApplySettings();
        }

        /// <summary>
        ///     Resets the input settings back to default
        /// </summary>
        public static void ResetSettings()
        {
            ClickThresholdEntry.ResetToDefault();
            ClickThresholdEntry.Save();

            DoubleClickThresholdEntry.ResetToDefault();
            DoubleClickThresholdEntry.Save();

            HoldTimeThresholdEntry.ResetToDefault();
            HoldTimeThresholdEntry.Save();

            TriggerThresholdEntry.ResetToDefault();
            TriggerThresholdEntry.Save();
        }

        private static void ApplySettings()
        {
            InputSettings.ClickTimeThreshold = ClickThresholdEntry.Value;
            InputSettings.DoubleClickTimeThreshold = DoubleClickThresholdEntry.Value;
            InputSettings.HoldTimeThreshold = HoldTimeThresholdEntry.Value;
            InputSettings.TriggerThreshold = TriggerThresholdEntry.Value;
            useFixedUpdate = FixedUpdateEntry.Value;
        }

        public override void OnFixedUpdate()
        {
            if (!useFixedUpdate) return;
            UpdateInputs();
        }

        public override void OnUpdate()
        {
            if (useFixedUpdate) return;
            UpdateInputs();
        }

        public override void OnPreferencesSaved()
        {
            ApplySettings();
        }

        private static void UpdateInputs()
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

    }

}