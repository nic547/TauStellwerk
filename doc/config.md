# Configuration
This document list and describes available configuration options.

## CommandSystems

Select the CommandSystem that TauStellwerk should interface with and set any command system specific settings.

### NullCommandSystem
```
"CommandSystem":{
    "Type":"Null"
}
```
A Command system that does nothing.

### ConsoleCommandSystem
```
"CommandSystem":{
    "Type":"Console"
}
```
### ECoS
```
"CommandSystem":{
    "Type":"ECoS",
    "IP": "192.168.1.153",
    "Port": 15471
}
```
(ECoS is not case-sensitive)


### DCC++ EX via serial connection

```
"CommandSystem": {
"Type": "DccExSerial",
"SerialPort": "COM3"
},
```

`SerialPort` identifies the USB-to-Serial adapter via which TauStellwerk can communicate 
with DCC++ Ex. If you installed DCC++ EX on your board yourself, you already had to select it to upload
the application.
On Windows such devices are usually called `COMX` (where X is any number), 
on Linux something like `/dev/ttyACMX` or `/dev/ttyUSBX` (X being any number again).

## Misc


```
"ResetEnginesWithoutState": true
```

By default, when a engine is first used after starting TauStellwerk, TauStellwerk will set speed, directions and functions of the engine to ensure the state shown to a user matches the state of the decoder. If this behaviour is unwanted, set this option to `false`.

```
"StopOnLastUserDisconnect" : true
```

By default, when a user disconnects and there are no users left, the CommandStation will be stopped. Set this option to false if this behaviour is undesired.