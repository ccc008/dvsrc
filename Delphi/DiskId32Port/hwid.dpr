//Copyright 2009-2010 by Victor Derevyanko, dvpublic0@gmail.com
//http://code.google.com/p/dvsrc/
//http://derevyanko.blogspot.com/2009/02/hardware-id-diskid32-delphi.html
//{$Id$}

program hwid;
//  This file is a part of DiskID for Delphi
//  Original code of DiskID can be taken here:
//  http://www.winsim.com/diskid32/diskid32.html
//  The code was ported from C++ to Delphi by Victor Derevyanko, dvpublic0@gmail.com
//  If you find any error please send me bugreport by email. Thanks in advance.
//  The translation was donated by efaktum (http://www.efaktum.dk).

{$APPTYPE CONSOLE}
uses
  SysUtils,
  hwid_impl in 'hwid_impl.pas',
  winioctl in 'winioctl.pas',
  crtdll_wrapper in 'crtdll_wrapper.pas';

var id: Integer;
    r: tresults_array_dv;
    //os: THandleStream;
begin
  { TODO -oUser -cConsole Main : Insert code here }
  test;
  StrToDate('1.08.2010'); 
//get hardware id for this computer
//Execude hwid.exe debug for displaying extended debug information
  SetPrintDebugInfo(ParamCount <> 0);
  id := getHardDriveComputerID(r);
end.
