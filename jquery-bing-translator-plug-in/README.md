#jquery Bing Translator Plug-in
The Bing Translator plug-in is intended to be used to embed the Bing Translator widget into elements on a page.

*This plugin takes advantage of the [Microsoft Bing Translator Widget](https://www.bing.com/widget/translator)*

###Required Javascript Libraries for the plug-in
* [jQuery 1.8.2](http://code.jquery.com/jquery-1.8.2.min.js) - minimum tested version

###Parameters
Param | Description
----- | -----------
mode  | Sets the mode on translation to "auto" or "manual (default) mode.  This specifies if user interaction is required to initiate translation
delay | Timer (in milliseconds) that delays the auto translation of the page **NOTE: This parameter is only used when the mode is set to auto**


###Usage
####Manual Translations
Loads the page in the default language of the site and requires user interaction to translate any content.  There are two ways to call the plug-in for manual mode.
```javascript
$("#element").languagePicker();
```

OR

```javascript
$("#element").languagePicker({mode:'manual'});
```

####Automatic Translation
Translates the page immediately as $(document).ready function occurs, or after a delay.  The language that the page is translated to is either specified in the dropdown by the user, or the browser language set when the user first visits the site is used.  There are two ways to call the plug-in for auto mode.
```javascript
$("#element").languagePicker({mode:'auto'});
```

OR

```javascript
$("#element").languagePicker({mode:'auto',delay:2000});
```