///required bing translator javascript library (url below) & the bootstrap dropdown functionality
// https://ssl.microsofttranslator.com/ajax/v3/WidgetV3.ashx?siteData=ueOIGRSKkd965FeEGM5JtQ**
$.fn.languagePicker = function(options){

	//Write Bing translator script to page
	var selectedControl = this;
	var script = document.createElement("script");
	if(script.readyState) {  //IE
		script.onreadystatechange = function() {
	 		if ( script.readyState === "loaded" || script.readyState === "complete" ) {
		 		script.onreadystatechange = null;
		 		LanguagePicker.loadPicker(selectedControl,options);
			}
		};
	} 
	else {  //Others
		script.onload = function() {
			LanguagePicker.loadPicker(selectedControl,options);
		};
	}

	script.src = "https://ssl.microsofttranslator.com/ajax/v3/WidgetV3.ashx?siteData=ueOIGRSKkd965FeEGM5JtQ**";
	var s0 = document.getElementsByTagName('script')[0];
	s0.parentNode.insertBefore(script, s0);

	
};
LanguagePicker = {
	defaultLang: null,
	currentLang: 'en',
	preferredLangKey: 'BING_TRANSLATE_PREFERRED_LANG',
	localizedLanguageNames: {},
	sortableLanguages:[],
	loadingImageUrl:"images/ajax-loader.gif",
	loadingImage:$('<div/>').css('position','absolute').css('top','-2px').css('left','-2px').css('background-color','#FFFFFF').css('padding','22% 45%').append($('<div/>').text('Translating...').css('font-size','14px')).append($('<img/>').attr('src',this.loadingImageUrl)),
	buildLanguageList: function(languages){
		languages = languages.sort(this.languageSort);
		var column;
		for(var i=0;i<languages.length;i++){
			if(i % 17 == 0)
				column = $('<div/>').addClass('column');
				
			column.append($('<li/>').attr('data-lang-code',languages[i].Code).text(languages[i].Name+' ('+this.localizedLanguageNames[languages[i].Code]+')').click(function(e){
					LanguagePicker.translate($(e.target).data('lang-code'));
			}));
			if(i % 17 == 16 || i+1==languages.length)
				$('.language-picker-list').append(column);
		}
		LanguagePicker.footer.appendTo('.language-picker-list');
	},
	buildPrefLangPicker: function(prefLang,container){
		var ul = $('<ul/>').addClass('dropdown-menu');
		var langs = this.sortableLanguages.sort(this.languageSort);
		for(var i in langs){
			$('<li/>').attr('data-lang',langs[i].Code).text(langs[i].Name).click(function(e){
				var code =$(this).data('lang');
				$('.dropdown').attr('data-selected',code);
				$('.lang-name').text(LanguagePicker.localizedLanguageNames[code]);
			}).appendTo(ul);
		}
		ul.appendTo(container)
		container.attr('data-selected',prefLang).prepend($('<div/>').addClass('lang-name').text(this.localizedLanguageNames[prefLang]));
	},
	changeLangSelected: function(e){
		translate($(e.target).data('lang-code'));
		e.preventDefault();
		return false;
	},
	getCurrentLanguageLanguageList: function(){
		Microsoft.Translator.Widget.GetLanguagesForTranslate(this.currentLang ? this.currentLang : this.defaultLang ,function(languages){
			LanguagePicker.buildLanguageList(languages);
		});
	},
	getPreferredLanguage: function(){
		var prefLang = locache.get(this.preferredLangKey);
		if(!prefLang){
			prefLang = (navigator.language ? navigator.language : navigator.browserLanguage).substring(0,2);
			this.setPreferredLanguage(prefLang);
		}
		return prefLang;
	},
	isIE: function(version, comparison) {
		var cc  		= 'IE',
			b 			= document.createElement('B'),
			docElem 	= document.documentElement,
			isIE;
		    
		if(version){
			cc += ' ' + version;
			if(comparison){ cc = comparison + ' ' + cc; }
		}
		
		b.innerHTML = '<!--[if '+ cc +']><b id="iecctest"></b><![endif]-->';
		docElem.appendChild(b);
		isIE = !!document.getElementById('iecctest');
		docElem.removeChild(b);
		return isIE;
	},
	languageSort: function(first,second){
		if(first.Name==second.Name) return 0;
		if(first.Name>second.Name) return 1;
		return -1;
	},
	loadPicker:function(ctrl,options){
		//Create the language picker DOM element
		var $this = $(ctrl).addClass('language-picker');
		var ul = $('<ul/>').addClass('language-picker-list').attr('translate','no');
		this.defaultLang = navigator.language.substring(0,2);
		
		//Build the list of languages in their  that can be used
		var localLangs = Microsoft.Translator.Widget.GetLanguagesForTranslateLocalized();
		for(var i = 0;i<localLangs.length;i++){
			LanguagePicker.localizedLanguageNames[localLangs[i].Code] = localLangs[i].Name;
			LanguagePicker.sortableLanguages.push({'Code':localLangs[i].Code,'Name':localLangs[i].Name});
		}
		
		LanguagePicker.footer = $('<div/>').addClass('language-picker-footer');
		$('<div/>').append($('<a/>').attr('href','#').addClass('orig-lang').text('Original Language: '+ LanguagePicker.localizedLanguageNames[LanguagePicker.defaultLang]).attr('onclick','Microsoft.Translator.Widget.RestoreOriginal();')).appendTo(LanguagePicker.footer);
		
		if(options && options["translate"]=='auto'){
			var preferredLanguage = LanguagePicker.getPreferredLanguage();
			
			var preferredLanguageElement = $('<div/>').addClass('preferred-language').addClass('no-edit').text('Preferred Language: ');
			$('<span/>').addClass('pref-lang-read').addClass('pref-lang-name').text(LanguagePicker.localizedLanguageNames[preferredLanguage]).appendTo(preferredLanguageElement);
			$('<button/>').addClass('pref-lang-read').text('change').click(function(e){
				$('.preferred-language').removeClass('no-edit').addClass('edit');
				return false;
			}).appendTo(preferredLanguageElement);
			var select = $('<div/>').addClass('pref-lang-write').addClass('dropdown').append($('<div/>').addClass('caret'));
			LanguagePicker.buildPrefLangPicker(preferredLanguage,select);
			select.appendTo(preferredLanguageElement);
			$('<button/>').addClass('pref-lang-write').text('save').click(function(e){
				var code = $('.dropdown').data('selected');
				LanguagePicker.setPreferredLanguage(code);
				$('.pref-lang-name').text(LanguagePicker.localizedLanguageNames[code]);
				$('.preferred-language').removeClass('edit').addClass('no-edit');
				return false;
			}).appendTo(preferredLanguageElement);
			
			LanguagePicker.footer.append(preferredLanguageElement);
			
			$(window).load(function(){
				setTimeout(function(){
					LanguagePicker.translate(preferredLanguage);
				},2000);
			});
		}
		else{
			LanguagePicker.getCurrentLanguageLanguageList();
		}

		if(this.isIE(8,'lte')){
			$this.css('margin-left','30px');
		}

		$this.text('Translator: ');
		$this.append($('<span/>').addClass('language-name').attr('translate','no').text(LanguagePicker.localizedLanguageNames[LanguagePicker.currentLang]));
		ul.appendTo($this);
	},
	setPreferredLanguage: function(langCode){
		locache.set(this.preferredLangKey,langCode);
	},
	translate: function(to){
		LanguagePicker.currentLang = to;
		$('.language-picker-list').append(LanguagePicker.loadingImage);
		Microsoft.Translator.Widget.Translate(this.defaultLang,to,this.translateProgress,this.translateError,this.translateComplete,this.translateRestoreOriginal,60000);
	},
	translateComplete: function(){
		$('#languagePicker').mouseleave();
		$('.language-name').text(LanguagePicker.localizedLanguageNames[LanguagePicker.currentLang]);
		$('.language-picker-list').empty();
		LanguagePicker.getCurrentLanguageLanguageList();
		LanguagePicker.loadingImage.remove('.language-picker-list');
	},
	translateError: function(error){
		console.log(error);
	},
	translateProgress: function(progress){},
	translateRestoreOriginal: function(){
		if(LanguagePicker.loadingImage.hasClass('language-picker-list')){
			translateComplete();
		}
	},
};
