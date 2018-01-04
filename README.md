# MoobSharp
C# Library for working with Dell iDracs

To use this library, first create a Rac with your server's info.  Then detect the iDrac type.  You can then authenticate and call either OpenJNLP to run the Java console or SetPxe to set the next boot.

Always make sure to call Logout when quitting your app so that the sessions don't get full on the iDrac.

# Create a Rac
```C#
var myRac = new Rac
{
  Hostname = "myserver.example.net",
  Username = "myUser",
  Password = "myPassword",
  NoSSL = true
}
```

# Detect Drac Type and get iLom
```C#
var iLom = myRac.DetectDracType();
if (iLom != null)
{
  // Found a dell iDrac.  Do stuff here.
}
```

# Authenticate
```C#
iLom.Authenticate();
```

# Open JNLP Console
```C#
iLom.JNLP();
```

# Set Next Boot to PXE
```C#
iLom.SetPxeBoot();
```

# Logout
```C#
iLom.Logout();
```
