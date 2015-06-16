Easy way to add filter capabilities to DataGrid+DataSet. Source codes contain class TFastFilterCommon. To create filter you should create instance of this class, and pass instance of TEdit and instance of TDataSet to constructor.

```
var 
  m_F1: TFastFilterCommon;
  ds: TDataSet;
  edit1: TEdit;
  .....
  m_F1 := TFastFilterCommon.Create(edit1, ds1);
```

Then connect ds1 with data grid. When you type text in edit1, content of ds1 will be filtered and data grid will display only records that contain typed text (in any field). If you want to filter using only specified set of fields, you should create instance of TFastFilterCommon in such way:

```
  m_F2 := TFastFilterCommon.Create(edit1, ds1, ['Name', 'Surname']); //add filter by fields "name" and "surname"
```


---

<a href=''>Original blog message (in Russian)</a>

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=20101003FastFilter.zip'>Source codes in zip archive</a>

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/Delphi/FastFilter'>Browse source codes</a>