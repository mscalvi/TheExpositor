// wwwroot/js/pwa.js
window.pwa = (function () {
    const isiOS = /iphone|ipad|ipod/i.test(navigator.userAgent);
    let installEvent = null;

    window.addEventListener('beforeinstallprompt', (e) => {
        e.preventDefault();
        installEvent = e;
    });

    async function showInstallPrompt() {
        if (installEvent) {
            const r = await installEvent.prompt();
            installEvent = null;
            return r?.outcome === 'accepted' ? 'installed' : 'dismissed';
        }
        if (window.matchMedia('(display-mode: standalone)').matches) return 'installed';
        if (isiOS) return 'ios'; // iOS precisa do passo manual (Compartilhar → Tela de Início)
        return 'unavailable';
    }

    function cacheUrls(urls) {
        if (!navigator.serviceWorker?.controller) return Promise.resolve(false);
        return new Promise((resolve) => {
            navigator.serviceWorker.controller.postMessage({ type: 'CACHE_URLS', urls });
            const onMsg = (ev) => {
                if (ev.data?.type === 'CACHE_DONE') {
                    navigator.serviceWorker.removeEventListener('message', onMsg);
                    resolve(true);
                }
            };
            navigator.serviceWorker.addEventListener('message', onMsg);
            setTimeout(() => resolve(false), 30000);
        });
    }

    // Tudo em um: tenta instalar e sempre faz o cache offline
    async function installAndCache(urls) {
        const installResult = await showInstallPrompt();
        await cacheUrls(urls);
        return installResult; // 'installed' | 'dismissed' | 'ios' | 'unavailable'
    }

    return { installAndCache, showInstallPrompt, cacheUrls };
})();
