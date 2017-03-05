# Project Butler `1.0.0`

[![forthebadge](http://forthebadge.com/images/badges/gluten-free.svg)](http://forthebadge.com)

Project Butler allows you to use commands to do stuff basically. Through it's Modular approach, you can create your own Modules that listen for certain registered commands and do stuff based on the recieved commands. You can give commands through the desktop UI or through the TCP connection.

## What are Modules?
Modules are basically WPF (Windows Presentation Foundation) class libraries (.dll) [C# 6.0./.Net 4.6] that will be loaded by Project Butler at runtime. They contain one metaphorical *Hook* class which is basically a class derived from the ***ModuleAPI.Module*** class with the ***Application Hook*** attribute. These hook classes act as a bridge between your application and Project Butler. 

Said *Hook* class has a `string Name`, `string SemVer`, `string Author`, `Uri Website`, 'string Prefix', and  
`Dictionary<string,Regex> Registered Commands`. An example of how commands are declared:- 

~~~~cs  
public override Dictionary<string, Regex> RegisteredCommands {
  get {
    return new Dictionary<string, Regex>() {
      ["Settings"] = new Regex(@"musicplayer settings", RegexOptions.IgnoreCase),
      ["SongList"] = new Regex(@"all songs?", RegexOptions.IgnoreCase)
    };
  }
}
~~~~

In order to invoke the command, you have to input the prefix of the Module containing the command followed by an input that matches the regular expression of the command. For example:- 

``music all songs``

~~~~cs
public override void OnCommandRecieved(Command cmd){
  if(cmd.LocalCommand == "SongList"){
    DisplaySongList();  
  }
}
~~~~

When the regular expression provided by a Module are matched, the ``OnCommandRecieved`` function is called. This function provides an object of the ``Command`` class which allows you to look at information such as the user input, command name, local IP of the device who gave the command, and also provides a method to write back to the device who called the command.

## How restricted is the API?
The API is there to communicate when your specified commands are received and that's it. Everything else is the responsibility of the 
module. This opens up huge possibilites of how a module can function.

## My Vision
My vision for Project Butler is a free market of amazing and helpful modules drien by a community of passionate developers 
making modules for the sole purpose of making something cool, useful, and handy.
