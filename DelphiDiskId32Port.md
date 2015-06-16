There is a beautiful application <a href='http://www.winsim.com/diskid32/diskid32.html'>DiskId32</a> that allows to get unique hardware id for computer. But this application is written on C++, so it's difficult to use this code in Delphi.

I have ported source codes of DiskId32 on Delphi 7 - 2010. To avoid output on console (f.e. in applications with GUI) please comment definition of macros
```
{$DEFINE PRINTING_TO_CONSOLE_ALLOWED}```

This translation was made for company <a href='http://www.efaktum.dk/'>Efaktum</a>. I am really appreciated that they have allowed to me to publish these codes.

I have tested these codes on Win XP, Win 2003, Win 7 x64. Please let me know if you found any error. <b>Important note</b>: if the port doesn't work (doesn't generate unique hardware id) please also check if <a href='http://www.winsim.com/diskid32/diskid32.html'>original DiskId32</a> works.


---

<a href='http://derevyanko.blogspot.com/2009/02/hardware-id-diskid32-delphi.html'>Как получить hardware id. Исходники DiskID32 на Delphi</a>

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=DiskId32PortForDelphi.rar&can=2&q='>Download source codes of DiskId32 for Delphi</a>

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/Delphi/DiskId32Port'>View source codes of DiskId32 for Delphi</a>