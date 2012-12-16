set CDIR=%CD%
apfsplitter.exe "%CDIR%\..\src" "%CDIR%\..\versions\pro"  "%CDIR%\..\versions\apfs_config.xml " pro
apfsplitter.exe "%CDIR%\..\src" "%CDIR%\..\versions\free" "%CDIR%\..\versions\apfs_config.xml " !pro
apfsplitter.exe "%CDIR%\..\src" "%CDIR%\..\versions\test" "%CDIR%\..\versions\apfs_config_test.xml " test

