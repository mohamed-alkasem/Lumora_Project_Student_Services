/* My Favorites page: heart click removes the card from the grid
   (the visitor is always a logged-in student here). */
(function () {
    "use strict";

    var page = document.getElementById("favPage");
    if (!page) {
        return;
    }

    var toggleUrl = page.dataset.toggleUrl;

    function antiForgeryToken() {
        var input = page.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : "";
    }

    page.addEventListener("click", function (e) {
        var heart = e.target.closest(".uni-card-heart");
        if (!heart) {
            return;
        }

        var card = heart.closest(".uni-card");
        var body = new URLSearchParams();
        body.set("universityId", heart.dataset.universityId);

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
                if (!data.isFavorite && card) {
                    card.remove();
                }

                var countBadge = document.getElementById("favPageCount");
                if (countBadge) {
                    countBadge.textContent = data.count;
                }

                if (data.count === 0) {
                    var empty = document.getElementById("favEmpty");
                    if (empty) {
                        empty.classList.remove("d-none");
                    }
                }
            })
            .catch(function () { /* keep the card on failure */ });
    });
})();
