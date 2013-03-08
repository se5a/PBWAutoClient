PBW2AutoPlrClient

what is it?
this is a little app that was designed to connect to pbw.spaceempires.net,
a website which hosts several turn based strategy games,
login, check if any turns for the user are availible,
allowing the user to download, unpack, the turn files
launch the game program itself (and where the game program allows, the turn itself).
then, after having played the turn, upload it to PBW.
without the anoyance of having to unpack the turn files to the 
correct directory each time, and find the plr file to upload once played.

current version works and has been tested for:
spaceempires IV
THANCS2
but should work with other games hosted by spaceempires.net if setup.

there's still things that need doing,

there are still some places where I'm not catching exceptions, these need to be weeded out. 

I'd like to check the download directory to see if a turn has already been downloaded
and the save directorys to see if a turn has been played etc.

the user login info is currently saved in user.config which is plaintext xml in user\appdata
not the best way to do this at all, for PBW it's not critical but I'd prefer to 
do it the right way and encrypt it.  

add an audable aleart when a new turn is availble.

minimise to tray would be good. 