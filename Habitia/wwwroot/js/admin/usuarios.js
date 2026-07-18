document.addEventListener("DOMContentLoaded", function () {


    // ==========================
    // MENU DE ACCIONES POR FILA
    // ==========================


    const botones =
        document.querySelectorAll('.btn-menu-fila');


    botones.forEach(btn => {


        btn.addEventListener('click', function (e) {

            e.stopPropagation();


            document
                .querySelectorAll('.dropdown-menu-fila')
                .forEach(menu => {

                    menu.style.display = "none";

                });



            const menu =
                this.nextElementSibling;


            if (menu) {
                menu.style.display = "block";
            }


        });


    });





    document.addEventListener('click', function () {


        document
            .querySelectorAll('.dropdown-menu-fila')
            .forEach(menu => {

                menu.style.display = "none";

            });


    });


});









// =================================
// ABRIR MODAL ROLES
// =================================


function abrirRoles(id, rolesActuales) {


    document.getElementById(
        "rolesUsuarioId"
    ).value = id;




    document
        .querySelectorAll(
            '#rolesContainer input'
        )
        .forEach(check => {

            check.checked = false;

        });






    rolesActuales.forEach(rol => {


        const check =
            document.querySelector(
                `#rolesContainer input[value="${rol}"]`
            );



        if (check) {
            check.checked = true;
        }



    });





    document
        .getElementById(
            "modalRoles"
        )
        .classList.add("visible");



}









// =================================
// CERRAR MODAL ROLES
// =================================


function cerrarModalRoles() {


    document
        .getElementById(
            "modalRoles"
        )
        .classList.remove("visible");


}









// =================================
// GUARDAR ROLES
// =================================


async function guardarRoles() {


    const id =
        document.getElementById(
            "rolesUsuarioId"
        ).value;





    const roles = [];



    document
        .querySelectorAll(
            '#rolesContainer input:checked'
        )
        .forEach(check => {


            roles.push(
                check.value
            );


        });






    if (roles.length === 0) {


        document
            .getElementById(
                "rolesError"
            )
            .style.display = "block";


        return;

    }






    document
        .getElementById(
            "rolesError"
        )
        .style.display = "none";








    const response =
        await fetch(
            '/Admin/Usuarios/EditarRoles',
            {

                method: "POST",

                headers:
                {
                    "Content-Type":
                        "application/json"
                },


                body:
                    JSON.stringify(
                        {
                            id: id,
                            roles: roles
                        })

            });






    const result =
        await response.json();





    if (result.success) {

        location.reload();

    }
    else {

        alert(
            result.message ??
            "No se pudieron guardar los roles."
        );

    }



}









// =================================
// APROBAR USUARIO
// =================================


async function aprobarUsuario(id) {


    const confirmar =
        confirm(
            "¿Desea aprobar este usuario?"
        );



    if (!confirmar)
        return;






    const response =
        await fetch(
            '/Admin/Usuarios/Aprobar',
            {


                method: "POST",


                headers:
                {
                    "Content-Type":
                        "application/json"
                },


                body:
                    JSON.stringify(
                        {
                            id: id
                        })


            });








    const result =
        await response.json();






    if (result.success) {

        alert(
            "Usuario aprobado correctamente."
        );


        location.reload();

    }
    else {

        alert(
            result.message ??
            "Error al aprobar usuario."
        );

    }



}









// =================================
// RECHAZAR USUARIO
// =================================


async function rechazarUsuario(id) {


    const confirmar =
        confirm(
            "¿Desea rechazar este usuario?"
        );



    if (!confirmar)
        return;







    const response =
        await fetch(
            '/Admin/Usuarios/Rechazar',
            {


                method: "POST",


                headers:
                {
                    "Content-Type":
                        "application/json"
                },


                body:
                    JSON.stringify(
                        {
                            id: id
                        })


            });









    const result =
        await response.json();






    if (result.success) {

        alert(
            "Usuario rechazado correctamente."
        );


        location.reload();

    }
    else {

        alert(
            result.message ??
            "Error al rechazar usuario."
        );

    }



}









// =================================
// ABRIR MODAL ELIMINAR
// =================================


function abrirEliminar(id, nombre) {


    document.getElementById(
        "eliminarUsuarioId"
    ).value = id;




    document.getElementById(
        "eliminarUsuarioNombre"
    ).textContent = nombre;




    document
        .getElementById(
            "eliminarError"
        )
        .style.display = "none";



    const modalEliminar =
        document.getElementById(
            "modalEliminar"
        );

    modalEliminar.classList.add("visible");
    modalEliminar.style.display = "flex";



}









// =================================
// CERRAR MODAL ELIMINAR
// =================================


function cerrarModalEliminar() {


    const modalEliminar =
        document.getElementById(
            "modalEliminar"
        );

    modalEliminar.classList.remove("visible");
    modalEliminar.style.display = "none";




    document.getElementById(
        "eliminarUsuarioId"
    ).value = "";


}









// =================================
// CONFIRMAR ELIMINAR USUARIO
// =================================


async function confirmarEliminarUsuario() {


    const id =
        document.getElementById(
            "eliminarUsuarioId"
        ).value;



    if (id === "")
        return;






    const boton =
        document.getElementById(
            "btnConfirmarEliminar"
        );



    boton.disabled = true;




    document
        .getElementById(
            "eliminarError"
        )
        .style.display = "none";








    const response =
        await fetch(
            '/Admin/Usuarios/Eliminar',
            {


                method: "POST",


                headers:
                {
                    "Content-Type":
                        "application/json"
                },


                body:
                    JSON.stringify(
                        {
                            id: id
                        })


            });








    const result =
        await response.json();






    if (result.success) {

        alert(
            "Usuario eliminado correctamente."
        );

        location.reload();

    }
    else {


        document
            .getElementById(
                "eliminarError"
            )
            .textContent =
            result.message ??
            "No se pudo eliminar el usuario.";



        document
            .getElementById(
                "eliminarError"
            )
            .style.display = "block";



        boton.disabled = false;

    }



}