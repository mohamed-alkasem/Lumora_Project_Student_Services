/* Universities directory: AJAX filtering + favorites.
   No inline JS lives in the .cshtml files — this module reads its
   endpoints and auth state from data-* attributes on #uniPage. */
(function () {
    "use strict";

    var page = document.getElementById("uniPage");
    if (!page) {
        return;
    }

    var listContainer = document.getElementById("uniList");
    var searchInput = document.getElementById("uniSearch");
    var searchClear = document.getElementById("uniSearchClear");
    var sortSelect = document.getElementById("uniSort");
    var tuitionRange = document.getElementById("tuitionRange");
    var tuitionLabel = document.getElementById("tuitionLabel");
    var resultsCount = document.getElementById("resultsCount");
    var favCount = document.getElementById("favCount");
    var resetBtn = document.getElementById("filtersReset");

    var isStudent = page.dataset.isStudent === "true";
    var listUrl = page.dataset.listUrl;
    var toggleUrl = page.dataset.toggleUrl;
    var pageUrl = page.dataset.pageUrl;
    var loginUrl = page.dataset.loginUrl;

    var TUITION_MAX = parseInt(tuitionRange.max, 10);

    /* Current filter state; initialised from the server-rendered controls. */
    var state = {
        page: 1,
        type: getActiveData(".uni-type-btn", "type"),
        cityId: getActiveData(".uni-city-chip", "cityId"),
        maxTuition: parseInt(tuitionRange.value, 10)
    };

    function getActiveData(selector, key) {
        var active = document.querySelector(selector + ".is-active");
        return active ? active.dataset[key] || "" : "";
    }

    function antiForgeryToken() {
        var input = page.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : "";
    }

    function buildParams() {
        var params = new URLSearchParams();
        var term = searchInput.value.trim();

        if (term) { params.set("SearchTerm", term); }
        if (state.type) { params.set("Type", state.type); }
        if (state.cityId) { params.set("CityId", state.cityId); }
        if (state.maxTuition < TUITION_MAX) { params.set("MaxAnnualTuitionUsd", state.maxTuition); }
        if (sortSelect.value !== "TopRated") { params.set("SortBy", sortSelect.value); }
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
                if (seq !== refreshSeq) { return; } // stale response, a newer request is in flight
                listContainer.innerHTML = html;
                listContainer.classList.remove("is-loading");

                var root = listContainer.querySelector(".uni-list-root");
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

    /* ---------- Search ---------- */
    searchInput.addEventListener("input", debouncedRefresh);

    searchClear.addEventListener("click", function () {
        if (searchInput.value) {
            searchInput.value = "";
            state.page = 1;
            refresh();
        }
        searchInput.focus();
    });

    /* ---------- Type buttons (single-select) ---------- */
    document.querySelectorAll(".uni-type-btn").forEach(function (btn) {
        btn.addEventListener("click", function () {
            document.querySelectorAll(".uni-type-btn").forEach(function (b) {
                b.classList.remove("is-active");
            });
            btn.classList.add("is-active");
            state.type = btn.dataset.type;
            state.page = 1;
            refresh();
        });
    });

    /* ---------- City chips (toggleable single-select) ---------- */
    document.querySelectorAll(".uni-city-chip").forEach(function (chip) {
        chip.addEventListener("click", function () {
            var wasActive = chip.classList.contains("is-active");
            document.querySelectorAll(".uni-city-chip").forEach(function (c) {
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

    /* ---------- Tuition slider ---------- */
    function renderTuitionLabel() {
        var value = parseInt(tuitionRange.value, 10);
        tuitionLabel.textContent =
            "$" + value.toLocaleString() + (value >= TUITION_MAX ? "+" : "");
    }

    tuitionRange.addEventListener("input", function () {
        renderTuitionLabel();
        state.maxTuition = parseInt(tuitionRange.value, 10);
        debouncedRefresh();
    });

    renderTuitionLabel();

    /* ---------- Sort ---------- */
    sortSelect.addEventListener("change", function () {
        state.page = 1;
        refresh();
    });

    /* ---------- Reset ---------- */
    resetBtn.addEventListener("click", function () {
        searchInput.value = "";
        sortSelect.value = "TopRated";
        tuitionRange.value = TUITION_MAX;
        renderTuitionLabel();

        document.querySelectorAll(".uni-city-chip").forEach(function (c) {
            c.classList.remove("is-active");
        });
        document.querySelectorAll(".uni-type-btn").forEach(function (b) {
            b.classList.toggle("is-active", b.dataset.type === "");
        });

        state = { page: 1, type: "", cityId: "", maxTuition: TUITION_MAX };
        refresh();
    });

    /* ---------- Pagination (delegated: grid is replaced by AJAX) ---------- */
    listContainer.addEventListener("click", function (e) {
        var pageBtn = e.target.closest(".uni-page-btn");
        if (pageBtn && !pageBtn.disabled) {
            state.page = parseInt(pageBtn.dataset.page, 10);
            refresh();
            listContainer.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    });

    /* ---------- Favorites (delegated) ---------- */
    listContainer.addEventListener("click", function (e) {
        var heart = e.target.closest(".uni-card-heart");
        if (!heart) {
            return;
        }

        if (!isStudent) {
            window.location.href = loginUrl;
            return;
        }

        var universityId = heart.dataset.universityId;
        var body = new URLSearchParams();
        body.set("universityId", universityId);

        fetch(toggleUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "RequestVerificationToken": antiForgeryToken()
            },
            body: body.toString()
        })
            .then(function (res) {
                if (!res.ok) { throw new Error("toggle failed"); }
                return res.json();
            })
            .then(function (data) {
                heart.classList.toggle("is-fav", data.isFavorite);
                heart.setAttribute("aria-pressed", String(data.isFavorite));
                if (favCount) {
                    favCount.textContent = data.count;
                }
            })
            .catch(function () { /* leave the heart untouched on failure */ });
    });
})();
