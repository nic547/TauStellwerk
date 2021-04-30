# PiStellwerk
A collection of Software cantered around "Teppichbahning" ("Carpet Railroading"), temporary model railways with changing layouts, users, devices and trains.
The end-goal is a simple user interface, fast setup, solid handling of multiple users and supporting most platforms.
No support for automatization or feedback modules is planned at this point in time.

## Current State
The software is still in a very experimental state. If you're interested in using this project, you should understand that things will break.
Currently only the ESU Command Station (ECoS) is supported and the only work so far happened on the Webserver and a Webapp.

## Important Notes
- This is just a hobbyist's project. I guarantee absolutely nothing.
- Restrict network access to the server. Do not use a public Wi-Fi and do not make the server accessible from the internet.

## Overview
Currently only the "ESU Command Station" (ECoS) is supported. Support for other Command Stations is blocked by a lack of Hardware.

### Webserver
The Webserver runs on .NET, currently Version 5.0. It should be able to run on Windows, GNU/Linux and MacOS and supports both x86 and ARM processors.
The PiStellwerk server uses a SQLite-database to keep it's data.

There's no user authentication, as managing user accounts does not align with the goals of this project. Instead Users can freely chose a username and access control should happen on a network level.

For the optional image processing the server relies on the ImageMagick suite. It is not included and should be installed separately if desired. Both IMv6 and IMv7 are supported, as many Linux distributions still only provide the older version in their repos.

### Webapp
The Webapp is based on HTML/CSS and Typescript. Currently no Frontend-Framework is used, to keep the Webapp as light as possible.
Generally the Webapp supports and is tested with the latest versions of Firefox, Chrome and Edge. Safari and MacOS/iOS aren't tested, but supported as far as possible.

Due to technical limitations and a lack of experience and motivation the web version of PiStellwerk will inevitable fall behind the planned mobile and desktop applications. It's more of a fallback for platforms that cannot use the other applications.

## Licences
- Unless noted otherwise, this project is licensed under the GNU GPLv3, see [here](/LICENSE) for more details.

## Next steps:
- Support for the OpenDCC command station(s)
- Desktop Application using Avalonia
- Mobile App using MAUI (depends on the Release of .NET 6)
- Switches