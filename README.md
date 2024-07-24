# Odyssey Experiment: Extensions and WebView2Ex (With WinAppSDK 1.4 esperimental)
* _Many files have some "<<<stashed shanges" things so you will need to manually remove them_
* This experiment fully support installing extension through Chrome Web Store, **And nothing else** (no event has been added and the new tab experience is incosistant)
* The titlebar is the default one because the WinAppSDK 1.4 experimental doesn't look to have a good support for ExtendContentIntoTitleBar = true;
* The overall experience is very slow (by design) and the code full of unused stuff


--------------------

### Avantages of WebView2Ex found in this experiement :
* Native WebView2 Donload Flyout (won't be used in stable Odyssey)
* Native Password autofill (idem, not tested)
* Multiple profiles
* Full, Chrome-like extensions experience (with WebView2 preview channel and some additional code for installing extensions. Work with Chrome Web Store for downloading, but not (and will probably never officially) with Edge Add-ons)
* Fixed DevTools opening (not behind the WebView2 window anymore)
* Fixed context menus opening (wont randomly open in random locations anymore) (won't be used in stable Odyssey)


### Limitations of WebView2Ex :
* No pointer style update (when hovering a link,...)
* Buggy page size (fix itself when the parent window is resized)
* Buggy text fields experience (especially on websites like Discord edit message text box)
* Use the experimental and removed API ContentExternalOutputLink, making the WebView2Ex only work in WinAppSDK 1.4 experiemtal and its experience very unstable

### Notes :
* ContentExternalOutputLink has been re-added to WinAppSDK 1.6 exp-2, so WebView2Ex should work with this version
* Starting with WinAppSDK 1.6 exp-2, WebView2 is now in its standalone package, adding many features from WebView2Ex directly to WebView2
