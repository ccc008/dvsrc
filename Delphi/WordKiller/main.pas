//Copyright 2009-2011 by Victor Derevyanko, dvpublic0@gmail.com
//http://code.google.com/p/dvsrc/
//{$Id$}

unit main;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, SvcMgr, Dialogs, WordThread,
  ExtCtrls, inifileman;

type
  tconfig_params = record
    ProcessExeFileName: String;
    IntervalForCheckOutdatedProcessesSEC: Integer;
    MaxAllowedTimeForProcessMS: Integer;
    TestFindAndKillProcessByWindow: Boolean;
    SrcFileNameForKillProcessByWindow: String;
  end;

type
  TWordKillerService = class(TService)
    TimerTestKillByWindow: TTimer;
    TimerOutdatedProcesses: TTimer;
    procedure TimerTestKillByWindowTimer(Sender: TObject);
    procedure ServiceStart(Sender: TService; var Started: Boolean);
    procedure TimerOutdatedProcessesTimer(Sender: TObject);
  private
    procedure load_config;
    { Private declarations }
  public
    function GetServiceController: TServiceController; override;
    { Public declarations }
  private
    m_Config: tconfig_params;
    m_Thread: TWordThread;
  end;

var
  WordKillerService: TWordKillerService;

implementation

uses ProcessKiller;

{$R *.DFM}
function get_full_path_by_related_path(const FilePath: String): String;
var s: String;
begin
  SetLength(s, MAX_PATH);
  GetModuleFileName(0, PWideChar(s), MAX_PATH);
  s := ExtractFilePath(PWideChar(s));
  s := IncludeTrailingPathDelimiter(s);
  Result := s + FilePath;
end;

procedure ServiceController(CtrlCode: DWord); stdcall;
begin
  WordKillerService.Controller(CtrlCode);
end;

function TWordKillerService.GetServiceController: TServiceController;
begin
  Result := ServiceController;
end;

procedure TWordKillerService.ServiceStart(Sender: TService;
  var Started: Boolean);
begin
  load_config;
  if m_Config.TestFindAndKillProcessByWindow then begin
    TimerTestKillByWindow.Enabled := true;
  end;
  if m_Config.ProcessExeFileName <> '' then begin
    TimerOutdatedProcesses.Interval := m_Config.IntervalForCheckOutdatedProcessesSEC;
    TimerOutdatedProcesses.Enabled := true;
  end;
end;

procedure TWordKillerService.TimerOutdatedProcessesTimer(Sender: TObject);
begin
  ProcessKiller.FindAndKillProcessByPid(m_Config.ProcessExeFileName, m_Config.MaxAllowedTimeForProcessMS);
end;

procedure TWordKillerService.TimerTestKillByWindowTimer(Sender: TObject);
begin
  m_Thread := TWordThread.Create(m_Config.SrcFileNameForKillProcessByWindow);
  m_Thread.Start;

  Sleep(3000);
  ProcessKiller.FindAndKillProcessByWindow('winword.exe', m_Config.SrcFileNameForKillProcessByWindow);
  Sleep(3000);
end;

procedure TWordKillerService.load_config;
    function read_int(const srcParamName: String; DefaultValue: Integer): Integer;
    var svalue: String;
    begin
      svalue := ReadStrFrominifile(get_full_path_by_related_path('WordKiller.ini'), 'data', srcParamName);
      if svalue = ''
        then Result := DefaultValue
        else Result := StrToInt(svalue);
    end;
    function read_str(const srcParamName: String; DefaultValue: String): String;
    var svalue: String;
    begin
      svalue := ReadStrFrominifile(get_full_path_by_related_path('WordKiller.ini'), 'data', srcParamName);
      if svalue = ''
        then Result := DefaultValue
        else Result := svalue;
    end;
begin
  m_Config.ProcessExeFileName := read_str('ProcessExeFileName', 'winword.exe');
  m_Config.IntervalForCheckOutdatedProcessesSEC := read_int('IntervalForCheckOutdatedProcessesSEC', 24*60*60);
  m_Config.MaxAllowedTimeForProcessMS := read_int('MaxAllowedTimeForProcessMS', 10*60*1000);
  m_Config.TestFindAndKillProcessByWindow := 0 <> read_int('TestFindAndKillProcessByWindow', 0);
  m_Config.SrcFileNameForKillProcessByWindow := read_str('SrcFileNameForKillProcessByWindow', 'testdata\src.doc');
  if ExtractFileDrive(m_Config.SrcFileNameForKillProcessByWindow) = '' then begin
    m_Config.SrcFileNameForKillProcessByWindow := get_full_path_by_related_path(m_Config.SrcFileNameForKillProcessByWindow);
  end;
end;


end.
