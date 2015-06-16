Assume, that you have localized application. F.e. it's localized on Danish: GUI is on DK language, all text messages and resources strings are on DK too. You need to de-localize it.

DxGetTextLangSwapper is able to replace all strings in source code by their translations using PO file. You can use PO file with translation DK->ENG to de-localize the application and replace all danish strings in source codes by their english analogs. At the same time, the utility generates reverese PO file with ENG->DK translation.

Original blog post (in Russian) here: [Делокализация Delphi приложения](http://derevyanko.blogspot.com/2011/04/delphi.html)

View [source codes on C#](http://code.google.com/p/dvsrc/source/browse/trunk/CSharp/DxGetTextLangSwapper/), download [7z-archive with exe+source codes](http://code.google.com/p/dvsrc/downloads/detail?name=20110423DxGetTextLangSwapper.7z)