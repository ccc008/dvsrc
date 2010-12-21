unit FastFilter;

interface
uses Windows, SysUtils, DB, ExtCtrls, ComCtrls, Classes, StrUtils, StdCtrls, Graphics;

type
  TFastFilter = class
  public
  //search text entries in fields of srcDs
    class function OnFilterRecord(const filterText: String; srcDs: TDataSet): Boolean; overload; //all fields
    class function OnFilterRecord(const filterText: String; srcDs: TDataSet; srcFields: array of String): Boolean; overload; //only selected fields

    class procedure OnTimer(const filterText: String; srcDs: TDataSet; timerFilter: TTimer);
    class procedure OnFilterChange(const filterText: String; srcDs: TDataSet; timerFilter: TTimer);
    class function OnPressKeyInFilter(const srcKey: Dword; srcDs: TDataSet; const filterText: String; timerFilter: TTimer): Boolean;
    class procedure InitializeFilter(edFilter: TEdit; timerFilter: TTimer); //helper function to make appearance of all filter edit boxes same
  private
    class function pos_case_insensitive(const subStrUpperCased, srcS: string): Boolean; //первый аргумент предполагается в UpperCase
  end;

//common implementation of the filter for TEdit
  TFastFilterCommon = class
  public
    constructor Create(const edFilter: TEdit; srcDs: TDataSet); overload;
    constructor Create(const edFilter: TEdit; srcDs: TDataSet; srcFields: array of String); overload;
    destructor Destroy; override;
  private
    procedure initialize(const edFilter: TEdit; srcDs: TDataSet);
    procedure on_filter_change(Sender: TObject);
    procedure on_timer(Sender: TObject);
    procedure on_filter_record(DataSet: TDataSet; var Accept: Boolean);
    procedure on_filter_keydown(Sender: TObject; var Key: Word; Shift: TShiftState);
  private
    m_Edit: TEdit;
    m_Ds: TDataSet;
    m_Timer: TTimer;
    m_Fields: array of String;
  end;

implementation

const
  COLOR_FILTER_BOX = clMoneyGreen; //all filters can be highlighted by same color
  DEFAULT_STRING_FIELD_SIZE = 200;
  DEFAULT_FILTER_TIMER_DELAY_MS = 1000;
  DEFAULT_FILTER_TOOLTIP = 'Use Ctrl+Enter to execute filter manually';

class procedure TFastFilter.OnTimer(const filterText: String; srcDs: TDataSet; timerFilter: TTimer);
begin  //executes filter by timer and turns timer off
  timerFilter.Enabled := false;
  if filterText <> '' then
  begin
    srcDs.Filtered := false;
    srcDs.Filtered := true;
  end else begin
    srcDs.Filtered := false;
  end;
end;

class function TFastFilter.OnFilterRecord(const filterText: String; srcDs: TDataSet): Boolean;
var text: String;
    i: Integer;
begin  //record is accepted if at least one field contains specified text
  text := AnsiUpperCase(filterText);
  Result := false;
  for i := 0 to srcDs.FieldCount-1 do begin
    if pos_case_insensitive(text, srcDs.Fields[i].DisplayText) then begin
      Result := true;
      break;
    end;
  end;
end;

class function TFastFilter.OnFilterRecord(const filterText: String; srcDs: TDataSet; srcFields: array of String): Boolean;
var text: String;
    i: Integer;
begin //record is approved if at least one of selected fields contains specified text
  text := AnsiUpperCase(filterText);
  Result := false;
  for i := Low(srcFields) to High(srcFields) do begin
    if pos_case_insensitive(text, srcDs.FieldByName(srcFields[i]).DisplayText) then begin
      Result := true;
      break;
    end;
  end;
end;

class procedure TFastFilter.OnFilterChange(const filterText: String; srcDs: TDataSet; timerFilter: TTimer);
begin //execute timer after any modification of filter string
      //if user won't press key during timer interfal then filtration will be executed automatically
  timerFilter.Enabled := false;
  timerFilter.Enabled := true;
  if filterText = '' then begin
    srcDs.Filtered := false;
  end;
end;

class function TFastFilter.OnPressKeyInFilter(const srcKey: Dword; srcDs: TDataSet;
  const filterText: String; timerFilter: TTimer): Boolean;
begin //allows user to change dataset position directly from filter edit box using Up and Down keys
  Result := true;
  if srcKey = VK_UP then srcDs.Prior
  else if srcKey = VK_DOWN then srcDs.Next
  else if (srcKey = VK_RETURN) then OnTimer(filterText, srcDs, timerFilter)
  else Result := false;
end;

class function TFastFilter.pos_case_insensitive(const subStrUpperCased, srcS: string): Boolean;
begin
  if Length(subStrUpperCased) > Length(srcS)
    then Result := false
    else Result := AnsiPos(subStrUpperCased, AnsiUpperCase(srcS)) <> 0;
  //!TODO: до тех пор, пока нет нормальной реализации ansi_posiex
  //первый аргумент рассматриваем как уже приведенный к upper case
  //все функции  OnFilterRecordXXX приводят его перед вызовом pos_case_insensitive
end;

class procedure TFastFilter.InitializeFilter(edFilter: TEdit;
  timerFilter: TTimer);
begin //helper function to make all filter edit boxes "same"
  edFilter.Color := COLOR_FILTER_BOX;
  timerFilter.Interval := DEFAULT_FILTER_TIMER_DELAY_MS;
  edFilter.Hint := DEFAULT_FILTER_TOOLTIP;
  edFilter.ShowHint := true;
end;

///////////////////////////////////////////////////////////////////////////////
// simple implementation of the filter for TEdit
constructor TFastFilterCommon.Create(const edFilter: TEdit; srcDs: TDataSet);
begin
  initialize(edFilter, srcDs);
end;

constructor TFastFilterCommon.Create(const edFilter: TEdit; srcDs: TDataSet;
  srcFields: array of String);
var i: Integer;
begin
  initialize(edFilter, srcDs);
  SetLength(m_Fields, Length(srcFields));
  for i := 0 to Length(srcFields)-1 do m_Fields[i] := srcFields[i];
end;

procedure TFastFilterCommon.initialize(const edFilter: TEdit; srcDs: TDataSet);
begin
  m_Edit := edFilter;
  m_Edit.ShowHint := true;
  m_Edit.Hint := DEFAULT_FILTER_TOOLTIP;

  m_Ds := srcDs;
  m_Timer := TTimer.Create(edFilter.Parent);
  m_Timer.Interval := DEFAULT_FILTER_TIMER_DELAY_MS;
  m_Timer.Enabled := false;

  assert(not assigned(m_Edit.OnChange));
  m_Edit.OnChange := on_filter_change;

  assert(not assigned(m_Ds.OnFilterRecord));
  m_Ds.OnFilterRecord := on_filter_record;

  assert(not assigned(m_Edit.OnKeyDown));
  m_Edit.OnKeyDown := on_filter_keydown;

  m_Timer.OnTimer := on_timer;
end;

destructor TFastFilterCommon.Destroy;
begin
  m_Timer.Free;
  inherited;
end;

procedure TFastFilterCommon.on_filter_change(Sender: TObject);
begin
  TFastFilter.OnFilterChange(m_Edit.Text, m_Ds, m_Timer)
end;

procedure TFastFilterCommon.on_filter_keydown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  TFastFilter.OnPressKeyInFilter(Key, m_Ds, m_Edit.Text, m_Timer);
end;

procedure TFastFilterCommon.on_filter_record(DataSet: TDataSet; var Accept: Boolean);
var list: TStringList;
    i: Integer;
begin
  list := TStringList.Create;
  try
    ExtractStrings([' '], [], PChar(m_Edit.Text), list);
    for i := 0 to list.Count - 1 do begin
      if Length(m_Fields) = 0
        then Accept := TFastFilter.OnFilterRecord(Trim(list[i]), m_Ds)
        else Accept := TFastFilter.OnFilterRecord(Trim(list[i]), m_Ds, m_Fields);
      if Accept then break;
    end;
  finally
    list.Free;
  end;
end;

procedure TFastFilterCommon.on_timer(Sender: TObject);
begin
  TFastFilter.OnTimer(m_Edit.Text, m_Ds, m_Timer);
end;

end.
