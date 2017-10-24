# Project Butler

[![forthebadge](http://forthebadge.com/images/badges/built-with-science.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg)](http://forthebadge.com)
[![forthebadge](http://forthebadge.com/images/badges/powered-by-electricity.svg)](http://forthebadge.com)

Project Butler is a customizable and modular combination of a GUI and a command-line (GUI-line as I like to call it). It combines the efficiency of a programmers keyboard with the design and accessibility of a GUI. Using the Module API, you can create custom libraries that can be dynamically loaded by Project Butler at runtime. Just set a few Regex commands and you're good to go!

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
