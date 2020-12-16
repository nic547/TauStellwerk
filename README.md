# PiStellwerk
An WIP open-source DCC Command Station for the Raspberry Pi and similar single board computers. 

## Repository Contents
- /Hardware - KiCAD design files and STM32CubeMX project file
- /Software - Server and Webapp

## Current State
Right now this is all very experimental. I will almost certainly make major changes as far as hardware is concerned and abbandon any older hardware revisions until I state the opposite. Same goes for Software.

## Important Notes
- This is just a hobbyist's project. I guarantee absolutely nothing.
- Restrict network access to the server. Do not use a public Wi-Fi and do not make the server accessible from the internet. The system isn't ready for the scale and sophistication of attacks any public system has to withstand.

## Planned Features
- 2.5A nominal current per booster
- compatible with the RaspberryPi 4 and similar SBCs (40-Pin Header and Mounting Holes)
- optional RS-485 based booster bus
- optional isolated USB Fullspeed compatible interface

## Overview
### Hardware
The Hardware is based around a 4-layer PCB, a STM32F103CxT MCU and a DRV8874 H-Bridge. 

### Webserver
The Webserver is a basic C# ASP.Core WebAPI using Kestrel, currently using .NET 5. Thanks to .NET Core, the Application should run on both Windows and Linux, on both ARM and x86 processors.

### Webapp
The Webapp is done with vanilla HTML/CSS and Typescript. As it's a website, it should run on almost any device with a recent browser.

## Licences
- /Software is licensed under the GNU GPLv3, see [here](/Software/LICENSE) for more details.