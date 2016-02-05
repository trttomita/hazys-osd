# Introduction #



---

# What do you need #
  * A svn tool to download and upload files. I personally recommend TortoiseSVN. You can download it from [here](http://tortoisesvn.net/).
  * A Google Account for permission to upload the code. If you haven't have one, you can create a account [here](https://accounts.google.com/NewAccount). Drop me a e-mail (hazyhxj@gmail.com) so I can add you to the committer group.

# Checkout files #

1. Open the Windows Explorer, navigate to an directory. This will be the directory that stores the code file. In this example I'll use "D:\HazyOSD". Right Click on the directory, click on the "SVN Checkout..." Menu, as follows

http://hazys-osd.googlecode.com/svn/wiki/image/svn1.JPG

2. Fill in repository's URL in the pop-up dialog, and click "OK"

http://hazys-osd.googlecode.com/svn/wiki/image/svn2.JPG

3. Wait till all files are checked out, click "OK".

http://hazys-osd.googlecode.com/svn/wiki/image/svn3.JPG

There will be two directories, i.e. "OSD" and "OSDConfig". The "OSD" directory contains the source files of the firmware, and the "OSDConfig" direcotry contains the source files of the config tool.


---

# Upload the new files #

It is easy to upload the new files using the SVN tool. All you need to do is:

1. Open the directory of the project.

2. Right click on the directory, click "SVN Commit..." in the menu.

3. Write down a message of what have been changed, and click "OK".

4. A dialog of user name and password may pop up if you haven't save them before. Input your Google Account and the password for Google Code. If you don't know the password, it can be found [here](http://code.google.com/hosting/settings).

5. Wait till upload is done.