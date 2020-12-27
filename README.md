# EventManager
Moniroting sensor events

Open issues:
  1. More than one event is created sometimes for a single sensor.
  2. Logging events is not created correctly.
  3. Order of incoming events is not preserved.
  4. For some reason, the command to remove an event is not captured, even though the command is binded with relative source to the Window.
  
  Probably the reason for the first 3 issues above is lack of synchrony between the threads that publish the sensor status change event.
  
