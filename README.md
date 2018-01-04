# MoobSharp
C# Library for working with Dell iDracs

To use this library, first get a Lom from Rac.  You can then authenticate and call either OpenJNLP to run the Java console or SetPxe to set the next boot.

Always make sure to call Logout when quitting your app so that the sessions don't get full on the iDrac.

# Detect Drac Type and get iLom
```C#
var iLom = Rac.GetDracForHost(Username, Password, Hostname, NoSSL);
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
