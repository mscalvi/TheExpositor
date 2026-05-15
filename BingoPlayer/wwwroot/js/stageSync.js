// wwwroot/js/stageSync.js

window.stageSync = (function () {

    // Canal entre abas/janelas
    const bc =
        ('BroadcastChannel' in window)
            ? new BroadcastChannel('bingo-stage')
            : null;

    // Envia mensagem
    function send(mode, id) {

        const msg = {
            mode: mode,
            id: id
        };

        // BroadcastChannel (rápido)
        if (bc) {
            bc.postMessage(msg);
        }

        // Fallback via localStorage
        try {

            localStorage.setItem(
                'bingo-stage',
                JSON.stringify({
                    ...msg,
                    ts: Date.now()
                })
            );

        } catch (err) {

            console.warn('Erro localStorage:', err);
        }
    }

    // Escuta mensagens
    function subscribe(dotnetRef) {

        // BroadcastChannel
        if (bc) {

            bc.onmessage = (e) => {

                try {

                    dotnetRef.invokeMethodAsync(
                        'OnStageMessage',
                        e.data.mode,
                        e.data.id || 0
                    );

                } catch (err) {

                    console.warn('Erro BroadcastChannel:', err);
                }
            };
        }

        // Fallback localStorage
        window.addEventListener('storage', (e) => {

            if (e.key !== 'bingo-stage' || !e.newValue)
                return;

            try {

                const data = JSON.parse(e.newValue);

                dotnetRef.invokeMethodAsync(
                    'OnStageMessage',
                    data.mode,
                    data.id || 0
                );

            } catch (err) {

                console.warn('Erro storage event:', err);
            }
        });
    }

    // API pública
    return {
        send,
        subscribe
    };

})();