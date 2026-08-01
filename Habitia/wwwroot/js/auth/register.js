document.addEventListener("DOMContentLoaded", function () {

    const pasos = document.querySelectorAll(".form-paso");
    const steps = document.querySelectorAll(".step-item");

    const btnPaso1 = document.getElementById("btnPaso1");
    const btnPaso2 = document.getElementById("btnPaso2");
    const btnPaso3 = document.getElementById("btnPaso3");

    const btnAtras2 = document.getElementById("btnAtras2");
    const btnAtras3 = document.getElementById("btnAtras3");

    const btnBackArrow = document.getElementById("btnBackArrow");

    const registerForm = document.getElementById("registerForm");

    const progressBar = document.getElementById("progressBar");

    let tipoRelacion = "";
    let tipoVivienda = "";
    let pasoActual = 1;


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

        pasoActual = numero;
    }


    // ==========================
    // FLECHA SUPERIOR IZQUIERDA
    // ==========================
    // En el paso 1 vuelve al inicio del sitio; en los demás pasos,
    // retrocede un paso del formulario (no saca de la pantalla de registro).
    if (btnBackArrow) {

        btnBackArrow.addEventListener("click", function () {

            if (pasoActual === 1) {
                window.location.href = "/";
            } else {
                mostrarPaso(pasoActual - 1);
            }

        });
    }


    // ==========================
    // BOTONES "ANTERIOR" DE CADA PASO
    // ==========================
    if (btnAtras2) {
        btnAtras2.addEventListener("click", function () {
            mostrarPaso(1);
        });
    }

    if (btnAtras3) {
        btnAtras3.addEventListener("click", function () {
            mostrarPaso(2);
        });
    }


    // ==========================
    // PASO 1 — DATOS PERSONALES
    // ==========================
    if (btnPaso1) {

        btnPaso1.addEventListener("click", async function () {

            let valido = true;

            document.querySelectorAll(".field-error").forEach(e => e.innerHTML = "");

            const tipoId = document.getElementById("TC_TipoIdentificacion").value;
            const numeroId = document.getElementById("TC_NumeroIdentificacion").value.trim();
            const nombre = document.getElementById("TC_NombreCompleto").value.trim();
            const telefono = document.getElementById("TC_Telefono").value.trim();
            const correo = document.getElementById("Email").value.trim();
            const password = document.getElementById("Password").value;
            const confirm = document.getElementById("ConfirmPassword").value;

            const regexCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            const regexTelefono = /^[0-9]{8,15}$/;
            const regexNumeroId = /^[A-Za-z0-9-]{4,20}$/;

            // Tipo de identificación
            if (tipoId === "") {
                document.getElementById("errTipoId").innerHTML = "Seleccione el tipo de identificación";
                valido = false;
            }

            // Número de identificación
            if (numeroId === "") {
                document.getElementById("errNumeroId").innerHTML = "Ingrese el número de identificación";
                valido = false;
            } else if (!regexNumeroId.test(numeroId)) {
                document.getElementById("errNumeroId").innerHTML = "Número de identificación inválido";
                valido = false;
            }

            // Nombre completo
            if (nombre === "") {
                document.getElementById("errNombre").innerHTML = "Ingrese su nombre";
                valido = false;
            } else if (nombre.split(" ").filter(p => p.length > 0).length < 2) {
                document.getElementById("errNombre").innerHTML = "Ingrese nombre y apellido";
                valido = false;
            }

            // Teléfono
            if (telefono === "") {
                document.getElementById("errTelefono").innerHTML = "Ingrese su teléfono";
                valido = false;
            } else if (!regexTelefono.test(telefono)) {
                document.getElementById("errTelefono").innerHTML = "Ingrese un teléfono válido (solo números)";
                valido = false;
            }

            // Correo
            if (correo === "") {
                document.getElementById("errEmail").innerHTML = "Ingrese su correo";
                valido = false;
            } else if (!regexCorreo.test(correo)) {
                document.getElementById("errEmail").innerHTML = "Ingrese un correo válido";
                valido = false;
            }

            // Contraseña — mismas reglas configuradas en Program.cs:
            // RequiredLength = 8, RequireDigit, RequireUppercase,
            // RequireNonAlphanumeric, y RequireLowercase (por defecto en Identity).
            if (password === "") {
                document.getElementById("errPassword").innerHTML = "Ingrese una contraseña";
                valido = false;
            } else {

                const erroresPassword = [];

                if (password.length < 8) {
                    erroresPassword.push("al menos 8 caracteres");
                }
                if (!/[a-z]/.test(password)) {
                    erroresPassword.push("una letra minúscula");
                }
                if (!/[A-Z]/.test(password)) {
                    erroresPassword.push("una letra mayúscula");
                }
                if (!/[0-9]/.test(password)) {
                    erroresPassword.push("un número");
                }
                if (!/[^a-zA-Z0-9]/.test(password)) {
                    erroresPassword.push("un carácter especial (ej. !@#$%)");
                }

                if (erroresPassword.length > 0) {
                    document.getElementById("errPassword").innerHTML =
                        "La contraseña debe tener " + erroresPassword.join(", ");
                    valido = false;
                }
            }

            // Confirmar contraseña
            if (confirm === "") {
                document.getElementById("errConfirmPassword").innerHTML = "Confirme la contraseña";
                valido = false;
            } else if (confirm !== password) {
                document.getElementById("errConfirmPassword").innerHTML = "Las contraseñas no coinciden";
                valido = false;
            }

            // Si algo del formato básico ya está mal, ni siquiera consultamos al servidor
            if (!valido) return;

            // ==========================================
            // VERIFICACIÓN CONTRA EL SERVIDOR:
            // correo e identificación ya registrados.
            // Se hacen en paralelo con Promise.all para no duplicar
            // el tiempo de espera de una consulta tras otra.
            // ==========================================
            const textoOriginal = btnPaso1.textContent;
            btnPaso1.disabled = true;
            btnPaso1.textContent = "Verificando...";

            try {

                const [respuestaEmail, respuestaId] = await Promise.all([
                    fetch(`/Account/VerificarEmailDisponible?email=${encodeURIComponent(correo)}`),
                    fetch(`/Account/VerificarIdentificacionDisponible?identificacion=${encodeURIComponent(numeroId)}`)
                ]);

                const dataEmail = await respuestaEmail.json();
                const dataId = await respuestaId.json();

                if (!dataEmail.disponible) {
                    document.getElementById("errEmail").innerHTML = "Ya existe una cuenta registrada con este correo";
                    valido = false;
                }

                if (!dataId.disponible) {
                    document.getElementById("errNumeroId").innerHTML = "Ya existe una cuenta registrada con este número de identificación";
                    valido = false;
                }

            } catch (error) {
                console.log(error);
                document.getElementById("errEmail").innerHTML = "No se pudo verificar la información. Intente de nuevo.";
                valido = false;
            } finally {
                btnPaso1.disabled = false;
                btnPaso1.textContent = textoOriginal;
            }

            // Solo avanza al paso 2 si TODO (formato + servidor) fue válido
            if (valido) {
                mostrarPaso(2);
            }

        });
    }


    // ==========================
    // PASO 2 — RELACIÓN CON LA VIVIENDA
    // ==========================
    document.querySelectorAll(".card-relacion").forEach(card => {

        card.addEventListener("click", function () {

            document.querySelectorAll(".card-relacion").forEach(c => c.classList.remove("border-success", "bg-light"));

            this.classList.add("border-success", "bg-light");

            tipoRelacion = this.dataset.valor;

            document.getElementById("errRelacion").innerHTML = "";

            const hidden = document.getElementById("TC_TipoRelacion");

            if (tipoRelacion === "Propietario") {
                hidden.value = 1;
            } else {
                hidden.value = 2;
            }

            const vive = document.getElementById("viveAhiSection");

            // Al cambiar de opción, se limpia cualquier selección previa de "vive ahí"
            document.querySelectorAll(".toggle-option").forEach(o => o.classList.remove("active"));
            document.getElementById("errViveAhi").innerHTML = "";

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

            document.querySelectorAll(".toggle-option").forEach(o => o.classList.remove("active"));

            this.classList.add("active");

            document.getElementById("TB_ViveAhi").value = this.dataset.valor;

            document.getElementById("errViveAhi").innerHTML = "";

        });
    });


    // ==========================
    // BOTON PASO 2
    // ==========================
    if (btnPaso2) {

        btnPaso2.addEventListener("click", function () {

            let valido = true;

            document.getElementById("errRelacion").innerHTML = "";
            document.getElementById("errViveAhi").innerHTML = "";

            if (tipoRelacion === "") {
                document.getElementById("errRelacion").innerHTML = "Seleccione una opción";
                valido = false;
            }

            if (tipoRelacion === "Propietario") {

                const vive = document.getElementById("TB_ViveAhi").value;

                if (vive === "") {
                    document.getElementById("errViveAhi").innerHTML = "Seleccione una opción";
                    valido = false;
                }
            }

            if (valido) {
                mostrarPaso(3);
            }
        });
    }


    // ==========================
    // PASO 3 — TIPO DE VIVIENDA
    // ==========================
    document.querySelectorAll(".card-vivienda").forEach(card => {

        card.addEventListener("click", function () {

            document.querySelectorAll(".card-vivienda").forEach(c => c.classList.remove("border-success", "bg-light"));

            this.classList.add("border-success", "bg-light");

            tipoVivienda = this.dataset.valor;

            document.getElementById("TC_TipoVivienda").value = tipoVivienda;

            document.getElementById("errTipoVivienda").innerHTML = "";
            document.getElementById("errVivienda").innerHTML = "";

            cargarViviendas(tipoVivienda);

        });
    });


    async function cargarViviendas(tipo) {

        const select = document.getElementById("TN_ViviendaId");
        const wrapper = document.getElementById("viviendaSelectWrapper");

        select.innerHTML = `<option value="">Seleccione una vivienda</option>`;

        const relacion = document.getElementById("TC_TipoRelacion").value; // "1" Propietario, "2" Inquilino

        try {
            const respuesta = await fetch(`/Account/ObtenerViviendas?tipo=${tipo}&relacion=${relacion}`);
            const viviendas = await respuesta.json();

            if (!viviendas || viviendas.length === 0) {
                document.getElementById("errVivienda").innerHTML = "No hay viviendas disponibles para esta opción";
            }

            viviendas.forEach(v => {
                const option = document.createElement("option");
                option.value = v.id;

                // Para "Ocupante" (Inquilino) el backend indica si la persona
                // va a quedar como Familiar o Inquilino en esa vivienda
                // específica, según si el propietario vive ahí o no.
                option.textContent = v.etiqueta
                    ? `${v.numero} (${v.etiqueta})`
                    : v.numero;

                select.appendChild(option);
            });

            wrapper.style.display = "block";

        } catch (error) {
            console.log(error);
            document.getElementById("errVivienda").innerHTML = "No se pudieron cargar las viviendas. Intente de nuevo.";
        }
    }


    // Limpia el error de vivienda apenas el usuario elige una
    document.addEventListener("change", function (e) {
        if (e.target && e.target.id === "TN_ViviendaId") {
            document.getElementById("errVivienda").innerHTML = "";
        }
    });


    // ==========================
    // ENVIO FINAL (AJAX)
    // ==========================
    if (registerForm) {

        registerForm.addEventListener("submit", async function (e) {

            const paso3Activo = document.getElementById("paso3").classList.contains("activo");

            if (!paso3Activo) return;

            e.preventDefault(); // el envío se maneja acá, no como submit tradicional

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

            if (!valido) return;

            const formData = new FormData(registerForm);

            btnPaso3.disabled = true;
            btnPaso3.textContent = "Creando cuenta...";

            try {

                const respuesta = await fetch(registerForm.action || "/Account/Register", {
                    method: "POST",
                    headers: {
                        "X-Requested-With": "XMLHttpRequest"
                    },
                    body: formData
                });

                const data = await respuesta.json();

                if (data.success) {

                    mostrarModalPendiente();

                } else {

                    mostrarErroresRegistro(data.errors);

                    btnPaso3.disabled = false;
                    btnPaso3.textContent = "Crear cuenta";

                }

            } catch (error) {

                console.error(error);

                mostrarErroresRegistro(["Ocurrió un error al crear la cuenta. Intente de nuevo."]);

                btnPaso3.disabled = false;
                btnPaso3.textContent = "Crear cuenta";

            }

        });
    }


    function mostrarErroresRegistro(errores) {

        document.getElementById("errVivienda").innerHTML =
            (errores && errores.length)
                ? errores.join("<br>")
                : "Ocurrió un error, intente de nuevo.";

    }


    function mostrarModalPendiente() {

        const modalEl = document.getElementById("modalPendiente");

        if (!modalEl) return;

        const modal = new bootstrap.Modal(modalEl, {
            backdrop: "static",
            keyboard: false
        });

        modal.show();

    }


    // ==========================
    // BOTON ENTENDIDO (cierra el modal y redirige al inicio)
    // ==========================
    const btnContinuar = document.getElementById("btnContinuar");

    if (btnContinuar) {

        btnContinuar.addEventListener("click", function () {

            window.location.href = "/";

        });

    }

});