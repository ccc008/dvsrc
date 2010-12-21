//Copyright by Victor Derevyanko, dvpublic0@gmail.com
//http://derevyanko.blogspot.com/2010/10/blog-post.html
//http://code.google.com/p/dvsrc/
//{$Id$}

program FastFilterDemo;

uses
  Forms,
  FormMainDemo in 'FormMainDemo.pas' {Form1},
  FastFilter in 'FastFilter.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.
