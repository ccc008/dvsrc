unit FormMainDemo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Grids, DBGrids, DB, DBClient, FastFilter;

type
  TForm1 = class(TForm)
    cds1: TClientDataSet;
    gr1: TDBGrid;
    gr2: TDBGrid;
    Label1: TLabel;
    Label2: TLabel;
    edFilter2: TEdit;
    edFilter1: TEdit;
    btClose: TButton;
    cds1ID: TIntegerField;
    cds1Name: TStringField;
    cds1Phone: TStringField;
    cds2: TClientDataSet;
    IntegerField1: TIntegerField;
    StringField1: TStringField;
    StringField2: TStringField;
    ds1: TDataSource;
    ds2: TDataSource;
    procedure FormCreate(Sender: TObject);
    procedure btCloseClick(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
  private
    { Private declarations }
    m_F1: TFastFilterCommon;
    m_F2: TFastFilterCommon;
    procedure load_data_to_cds(cds: TClientDataSet);
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

procedure TForm1.btCloseClick(Sender: TObject);
begin
  Close;
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
  load_data_to_cds(cds1);
  load_data_to_cds(cds2);

  m_F1 := TFastFilterCommon.Create(edFilter1, cds1); //add filter by all fields
  m_F2 := TFastFilterCommon.Create(edFilter2, cds2, ['Name']); //add filter by field "name" only
end;

procedure TForm1.FormDestroy(Sender: TObject);
begin
  m_F1.Free;
  m_F2.Free;
end;

procedure TForm1.load_data_to_cds(cds: TClientDataSet);
const
  NUM_RECORDS = 5;
  names: array[0..NUM_RECORDS-1] of String = ('Иван Иванов', 'Петр Петров', 'Jhon Smit', 'Bill', 'Serge');
  phones: array[0..NUM_RECORDS-1] of String = ('22-11-22-33', '11-12-13-14', '33-55-66-77', '22-11-22-77', '44-88-99-88');
var i: Integer;
begin
  for i := 0 to NUM_RECORDS-1 do begin
    cds.Append;
    cds.Fields[0].AsInteger := i + 1;
    cds.Fields[1].AsString := names[i];
    cds.Fields[2].AsString := phones[i];
    cds.Post;
  end;
end;

end.
