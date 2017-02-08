```NOTE: Currently having some problems with dependancy management.```

# Project Butler `0.1`
Project Butler is a simple tool that enables applications to hook into a simple command based GUI in order to make
users more productive. The user can press a system wide hotkey to bring up a GUI that can detect commands and serve them to 
available add-ons or I we call them 'Modules' (yeah, very original name).

## Why would someone use this?
Ease of execution and infinite productivity. Through my modular add-on system, sharing new modules is a matter of copying 
one or more files to a folder. In the future, this will be controlled by a package manager though.

## Module Design
Modules will be created to solve a specific problem and have a particular focus. Thus, the size of the modules will generally 
be small and will have simple functionality like say playing music from a user created list instead of being a full media player
that supports 20 codecs and H.265 video.

# Developer Guide

## How the API works.
Each module has a main class or the 'Hook class' that is derived from the Module class and contains the attribute 'ApplicationHook'. 
That class provides a `Name`, `Semantic Version`, `Author`, `Website` and `Registered Commands`.
Registered commands are basically a hash table/dictionary of [String -> Regex]. An example 

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
Now, whenever the user types in 'all songs' or 'all song', an OnCommandRecieved function will be invoked in the module containting
the name of the command and the users exact input, in this case 'SongList'. The application can respond to the command.

~~~~cs
public override void OnCommandRecieved(string commandName, string UserInput){
  if(commandName == "SongList"){
    DisplaySongList();  
  }
}
~~~~


## How restricted is the API?
The API is there to communicate when your specified commands are received and that's it. Everything else is the responsibility of the 
module. This opens up huge possibilites of what functions a module can have.
In fact, pre-existing applications can add one 'Hook class' and voila, Butler can know communicate with that application. 

## What exactly are 'Modules'?
Modules are WPF (Windows Presentation Foundation) class libraries (.dll) programmed in C# 6.0./.Net 4.6. Any WPF application can 
be converted to a class library through a few simple modifications.

## My Vision
My vision for Project Butler is a free market of amazing and helpful modules driven by a community of passionate developers 
making modules for the sole purpose of helping others. Money has never been my goal while making new things ever since I saw
my little brother playing the iOS game I made back when I was 14. This project was inspired by the addon system of text editors
such as Sublime Text, Atom, VSCode. 
