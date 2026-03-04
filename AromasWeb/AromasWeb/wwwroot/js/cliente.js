// cliente.js - Gestión de clientes
document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (usa initTablePagination de site.js)
    // ============================================
    if (document.getElementById('laTablaDeClientes')) {
        initTablePagination({
            tableId: 'laTablaDeClientes',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // ============================================
    // HOVER EFFECTS EN TABLA
    // ============================================
    const tabla = document.getElementById('laTablaDeClientes');
    if (tabla) {
        tabla.querySelectorAll('tbody tr').forEach(fila => {
            fila.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
                this.style.transition = 'all 0.2s ease';
            });
            fila.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        });
    }

    // ============================================
    // ANIMACIÓN DE ENTRADA
    // ============================================
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    document.querySelectorAll('.feature-card, .admin-form-wrapper').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // ============================================
    // MODAL DETALLES CLIENTE
    // ============================================
    document.querySelectorAll('.btn-detalles-cliente').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };

            set('detalles-id-cliente', '#' + this.dataset.id);
            set('detalles-nombre-cliente', this.dataset.nombre + ' ' + this.dataset.apellidos);
            set('detalles-identificacion-cliente', this.dataset.identificacion);
            set('detalles-correo-cliente', this.dataset.correo);
            set('detalles-telefono-cliente', this.dataset.telefono);
            set('detalles-fecha-cliente', this.dataset.fecha);

            const badge = document.getElementById('detalles-estado-badge-cliente');
            if (badge) {
                const estado = this.dataset.estado;
                badge.textContent = estado;
                badge.style.background = estado === 'Activo' ? 'var(--green)' : 'var(--red)';
                badge.style.color = 'white';
            }
        });
    });

    // ============================================
    // VALIDACIÓN DE CONTRASEÑAS
    // ============================================
    validarContrasenas();
});

// ============================================
// VALIDACIÓN DE CONTRASEÑAS
// ============================================
function validarContrasenas() {
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function (e) {
            const contrasena             = this.querySelector('input[name="Contrasena"]');
            const confirmarContrasena    = this.querySelector('input[name="ConfirmarContrasena"]');
            const contrasenaNueva        = this.querySelector('input[name="ContrasenaNueva"]');
            const confirmarNueva         = this.querySelector('input[name="ConfirmarContrasenaNueva"]');

            if (contrasena && confirmarContrasena) {
                if (contrasena.value !== confirmarContrasena.value) {
                    e.preventDefault();
                    mostrarNotificacion('Las contraseñas no coinciden', 'error');
                    contrasena.style.borderColor = 'var(--red)';
                    confirmarContrasena.style.borderColor = 'var(--red)';
                    return;
                }
                if (contrasena.value.length > 0 && contrasena.value.length < 8) {
                    e.preventDefault();
                    mostrarNotificacion('La contraseña debe tener al menos 8 caracteres', 'error');
                    contrasena.style.borderColor = 'var(--red)';
                    return;
                }
            }

            if (contrasenaNueva && confirmarNueva) {
                if (contrasenaNueva.value !== confirmarNueva.value) {
                    e.preventDefault();
                    mostrarNotificacion('Las contraseñas nuevas no coinciden', 'error');
                    contrasenaNueva.style.borderColor = 'var(--red)';
                    confirmarNueva.style.borderColor = 'var(--red)';
                    return;
                }
                if (contrasenaNueva.value.length > 0 && contrasenaNueva.value.length < 8) {
                    e.preventDefault();
                    mostrarNotificacion('La contraseña nueva debe tener al menos 8 caracteres', 'error');
                    contrasenaNueva.style.borderColor = 'var(--red)';
                    return;
                }
            }
        });
    });

    // Limpiar borde rojo al escribir
    document.querySelectorAll('input[type="password"]').forEach(input => {
        input.addEventListener('input', function () {
            this.style.borderColor = '';
        });
    });
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos  = { success: 'fa-check-circle', error: 'fa-exclamation-triangle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
    const colores = { success: 'var(--green)', error: 'var(--red)', warning: 'var(--yellow)', info: 'var(--gold)' };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed; top: 100px; right: 20px;
        background: ${colores[tipo]}; color: white;
        padding: 1.5rem 2rem; border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001; animation: slideIn 0.3s ease;
        font-weight: 600; display: flex; align-items: center; gap: 1rem; min-width: 300px;
    `;
    notification.innerHTML = `<i class="fas ${iconos[tipo]}" style="font-size: 1.5rem;"></i><span>${mensaje}</span>`;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// ============================================
// ESTILOS DE ANIMACIÓN
// ============================================
if (!document.getElementById('cliente-styles')) {
    const s = document.createElement('style');
    s.id = 'cliente-styles';
    s.innerHTML = `
        @keyframes slideIn { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOut { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
    `;
    document.head.appendChild(s);
}