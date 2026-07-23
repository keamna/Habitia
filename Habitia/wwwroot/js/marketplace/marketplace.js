// =====================================================
// MARKETPLACE - HABITIA
// =====================================================

document.addEventListener("DOMContentLoaded", function () {
    inicializarSelectorTipo();
    inicializarSubidaImagenes();
    inicializarGaleriaDetalle();
    inicializarConfirmaciones();
    inicializarFotoPerfil();
    inicializarRating();
    inicializarBuscador();
});


// ===== Mostrar/ocultar campos según tipo =====
function inicializarSelectorTipo() {

    var radios = document.querySelectorAll("input[name='Tipo']");
    if (radios.length === 0) return;

    var camposProducto = document.querySelector(".campos-producto");
    var camposServicio = document.querySelector(".campos-servicio");

    function actualizar() {
        var sel = document.querySelector("input[name='Tipo']:checked");
        if (!sel) return;

        var esProducto = sel.value === "1";

        if (camposProducto) camposProducto.classList.toggle("visible", esProducto);
        if (camposServicio) camposServicio.classList.toggle("visible", !esProducto);
    }

    radios.forEach(function (r) { r.addEventListener("change", actualizar); });
    actualizar();
}


// ===== Preview de imágenes al subir =====
function inicializarSubidaImagenes() {

    var drop = document.getElementById("imagenDrop");
    var input = document.getElementById("inputImagenes");
    var previews = document.getElementById("imagenPreviews");

    if (!drop || !input || !previews) return;

    drop.addEventListener("click", function () { input.click(); });

    input.addEventListener("change", function () {
        previews.innerHTML = "";

        Array.prototype.forEach.call(input.files, function (archivo) {
            if (!archivo.type.startsWith("image/")) return;

            var lector = new FileReader();
            lector.onload = function (e) {
                var div = document.createElement("div");
                div.className = "mk-preview";

                var img = document.createElement("img");
                img.src = e.target.result;

                div.appendChild(img);
                previews.appendChild(div);
            };
            lector.readAsDataURL(archivo);
        });
    });
}


// ===== Galería en detalle =====
function inicializarGaleriaDetalle() {

    var principal = document.getElementById("imagenPrincipal");
    var miniaturas = document.querySelectorAll(".mk-mini");

    if (!principal || miniaturas.length === 0) return;

    miniaturas.forEach(function (mini) {
        mini.addEventListener("click", function () {
            principal.src = this.src;
            miniaturas.forEach(function (m) { m.classList.remove("activa"); });
            this.classList.add("activa");
        });
    });
}


// ===== Confirmación al eliminar / finalizar =====
function inicializarConfirmaciones() {
    document.querySelectorAll("form[data-confirmar]").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            var mensaje = form.getAttribute("data-confirmar");
            if (!confirm(mensaje)) e.preventDefault();
        });
    });
}


// ===== Subir foto de perfil =====
function inicializarFotoPerfil() {

    var visible = document.getElementById("inputFotoPerfil");
    var real = document.getElementById("fotoPerfilReal");
    var form = document.getElementById("formFotoPerfil");

    if (!visible || !real || !form) return;

    visible.addEventListener("change", function () {
        if (visible.files.length === 0) return;
        real.files = visible.files;
        form.submit();
    });
}


// ===== Estrellas del formulario de reseña =====
function inicializarRating() {

    var contenedor = document.getElementById("mkRating");
    if (!contenedor) return;

    var estrellas = contenedor.querySelectorAll(".mk-rating-star");

    estrellas.forEach(function (star) {
        star.addEventListener("click", function () {
            var valor = this.getAttribute("data-valor");
            var radio = document.getElementById("star" + valor);
            if (radio) radio.checked = true;
        });
    });
}


// =====================================================
// BUSCADOR CON SUGERENCIAS
// =====================================================
function inicializarBuscador() {

    var input = document.getElementById("inputBuscar");
    var panel = document.getElementById("mkSugerencias");

    if (!input || !panel) return;

    var temporizador = null;


    input.addEventListener("input", function () {

        var texto = input.value.trim();

        clearTimeout(temporizador);

        if (texto.length < 2) {
            cerrar();
            return;
        }

        // Espera un poquito para no consultar en cada tecla
        temporizador = setTimeout(function () {
            buscar(texto);
        }, 250);
    });


    input.addEventListener("focus", function () {
        if (input.value.trim().length >= 2 && panel.innerHTML !== "")
            panel.classList.add("visible");
    });


    // Cierra al hacer clic fuera
    document.addEventListener("click", function (e) {
        if (!panel.contains(e.target) && e.target !== input)
            cerrar();
    });


    // Cierra con Escape
    input.addEventListener("keydown", function (e) {
        if (e.key === "Escape") cerrar();
    });



    function cerrar() {
        panel.classList.remove("visible");
    }


    function buscar(texto) {

        fetch("/Residente/Marketplace/Sugerencias?q=" + encodeURIComponent(texto))
            .then(function (r) { return r.json(); })
            .then(function (datos) { pintar(datos, texto); })
            .catch(function () { cerrar(); });
    }


    function pintar(datos, texto) {

        panel.innerHTML = "";


        if (!datos || datos.length === 0) {
            panel.innerHTML =
                '<div class="mk-sug-vacio">' +
                '<i class="bi bi-search"></i> ' +
                'No hay artículos disponibles con este nombre.' +
                '</div>';

            panel.classList.add("visible");
            return;
        }


        var publicaciones = datos.filter(function (d) { return d.tipo === "publicacion"; });
        var categorias = datos.filter(function (d) { return d.tipo === "categoria"; });
        var residentes = datos.filter(function (d) { return d.tipo === "residente"; });


        if (publicaciones.length > 0) {
            agregarTitulo("Publicaciones");
            publicaciones.forEach(function (d) {
                agregarItem(
                    "/Residente/Marketplace/Detalle/" + d.id,
                    '<i class="bi bi-box-seam mk-sug-icono"></i>',
                    d.texto,
                    d.extra
                );
            });
        }


        if (categorias.length > 0) {
            agregarTitulo("Categorías");
            categorias.forEach(function (d) {
                agregarItem(
                    "/Residente/Marketplace?categoria=" + d.id,
                    '<i class="bi bi-tags mk-sug-icono"></i>',
                    d.texto,
                    d.extra
                );
            });
        }


        if (residentes.length > 0) {
            agregarTitulo("Residentes");
            residentes.forEach(function (d) {

                var avatar = d.foto
                    ? '<img src="' + d.foto + '" class="mk-sug-foto" alt="" />'
                    : '<i class="bi bi-person-circle mk-sug-icono"></i>';

                agregarItem(
                    "/Residente/Marketplace?vendedor=" + encodeURIComponent(d.id),
                    avatar,
                    d.texto,
                    "Ver sus publicaciones"
                );
            });
        }


        // Opción para buscar el texto tal cual
        agregarItem(
            "/Residente/Marketplace?busqueda=" + encodeURIComponent(texto),
            '<i class="bi bi-search mk-sug-icono"></i>',
            'Buscar "' + texto + '"',
            "En todo el Marketplace",
            true
        );


        panel.classList.add("visible");
    }


    function agregarTitulo(texto) {
        var div = document.createElement("div");
        div.className = "mk-sug-titulo";
        div.textContent = texto;
        panel.appendChild(div);
    }


    function agregarItem(url, iconoHtml, titulo, extra, esBuscar) {

        var a = document.createElement("a");
        a.href = url;
        a.className = "mk-sug-item" + (esBuscar ? " mk-sug-buscar" : "");

        a.innerHTML =
            iconoHtml +
            '<div class="mk-sug-texto">' +
            '<span class="mk-sug-nombre"></span>' +
            '<span class="mk-sug-extra"></span>' +
            '</div>';

        a.querySelector(".mk-sug-nombre").textContent = titulo;
        a.querySelector(".mk-sug-extra").textContent = extra || "";

        panel.appendChild(a);
    }
}