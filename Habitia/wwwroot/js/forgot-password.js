document.querySelectorAll('.auth-clear').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var input = document.getElementById(btn.dataset.target);
        input.value = '';
        input.focus();
    });
});

var forgotForm = document.getElementById('forgotForm');

if (forgotForm) {
    forgotForm.addEventListener('submit', function (e) {
        var valido = true;

        document.querySelectorAll('.auth-error').forEach(function (el) {
            el.innerHTML = '';
        });

        var email = document.getElementById('Email').value.trim();
        var regexCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email === '') {
            document.getElementById('errEmail').innerHTML = 'Ingrese su correo';
            valido = false;
        } else if (!regexCorreo.test(email)) {
            document.getElementById('errEmail').innerHTML = 'Ingrese un correo válido';
            valido = false;
        }

        if (!valido) {
            e.preventDefault();
        }
    });
}