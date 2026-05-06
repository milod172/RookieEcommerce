document.addEventListener('htmx:beforeHistoryUpdate', function (event) {
    let url = new URL(event.detail.history.path, location.origin);
    for (let [k, v] of [...url.searchParams])
        if (!v) url.searchParams.delete(k);
    event.detail.history.path = url.pathname + url.search;
});