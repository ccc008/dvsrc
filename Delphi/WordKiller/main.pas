unit main;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, SvcMgr, Dialogs, WordThread,
  ExtCtrls;

type
  TWordKillerService = class(TService)
    Timer1: TTimer;
    procedure Timer1Timer(Sender: TObject);
    procedure ServiceStart(Sender: TService; var Started: Boolean);
  private
    procedure kill_specified_process;
    procedure kill_outdated_processes(timeMS: Integer);
    { Private declarations }
  public
    function GetServiceController: TServiceController; override;
    { Public declarations }
  private
     m_Thread: TWordThread;
     m_srcFileName: String;
     m_destFileName: String;
  end;

var
  WordKillerService: TWordKillerService;

implementation

uses ProcessKiller;

{$R *.DFM}
//'{604547EE-C3DA-4b5a-A13A-19A6C06BC82A}';

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

procedure TWordKillerService.kill_outdated_processes(timeMS: Integer);
var
  m_exe: TMatcher;
  m_d: ProcessKiller.TPidOutdatedMatcher;
begin
  m_d := nil;
  m_exe := TMatcher.Create('winword.exe');
  try
    m_d := TPidOutdatedMatcher.Create(timeMS);
    ProcessKiller.FindAndKillProcessByPid(m_exe.EqualToI, m_d.IsOutdated);
  finally
    m_d.Free;
    m_exe.Free;
  end;
end;

procedure TWordKillerService.kill_specified_process;
var
  m_exe: TMatcher;
  m_str: TMatcher;
begin
  m_str := nil;
  m_exe := TMatcher.Create('winword.exe');
  try
    m_str := TMatcher.Create(m_srcFileName);
    ProcessKiller.FindAndKillProcessByWindow(m_exe.EqualToI, m_str.EndWithI);
  finally
    m_exe.Free;
    m_str.Free;
  end;
end;

procedure TWordKillerService.ServiceStart(Sender: TService;
  var Started: Boolean);
begin
  Timer1.Enabled := true;
  m_srcFileName := get_full_path_by_related_path('testdata\src.doc');
end;

procedure TWordKillerService.Timer1Timer(Sender: TObject);
begin
  m_Thread := TWordThread.Create(m_srcFileName);
  m_Thread.Start;

  Sleep(5000);

  kill_outdated_processes(100);
  kill_specified_process;
end;

end.
