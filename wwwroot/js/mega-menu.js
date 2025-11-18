(function () {

    const mm = document.getElementById("mega-menu");
    if (!mm) return;

    const rows = mm.querySelectorAll(".mm-cats");
    const panels = mm.querySelectorAll(".mm-panel");

    // -------------------------
    // GET TARGET FROM URL (/k/vin, /k/vona...)
    // -------------------------
    function getTarget() {
        const segs = location.pathname.split("/").filter(Boolean);
        const idx = segs.indexOf("k");
        if (idx !== -1 && segs[idx + 1]) return segs[idx + 1];
        return "vona"; // дефолт
    }

    function showRow(target) {
        rows.forEach(r => r.classList.toggle("active", r.dataset.row === target));
        panels.forEach(p => p.classList.remove("active"));
    }

    showRow(getTarget());

    // -------------------------
    // CLICK ON TOP MAIN CATEGORY
    // -------------------------
    document.querySelectorAll("#header-white .main-targets a").forEach(a => {
        a.addEventListener("click", e => {
            const target = a.getAttribute("href").split("/").pop();
            showRow(target);
        });
    });

    // -------------------------
    // CLICK ON SUBCATEGORY (Одяг/Взуття)
    // -------------------------
    mm.querySelectorAll(".mm-cats [data-panel]").forEach(li => {
        const id = li.getAttribute("data-panel");

        li.addEventListener("mouseenter", () => {
            panels.forEach(p => p.classList.toggle("active", p.dataset.panel === id));
        });

        li.addEventListener("click", () => {
            panels.forEach(p => p.classList.toggle("active", p.dataset.panel === id));
        });
    });

    // hide when mouse leaves
    mm.addEventListener("mouseleave", () => {
        panels.forEach(p => p.classList.remove("active"));
    });

})();
