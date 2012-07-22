This is the Overgrowth Updater v1.7. It will update your Overgrowth game to the latest version, regardless of 
what version you are currently on. (Assuming it is higher than a112; that's when the updates started)

Quick how to use:
Press the top "Browse..." button and navigate to where you installed Overgrowth, and select your Overgrowth.exe.
Press the bottom "Browse..." button and point to a folder where you would like to download the update files.
Click the "Download and Update" option at the bottom left
Click the "Download and Update" button at the bottom right
Wait for things to download and install
Assuming no errors popped up, your Overgrowth game is now updated.



There are mouseover tooltips for just about every option, so if you are confused about something, then 
hover over it with your mouse, and it should tell you what you need to know.

A few things to note:
-If Overgrowth leaves alpha, or the alpha version goes above a999, then you will need an updated version of this tool.
-It isn't hard drive space-conservative. I suggest clicking "File->Clean update folder" after you have verified
   that the update procedure has worked. Make sure you do it after you verify it has worked, because it will
   also delete the backup files (if you have the backup option selected), which is your best way to revert to
   a previous version if the update procedure messes up. If you have plenty of hard drive space, then no worries.


Built with the .Net framework 4, which you can get at 
   http://www.microsoft.com/downloads/en/details.aspx?FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7
If there are some people that would like it in a lower (like 3), then say something


Updating this updater: 
-Drag the OvergrowthUpdater.exe from the .zip file into the folder to replace the old one.
-Add any new files that were not there previously
IMPORTANT: If you installed a version of this updater (like 1.3) and I released a new version that says
   "Major bug fix" (like 1.5), then you might want to do the following:
	1.)Go to File->Revert game version...	(only in 1.5 or higher)
	2.)In the first dropdown box, chose a version from before you started using this tool. Leave the
	second box blank. Hit "Ok". (it was a118 for me)
	3.)Run the updater as normal.
	This will update your game from before you started using the tool to the latest version.
	If you still have some of the .zip update files, then you can minimize the amount you need to 
	re-download.

Icon made by ZramuliZ from the Overgrowth forums.


Changelog:
v1.7:=========================================
Major bug fix
-Alpha 185 is named as overgrowth-a185.zip instead of just a185.zip on the Overgrowth servers, so I made this able
to detect that
-Started on localization
v1.6:=========================================
-It can now download more than two files at a time
-Put more information in the status strip (thing at the very bottom) that tells you which update 
is being worked on.
-Doesn't re-download any files that you already downloaded.
v1.5:=========================================
Major bug fix
-Again, a silly error on my part that stopped some of the update files from being copied over, which is now fixed.
Features:
-File->Revert game version...
   Added a feature to revert to a previous version (only modifies version.xml) and then update to either the latest
   version, or a version of your choosing.
-Behind-the-scenes work on removing files to make the reverting process painless. It initially hurt.
-Added a timestamp to the log file, rather than just a day month and year.
-Fixed a bug where the logging option wasn't being restored from the config.txt each time it was run.
-Moved File->Clean Update Folder to its own thread, because it started to freeze the UI thread
v1.4:=========================================
-Added a feature to log the changes that the program makes. If enabled in the 'Options' dropdown menu, then
   a file will be created in the directory of the updater named log.txt. You can compare what the log says
   to what you see in the updates folder/install folder to verify that everything that needed to be copied/made
   actually got copied/made.
v1.3:=========================================
Major bug fix
-Again, fixed logic for adding files from the update that didn't previously exist. I am 99% that all of the
   code for applying the file changes to the Overgrowth game are now correct. I would like it if someone would
   confirm, but I followed exactly what was going on, and it was working properly.
-Added code to automatically add the Overgrowth.exe path upon starting up for the first time if the user has
    installed Overgrowth to the default directory (Program Files x86).
    TODO: Support for drag and drop the Overgrowth.exe into the form and it will automatigically fill in what you need.
v1.2:=========================================
Major bug fix
-Fixed logic error where I didn't take in to account of the fact that there could be new directories in the update.
-Added a background worker for the updating process. No more UI freezes!
v1.1:=========================================
-Added the icon image.