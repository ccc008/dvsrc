Android API doesn't allow to create animated widget. Anyway, there are some workarounds.

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/Android/TestAnimatedWidget'>TestAnimatedWidget</a> is example of creation animated widget. The animation is very simple - a wheel. Click on widget and the wheel will make one circle.

TestAnimatedWidget tests three approaches for animation emulation. First one uses ```
remoteViews.setImageViewResource(R.id.AAA, R.drawable.BBB)``` to pass id of next animation frame to remote view. Second one uses ```
remoteViews.setImageViewBitmap(R.id.AAA, bitmap)``` to pass "generated" bitmap to remote view. Bitmaps are simply loaded from resources. Last one uses ```
remoteViews.setImageViewUri(R.id.AAA, uri)``` to pass uri of "generated" bitmap to remote view. Bitmaps are simply loaded from resources and stored on disk. To select approach, please set appropriate value of flag USE\_GENERATED\_BITMAPS in class TestAnimatedWidgetProvider.

First approach is fast enough. Second is terribly slow and generates errors "!! FAILED BINDER TRANSACTION !!". Third is slow too, but doesn't generate errors. Instead of it, it skips most animation frames that looks very badly. So, it's possible to made conclusion, that it's a problem to create animated widget that generates animation bitmaps on the fly.


---

My <a href='http://derevyanko.blogspot.com/2010/10/android-how-to.html'>original blog message</a>, in Russian.

<a href='http://code.google.com/p/dvsrc/source/browse/trunk/Android/TestAnimatedWidget'>Source codes of TestAnimatedWidget</a>

<a href='http://code.google.com/p/dvsrc/downloads/detail?name=TestAnimatedWidget.7z&can=2&q='>Download TestAnimatedWidget source codes as 7z-archive</a>.

Alternative desktop <a href='http://code.google.com/p/android-launcher-plus/'>Launcher plus</a> and <a href='http://code.google.com/p/animated-widget-for-hpp/'>sample of animated widget</a> for this desktop.

Animated widget <a href='http://androidcommunity.com/forums/f7/animated-widgets-on-with-google-devices-26383/'>Flip Clock</a> that uses only static animation from resources.

<a href='http://andytsui.wordpress.com/2010/06/06/animating-android-home-screen-widget/'>Animating Android home-screen widget</a> - detailed description of common principles of animated widget creation.

Some useful issues on android google codes:
<a href='http://code.google.com/p/android/issues/detail?id=9580&q=animation%20widget&colspec=ID%20Type%20Status%20Owner%20Summary%20Stars'>animated widget</a>, <a href='http://code.google.com/p/android/issues/detail?id=8489&q=setImageViewBitmap&colspec=ID%20Type%20Status%20Owner%20Summary%20Stars'>setImageViewBitmap() doesn't work on android 2.2</a>, <a href='http://code.google.com/p/android/issues/detail?id=6957&q=setImageViewBitmap&colspec=ID%20Type%20Status%20Owner%20Summary%20Stars'>possibility to use setImageViewUri instead of setImageViewBitmap</a>.