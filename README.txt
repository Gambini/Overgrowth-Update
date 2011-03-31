This is the Overgrowth Updater v1.4. It will update your Overgrowth game to the latest version, regardless of 
what version you are currently on.

Quick how to use:
Press the top "Browse..." button and navigate to where you installed Overgrowth, and select your Overgrowth.exe.
Press the bottom "Browse..." button and point to a folder where you would like to download the update files.
Click the "Download and Update" option at the bottom left
Click the "Download and Update" button at the bottom right
Wait for things to download
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
IMPORTANT: If you installed a version of this updater (like 1.1) and I released a new version that says
   "Major bug fix" (like 1.3), then you might want to do the following:
	1.)Clean/delete the updates folder. You can do this from within the updater, or just delete the folder(s) yourself.
	2.)Navigate to [Overgrowth install location]\Data\version.xml and open it up. Modify the 'shortname' property to 
	  a version from before you used the updater. The program only looks at shortname, so don't need to modify others.
	3.)Run the updater as normal.

Icon made by ZramuliZ from the Overgrowth forums.


Changelog:
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