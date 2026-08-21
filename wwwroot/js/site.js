// Cierra el menú móvil al elegir una opción de navegación.
document.addEventListener("DOMContentLoaded", function () {
    var menu = document.getElementById("navMenu");
    if (!menu) return;

    menu.querySelectorAll(".nav-link").forEach(function (link) {
        link.addEventListener("click", function () {
            if (menu.classList.contains("show")) {
                new bootstrap.Collapse(menu).hide();
            }
        });
    });
});
