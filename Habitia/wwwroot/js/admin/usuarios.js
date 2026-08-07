// =================================
// ABRIR MODAL ROLES
// =================================

function abrirRoles(id, rolesActuales, tiposMantenimientoActuales) {

    document.getElementById("rolesUsuarioId").value = id;

    document.querySelectorAll('#rolesContainer input').forEach(check => {
        check.checked = false;
    });

    rolesActuales.forEach(rol => {
        const check = document.querySelector(`#rolesContainer input[value="${rol}"]`);
        if (check) {
            check.checked = true;
        }
    });

    // Precarga y muestra/oculta la sección de tipos de mantenimiento
    const tieneMantenimiento = rolesActuales.includes('Mantenimiento');
    const contenedorTipos = document.getElementById("tiposMantenimientoModalContainer");
    contenedorTipos.style.display = tieneMantenimiento ? "block" : "none";

    document.querySelectorAll('input[name="tipoMantenimientoModal"]').forEach(check => {
        check.checked = false;
    });

    const tiposActuales = tiposMantenimientoActuales || [];
    tiposActuales.forEach(idTipo => {
        const check = document.querySelector(`input[name="tipoMantenimientoModal"][value="${idTipo}"]`);
        if (check) {
            check.checked = true;
        }
    });

    document.getElementById("rolesError").style.display = "none";
    document.getElementById("modalRoles").classList.add("visible");
}


// =================================
// CERRAR MODAL ROLES
// =================================

function cerrarModalRoles() {
    document.getElementById("modalRoles").classList.remove("visible");
}


// =================================
// GUARDAR ROLES
// =================================

async function guardarRoles() {

    const id = document.getElementById("rolesUsuarioId").value;
    const rolSeleccionado = document.querySelector('#rolesContainer input:checked');

    if (!rolSeleccionado) {
        document.getElementById("rolesError").textContent = "Debe seleccionar un rol.";
        document.getElementById("rolesError").style.display = "block";
        return;
    }

    const rol = rolSeleccionado.value;
    const tiposMantenimientoIds = [];

    if (rol === "Mantenimiento") {

        document.querySelectorAll('input[name="tipoMantenimientoModal"]:checked').forEach(check => {
            tiposMantenimientoIds.push(parseInt(check.value));
        });

        if (tiposMantenimientoIds.length === 0) {
            document.getElementById("rolesError").textContent = "Debe asignar al menos un tipo de mantenimiento.";
            document.getElementById("rolesError").style.display = "block";
            return;
        }
    }

    document.getElementById("rolesError").style.display = "none";

    const response = await fetch('/Admin/Usuarios/EditarRoles', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: id, rol: rol, tiposMantenimientoIds: tiposMantenimientoIds })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("usuarioToastMensaje", "Roles actualizados correctamente.");
        location.reload();
    } else {
        document.getElementById("rolesError").textContent = result.message ?? "No se pudieron guardar los roles.";
        document.getElementById("rolesError").style.display = "block";
    }
}


// =================================
// APROBAR USUARIO
// =================================

async function aprobarUsuario(id, idVivienda) {

    const confirmar = confirm("¿Desea aprobar este usuario?");
    if (!confirmar) return;

    const response = await fetch('/Admin/Usuarios/Aprobar', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: id, idVivienda: idVivienda })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("usuarioToastMensaje", "Usuario aprobado correctamente.");
        location.reload();
    } else {
        alert(result.message ?? "Error al aprobar usuario.");
    }
}


// =================================
// RECHAZAR USUARIO
// =================================

async function rechazarUsuario(id, idVivienda) {

    const confirmar = confirm("¿Desea rechazar este usuario?");
    if (!confirmar) return;

    const response = await fetch('/Admin/Usuarios/Rechazar', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: id, idVivienda: idVivienda })
    });

    const result = await response.json();

    if (result.success) {
        sessionStorage.setItem("usuarioToastMensaje", "Usuario rechazado correctamente.");
        location.reload();
    } else {
        alert(result.message ?? "Error al rechazar usuario.");
    }
}


// =================================
// SUSPENDER USUARIO (Bootstrap Modal)
// =================================

let modalSuspenderInstance = null;

function abrirSuspenderUsuario(id, nombre) {
    document.getElementById("suspenderUsuarioId").value = id;
    document.getElementById("suspenderUsuarioNombre").textContent = nombre;

    const modalEl = document.getElementById("modalSuspenderUsuario");
    modalSuspenderInstance = new bootstrap.Modal(modalEl);
    modalSuspenderInstance.show();
}

function confirmarSuspenderUsuario() {
    document.getElementById("formSuspender").submit();
}


// =================================
// REACTIVAR USUARIO (Bootstrap Modal)
// =================================

let modalReactivarInstance = null;

function abrirReactivarUsuario(id, nombre) {
    document.getElementById("reactivarUsuarioId").value = id;
    document.getElementById("reactivarUsuarioNombre").textContent = nombre;

    const modalEl = document.getElementById("modalReactivarUsuario");
    modalReactivarInstance = new bootstrap.Modal(modalEl);
    modalReactivarInstance.show();
}

function confirmarReactivarUsuario() {
    document.getElementById("formReactivar").submit();
}


// =================================
// ELIMINAR USUARIO (Bootstrap Modal)
// =================================

let modalEliminarUsuarioInstance = null;

function abrirEliminar(id, nombre) {

    document.getElementById("eliminarUsuarioId").value = id;
    document.getElementById("eliminarUsuarioNombre").textContent = nombre;

    const errorBox = document.getElementById("eliminarError");
    errorBox.style.display = "none";
    errorBox.textContent = "";

    document.getElementById("btnConfirmarEliminar").disabled = false;

    const modalEl = document.getElementById("modalEliminarUsuario");
    modalEliminarUsuarioInstance = new bootstrap.Modal(modalEl);
    modalEliminarUsuarioInstance.show();
}

async function confirmarEliminarUsuario() {

    const id = document.getElementById("eliminarUsuarioId").value;
    if (id === "") return;

    const boton = document.getElementById("btnConfirmarEliminar");
    boton.disabled = true;

    const errorBox = document.getElementById("eliminarError");
    errorBox.style.display = "none";

    try {
        const response = await fetch('/Admin/Usuarios/Eliminar', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id: id })
        });

        const result = await response.json();

        if (result.success) {
            sessionStorage.setItem("usuarioToastMensaje", "Usuario eliminado correctamente.");
            location.reload();
        } else {
            errorBox.textContent = result.message ?? "No se pudo eliminar el usuario.";
            errorBox.style.display = "block";
            boton.disabled = false;
        }
    } catch (error) {
        errorBox.textContent = "No fue posible completar la operación. Intente nuevamente.";
        errorBox.style.display = "block";
        boton.disabled = false;
    }
}