namespace InputSystem
{

    using System.Collections.Generic;

    internal class InputValues<T>
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