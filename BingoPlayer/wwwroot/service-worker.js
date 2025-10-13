const CACHE = 'bingo-cache-v1';
const CORE_ASSETS = [
    '/', 'index.html', 'manifest.webmanifest'
    // adicione css/js principais se quiser
];

self.addEventListener('install', (e) => {
    e.waitUntil(caches.open(CACHE).then(c => c.addAll(CORE_ASSETS)));
    self.skipWaiting();
});

self.addEventListener('activate', (e) => {
    e.waitUntil(self.clients.claim());
});

// network-first para JSON, cache-first para imagens
self.addEventListener('fetch', (e) => {
    const url = new URL(e.request.url);
    if (url.pathname.startsWith('/images/')) {
        e.respondWith(
            caches.match(e.request).then(resp => resp || fetch(e.request).then(r => {
                const copy = r.clone();
                caches.open(CACHE).then(c => c.put(e.request, copy));
                return r;
            }))
        );
    }
});

// mensagem para pré-cachear URLs arbitrárias (imagens, json, etc.)
self.addEventListener('message', async (e) => {
    const { type, urls } = e.data || {};
    if (type === 'CACHE_URLS' && Array.isArray(urls)) {
        const cache = await caches.open(CACHE);
        await cache.addAll(urls);
        // resposta opcional
        const clientsList = await self.clients.matchAll();
        clientsList.forEach(c => c.postMessage({ type: 'CACHE_DONE', count: urls.length }));
    }
});
