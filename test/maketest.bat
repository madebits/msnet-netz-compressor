copy /Y ..\zip.dll *.*
csc /target:library testlib.cs
csc /target:winexe testapp.cs /r:testlib.dll