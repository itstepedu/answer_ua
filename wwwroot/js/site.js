// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("click", function (e) {

    // Натиснув на TargetCategory?
    if (e.target.classList.contains("menu-answear-target")) {

        e.preventDefault();

        const id = e.target.dataset.id;

        fetch(`/Menu?activeTargetCategoryId=${id}`)
            .then(r => r.text())
            .then(html => {
                document.querySelector("#mega-menu").outerHTML = html;
            });
    }
});


//ЦЕ ДЛЯ МІЙ ВИБІР
/*const checkAll = document.getElementById("checkAll");
const checkAllText = document.getElementById("checkAllText");
const wishActions = document.getElementById("wishActions");

checkAll.addEventListener("change", () => {
    let checkboxes = document.querySelectorAll(".wish-checkbox");

    checkboxes.forEach(ch => ch.checked = checkAll.checked);

    if (checkAll.checked) {
        checkAllText.innerText = "Зняти всі відмітки";
        wishActions.style.display = "flex";
    } else {
        checkAllText.innerText = "Зазначити все";
        wishActions.style.display = "none";
    }
});*/

document.getElementById("sortSelect").addEventListener("change", function () {
    window.location = "/Wish/Index?sort=" + this.value;
});




// Окремо — якщо користувач тисне галочки вручну:
/*document.querySelectorAll(".wish-checkbox").forEach(cb => {
    cb.addEventListener("change", () => {
        let anyChecked =
            [...document.querySelectorAll(".wish-checkbox")]
                .some(x => x.checked);

        if (anyChecked) {
            wishActions.style.display = "flex";
            checkAllText.innerText = "Зняти всі відмітки";
        } else {
            wishActions.style.display = "none";
            checkAll.checked = false;
            checkAllText.innerText = "Зазначити все";
        }
    });
});*/