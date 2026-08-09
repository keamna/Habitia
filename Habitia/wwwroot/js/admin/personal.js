// =================================
// EDITAR PERSONAL
// =================================

let modalEditarPersonalInstance = null;

function abrirEditarPersonal(fila) {
    document.getElementById("editarPersonalId").value = fila.dataset.id;
    document.getElementById("editarPersonalNombre").value = fila.dataset.nombre || "";
    document.getElementById("editarPersonalApellido").value = fila.dataset.apellido || "";
    document.getElementById("editarPersonalTelefono").value = fila.dataset.telefono || "";

    const tiposActuales = (fila.dataset.tipos || "").split(",").filter(v => v !== "");

    document.querySelectorAll('input[name="editarPersonalTipo"]').forEach(check => {
        check.checked = tiposActuales.includes(check.value);
    });

    const errorBox = document.getElementById("editarPersonalError");
    errorBox.style.display = "none";
    errorBox.textContent = "";

    const modalEl = document.getElementById("modalEditarPersonal");
    modalEditarPersonalInstance = new bootstrap.Modal(modalEl);
    modalEditarPersonalInstance.show();
}

async function guardarEditarPersonal() {

    const id = document.getElementById("editarPersonalId").value;
    const nombre = document.getElementById("editarPersonalNombre").value.trim();
    const apellido = document.getElementById("editarPersonalApellido").value.trim();
    const telefono = document.getElementById("editarPersonalTelefono").value.trim();

    const tiposMantenimientoIds = [];
    document.querySelectorAll('input[name="editarPersonalTipo"]:checked').forEach(check => {
        tiposMantenimientoIds.push(parseInt(check.value));
    });

    const errorBox = document.getElementById("editarPersonalError");

    if (!nombre || !apellido) {
        errorBox.textContent = "Nombre y apellido son obligatorios.";
        errorBox.style.display = "block";
        return;
    }

    if (tiposMantenimientoIds.length === 0) {
        errorBox.textContent = "Debe asignar al menos un tipo de mantenimiento.";
        errorBox.style.display = "block";
        return;
    }

    errorBox.style.display = "none";

    const response = await fetch('/Admin/Mantenimientos/EditarPersonal', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            id: id,
            nombre: nombre,
            apellido: apellido,
            telefono: telefono,
            tiposMantenimientoIds: tiposMantenimientoIds
        })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("personalToastMensaje", "Personal actualizado correctamente.");
        location.reload();
    } else {
        errorBox.textContent = result.message ?? "No se pudo actualizar el personal.";
        errorBox.style.display = "block";
    }
}


// =================================
// SUSPENDER PERSONAL
// =================================

let modalSuspenderPersonalInstance = null;
let idPersonalSuspender = null;

function abrirSuspenderPersonal(id, nombre) {
    idPersonalSuspender = id;
    document.getElementById("suspenderPersonalNombre").textContent = nombre;

    const modalEl = document.getElementById("modalSuspenderPersonal");
    modalSuspenderPersonalInstance = new bootstrap.Modal(modalEl);
    modalSuspenderPersonalInstance.show();
}

async function confirmarSuspenderPersonal() {
    const response = await fetch('/Admin/Mantenimientos/SuspenderPersonal', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: idPersonalSuspender })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("personalToastMensaje", "Personal suspendido correctamente.");
        location.reload();
    } else {
        alert(result.message ?? "No se pudo suspender al personal.");
    }
}


// =================================
// REACTIVAR PERSONAL
// =================================

let modalReactivarPersonalInstance = null;
let idPersonalReactivar = null;

function abrirReactivarPersonal(id, nombre) {
    idPersonalReactivar = id;
    document.getElementById("reactivarPersonalNombre").textContent = nombre;

    const modalEl = document.getElementById("modalReactivarPersonal");
    modalReactivarPersonalInstance = new bootstrap.Modal(modalEl);
    modalReactivarPersonalInstance.show();
}

async function confirmarReactivarPersonal() {
    const response = await fetch('/Admin/Mantenimientos/ReactivarPersonal', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: idPersonalReactivar })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("personalToastMensaje", "Personal reactivado correctamente.");
        location.reload();
    } else {
        alert(result.message ?? "No se pudo reactivar al personal.");
    }
}


// =================================
// ELIMINAR PERSONAL
// =================================

let modalEliminarPersonalInstance = null;

function abrirEliminarPersonal(id, nombre) {
    document.getElementById("eliminarPersonalId").value = id;
    document.getElementById("eliminarPersonalNombre").textContent = nombre;

    const errorBox = document.getElementById("eliminarPersonalError");
    errorBox.style.display = "none";
    errorBox.textContent = "";

    document.getElementById("btnConfirmarEliminarPersonal").disabled = false;

    const modalEl = document.getElementById("modalEliminarPersonal");
    modalEliminarPersonalInstance = new bootstrap.Modal(modalEl);
    modalEliminarPersonalInstance.show();
}

async function confirmarEliminarPersonal() {
    const id = document.getElementById("eliminarPersonalId").value;
    if (!id) return;

    const boton = document.getElementById("btnConfirmarEliminarPersonal");
    boton.disabled = true;

    const errorBox = document.getElementById("eliminarPersonalError");
    errorBox.style.display = "none";

    try {
        const response = await fetch('/Admin/Mantenimientos/EliminarPersonal', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id: id })
        });

        const result = await response.json();

        if (result.success) {
            sessionStorage.setItem("personalToastMensaje", "Personal eliminado correctamente.");
            location.reload();
        } else {
            errorBox.textContent = result.message ?? "No se pudo eliminar el personal.";
            errorBox.style.display = "block";
            boton.disabled = false;
        }
    } catch (error) {
        errorBox.textContent = "No fue posible completar la operación. Intente nuevamente.";
        errorBox.style.display = "block";
        boton.disabled = false;
    }
}