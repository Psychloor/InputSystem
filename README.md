# InputSystem (Utility for modders)
Universal Input system for modders where you can register different type of actions to be done for specific input types (click, double click, hold and hold released) action to register into their own inputhandling. don't have to check if it's been activated as this system will tell you when it happened.

It won't trigger click if you hold a key/axis and won't trigger hold if clicked, so it can feel slightly delayed.
this behaviour is purposely made to mimic game behaviour that can't be combined



You have to give it an specific id to associate with incase it loads/unloads itself which can either be static or runtime generated per instance using Guid.Newguid and using it per action or overall actions in the same class/mod

# Example
includes a quick example/sample to test it out
