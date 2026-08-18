document.querySelectorAll('.auth-toggle-password').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var input = document.getElementById(btn.dataset.target);
        var icon = btn.querySelector('i');
        if (input.type === 'password') {
            input.type = 'text';
            icon.classList.remove('bi-eye');
            icon.classList.add('bi-eye-slash');
        } else {
            input.type = 'password';
            icon.classList.remove('bi-eye-slash');
            icon.classList.add('bi-eye');
        }
    });
});

document.querySelectorAll('.auth-clear').forEach(function (btn) {
    btn.addEventListener('click', function () {
        var input = document.getElementById(btn.dataset.target);
        input.value = '';
        input.focus();
    });
});

var loginForm = document.getElementById('loginForm');

if (loginForm) {
    loginForm.addEventListener('submit', function (e) {
        var valido = true;

        document.querySelectorAll('.auth-error').forEach(function (el) {
            el.innerHTML = '';
        });

        var email = document.getElementById('Email').value.trim();
        var password = document.getElementById('Password').value;
        var regexCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email === '') {
            document.getElementById('errEmail').innerHTML = 'Ingrese su correo';
            valido = false;
        } else if (!regexCorreo.test(email)) {
            document.getElementById('errEmail').innerHTML = 'Ingrese un correo válido';
            valido = false;
        }

        if (password === '') {
            document.getElementById('errPassword').innerHTML = 'Ingrese su contraseña';
            valido = false;
        }

        if (!valido) {
            e.preventDefault();
        }
    });
}