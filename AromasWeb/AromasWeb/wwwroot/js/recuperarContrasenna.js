// ============================================
// RESTABLECER-CONTRASENA.JS
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    initPasswordToggle();
    initPasswordStrength();
    initPasswordMatch();
    initPasswordRequirements();
    initFormSubmit();
    initInputAnimations();
});

// ============================================
// TOGGLE PASSWORD VISIBILITY
// ============================================
function initPasswordToggle() {
    // Toggle para contraseña nueva
    const togglePassword = document.getElementById('toggle-password');
    const passwordInput = document.getElementById('password-input');
    const eyeIcon = document.getElementById('eye-icon');

    if (togglePassword && passwordInput) {
        togglePassword.addEventListener('click', function () {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);

            if (type === 'password') {
                eyeIcon.classList.remove('fa-eye-slash');
                eyeIcon.classList.add('fa-eye');
            } else {
                eyeIcon.classList.remove('fa-eye');
                eyeIcon.classList.add('fa-eye-slash');
            }
        });
    }

    // Toggle para confirmar contraseña
    const toggleConfirmPassword = document.getElementById('toggle-confirm-password');
    const confirmPasswordInput = document.getElementById('confirm-password-input');
    const eyeIconConfirm = document.getElementById('eye-icon-confirm');

    if (toggleConfirmPassword && confirmPasswordInput) {
        toggleConfirmPassword.addEventListener('click', function () {
            const type = confirmPasswordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            confirmPasswordInput.setAttribute('type', type);

            if (type === 'password') {
                eyeIconConfirm.classList.remove('fa-eye-slash');
                eyeIconConfirm.classList.add('fa-eye');
            } else {
                eyeIconConfirm.classList.remove('fa-eye');
                eyeIconConfirm.classList.add('fa-eye-slash');
            }
        });
    }
}

// ============================================
// PASSWORD STRENGTH INDICATOR
// ============================================
function initPasswordStrength() {
    const passwordInput = document.getElementById('password-input');
    if (!passwordInput) return;

    const strength1 = document.getElementById('strength-1');
    const strength2 = document.getElementById('strength-2');
    const strength3 = document.getElementById('strength-3');
    const strength4 = document.getElementById('strength-4');
    const strengthText = document.getElementById('strength-text');

    passwordInput.addEventListener('input', function () {
        const password = this.value;
        const strength = calculatePasswordStrength(password);

        // Reset all bars
        [strength1, strength2, strength3, strength4].forEach(bar => {
            bar.style.background = 'var(--gray-light)';
        });

        // Update based on strength
        if (strength === 0) {
            strengthText.textContent = '';
        } else if (strength === 1) {
            strength1.style.background = 'var(--red)';
            strengthText.textContent = 'Muy débil';
            strengthText.style.color = 'var(--red)';
        } else if (strength === 2) {
            strength1.style.background = 'var(--yellow)';
            strength2.style.background = 'var(--yellow)';
            strengthText.textContent = 'Débil';
            strengthText.style.color = 'var(--yellow)';
        } else if (strength === 3) {
            strength1.style.background = 'var(--yellow)';
            strength2.style.background = 'var(--yellow)';
            strength3.style.background = 'var(--yellow)';
            strengthText.textContent = 'Aceptable';
            strengthText.style.color = 'var(--yellow)';
        } else if (strength === 4) {
            strength1.style.background = 'var(--green)';
            strength2.style.background = 'var(--green)';
            strength3.style.background = 'var(--green)';
            strength4.style.background = 'var(--green)';
            strengthText.textContent = 'Fuerte';
            strengthText.style.color = 'var(--green)';
        }
    });
}

function calculatePasswordStrength(password) {
    if (password.length === 0) return 0;

    let strength = 0;

    // Longitud
    if (password.length >= 8) strength++;
    if (password.length >= 12) strength++;

    // Tiene números
    if (/\d/.test(password)) strength++;

    // Tiene mayúsculas y minúsculas
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;

    // Tiene caracteres especiales
    if (/[^a-zA-Z0-9]/.test(password)) strength++;

    return Math.min(strength, 4);
}

// ============================================
// PASSWORD MATCH VALIDATION
// ============================================
function initPasswordMatch() {
    const passwordInput = document.getElementById('password-input');
    const confirmPasswordInput = document.getElementById('confirm-password-input');
    const matchMessage = document.getElementById('password-match-message');

    if (!passwordInput || !confirmPasswordInput || !matchMessage) return;

    function checkMatch() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        if (confirmPassword.length === 0) {
            matchMessage.innerHTML = '';
            confirmPasswordInput.style.borderColor = '';
            return;
        }

        if (password === confirmPassword) {
            matchMessage.innerHTML = `
                <div style="display: flex; align-items: center; gap: 0.5rem; color: var(--green); font-size: 0.9rem;">
                    <i class="fas fa-check-circle"></i>
                    <span>Las contraseñas coinciden</span>
                </div>
            `;
            confirmPasswordInput.style.borderColor = 'var(--green)';
        } else {
            matchMessage.innerHTML = `
                <div style="display: flex; align-items: center; gap: 0.5rem; color: var(--red); font-size: 0.9rem;">
                    <i class="fas fa-times-circle"></i>
                    <span>Las contraseñas no coinciden</span>
                </div>
            `;
            confirmPasswordInput.style.borderColor = 'var(--red)';
        }
    }

    passwordInput.addEventListener('input', checkMatch);
    confirmPasswordInput.addEventListener('input', checkMatch);
}

// ============================================
// PASSWORD REQUIREMENTS CHECK
// ============================================
function initPasswordRequirements() {
    const passwordInput = document.getElementById('password-input');
    if (!passwordInput) return;

    const reqLength = document.getElementById('req-length');
    const reqUppercase = document.getElementById('req-uppercase');
    const reqLowercase = document.getElementById('req-lowercase');
    const reqNumber = document.getElementById('req-number');

    passwordInput.addEventListener('input', function () {
        const password = this.value;

        // Check length
        updateRequirement(reqLength, password.length >= 8);

        // Check uppercase
        updateRequirement(reqUppercase, /[A-Z]/.test(password));

        // Check lowercase
        updateRequirement(reqLowercase, /[a-z]/.test(password));

        // Check number
        updateRequirement(reqNumber, /\d/.test(password));
    });

    function updateRequirement(element, isValid) {
        const icon = element.querySelector('i');
        const span = element.querySelector('span');

        if (isValid) {
            icon.classList.remove('fa-circle');
            icon.classList.add('fa-check-circle');
            icon.style.color = 'var(--green)';
            icon.style.fontSize = '1rem';
            span.style.color = 'var(--green)';
            span.style.fontWeight = '600';
        } else {
            icon.classList.remove('fa-check-circle');
            icon.classList.add('fa-circle');
            icon.style.color = 'var(--gray)';
            icon.style.fontSize = '0.5rem';
            span.style.color = 'var(--gray)';
            span.style.fontWeight = '400';
        }
    }
}

// ============================================
// FORM SUBMIT
// ============================================
function initFormSubmit() {
    const btnRestablecer = document.getElementById('btn-restablecer');
    if (!btnRestablecer) return;

    btnRestablecer.addEventListener('click', function (e) {
        e.preventDefault();

        const passwordInput = document.getElementById('password-input');
        const confirmPasswordInput = document.getElementById('confirm-password-input');
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        // Validar que no estén vacíos
        if (!password || password.trim() === '') {
            mostrarNotificacion('Por favor ingresa tu nueva contraseña', 'warning');
            passwordInput.focus();
            passwordInput.style.borderColor = 'var(--yellow)';
            return;
        }

        // Validar longitud mínima
        if (password.length < 8) {
            mostrarNotificacion('La contraseña debe tener al menos 8 caracteres', 'error');
            passwordInput.focus();
            passwordInput.style.borderColor = 'var(--red)';
            return;
        }

        // Validar mayúscula
        if (!/[A-Z]/.test(password)) {
            mostrarNotificacion('La contraseña debe incluir al menos una letra mayúscula', 'error');
            passwordInput.focus();
            passwordInput.style.borderColor = 'var(--red)';
            return;
        }

        // Validar minúscula
        if (!/[a-z]/.test(password)) {
            mostrarNotificacion('La contraseña debe incluir al menos una letra minúscula', 'error');
            passwordInput.focus();
            passwordInput.style.borderColor = 'var(--red)';
            return;
        }

        // Validar número
        if (!/\d/.test(password)) {
            mostrarNotificacion('La contraseña debe incluir al menos un número', 'error');
            passwordInput.focus();
            passwordInput.style.borderColor = 'var(--red)';
            return;
        }

        // Validar confirmación
        if (password !== confirmPassword) {
            mostrarNotificacion('Las contraseñas no coinciden', 'error');
            confirmPasswordInput.focus();
            confirmPasswordInput.style.borderColor = 'var(--red)';
            return;
        }

        // Todo está bien, procesar
        const originalText = this.innerHTML;
        this.innerHTML = '<span><i class="fas fa-spinner fa-spin"></i> Restableciendo...</span>';
        this.disabled = true;

        // Simular proceso (aquí iría la lógica real)
        setTimeout(() => {
            mostrarNotificacion('¡Contraseña restablecida exitosamente!', 'success');

            setTimeout(() => {
                window.location.href = '/Auth/Login';
            }, 2000);
        }, 1500);
    });
}

// ============================================
// INPUT ANIMATIONS
// ============================================
function initInputAnimations() {
    const inputs = document.querySelectorAll('.form-control');

    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.style.transform = 'scale(1.02)';
            this.style.boxShadow = '0 0 0 4px rgba(32, 116, 118, 0.1)';
        });

        input.addEventListener('blur', function () {
            this.style.transform = '';
            this.style.boxShadow = '';
        });

        // Limpiar borde de error al escribir
        input.addEventListener('input', function () {
            if (this.style.borderColor === 'rgb(231, 76, 60)' || this.style.borderColor === 'rgb(243, 156, 18)') {
                this.style.borderColor = '';
            }
        });
    });
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = {
        'success': 'fa-check-circle',
        'error': 'fa-exclamation-triangle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    };

    const colores = {
        'success': 'var(--green)',
        'error': 'var(--red)',
        'warning': 'var(--yellow)',
        'info': '#3498db'
    };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 100px;
        right: 20px;
        background: ${colores[tipo]};
        color: white;
        padding: 1.5rem 2rem;
        border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        animation: slideIn 0.3s ease;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 1rem;
        min-width: 300px;
    `;

    notification.innerHTML = `
        <i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i>
        <span>${mensaje}</span>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 4000);
}

// Animaciones
const style = document.createElement('style');
style.innerHTML = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

console.log('%c✓ Script de restablecer contraseña cargado correctamente', 'color: var(--green); font-weight: bold;');