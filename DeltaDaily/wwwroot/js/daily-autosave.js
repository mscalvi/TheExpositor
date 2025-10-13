window.DeltaDaily_AutoSave = (function () {
    let dotnet;

    function safeInvoke() {
        if (!dotnet) return;
        try {
            // Fire and forget — não bloqueia a navegação
            dotnet.invokeMethodAsync('SaveFromJs');
        } catch (e) {
            // silencioso
        }
    }

    function onVisibilityChange() {
        if (document.visibilityState === 'hidden') {
            safeInvoke();
        }
    }

    function onPageHide() {
        safeInvoke();
    }

    return {
        register: function (dotNetRef) {
            dotnet = dotNetRef;
            document.addEventListener('visibilitychange', onVisibilityChange, { passive: true });
            // pagehide cobre iOS/Safari melhor que beforeunload
            window.addEventListener('pagehide', onPageHide, { passive: true });
        },
        unregister: function () {
            document.removeEventListener('visibilitychange', onVisibilityChange);
            window.removeEventListener('pagehide', onPageHide);
            dotnet = null;
        }
    };
})();
