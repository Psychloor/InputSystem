# InputSystem (Utility for modders)
Universal Input system for modders where you can register different type of actions to be done for specific input actions to register into their own inputhandling. don't have to check if it's been activated as this system will tell you when it happened.

It won't trigger click if you hold a key/axis and won't trigger hold if clicked, so it can feel slightly delayed.
this behaviour is purposely made to mimic game behaviour that can't be combined.

For Action ID's you can either use a const string specific to whoole mod/subsystem and if you want it to be dynamic you could create a GUID and store it within the class itself as a field and use that.

# Actions

Click - Whenever the input has been quickly pressed. Won't trigger if going to Hold.

Double Click - Whenever the input has been quickly pressed twice within the time threshold.

Hold Started - When the input has been hold down long enough to start counting as being held down.

Hold Repeat - When the input is still being held down. won't trigger at Hold Start/End.

Hold Released - When the input has stopped being held down.
