copy /Y ..\..\zip.dll *.*
csc /target:library x.cs /debug+
csc /target:library y.cs /r:x.dll /debug+
csc /target:library z.cs /r:x.dll /debug+
csc /target:exe m.cs /r:y.dll,z.dll,x.dll /debug+