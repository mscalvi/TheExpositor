// wwwroot/js/stageSync.js
window.stageSync = (function () {
    const bc = ('BroadcastChannel' in window) ? new BroadcastChannel('bingo-stage') : null;

    function send(mode, id) {
        const msg = { mode, id };
        if (bc) bc.postMessage(msg);
        try { localStorage.setItem('bingo-stage', JSON.stringify({ ...msg, ts: Date.now() })); } catch { }
    }

    function subscribe(dotnetRef) {
        if (bc) bc.onmessage = (e) => dotnetRef.invokeMethodAsync('OnStageMessage', e.data.mode, e.data.id || 0);
        window.addEventListener('storage', (e) => {
            if (e.key === 'bingo-stage' && e.newValue) {
                const data = JSON.parse(e.newValue);
                dotnetRef.invokeMethodAsync('OnStageMessage', data.mode, data.id || 0);
            }
        });
    }

    return { send, subscribe };
})();
