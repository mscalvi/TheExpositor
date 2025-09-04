// Blazor PWA SW (produção)
self.importScripts('./service-worker-assets.js');
const CACHE = 'contajunsta-v1';
const ASSETS = self.assetsManifest.assets.map(a => new URL(a.url, self.location).toString()).concat(['./', 'index.html']);

self.addEventListener('install', e => {
    e.waitUntil((async () => {
        const cache = await caches.open(CACHE);
        await cache.addAll(ASSETS);
        await self.skipWaiting();
    })());
});

self.addEventListener('activate', e => {
    e.waitUntil((async () => {
        const keys = await caches.keys();
        await Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)));
        await self.clients.claim();
    })());
});

self.addEventListener('fetch', e => {
    const req = e.request;
    if (req.method !== 'GET') return;
    e.respondWith((async () => {
        const cache = await caches.open(CACHE);
        const cached = await cache.match(req);
        if (cached) return cached;
        try {
            const resp = await fetch(req);
            // Cache apenas navegação/estáticos
            if (resp.ok && (req.destination === 'document' || req.destination === 'script' || req.destination === 'style' || req.destination === 'image' || req.destination === 'font')) {
                cache.put(req, resp.clone());
            }
            return resp;
        } catch {
            return cached ?? Response.error();
        }
    })());
});
