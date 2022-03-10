# Configuration
This document list and describes available configuration options.

## CommandSystems

### NullCommandSystem
```
"CommandSystem":{
    "Type":"NullCommandSystem"
}
```
A Command system that does nothing.

### ConsoleCommandSystem
```
"CommandSystem":{
    "Type":"ConsoleCommandSystem"
}
```
### EsuCommandStation
```
"CommandSystem":{
    "Type":"EsuCommandStation",
    "IP": "192.168.1.153",
    "Port": 15471
}
```

## Misc


```
"ResetEnginesWithoutState": true
```

By default, when a engine is first used after starting TauStellwerk, TauStellwerk will set speed, directions and functions of the engine to ensure the state shown to a user matches the state of the decoder. If this behaviour is unwanted, set this option to `false`.
