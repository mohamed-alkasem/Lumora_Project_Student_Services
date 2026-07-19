/* Housing directory: AJAX filtering (city + max price + pagination).
   Reads its endpoints from data-* attributes on #housingPage — no inline JS. */
(function () {
    "use strict";

    var page = document.getElementById("housingPage");
    if (!page) {
        return;
    }

    var listContainer = document.getElementById("houList");
    var priceRange = document.getElementById("priceRange");
    var priceLabel = document.getElementById("priceLabel");
    var resultsCount = document.getElementById("resultsCount");
    var resetBtn = document.getElementById("filtersReset");

    var listUrl = page.dataset.listUrl;
    var pageUrl = page.dataset.pageUrl;
    var PRICE_MAX = parseInt(priceRange.max, 10);

    var activeChip = document.querySelector(".hou-city-chip.is-active");
    var state = {
        page: 1,
        cityId: activeChip ? activeChip.dataset.cityId : "",
        maxPrice: parseInt(priceRange.value, 10)
    };

    function buildParams() {
        var params = new URLSearchParams();
        if (state.cityId) { params.set("CityId", state.cityId); }
        if (state.maxPrice < PRICE_MAX) { params.set("MaxPricePerMonthUsd", state.maxPrice); }
        if (state.page > 1) { params.set("Page", state.page); }
        return params;
    }

    var refreshSeq = 0;
    function refresh() {
        var seq = ++refreshSeq;
        var params = buildParams();

        listContainer.classList.add("is-loading");

        fetch(listUrl + "?" + params.toString(), {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(function (res) { return res.text(); })
            .then(function (html) {
                if (seq !== refreshSeq) { return; }
                listContainer.innerHTML = html;
                listContainer.classList.remove("is-loading");

                var root = listContainer.querySelector(".hou-list-root");
                if (root && resultsCount) {
                    resultsCount.textContent = root.dataset.total;
                }

                var query = params.toString();
                history.replaceState(null, "", query ? pageUrl + "?" + query : pageUrl);
            })
            .catch(function () {
                listContainer.classList.remove("is-loading");
            });
    }

    function debounce(fn, delay) {
        var timer = null;
        return function () {
            clearTimeout(timer);
            timer = setTimeout(fn, delay);
        };
    }

    var debouncedRefresh = debounce(function () {
        state.page = 1;
        refresh();
    }, 300);

    /* ---------- City chips (toggleable single-select) ---------- */
    document.querySelectorAll(".hou-city-chip").forEach(function (chip) {
        chip.addEventListener("click", function () {
            var wasActive = chip.classList.contains("is-active");
            document.querySelectorAll(".hou-city-chip").forEach(function (c) {
                c.classList.remove("is-active");
            });
            if (!wasActive) {
                chip.classList.add("is-active");
            }
            state.cityId = wasActive ? "" : chip.dataset.cityId;
            state.page = 1;
            refresh();
        });
    });

    /* ---------- Price slider ---------- */
    function renderPriceLabel() {
        var value = parseInt(priceRange.value, 10);
        priceLabel.textContent =
            "$" + value.toLocaleString() + (value >= PRICE_MAX ? "+" : "");
    }

    priceRange.addEventListener("input", function () {
        renderPriceLabel();
        state.maxPrice = parseInt(priceRange.value, 10);
        debouncedRefresh();
    });

    renderPriceLabel();

    /* ---------- Reset ---------- */
    resetBtn.addEventListener("click", function () {
        priceRange.value = PRICE_MAX;
        renderPriceLabel();
        document.querySelectorAll(".hou-city-chip").forEach(function (c) {
            c.classList.remove("is-active");
        });
        state = { page: 1, cityId: "", maxPrice: PRICE_MAX };
        refresh();
    });

    /* ---------- Pagination (delegated) ---------- */
    listContainer.addEventListener("click", function (e) {
        var pageBtn = e.target.closest(".hou-page-btn");
        if (pageBtn && !pageBtn.disabled) {
            state.page = parseInt(pageBtn.dataset.page, 10);
            refresh();
            listContainer.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    });
})();
