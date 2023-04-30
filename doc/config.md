# Configuration

This document list and describes available configuration options.

## Command Station

Select the command station that TauStellwerk should interface with and set any system specific settings.

### NullCommandStation

```json
"CommandSystem":{
    "Type":"Null"
}
```

A Command system that does nothing.

### ConsoleCommandStation

```json
"CommandSystem":{
    "Type":"Console"
}
```

Prints everything to console. Usefull for development and testing, not usefull otherwise.

### ECoS

```json
"CommandSystem":{
    "Type":"ECoS",
    "IP": "192.168.1.153",
    "Port": 15471
}
```

(ECoS is not case-sensitive)

### DCC-EX Command Station via serial connection

```json
"CommandSystem": {
"Type": "DccExSerial",
"SerialPort": "COM3",
"UseJoinMode": false
},
```

`SerialPort` identifies the USB-to-Serial adapter via which TauStellwerk can communicate 
with DCC Ex. If you installed DCC EX on your board yourself, you already had to select it to upload
the application.
On Windows such devices are usually called `COMX` (where X is any number), on Linux something like `/dev/ttyACMX` or `/dev/ttyUSBX` (X being any number again).

`UseJoinMode`: DCC EX has a JOIN function, that ensures that a command sent to the "normal" track is also sent to the programming track. This means that a electrically isolated siding can be used as a programming track, where a engine can simply drive on and off the programming track.

## Misc

```json
"ResetEnginesWithoutState": true
```

By default, when a engine is first used after starting TauStellwerk, TauStellwerk will set speed, directions and functions of the engine to ensure the state shown to a user matches the state of the decoder. If this behaviour is unwanted, set this option to `false`.

```json
"StopOnLastUserDisconnect" : true
```

By default, when a user disconnects and there are no users left, the CommandStation will be stopped. Set this option to false if this behaviour is undesired.
