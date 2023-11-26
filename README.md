# Odyssey Experiment 1: Extensions and WebView2Ex
* _Many files may include "<<<stashed shanges" things so remove them_
* This experiment fully support installing extension through Chrome Web Store, **And nothing else** (no event has been added and the new tab experience is incosistant)
* The titlebar is the default one because the WinAppSDK 1.4 experimental doesn't look to have a good support for ExtendContentIntoTitleBar = true;
* The overall experience is very slow (by design) and full of unused Nuget packages 


--------------------

### Avantages of WebView2Ex found in this experiement :
* Native WebView2 Donload Flyout (won't be used in stable Odyssey)
* Native Password autofill (idem, not tested)
* Multiple profiles
* Full, Chrome-like extensions experience (with WebView2 preview channel and some additional code for installing extensions. Work with Chrome Web Store but not (and will probably never officially) with Edge Add-ons)
* Full control over WebView2 data (not tested)
* Fixed DevTools opening (not behind the WebView2 window anymore)
* Fixed context menus opening (wont randomly open in random locations in certain condiditons anymore) (won't be used in stable Odyssey)


### Limitations of WebView2Ex :
* No pointer style update (when hovering a link,...)
* Buggy page size (fix itself when the parent window is resizing)
* Buggy text fields experience (especially on websites like Discord edit message text box)
* Use the experimental and removed API ContentExternalOutputLink, making the WebView2Ex only work in WinAppSDK 1.4 experiemtal and its experience very unstable 

