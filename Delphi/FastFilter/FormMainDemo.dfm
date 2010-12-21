object Form1: TForm1
  Left = 0
  Top = 0
  Caption = 'Fast filter demo'
  ClientHeight = 406
  ClientWidth = 623
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poDesktopCenter
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 80
    Height = 13
    Caption = 'Filter by all fields'
  end
  object Label2: TLabel
    Left = 319
    Top = 8
    Width = 96
    Height = 13
    Caption = 'Filter by Name only:'
  end
  object gr1: TDBGrid
    Left = 8
    Top = 35
    Width = 297
    Height = 334
    DataSource = ds1
    Options = [dgTitles, dgIndicator, dgColumnResize, dgColLines, dgRowLines, dgTabs, dgRowSelect, dgConfirmDelete, dgCancelOnExit, dgTitleClick, dgTitleHotTrack]
    ReadOnly = True
    TabOrder = 3
    TitleFont.Charset = DEFAULT_CHARSET
    TitleFont.Color = clWindowText
    TitleFont.Height = -11
    TitleFont.Name = 'Tahoma'
    TitleFont.Style = []
    Columns = <
      item
        Expanded = False
        FieldName = 'ID'
        Visible = True
      end
      item
        Expanded = False
        FieldName = 'Name'
        Width = 100
        Visible = True
      end
      item
        Expanded = False
        FieldName = 'Phone'
        Width = 100
        Visible = True
      end>
  end
  object gr2: TDBGrid
    Left = 319
    Top = 35
    Width = 298
    Height = 334
    DataSource = ds2
    Options = [dgTitles, dgIndicator, dgColumnResize, dgColLines, dgRowLines, dgTabs, dgRowSelect, dgConfirmDelete, dgCancelOnExit, dgTitleClick, dgTitleHotTrack]
    ReadOnly = True
    TabOrder = 4
    TitleFont.Charset = DEFAULT_CHARSET
    TitleFont.Color = clWindowText
    TitleFont.Height = -11
    TitleFont.Name = 'Tahoma'
    TitleFont.Style = []
    Columns = <
      item
        Expanded = False
        FieldName = 'ID'
        Visible = True
      end
      item
        Expanded = False
        FieldName = 'Name'
        Width = 100
        Visible = True
      end
      item
        Expanded = False
        FieldName = 'Phone'
        Width = 100
        Visible = True
      end>
  end
  object edFilter2: TEdit
    Left = 421
    Top = 8
    Width = 196
    Height = 21
    TabOrder = 1
  end
  object edFilter1: TEdit
    Left = 94
    Top = 8
    Width = 211
    Height = 21
    TabOrder = 0
  end
  object btClose: TButton
    Left = 542
    Top = 375
    Width = 75
    Height = 25
    Cancel = True
    Caption = 'Close'
    Default = True
    TabOrder = 2
    OnClick = btCloseClick
  end
  object cds1: TClientDataSet
    Active = True
    Aggregates = <>
    Params = <>
    Left = 72
    Top = 160
    Data = {
      580000009619E0BD010000001800000003000000000003000000580002494404
      00010000000000044E616D650100490000000100055749445448020002006400
      0550686F6E65010049000000010005574944544802000200C8000000}
    object cds1ID: TIntegerField
      FieldName = 'ID'
    end
    object cds1Name: TStringField
      FieldName = 'Name'
      Size = 100
    end
    object cds1Phone: TStringField
      FieldName = 'Phone'
      Size = 200
    end
  end
  object cds2: TClientDataSet
    Active = True
    Aggregates = <>
    Params = <>
    Left = 528
    Top = 160
    Data = {
      580000009619E0BD010000001800000003000000000003000000580002494404
      00010000000000044E616D650100490000000100055749445448020002006400
      0550686F6E65010049000000010005574944544802000200C8000000}
    object IntegerField1: TIntegerField
      FieldName = 'ID'
    end
    object StringField1: TStringField
      FieldName = 'Name'
      Size = 100
    end
    object StringField2: TStringField
      FieldName = 'Phone'
      Size = 200
    end
  end
  object ds1: TDataSource
    DataSet = cds1
    Left = 120
    Top = 160
  end
  object ds2: TDataSource
    DataSet = cds2
    Left = 568
    Top = 160
  end
end
