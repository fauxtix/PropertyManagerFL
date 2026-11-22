(function () {
    'use strict';

    var STORAGE_KEY = 'theme';
    var LIGHT_LINK_ID = 'sfThemeLight';
    var DARK_LINK_ID = 'sfThemeDark';
    var DEFAULT_THEME = 'dark';

    function currentTheme() {
        var stored = null;
        try {
            stored = localStorage.getItem(STORAGE_KEY);
        } catch (e) { /* localStorage pode estar indisponível (modo privado) */ }
        return stored === 'light' ? 'light' : DEFAULT_THEME;
    }

    function setLinkDisabled(id, disabled) {
        var link = document.getElementById(id);
        if (link) {
            link.disabled = disabled;
        }
    }

    window.applyTheme = function (theme) {
        var isDark = theme !== 'light';
        try {
            localStorage.setItem(STORAGE_KEY, isDark ? 'dark' : 'light');
        } catch (e) { /* preferência não persistida; tema ainda é aplicado */ }

        setLinkDisabled(LIGHT_LINK_ID, isDark);
        setLinkDisabled(DARK_LINK_ID, !isDark);
        document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
        if (document.body) {
            document.body.classList.toggle('dark-theme', isDark);
        }
    };

    window.getCurrentTheme = function () {
        return currentTheme();
    };

    window.toggleTheme = function () {
        var next = currentTheme() === 'dark' ? 'light' : 'dark';
        window.applyTheme(next);
        return next;
    };

    // O script inline no <head> já aplicou o tema antes do primeiro paint.
    // Aqui aplicamos também a classe no body e garantimos coerência com a preferência guardada.
    window.applyTheme(currentTheme());
})();
