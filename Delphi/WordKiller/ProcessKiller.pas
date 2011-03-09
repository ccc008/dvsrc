unit ProcessKiller;

interface
uses Windows, SysUtils, Classes, tlhelp32, Messages;

type
  tmatch_functor = function (srcName: PWideChar): Boolean of object;
  pmatch_functor = ^tmatch_functor;
  TMatcher = class
  public
    constructor Create(const sample: String);
    function EqualToI(srcName: PWideChar): Boolean;
    function EndWithI(srcName: PWideChar): Boolean;
  private
    m_Sample: String;
  end;

  tpid_matcher_functor = function (srcPid: LongInt): Boolean of object;
  ppid_matcher_functor = ^tpid_matcher_functor;

  TPidOutdatedMatcher = class
  public
    constructor Create(intervalMS: Integer);
    function IsOutdated(srcPid: LongInt): Boolean;
  private
    m_IntervalMS: Integer;
  end;

//find child process
//enumerate windows in the process and find one with captionString in its caption
//if window is found then the function kills the process
procedure FindAndKillProcessByWindow(
  fMatchExeName: tmatch_functor; //selects process by exe filename
  fMatchWindowCaption: tmatch_functor  //selects window by window caption
);

procedure FindAndKillProcessByPid(fMatchExeName: tmatch_functor; fMatchPid: tpid_matcher_functor);

//find all processes with exe file names that match to search criteria
procedure GetListProcesses(fMatchExeName: tmatch_functor; dest: TStringList);

//find all windows in specified process which captions match to search criteria
function FindWindowInProcess(const processId: Cardinal; fMatchWindowCaption: tmatch_functor): Boolean;

//kill specified process
procedure KillProcess(const processId: Cardinal);

implementation

procedure GetListProcesses(fMatchExeName: tmatch_functor; dest: TStringList);
var handle: THandle;
  ProcStruct: PROCESSENTRY32; // from "tlhelp32" in uses clause
begin
  handle := CreateToolhelp32Snapshot(TH32CS_SNAPALL, 0);
  if handle <> 0 then try
    ProcStruct.dwSize := sizeof(PROCESSENTRY32);
    if Process32First(handle, ProcStruct) then repeat
      if fMatchExeName(PWideChar(@ProcStruct.szExeFile)) then begin
        dest.AddObject('', Pointer(ProcStruct.th32ProcessID));
      end;
    until not Process32Next(handle, ProcStruct);
  finally
    CloseHandle(handle);
  end;
end;

procedure FindAndKillProcessByPid(fMatchExeName: tmatch_functor; fMatchPid: tpid_matcher_functor);
var list: TStringList;
    i: Integer;
    pid: Cardinal;
begin
  list := TStringList.Create;
  try
    GetListProcesses(fMatchExeName, list);
    for i := 0 to list.Count - 1 do begin
      pid := Cardinal(list.Objects[i]);
      if fMatchPid(pid) then begin
        KillProcess(pid);
      end;
    end;
  finally
    list.Free;
  end;
end;

procedure FindAndKillProcessByWindow(fMatchExeName: tmatch_functor; fMatchWindowCaption: tmatch_functor);
var list: TStringList;
    i: Integer;
    pid: Cardinal;
begin
  list := TStringList.Create;
  try
    GetListProcesses(fMatchExeName, list);
    for i := 0 to list.Count - 1 do begin
      pid := Cardinal(list.Objects[i]);
      if FindWindowInProcess(pid, fMatchWindowCaption) then begin
        KillProcess(pid);
      end;
    end;
  finally
    list.Free;
  end;
end;

type
  wrapper = record  //helper class to pass functor to enum_window_proc
    m_F: tmatch_functor;
  end;
  pwrapper = ^wrapper;

function enum_window_proc(hWnd: HWND; lparam: LPARAM): BOOL; stdcall;
var buffer: String;
begin
  SetLength(buffer, MAX_PATH);
  Result := true;
  if SendMessage(hWnd, WM_GETTEXT, MAX_PATH, Integer(@buffer[1])) <> 0 then begin
    if pwrapper(lparam)^.m_F(@buffer[1]) then Result := false;
  end;
end;

function FindWindowInProcess(const processId: Cardinal; fMatchWindowCaption: tmatch_functor): Boolean;
var snap_proc_handle: THandle;
  next_proc: Boolean;
  thread_entry: TThreadEntry32;
  w: wrapper;
begin
  w.m_F := fMatchWindowCaption;
  Result := false;
  snap_proc_handle := CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0); //enumerate all threads
  if (snap_proc_handle <> INVALID_HANDLE_VALUE) then try
    thread_entry.dwSize := SizeOf(thread_entry);
    next_proc := Thread32First(snap_proc_handle, thread_entry);
    while next_proc do begin
      if thread_entry.th32OwnerProcessID = processId then begin //check the owner Pid against the PID requested
        if not EnumThreadWindows(thread_entry.th32ThreadID, @enum_window_proc, LPARAM(@w)) then begin
          Result := true;
          break;
        end;
      end;
      next_proc := Thread32Next(snap_proc_handle, thread_entry);
    end;
  finally
    CloseHandle(snap_proc_handle);
  end;
end;

procedure KillProcess(const processId: Cardinal);
var nerror: Integer;
    process: THandle;
    fdwExit: DWORD;
begin //see http://stackoverflow.com/questions/4690472/error-invalid-handle-on-terminateprocess-vs-c
  process := OpenProcess(PROCESS_TERMINATE, TRUE, processId);
  fdwExit := 0;
  GetExitCodeProcess(process, fdwExit);
  if not TerminateProcess(process, fdwExit) then begin
    nerror := GetLastError;
  end;
  CloseHandle(process);
end;

{ TMatcher }
constructor TMatcher.Create(const sample: String);
begin
  m_Sample := sample;
end;

function TMatcher.EndWithI(srcName: PWideChar): Boolean;
var
  len: Integer;
  len_sample: Integer;
  pw: PWideChar;
begin
  Result := false;
  len_sample := Length(m_Sample);
  len := lstrlen(srcName);
  if (len >= len_sample) then begin
      pw := PWideChar(@srcName[len - len_sample]);
      if lstrcmpi(pw, PWideChar(m_Sample)) = 0 then begin
        Result := true;
      end;
    end;
end;

function TMatcher.EqualToI(srcName: PWideChar): Boolean;
begin
  Result := lstrcmpi(srcName, PWideChar(m_Sample)) = 0;
end;

{ TPidOutdatedMatcher }
constructor TPidOutdatedMatcher.Create(intervalMS: Integer);
begin
  m_IntervalMS := intervalMS;
end;

function TPidOutdatedMatcher.IsOutdated(srcPid: Integer): Boolean;
var tc, te, tk, tu, current: FILETIME;
    u0, u1: ULARGE_INTEGER;
    process: THandle;
    st: SYSTEMTIME;
begin
  Result := false;
  process := OpenProcess(PROCESS_ALL_ACCESS, TRUE, srcPid);
  try
    if GetProcessTimes(process, tc, te, tk, tu) then begin
      FileTimeToSystemTime(tc, st);
      GetSystemTimeAsFileTime(current);

      FileTimeToSystemTime(current, st);
      u1.LowPart := current.dwLowDateTime;
      u1.HighPart := current.dwHighDateTime;

      u0.LowPart := tc.dwLowDateTime;
      u0.HighPart := tc.dwHighDateTime;

      if (u1.QuadPart - u0.QuadPart) / 10000 > m_IntervalMS then Result := true;
    end;
  finally
    CloseHandle(process);
  end;
end;

end.
