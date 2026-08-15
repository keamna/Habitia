let modalEliminarAreaInstance = null;

async function abrirEliminarArea(id, nombre) {
    const modalEl = document.getElementById("modalEliminarArea");
    const texto = modalEl.querySelector("#eliminarAreaConfirmacionTexto");
    const nombreSpan = modalEl.querySelector("#eliminarAreaNombre");
    const errorBox = modalEl.querySelector("#eliminarAreaError");
    const idInput = modalEl.querySelector("#eliminarAreaId");
    const verificando = modalEl.querySelector("#eliminarAreaVerificando");
    const botones = modalEl.querySelector("#eliminarAreaBotones");
    const entendido = modalEl.querySelector("#eliminarAreaEntendido");
    const btnConfirmar = modalEl.querySelector("#btnConfirmarEliminarArea");

    idInput.value = id;
    nombreSpan.textContent = nombre;

    errorBox.style.display = "none";
    errorBox.textContent = "";

    texto.style.display = "block";
    verificando.style.display = "block";
    botones.style.display = "none";
    entendido.style.display = "none";

    modalEliminarAreaInstance = new bootstrap.Modal(modalEl);
    modalEliminarAreaInstance.show();

    try {
        const response = await fetch(`/Admin/AreaComun/VerificarEliminacion?id=${id}`);
        const result = await response.json();

        verificando.style.display = "none";

        if (result.puedeEliminar) {
            botones.style.display = "flex";
            btnConfirmar.disabled = false;
        } else {
            mostrarErrorEliminarArea(result.message);
        }
    } catch (error) {
        verificando.style.display = "none";
        mostrarErrorEliminarArea("No fue posible completar la operación. Intente nuevamente.");
    }
}

function obtenerTokenArea() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : null;
}

async function confirmarEliminarArea() {
    const modalEl = document.getElementById("modalEliminarArea");
    const id = modalEl.querySelector("#eliminarAreaId").value;
    if (!id) return;

    const boton = modalEl.querySelector("#btnConfirmarEliminarArea");
    boton.disabled = true;

    const errorBox = modalEl.querySelector("#eliminarAreaError");
    errorBox.style.display = "none";

    try {
        const response = await fetch('/Admin/AreaComun/Eliminar', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": obtenerTokenArea()
            },
            body: JSON.stringify({ id: parseInt(id) })
        });

        if (!response.ok) {
            mostrarErrorEliminarArea("No fue posible completar la operación. Intente nuevamente.");
            return;
        }

        const result = await response.json();

        if (result.success) {
            location.reload();
        } else {
            mostrarErrorEliminarArea(result.message ?? "No se pudo eliminar el área común.");
        }
    } catch (error) {
        mostrarErrorEliminarArea("No fue posible completar la operación. Intente nuevamente.");
    }
}

function mostrarErrorEliminarArea(mensaje) {
    const modalEl = document.getElementById("modalEliminarArea");

    modalEl.querySelector("#eliminarAreaError").textContent = mensaje;
    modalEl.querySelector("#eliminarAreaError").style.display = "block";

    modalEl.querySelector("#eliminarAreaConfirmacionTexto").style.display = "none";
    modalEl.querySelector("#eliminarAreaVerificando").style.display = "none";
    modalEl.querySelector("#eliminarAreaBotones").style.display = "none";
    modalEl.querySelector("#eliminarAreaEntendido").style.display = "flex";
}