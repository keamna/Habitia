let modalEliminarViviendaInstance = null;

async function abrirEliminarVivienda(id, numero) {
    document.getElementById("eliminarViviendaId").value = id;
    document.getElementById("eliminarViviendaNumero").textContent = numero;

    const errorBox = document.getElementById("eliminarViviendaError");
    errorBox.style.display = "none";
    errorBox.textContent = "";

    // Reset visual antes de verificar
    document.getElementById("eliminarViviendaBotonesConfirmar").style.display = "none";
    document.getElementById("eliminarViviendaBotonEntendido").style.display = "none";

    const modalEl = document.getElementById("modalEliminarVivienda");
    modalEliminarViviendaInstance = new bootstrap.Modal(modalEl);
    modalEliminarViviendaInstance.show();

    // Verificar antes de mostrar los botones correctos
    try {
        const response = await fetch(`/Admin/Viviendas/VerificarEliminacion?id=${id}`);
        const result = await response.json();

        if (result.puedeEliminar) {
            document.getElementById("eliminarViviendaBotonesConfirmar").style.display = "flex";
            document.getElementById("btnConfirmarEliminarVivienda").disabled = false;
        } else {
            mostrarErrorEliminarVivienda(result.message);
        }
    } catch (error) {
        mostrarErrorEliminarVivienda("No fue posible completar la operación. Intente nuevamente.");
    }
}

function obtenerAntiForgeryToken() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : null;
}

async function confirmarEliminarVivienda() {
    const id = document.getElementById("eliminarViviendaId").value;
    if (!id) return;

    const boton = document.getElementById("btnConfirmarEliminarVivienda");
    boton.disabled = true;

    const errorBox = document.getElementById("eliminarViviendaError");
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
    const errorBox = document.getElementById("eliminarViviendaError");
    errorBox.textContent = mensaje;
    errorBox.style.display = "block";

    // Ocultar Cancelar/Eliminar, mostrar solo "Entendido"
    document.getElementById("eliminarViviendaBotonesConfirmar").style.display = "none";
    document.getElementById("eliminarViviendaBotonEntendido").style.display = "flex";
}