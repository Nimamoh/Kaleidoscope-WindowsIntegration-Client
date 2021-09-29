# Kaleidoscope-WindowsIntegration-Client (a.k.a Kaleidoscope Companion)

Windows application for integration with [Keyboard.io](https://keyboard.io) model 01 with the OS.

*At the moment, the only feature is the on-the-fly layer switching depending on the focused window*

It uses [Kaleidoscope-WindowsIntegration](https://github.com/Nimamoh/Kaleidoscope-WindowsIntegration) to communicate with the keyboard. Don't forget to install it on your keyboard.

## How to install

Go to [releases](https://github.com/Nimamoh/Kaleidoscope-WindowsIntegration-Client/releases). Grab the latest zip and run the .exe file.

### Which `zip` to choose

 - `x86` or `x64`: In doubt, choose `x64`
 - `self-contained` or not: In doubt, choose a `self-contained` package.
 
`self-contained` package embeds the whole runtime, allowing you to run the application without installing any dependency.
Otherwise, you will need to install the [.NET5 Runtime](https://dotnet.microsoft.com/download/dotnet/5.0) before being able to run the application.

## How to start application with the system

Go to settings menu and check the option `Start with user session`.

It creates a shortcut to the `shell:startup` folder, making the Kaleidoscope Companion starting with your user session. 
It relies on finding the executable on the path it was on setting up the option. **If you moved the utility, just uncheck and re-check the option and you should be good to go.**

## How to use

The usage is very straightforward. Connect to your keyboard choosing the correct serial port. Then you have this screen:

![alt Application - layer screen](./mappings.png)


Use the bottom menu to choose an application and a keyboard layer. 
As long as the application is running, it will apply the configured layers depending on your focused application.

You can configure several layers for a same application. Although it will deactivate layers on window unfocus. I recommand defining keyboard layers specifically to be used with this application. 


# For developers - me of the future

- Open the .sln file with rider or visual studio.
- .NET Core 3 with platform extensions is used
- The app is generated as a single fat executable. see the publish script to see the commands
- Once publish script finished, go to the github page to make a manual release