AniClick is a demo application. It shows how to implement "animated" widget on Android.

The widget shows a wheel (i.e. first frame of wheel animation). But if you click on the wheel then it will start to spin. If you click on the wheel again, it will stop spinning. After that, the widget will show clicked frame from wheel animation.

Actually, the widget is static. Animation is implemented on activity. When you clicks the widget, new activity is created. The activity overrides whole desktop. And it is absolutely transparent - you can see original widget througt it. Activity draws animation exactly on the same screen place where original widget located. So, it looks for user as animated widget. When you click on the "widget" second time, activity closes itself and sends command to widget. Widget updates its content and starts to show clicked frame.

This approach is limited, but animation FPS is much better then real <a href='http://code.google.com/p/dvsrc/wiki/AndroidTestAnimatedWidget'>Animated Widget</a> is able to provide.


---

My <a href='http://derevyanko.blogspot.com/2010/12/android.html'>original blog message</a>, in Russian.

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/Android/AniClick'>View</a> source codes.

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=20101102_AniClick_src.7z&can=2&q='>Downaload</a> source codes as 7zip-archive.

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=20101102_AniClick_apk.7z&can=2&q='>Download apk</a> (api 2.1).