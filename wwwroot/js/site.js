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






