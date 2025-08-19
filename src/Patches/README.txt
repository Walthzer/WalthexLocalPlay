This folder should contain all of the Patches you want to apply to the Mage Arena game

What is a patch? 
A Patch allows you do directly add logic into the game. This is done using `Harmony`
You can run code before (PreFix) or after (PostFix) any existing method in the game.
Read more about patching with harmony here:
https://harmony.pardeike.net/articles/patching.html

You can use dnSpy to find every method in the game, either on its own or using UnityExplorer (Recommended)

https://github.com/dnSpyEx/dnSpy
https://github.com/yukieiji/UnityExplorer

To ease the project structure you should put Patches of equal types into named subfolders:

Controllers go into a Controllers subfolder
Managers go into a Managers subfolder

Read the name of the class you are patching to see what the type is.
This ModTemplate contains an example patch the `MageBookController`, so it is contained in the Controllers subfolder.

For more example patches see the BlackMagicAPI github:
https://github.com/D1GQ/BlackMagicAPI/tree/master/Patches