MofoMojo's Sleep and MotD (Message of the Day) Dedicated Server Mod 1.1

This mod solves two problems I wanted to achieve for my dedicated server. 

A Message of the Day which displays for a few moments after a user has logged in
A Sleep notification to let players know how many vikings are currently asleep. 

This is 100% dedicated server side mod. No client mod required.

"[4:06 PM]
gorth123:
	sleeping participation has gone up 92% since implementing server-side mod! love it!"


Customize your sleep message
3/4 Vikings are attempting to get their beauty rest

Customize your MOTD
If you want to use single or double quotes, they need to be escaped. 
\' and \" should be used. For instance

Welcome to MofoMojo\'s Dedicated Server

:: REQUIREMENTS ::
BepInEx - 
Dedicated Server

:: INSTALLATION ::
Place the MofoMojo.MMServerMessages.dll in your \Valheim dedicated server\BepInEx\plugins folder
Start your dedicated server once, to generated a .CFG file
Modify the \BepInEx\Config\MofoMojo.MMServerMessages.cfg as you see fit.
Restart your dedicated server

:: UNINSTALLATION ::
Remove the .DLL and the .CFG file from the \Plugins and \Config folders respectively.

:: UPGRADING ::
No upgrades yet

:: FEATURES ::
Can specify a custom sleep message
Can specify a custom Message of the Day
Settings refresh every 5 minutes (can configure)

:: TODO ::
Consider allowing admins to configure MOTD and SLEEP messages remotely
Consider a way to perform Sleep check without being in the Game Update() loop (done)
Consider a way to do the MOTD check without being in the Game Update() loop )(done)
Consider a way to show message only once a day per login rather than once after each login
Entertain removing dedicated requirement

::  KNOWN ISSUES ::
Not aware of any at this time...

:: VERSIONS :: 
1.2 Added force of showing new message of the day to players when it changes
1.1 Added config refreshing
1.0 Initial Release

::  THANKS ::
Special thanks to my server mates for putting up with my contant modding and testing
T.B.
B.H.
I.R.
D.G.

::  CREDITS ::
♦ https://www.youtube.com/watch?v=p_gsFASlvRw
♦ https://harmony.pardeike.net/ - Harmony Documentation
♦ https://github.com/Valheim-Modding/Wiki/wiki - Valheim modding

:: SAMPLE CONFIG ::

My mods' source, and my personal mods that aren't published or are WIP are available here:
https://mofomojo.visualstudio.com/MofoMojoValheimMods/_git/MofoMojoValheimMods