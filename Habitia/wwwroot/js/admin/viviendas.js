let modalEliminarViviendaInstance = null;

async function abrirEliminarVivienda(id, numero) {
    const modalEl = document.getElementById("modalEliminarVivienda");
    const confirmacionTexto = modalEl.querySelector("#eliminarViviendaConfirmacionTexto");
    const numeroSpan = modalEl.querySelector("#eliminarViviendaNumero");
    const errorBox = modalEl.querySelector("#eliminarViviendaError");
    const idInput = modalEl.querySelector("#eliminarViviendaId");
    const verificando = modalEl.querySelector("#eliminarViviendaVerificando");
    const botonesConfirmar = modalEl.querySelector("#eliminarViviendaBotonesConfirmar");
    const botonEntendido = modalEl.querySelector("#eliminarViviendaBotonEntendido");
    const btnConfirmar = modalEl.querySelector("#btnConfirmarEliminarVivienda");

    idInput.value = id;
    numeroSpan.textContent = numero;

    errorBox.style.display = "none";
    errorBox.textContent = "";

    // El texto de confirmación se ve desde el inicio; mientras se verifica
    // solo se ocultan los botones, para que no se pueda confirmar antes de
    // saber si la vivienda realmente se puede eliminar.
    confirmacionTexto.style.display = "block";
    verificando.style.display = "block";
    botonesConfirmar.style.display = "none";
    botonEntendido.style.display = "none";

    modalEliminarViviendaInstance = new bootstrap.Modal(modalEl);
    modalEliminarViviendaInstance.show();

    try {
        const response = await fetch(`/Admin/Viviendas/VerificarEliminacion?id=${id}`);
        const result = await response.json();

        verificando.style.display = "none";

        if (result.puedeEliminar) {
            botonesConfirmar.style.display = "flex";
            btnConfirmar.disabled = false;
        } else {
            mostrarErrorEliminarVivienda(result.message);
        }
    } catch (error) {
        verificando.style.display = "none";
        mostrarErrorEliminarVivienda("No fue posible completar la operación. Intente nuevamente.");
    }
}

function obtenerAntiForgeryToken() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : null;
}

async function confirmarEliminarVivienda() {
    const modalEl = document.getElementById("modalEliminarVivienda");
    const idInput = modalEl.querySelector("#eliminarViviendaId");
    const id = idInput.value;
    if (!id) return;

    const boton = modalEl.querySelector("#btnConfirmarEliminarVivienda");
    boton.disabled = true;

    const errorBox = modalEl.querySelector("#eliminarViviendaError");
    errorBox.style.display = "none";

    try {
        const token = obtenerAntiForgeryToken();
        const response = await fetch('/Admin/Viviendas/Eliminar', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify({ id: parseInt(id) })
        });

        if (!response.ok) {
            mostrarErrorEliminarVivienda("No fue posible completar la operación. Intente nuevamente.");
            return;
        }

        const result = await response.json();
        if (result.success) {
            location.reload();
        } else {
            mostrarErrorEliminarVivienda(result.message ?? "No se pudo eliminar la vivienda.");
        }
    } catch (error) {
        mostrarErrorEliminarVivienda("No fue posible completar la operación. Intente nuevamente.");
    }
}

function mostrarErrorEliminarVivienda(mensaje) {
    const modalEl = document.getElementById("modalEliminarVivienda");
    const errorBox = modalEl.querySelector("#eliminarViviendaError");
    const confirmacionTexto = modalEl.querySelector("#eliminarViviendaConfirmacionTexto");
    const verificando = modalEl.querySelector("#eliminarViviendaVerificando");
    const botonesConfirmar = modalEl.querySelector("#eliminarViviendaBotonesConfirmar");
    const botonEntendido = modalEl.querySelector("#eliminarViviendaBotonEntendido");

    errorBox.textContent = mensaje;
    errorBox.style.display = "block";

    // Si no se puede eliminar, la pregunta ya no aplica: se deja solo el
    // motivo y el botón "Entendido".
    confirmacionTexto.style.display = "none";
    verificando.style.display = "none";
    botonesConfirmar.style.display = "none";
    botonEntendido.style.display = "flex";
}