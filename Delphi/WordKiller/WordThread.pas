//Copyright 2009-2011 by Victor Derevyanko, dvpublic0@gmail.com
//http://code.google.com/p/dvsrc/
//{$Id$}

unit WordThread;

interface
uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, SvcMgr, Dialogs, ExtCtrls, ActiveX;

type
  TWordThread = class (TThread)
  public
    constructor Create(const srcFileName: String);
    destructor Destroy; override;

    procedure Execute; override;
    function GetResultErrorCode: Integer;
  private
    m_SrcFileName: String;
    m_ResultErrorCode: Integer;
  end;

implementation

uses WordUtils;

{ TThreadConverter }

constructor TWordThread.Create(const srcFileName: String);
begin
  inherited Create(true);
  m_SrcFileName := srcFileName;
  m_ResultErrorCode := 0;
end;

destructor TWordThread.Destroy;
begin
  inherited;

end;

procedure TWordThread.Execute;
begin
  inherited;
  try
    CoInitialize(nil);
    try
      WordUtils.OpenWord(m_SrcFileName);
    finally
      CoUninitialize();
    end;
  except on ex: Exception do begin
      m_ResultErrorCode := -1;
    end;
  end;
end;

function TWordThread.GetResultErrorCode: Integer;
begin
  Result := m_ResultErrorCode;
end;

end.
