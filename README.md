# Event Manager
Moniroting sensor events

Open issues:
  1. More than one event is created sometimes for a single sensor.
  2. Logging events is not created correctly.
  3. Order of incoming events is not preserved.
  4. For some reason, the command to remove an event is not captured, even though the command is binded with relative source to the Window.
  5. The most recent events should be displayed from top to bottom. But this issue should be resolved only after issues 1-3 are resolved.
  
  Probably the reason for the first 3 issues above is lack of synchrony between the threads that publish the sensor status change event.
  
