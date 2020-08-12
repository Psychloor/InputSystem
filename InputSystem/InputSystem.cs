using MelonLoader;

using BuildInfo = InputSystem.BuildInfo;

[assembly: MelonInfo(typeof(InputSystem.InputSystem), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author, BuildInfo.DownloadLink)]
[assembly: MelonGame]

namespace InputSystem
{

    using System;
    using System.Collections.Generic;

    using MelonLoader;

    using UnityEngine;

    public sealed class InputSystem : MelonMod
    {

        public struct Settings
        {
            
            public float ClickTimeThreshold;

            public float DoubleClickTimeThreshold;

            public float HoldTimeThreshold;

            public float TriggerThreshold;

        }
        
        private const string SettingsCategory = "InputSystem";

        public static Settings InputSettings;

        private static readonly Dictionary<string, InputValues<float>> AxisDictionary = new Dictionary<string, InputValues<float>>();

        private static readonly Dictionary<KeyCode, InputValues<bool>> KeyCodeDictionary = new Dictionary<KeyCode, InputValues<bool>>();

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

        public static void RegisterHoldAction(string id, Action action, string axis)
        {
            RegisterAxis(axis);
            AxisDictionary[axis].HoldActions.Add(new InputAction { Id = id, Action = action });
        }

        public static void RegisterHoldAction(string id, Action action, KeyCode keyCode)
        {
            RegisterKey(keyCode);
            KeyCodeDictionary[keyCode].HoldActions.Add(new InputAction { Id = id, Action = action });
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
            RemoveHoldAction(id);
            RemoveHoldReleasedAction(id);
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

        public static void RemoveHoldAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.HoldActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.HoldActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void RemoveHoldReleasedAction(string id)
        {
            foreach (InputValues<float> value in AxisDictionary.Values)
                value.HoldReleasedActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (InputValues<bool> value in KeyCodeDictionary.Values)
                value.HoldReleasedActions.RemoveAll(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public override void OnApplicationStart()
        {
            InputSettings.ClickTimeThreshold = 0.3f;
            InputSettings.DoubleClickTimeThreshold = 0.5f;
            InputSettings.HoldTimeThreshold = 1f;
            InputSettings.TriggerThreshold = 0.4f;
            
            MelonPrefs.RegisterCategory(SettingsCategory, "Input System");
            MelonPrefs.RegisterFloat(SettingsCategory, "ClickThreshold", InputSettings.ClickTimeThreshold, "Click Time-Threshold");
            MelonPrefs.RegisterFloat(SettingsCategory, "DoubleClickThreshold", InputSettings.DoubleClickTimeThreshold, "Double-Click Time-Threshold");
            MelonPrefs.RegisterFloat(SettingsCategory, "HoldThreshold", InputSettings.HoldTimeThreshold, "Hold Time-Threshold");
            MelonPrefs.RegisterFloat(SettingsCategory, "TriggerThreshold", InputSettings.TriggerThreshold, "Trigger Threshold");

            LoadSettings();
        }

        /// <summary>
        /// Save settings for manually changing them
        /// Will also call MelonPrefs.SaveConfig
        /// </summary>
        public static void SaveSettings()
        {
            MelonPrefs.SetFloat(SettingsCategory, "ClickThreshold", InputSettings.ClickTimeThreshold);
            MelonPrefs.SetFloat(SettingsCategory, "DoubleClickThreshold", InputSettings.DoubleClickTimeThreshold);
            MelonPrefs.SetFloat(SettingsCategory, "HoldThreshold", InputSettings.HoldTimeThreshold);
            MelonPrefs.SetFloat(SettingsCategory, "TriggerThreshold", InputSettings.TriggerThreshold);
            MelonPrefs.SaveConfig();
        }

        public override void OnModSettingsApplied()
        {
            LoadSettings();
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
                                value.HoldActions.ForEach(action => action.Action?.Invoke());
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
                                value.DoubleClickActions.ForEach(action => action.Action?.Invoke());
                            }
                            else
                            {
                                value.LastTimeClicked = Time.time;
                                value.ClickActions.ForEach(action => action.Action?.Invoke());
                            }
                        }

                        if (value.HoldTriggered) value.HoldReleasedActions.ForEach(action => action.Action?.Invoke());

                        value.HoldTriggered = false;
                        value.TimeHeld = 0f;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.LogError(e.Message);
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
                                value.HoldActions.ForEach(action => action.Action?.Invoke());
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
                                value.DoubleClickActions.ForEach(action => action.Action?.Invoke());
                            }
                            else
                            {
                                value.LastTimeClicked = Time.time;
                                value.ClickActions.ForEach(action => action.Action?.Invoke());
                            }
                        }

                        if (value.HoldTriggered) value.HoldReleasedActions.ForEach(action => action.Action?.Invoke());

                        value.HoldTriggered = false;
                        value.TimeHeld = 0f;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.LogError(e.Message);
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

        private static void LoadSettings()
        {
            InputSettings.ClickTimeThreshold = MelonPrefs.GetFloat(SettingsCategory, "ClickThreshold");
            InputSettings.DoubleClickTimeThreshold = MelonPrefs.GetFloat(SettingsCategory, "DoubleClickThreshold");
            InputSettings.HoldTimeThreshold = MelonPrefs.GetFloat(SettingsCategory, "HoldThreshold");
            InputSettings.TriggerThreshold = MelonPrefs.GetFloat(SettingsCategory, "TriggerThreshold");
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

            public readonly List<InputAction> HoldActions = new List<InputAction>();

            public readonly List<InputAction> HoldReleasedActions = new List<InputAction>();

            public T Current { get; set; }

            public bool HoldTriggered { get; set; }

            public float LastTimeClicked { get; set; }

            public T Previous { get; set; }

            public float TimeHeld { get; set; }

        }

    }

}