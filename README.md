# Project Butler

[![forthebadge](http://forthebadge.com/images/badges/built-with-science.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/powered-by-electricity.svg)](http://forthebadge.com)

Project Butler is a customizable and modular combination of a GUI and a command-line (GUI-line as I like to call it). It combines the efficiency of a programmers keyboard with the design and accessibility of a GUI. Using the Module API, you can create custom libraries that can be dynamically loaded by Project Butler at runtime. Just set a few Regex commands and you're good to go!

![Project Butler Main Window](https://i.imgur.com/HvnoavP.png)

## How do I use it?
As Project Butler is merely a "mediator", it does not have any inherent functionality other than the ability to communicate commands between Modules and your keyboard, phone, or another computer. It's functionality comes from the various Modules written for it. 

Currently available modules:- 
1. [Music Player](https://github.com/aryan-mann/Mod-MusicPlayer)
2. [Youtube Navigator](https://github.com/aryan-mann/Mod-YoutubeViewer)
3. [Experimental / Unassorted](https://github.com/aryan-mann/Mod-Random)

To add an external Module, you need to copy the main \*.dll file and all its dependancies to a new folder under the Modules directory. If valid, Project Butler will show the name and details of that Module in the Main Window, otherwise you can check for errors using the Logs tab on the top right.

Before use, you need to activate the Module by flipping the red switch next to its name. The supported Regex commands are listed below its name along with the prefix that the Module uses.

## How do I connect to it using WiFi?
Press the **Start** button on the top right of the screen in the Main Window and wait for it to turn green. Once it's green, using a TCP client, connect to the network address of your computer at port `4144`

# DEVELOPER GUIDE

## What are Modules?
Essentially, *Modules* are WPF (Windows Presentation Foundation) class libraries (.dll) [C# 6.0./.Net 4.6] that are dynamically loaded by Project Butler at runtime. They contain atleast one main class (that has the *Application Hook* attribute) which acts as the entry point to the Module. Therefore, the hook class acts as a bridge between your application and Project Butler. 

Said *Application Hook* class has a couple of properies: `string Name`, `string SemVer`, `string Author`, `Uri Website`, `string Prefix`, and  `Dictionary<string,Regex> Registered Commands`. An example of how commands are declared:- 

~~~cs
public override Dictionary<string, Regex> RegisteredCommands {
  get {
    return new Dictionary<string, Regex>() {
      ["Settings"] = new Regex(@"settings", RegexOptions.IgnoreCase),
      ["SongList"] = new Regex(@"all songs?", RegexOptions.IgnoreCase)
    };
  }
}
~~~~

In order to invoke the command, you have to input the prefix of the Module followed by an input that matches the regular expression of any defined command. For example:- 

``music all songs``

~~~~cs
public override void OnCommandRecieved(Command cmd){
  if(cmd.LocalCommand == "SongList"){
    DisplaySongList();  
  }
}
~~~~

When the regular expression provided by a Module are matched, the ``OnCommandRecieved`` function is called. This function provides an object of the ``Command`` class which allows you to look at information such as the user input, command name, local IP of the device which gave the command, and also provides a method to reply back to the device who called the command.

## How restricted is the API?
The API is there to communicate when your specified commands are received, provide a way to give a response, and that's it. Your custom module is only limited by the language itself (so not much at all).

## How do I setup my module?
The setup is fairly straightforward and ideal for development with rapid changes.

1. Create a WPF Class Library that targets .NET 4.6
2. Include a reference to ModuleAPI
3. Change your debug mode to **External Application** and point it to the executable for Project Butler

Finally, in your post-build events, add the following code:- 

```
mkdir "{{Modules Directory of Project Butler}}\$(ProjectName)"
copy "$(TargetPath)" "{{Modules Directory of Project Butler}}\$(ProjectName)\$(TargetFileName)"
```

Replace `{{Modules Directory of Project Butler}}` with well.. the Modules directory of Project Butler.

## How do I get started with my code?
First, create a class that inherits from ModuleAPI.Module. Override Name, SemVer, Author, Website, and Prefix propreties.
Then override and implement ConfigureSettings, OnShutdown, and OnInitialized functions.

The RegisteredCommands property is a dictionary of type <string, Regex> where the key is an alias for your Regex command. Upon detection of a command, Project Butler will invoke the OnCommandRecieved function with the name (key) of the detected Regex command. Using that, you can get the matched groups of the Regular Expression and use them as parameters for your future functions. 
