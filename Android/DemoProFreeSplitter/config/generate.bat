set ROOTDIR=%CD%
call %ROOTDIR%\apfsplitter.exe "%ROOTDIR%\..\sources" "%ROOTDIR%\..\versions\pro" "%ROOTDIR%\apfs_config.xml " pro
call %ROOTDIR%\apfsplitter.exe "%ROOTDIR%\..\sources" "%ROOTDIR%\..\versions\free" "%ROOTDIR%\apfs_config.xml " !pro