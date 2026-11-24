/*!
 * help-dialog.js — melhora o manual exibido pelo HelpDialog.
 *
 * O conteúdo Markdown é injetado pelo Blazor dentro de um SfDialog que é
 * criado/removido dinamicamente. Por isso usamos delegação de eventos e um
 * MutationObserver em vez de procurar os elementos apenas no carregamento.
 *
 * - links do índice (href="#...") fazem scroll dentro da área do manual;
 * - o botão "voltar ao topo" só aparece depois de rolar o conteúdo.
 */
(function () {
    'use strict';

    var DIALOG_SELECTOR = '.help-dialog';
    var CONTENT_SELECTOR = '.help-dialog .help-content';
    var BUTTON_SELECTOR = '.help-dialog .help-scroll-top';
    var LINK_SELECTOR = '.help-dialog .markdown-body a[href^="#"]';
    var SHOW_AFTER_PX = 80;

    // WeakMap: as entradas desaparecem automaticamente quando o diálogo é
    // removido do DOM pelo Syncfusion.
    var wired = new WeakMap();

    function updateButton(button, content) {
        if (!button || !content) {
            return;
        }

        var visible = content.scrollTop > SHOW_AFTER_PX;
        button.classList.toggle('is-visible', visible);
        button.setAttribute('aria-hidden', visible ? 'false' : 'true');
        button.tabIndex = visible ? 0 : -1;
    }

    function wireContent(content) {
        if (!content) {
            return;
        }

        var dialog = content.closest(DIALOG_SELECTOR);
        var button = dialog ? dialog.querySelector(BUTTON_SELECTOR) : null;

        // O Syncfusion pode inserir o conteúdo e o botão em renders separados.
        // Não marcar como ligado enquanto o botão não existir, nem guardar um
        // botão desatualizado se o diálogo for recriado.
        if (!button) {
            return;
        }

        var state = wired.get(content);
        if (state) {
            state.button = button;
            updateButton(button, content);
            return;
        }

        state = { button: button };
        wired.set(content, state);

        content.addEventListener('scroll', function () {
            updateButton(state.button, content);
        }, { passive: true });

        updateButton(button, content);
    }

    function wireAll() {
        var contents = document.querySelectorAll(CONTENT_SELECTOR);
        for (var i = 0; i < contents.length; i++) {
            wireContent(contents[i]);
        }
    }

    function decodeFragment(raw) {
        try {
            return decodeURIComponent(raw);
        } catch (e) {
            return raw;
        }
    }

    function scrollToElement(content, target) {
        var offset = target.getBoundingClientRect().top - content.getBoundingClientRect().top + content.scrollTop - 12;

        if (typeof content.scrollTo === 'function') {
            content.scrollTo({ top: offset, behavior: 'smooth' });
        } else {
            content.scrollTop = offset;
        }
    }

    function scrollToTop(content) {
        if (!content) {
            return;
        }

        if (typeof content.scrollTo === 'function') {
            content.scrollTo({ top: 0, behavior: 'smooth' });
        } else {
            content.scrollTop = 0;
        }

        content.focus && content.focus({ preventScroll: true });
    }

    document.addEventListener('click', function (event) {
        if (!event.target || typeof event.target.closest !== 'function') {
            return;
        }

        var link = event.target.closest(LINK_SELECTOR);
        if (link) {
            var content = link.closest(CONTENT_SELECTOR);
            var fragment = decodeFragment(link.getAttribute('href').slice(1));
            var target = fragment ? document.getElementById(fragment) : null;

            if (content && target && content.contains(target)) {
                event.preventDefault();
                scrollToElement(content, target);
            }
            return;
        }

        var button = event.target.closest(BUTTON_SELECTOR);
        if (button) {
            var dialog = button.closest(DIALOG_SELECTOR);
            scrollToTop(dialog ? dialog.querySelector(CONTENT_SELECTOR) : null);
        }
    });

    if (typeof MutationObserver === 'function') {
        var observer = new MutationObserver(wireAll);
        observer.observe(document.documentElement, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', wireAll);
    } else {
        wireAll();
    }
})();
