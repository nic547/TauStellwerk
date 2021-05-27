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