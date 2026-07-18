document.addEventListener("DOMContentLoaded", function () {


    const pasos = document.querySelectorAll(".form-paso");
    const steps = document.querySelectorAll(".step-item");

    const btnPaso1 = document.getElementById("btnPaso1");
    const btnPaso2 = document.getElementById("btnPaso2");
    const btnPaso3 = document.getElementById("btnPaso3");

    const registerForm = document.getElementById("registerForm");

    const progressBar = document.getElementById("progressBar");

    let tipoRelacion = "";
    let tipoVivienda = "";


    // ==========================
    // CAMBIO DE PASOS
    // ==========================
    function mostrarPaso(numero) {

        pasos.forEach(p => p.classList.remove("activo"));
        steps.forEach(s => s.classList.remove("activo"));

        const paso = document.getElementById(`paso${numero}`);
        const step = document.getElementById(`step${numero}`);

        if (paso) paso.classList.add("activo");
        if (step) step.classList.add("activo");

        if (progressBar) {
            progressBar.style.width = `${numero * 33.3}%`;
        }
    }


    // ==========================
    // PASO 1
    // ==========================
    if (btnPaso1) {

        btnPaso1.addEventListener("click", function () {

            let valido = true;

            document.querySelectorAll(".field-error").forEach(e => e.innerHTML = "");

            const nombre = document.getElementById("TC_NombreCompleto").value.trim();
            const correo = document.getElementById("Email").value.trim();
            const password = document.getElementById("Password").value;
            const confirm = document.getElementById("ConfirmPassword").value;

            if (nombre === "") {
                document.getElementById("errNombre").innerHTML = "Ingrese su nombre";
                valido = false;
            }

            if (correo === "") {
                document.getElementById("errEmail").innerHTML = "Ingrese su correo";
                valido = false;
            }

            if (password === "") {
                document.getElementById("errPassword").innerHTML = "Ingrese una contraseña";
                valido = false;
            }

            if (confirm !== password) {
                document.getElementById("errConfirmPassword").innerHTML = "Las contraseñas no coinciden";
                valido = false;
            }

            if (valido) {
                mostrarPaso(2);
            }

        });
    }


    // ==========================
    // PASO 2
    // ==========================
    document.querySelectorAll(".card-relacion").forEach(card => {

        card.addEventListener("click", function () {

            document.querySelectorAll(".card-relacion").forEach(c => c.classList.remove("selected"));

            this.classList.add("selected");

            tipoRelacion = this.dataset.valor;

            const hidden = document.getElementById("TC_TipoRelacion");

            if (tipoRelacion === "Propietario") {
                hidden.value = 1;
            } else {
                hidden.value = 2;
            }

            const vive = document.getElementById("viveAhiSection");

            if (tipoRelacion === "Propietario") {
                vive.style.display = "block";
                document.getElementById("TB_ViveAhi").value = "";
            } else {
                vive.style.display = "none";
                document.getElementById("TB_ViveAhi").value = "false";
            }

        });
    });


    // ==========================
    // VIVE AHI
    // ==========================
    document.querySelectorAll(".toggle-option").forEach(opcion => {

        opcion.addEventListener("click", function () {

            document.querySelectorAll(".toggle-option").forEach(o => o.classList.remove("selected"));

            this.classList.add("selected");

            document.getElementById("TB_ViveAhi").value = this.dataset.valor;

        });
    });


    // ==========================
    // BOTON PASO 2
    // ==========================
    if (btnPaso2) {

        btnPaso2.addEventListener("click", function () {

            document.getElementById("errRelacion").innerHTML = "";
            document.getElementById("errViveAhi").innerHTML = "";

            if (tipoRelacion === "") {
                document.getElementById("errRelacion").innerHTML = "Seleccione una opción";
                return;
            }

            if (tipoRelacion === "Propietario") {

                const vive = document.getElementById("TB_ViveAhi").value;

                if (vive === "") {
                    document.getElementById("errViveAhi").innerHTML = "Seleccione una opción";
                    return;
                }
            }

            mostrarPaso(3);
        });
    }


    // ==========================
    // PASO 3
    // ==========================
    document.querySelectorAll(".card-vivienda").forEach(card => {

        card.addEventListener("click", function () {

            document.querySelectorAll(".card-vivienda").forEach(c => c.classList.remove("selected"));

            this.classList.add("selected");

            tipoVivienda = this.dataset.valor;

            document.getElementById("TC_TipoVivienda").value = tipoVivienda;

            document.getElementById("errTipoVivienda").innerHTML = "";

            cargarViviendas(tipoVivienda);

        });
    });


    async function cargarViviendas(tipo) {

        const select = document.getElementById("TN_ViviendaId");
        const wrapper = document.getElementById("viviendaSelectWrapper");

        select.innerHTML = `<option value="">Seleccione una vivienda</option>`;

        try {
            const respuesta = await fetch(`/Account/ObtenerViviendas?tipo=${tipo}`);
            const viviendas = await respuesta.json();

            viviendas.forEach(v => {
                const option = document.createElement("option");
                option.value = v.id;
                option.textContent = v.numero;
                select.appendChild(option);
            });

            wrapper.style.display = "block";

        } catch (error) {
            console.log(error);
        }
    }


    // ==========================
    // ENVIO FINAL
    // ==========================
    if (registerForm) {

        registerForm.addEventListener("submit", function (e) {

            const paso3Activo = document.getElementById("paso3").classList.contains("activo");

            if (!paso3Activo) return;

            document.getElementById("errTipoVivienda").innerHTML = "";
            document.getElementById("errVivienda").innerHTML = "";

            let valido = true;

            if (tipoVivienda === "") {
                document.getElementById("errTipoVivienda").innerHTML = "Seleccione un tipo de vivienda";
                valido = false;
            }

            const viviendaSeleccionada = document.getElementById("TN_ViviendaId").value;

            if (viviendaSeleccionada === "") {
                document.getElementById("errVivienda").innerHTML = "Seleccione una vivienda";
                valido = false;
            }

            if (!valido) {
                e.preventDefault();
            }

        });
    }


    // ==========================
    // MODAL REGISTRO
    // ==========================
    const registroPendiente = document.getElementById("registroPendiente");

    if (registroPendiente &&
        registroPendiente.value.trim().toLowerCase() === "true") {

        const modal = document.getElementById("modalPendiente");

        if (modal) {
            modal.classList.add("visible");
            modal.style.display = "flex"; // 👈 asegura que se vea
        }
    }


    // ==========================
    // BOTON CONTINUAR → INICIO
    // ==========================
    const btnContinuar = document.getElementById("btnContinuar");

    if (btnContinuar) {

        btnContinuar.addEventListener("click", function (e) {

            e.preventDefault(); 
            window.location.href = "/";

        });

    }

});

document.addEventListener("DOMContentLoaded", function () {

    // ==========================
    // MOSTRAR MODAL SI VIENE DE REGISTRO
    // ==========================
    const input = document.getElementById("registroPendiente");

    console.log("Valor TempData:", input?.value);

    if (input && input.value && input.value.toLowerCase() === "true") {

        const modal = document.getElementById("modalPendiente");

        if (modal) {
            modal.classList.add("visible");
        }
    }


    // ==========================
    // BOTÓN → IR AL INICIO
    // ==========================
    const btn = document.getElementById("btnContinuar");

    if (btn) {
        btn.addEventListener("click", function () {
            window.location.href = "/";
        });
    }

});