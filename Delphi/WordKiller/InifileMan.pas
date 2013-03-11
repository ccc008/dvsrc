unit InifileMan;

interface
USES inifiles, Classes, registry ;


Function ReadStrFromInifile(filename,Section,Ident:String):String;
Procedure WriteStrToInifile(filename,Section,Ident,Value:String);
Function ReadSectionFromIniFile(FileName,Section:String):TStringList;
Function ReadStrFromRegistry(Section,Ident:String):String;
Procedure WriteStrToRegistry(Section,Ident,Value:String);



implementation

Function ReadSectionFromIniFile(FileName,Section:String):TStringList;
Var Thefile : TIniFile;
Begin
  TheFile := TInifile.Create(filename);
  try
    Result := TStringList.Create;
    TheFile.ReadSection(Section,Result);
  finally
    TheFile.Free;
  end;
End;


Procedure WriteStrToInifile(filename,Section,Ident,Value:String);
Var Thefile : TIniFile;
Begin
  TheFile := TInifile.Create(filename);
  try
    TheFile.WriteString(Section,Ident,Value);
  finally
    TheFile.Free;
  end;
End;

Procedure WriteStrToRegistry(Section,Ident,Value:String);
Var reg : TRegistry;
Begin
  reg := TRegistry.Create;
  try
    if reg.OpenKey(Section,true) then
       reg.WriteString(ident,value);
  finally
    Reg.Free;
  end;
End;


Function ReadStrFromInifile(filename,Section,Ident:String):String;
Var Thefile : TIniFile;
Begin
  TheFile := TInifile.Create(filename);
  try
    result := TheFile.ReadString(section,ident,'');
  finally
    TheFile.Free;
  end;
End;

Function ReadStrFromRegistry(Section,Ident:String):String;
Var reg : TRegistry;
Begin
  reg := TRegistry.Create;
  try
    if reg.OpenKey(Section,false) then
       result := reg.ReadString(ident)
    else result := '';

    //TheFile.ReadString(section,ident,'');
  finally
    Reg.Free;
  end;
End;

end.
