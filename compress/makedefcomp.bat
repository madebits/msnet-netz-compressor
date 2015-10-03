..\starter\resources\makeres defcomp head tmpl_head.txt body tmpl_body.txt
csc /target:library defcomp.cs /r:..\bin\release\netz.exe,zip.dll /res:defcomp.resources