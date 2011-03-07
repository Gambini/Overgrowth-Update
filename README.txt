This is the Overgrowth Updater. It will update your Overgrowth game to the latest version, regardless of 
what version you are currently on.

Quick how to use:
Press the top "Browse..." button and navigate to where you installed Overgrowth, and select your Overgrowth.exe.
Press the bottom "Browse..." button and point to a folder where you would like to download the update files.
Click the "Download and Update" option at the bottom left
Click the "Download and Update" button at the bottom right
Wait for things to download
If the program freezes right after the download, then don't freak out. It is just doing work
Assuming no errors popped up, your overgrowth game is now updated.



There are mouseover tooltips for just about every option, so if you are confused about something, then 
hover over it with your mouse, and it should tell you what you need to know.

A few things to note:
-If Overgrowth leaves alpha, or the alpha version goes above a999, then you will need an updated version of this tool.
-When it is applying the updates, the program will freeze. This is because I do not trust my skills with
   multi-threading, so I run the update function on the UI thread. It includes unpacking the .zip, copying files,
   and (if you have the backup selected,) compressing the backup. That is a lot of things to do, so give it
   some time if you are more than 4 versions behind.
-It isn't hard drive space-conservative. I suggest clicking "File->Clean update folder" after you have verified
   that the update procedure has worked. Make sure you do it after you verify it has worked, because it will
   also delete the backup files (if you have the backup option selected), which is your best way to revert to
   a previous version if the update procedure messes up. If you have plenty of hard drive space, then no worries.


Built with the .Net framework 4, which you can get at 
   http://www.microsoft.com/downloads/en/details.aspx?FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7
If there are some people that would like it in a lower (like 3), then say something