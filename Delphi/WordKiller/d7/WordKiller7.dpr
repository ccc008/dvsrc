program WordKiller7;

uses
  SvcMgr,
  main in 'main.pas' {WordKillerService: TService},
  ProcessKiller in 'ProcessKiller.pas',
  WordThread in 'WordThread.pas',
  WordUtils in 'WordUtils.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TWordKillerService, WordKillerService);
  Application.Run;
end.
