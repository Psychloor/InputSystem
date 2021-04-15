namespace InputSystem
{

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
                if (InputSystem.ClickThresholdEntry != null)
                {
                    InputSystem.ClickThresholdEntry.Value = value;
                    InputSystem.ClickThresholdEntry.Save();
                }

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
                if (InputSystem.DoubleClickThresholdEntry != null)
                {
                    InputSystem.DoubleClickThresholdEntry.Value = value;
                    InputSystem.DoubleClickThresholdEntry.Save();
                }

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
                if (InputSystem.HoldTimeThresholdEntry != null)
                {
                    InputSystem.HoldTimeThresholdEntry.Value = value;
                    InputSystem.HoldTimeThresholdEntry.Save();
                }

                holdTimeThreshold = value;
            }
        }

        private float triggerThreshold;

        /// <summary>
        ///     How much the axis needs to be pressed until it triggers axis actions
        /// </summary>
        public float TriggerThreshold
        {
            get => triggerThreshold;
            set
            {
                if (InputSystem.TriggerThresholdEntry != null)
                {
                    InputSystem.TriggerThresholdEntry.Value = value;
                    InputSystem.TriggerThresholdEntry.Save();
                }

                triggerThreshold = value;
            }
        }

    }

}