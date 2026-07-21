let modalEliminarViviendaInstance = null;

function abrirEliminarVivienda(id, numero) {

    document.getElementById("eliminarViviendaId").value = id;
    document.getElementById("eliminarViviendaNumero").textContent = numero;

    const errorBox = document.getElementById("eliminarViviendaError");
    errorBox.style.display = "none";
    errorBox.textContent = "";

    const modalEl = document.getElementById("modalEliminarVivienda");
    modalEliminarViviendaInstance = new bootstrap.Modal(modalEl);
    modalEliminarViviendaInstance.show();
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
            errorBox.textContent = `Error del servidor (${response.status}). No se pudo eliminar la vivienda.`;
            errorBox.style.display = "block";
            boton.disabled = false;
            return;
        }

        const result = await response.json();

        if (result.success) {
            location.reload();
        } else {
            errorBox.textContent = result.message ?? "No se pudo eliminar la vivienda.";
            errorBox.style.display = "block";
            boton.disabled = false;
        }

    } catch (error) {
        errorBox.textContent = "Ocurrió un error al eliminar la vivienda.";
        errorBox.style.display = "block";
        boton.disabled = false;
    }
}