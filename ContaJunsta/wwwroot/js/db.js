(() => {
    const DB_NAME = "contajunsta-db";
    const DB_VERSION = 2; // camelCase
    const STORES = { events: "events", persons: "persons", bills: "bills" };
    let dbPromise;

    const reqP = (req) => new Promise((ok, err) => { req.onsuccess = () => ok(req.result); req.onerror = () => err(req.error); });
    const txDone = (tx) => new Promise((ok, err) => { tx.oncomplete = () => ok(); tx.onerror = () => err(tx.error); tx.onabort = () => err(tx.error); });

    function getDb() {
        if (!dbPromise) {
            dbPromise = new Promise((ok, err) => {
                const req = indexedDB.open(DB_NAME, DB_VERSION);
                req.onupgradeneeded = () => {
                    const db = req.result;
                    if (db.objectStoreNames.contains(STORES.events)) db.deleteObjectStore(STORES.events);
                    if (db.objectStoreNames.contains(STORES.persons)) db.deleteObjectStore(STORES.persons);
                    if (db.objectStoreNames.contains(STORES.bills)) db.deleteObjectStore(STORES.bills);

                    db.createObjectStore(STORES.events, { keyPath: "id" });
                    const p = db.createObjectStore(STORES.persons, { keyPath: "id" });
                    p.createIndex("eventId", "eventId", { unique: false });

                    const b = db.createObjectStore(STORES.bills, { keyPath: "id" });
                    b.createIndex("eventId", "eventId", { unique: false });
                    b.createIndex("responsiblePersonId", "responsiblePersonId", { unique: false });
                };
                req.onsuccess = () => ok(req.result);
                req.onerror = () => err(req.error);
            });
        }
        return dbPromise;
    }

    // normalizadores
    const normEvent = (e) => ({
        id: e?.id ?? e?.Id,
        name: e?.name ?? e?.Name ?? "",
        status: e?.status ?? e?.Status ?? "Open",
        createdAt: e?.createdAt ?? e?.CreatedAt ?? new Date().toISOString(),
        closedAt: e?.closedAt ?? e?.ClosedAt ?? null
    });
    const normPerson = (p) => ({
        id: p?.id ?? p?.Id, eventId: p?.eventId ?? p?.EventId, name: p?.name ?? p?.Name ?? ""
    });
    const normBill = (b) => ({
        id: b?.id ?? b?.Id,
        eventId: b?.eventId ?? b?.EventId,
        description: b?.description ?? b?.Description ?? "",
        cents: b?.cents ?? b?.Cents ?? 0,
        responsiblePersonId: b?.responsiblePersonId ?? b?.ResponsiblePersonId ?? "",
        participantIds: b?.participantIds ?? b?.ParticipantIds ?? [],
        createdAt: b?.createdAt ?? b?.CreatedAt ?? new Date().toISOString()
    });

    // EVENTS
    async function addEvent(evt) { const db = await getDb(); const tx = db.transaction([STORES.events], "readwrite"); await reqP(tx.objectStore(STORES.events).put(normEvent(evt))); await txDone(tx); return true; }
    async function updateEvent(evt) { const db = await getDb(); const tx = db.transaction([STORES.events], "readwrite"); await reqP(tx.objectStore(STORES.events).put(normEvent(evt))); await txDone(tx); return true; }
    async function getAllEvents(onlyOpen) {
        const db = await getDb(); const tx = db.transaction([STORES.events], "readonly");
        const list = await reqP(tx.objectStore(STORES.events).getAll()); await txDone(tx);
        const filtered = onlyOpen ? list.filter(e => (e.status ?? "Open") === "Open") : list;
        return filtered.sort((a, b) => String(a.createdAt || "").localeCompare(String(b.createdAt || "")));
    }

    // PERSONS
    async function addPersons(persons) {
        if (!persons?.length) return true;
        const db = await getDb(); const tx = db.transaction([STORES.persons], "readwrite"); const store = tx.objectStore(STORES.persons);
        for (const p of persons) { await reqP(store.put(normPerson(p))); }
        await txDone(tx); return true;
    }
    async function getPersonsByEvent(eventId) {
        const db = await getDb(); const tx = db.transaction([STORES.persons], "readonly");
        const idx = tx.objectStore(STORES.persons).index("eventId"); const res = await reqP(idx.getAll(IDBKeyRange.only(eventId)));
        await txDone(tx); return res;
    }

    // BILLS
    async function addBill(bill) { const db = await getDb(); const tx = db.transaction([STORES.bills], "readwrite"); await reqP(tx.objectStore(STORES.bills).put(normBill(bill))); await txDone(tx); return true; }
    async function getBillsByEvent(eventId) {
        const db = await getDb(); const tx = db.transaction([STORES.bills], "readonly");
        const idx = tx.objectStore(STORES.bills).index("eventId"); const res = await reqP(idx.getAll(IDBKeyRange.only(eventId)));
        await txDone(tx); return res;
    }
    async function deleteBill(billId) {
        const db = await getDb(); const tx = db.transaction([STORES.bills], "readwrite");
        await reqP(tx.objectStore(STORES.bills).delete(billId)); await txDone(tx); return true;
    }

    async function deleteEventCascade(eventId) {
        const db = await getDb();
        const tx = db.transaction([STORES.events, STORES.persons, STORES.bills], "readwrite");
        tx.objectStore(STORES.events).delete(eventId);

        const pIdx = tx.objectStore(STORES.persons).index("eventId");
        const persons = await reqP(pIdx.getAll(IDBKeyRange.only(eventId)));
        for (const p of persons) tx.objectStore(STORES.persons).delete(p.id);

        const bIdx = tx.objectStore(STORES.bills).index("eventId");
        const bills = await reqP(bIdx.getAll(IDBKeyRange.only(eventId)));
        for (const b of bills) tx.objectStore(STORES.bills).delete(b.id);

        await txDone(tx); return true;
    }

    window.ContaJunstaDb = {
        addEvent, updateEvent, getAllEvents,
        addPersons, getPersonsByEvent,
        addBill, getBillsByEvent, deleteBill,
        deleteEventCascade
    };
})();
