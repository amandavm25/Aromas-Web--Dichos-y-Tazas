// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    // Animar elementos al cargar
    animateOnLoad();

    // Inicializar tooltips
    initializeTooltips();

    // Efectos hover en filas
    initializeTableHoverEffects();

    // Inicializar paginación
    mostrarPagina();

    // Inicializar modales
    initializeModals();

    // Validación de contraseñas
    validarContrasenas();
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    // Animar cards de estadísticas
    const statCards = document.querySelectorAll('[style*="linear-gradient(135deg, #"]');
    statCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px) scale(0.9)';

        setTimeout(() => {
            card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0) scale(1)';
        }, index * 100);
    });

    // Animar tabla
    const tableRows = document.querySelectorAll('tbody tr');
    tableRows.forEach((row, index) => {
        row.style.opacity = '0';
        row.style.transform = 'translateX(-20px)';

        setTimeout(() => {
            row.style.transition = 'all 0.4s ease';
            row.style.opacity = '1';
            row.style.transform = 'translateX(0)';
        }, 300 + (index * 50));
    });
}

// ============================================
// EFECTOS HOVER MEJORADOS
// ============================================
function initializeTableHoverEffects() {
    const tables = document.querySelectorAll('table');

    tables.forEach(table => {
        const tbody = table.querySelector('tbody');
        if (!tbody) return;

        const rows = tbody.querySelectorAll('tr');

        rows.forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
            });

            row.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        });
    });
}

// ============================================
// TOOLTIPS PERSONALIZADOS
// ============================================
function initializeTooltips() {
    const elementsWithTitle = document.querySelectorAll('[title]');

    elementsWithTitle.forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);

        element.addEventListener('mouseenter', function (e) {
            showTooltip(e, title);
        });

        element.addEventListener('mouseleave', function () {
            hideTooltip();
        });
    });
}

let tooltipElement = null;

function showTooltip(event, text) {
    hideTooltip();

    tooltipElement = document.createElement('div');
    tooltipElement.textContent = text;
    tooltipElement.style.cssText = `
        position: fixed;
        background: linear-gradient(135deg, var(--dark-teal), var(--teal));
        color: white;
        padding: 0.7rem 1.2rem;
        border-radius: 12px;
        font-size: 0.9rem;
        z-index: 10000;
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
        pointer-events: none;
        white-space: nowrap;
        animation: tooltipFadeIn 0.3s ease;
    `;

    document.body.appendChild(tooltipElement);

    const rect = tooltipElement.getBoundingClientRect();
    tooltipElement.style.left = (event.clientX - rect.width / 2) + 'px';
    tooltipElement.style.top = (event.clientY - rect.height - 10) + 'px';
}

function hideTooltip() {
    if (tooltipElement) {
        tooltipElement.style.animation = 'tooltipFadeOut 0.2s ease';
        setTimeout(() => {
            if (tooltipElement && tooltipElement.parentNode) {
                tooltipElement.remove();
            }
            tooltipElement = null;
        }, 200);
    }
}

// ============================================
// PAGINACIÓN
// ============================================
let paginaActual = 1;
const registrosPorPagina = 10;

function paginaAnterior() {
    if (paginaActual > 1) {
        paginaActual--;
        mostrarPagina();
    }
}

function paginaSiguiente() {
    const tabla = document.getElementById('laTablaDeClientes');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const filas = Array.from(tbody.querySelectorAll('tr')).filter(fila => fila.style.display !== 'none');
    const totalPaginas = Math.ceil(filas.length / registrosPorPagina);

    if (paginaActual < totalPaginas) {
        paginaActual++;
        mostrarPagina();
    }
}

function mostrarPagina() {
    const tabla = document.getElementById('laTablaDeClientes');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const todasLasFilas = tbody.querySelectorAll('tr');
    const filasVisibles = Array.from(todasLasFilas).filter(fila => fila.style.display !== 'none');

    const inicio = (paginaActual - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;

    todasLasFilas.forEach((fila, index) => {
        if (index >= inicio && index < fin) {
            fila.style.display = '';
        } else {
            fila.style.display = 'none';
        }
    });

    // Actualizar información de paginación
    const totalRegistros = filasVisibles.length;
    const totalPaginas = Math.ceil(totalRegistros / registrosPorPagina);

    const startRecord = document.getElementById('startRecord');
    const endRecord = document.getElementById('endRecord');
    const totalRecordsEl = document.getElementById('totalRecords');

    if (startRecord) startRecord.textContent = totalRegistros > 0 ? inicio + 1 : 0;
    if (endRecord) endRecord.textContent = Math.min(fin, totalRegistros);
    if (totalRecordsEl) totalRecordsEl.textContent = totalRegistros;

    // Deshabilitar botones según corresponda
    const btnAnterior = document.getElementById('btnAnterior');
    const btnSiguiente = document.getElementById('btnSiguiente');

    if (btnAnterior) {
        btnAnterior.disabled = paginaActual === 1;
        btnAnterior.style.opacity = paginaActual === 1 ? '0.5' : '1';
        btnAnterior.style.cursor = paginaActual === 1 ? 'not-allowed' : 'pointer';
    }

    if (btnSiguiente) {
        btnSiguiente.disabled = paginaActual === totalPaginas || totalRegistros === 0;
        btnSiguiente.style.opacity = (paginaActual === totalPaginas || totalRegistros === 0) ? '0.5' : '1';
        btnSiguiente.style.cursor = (paginaActual === totalPaginas || totalRegistros === 0) ? 'not-allowed' : 'pointer';
    }
}

// ============================================
// MODAL DETALLES CLIENTE
// ============================================
function initializeModals() {
    const botonesDetalles = document.querySelectorAll('.btn-detalles');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const identificacion = this.getAttribute('data-identificacion');
            const correo = this.getAttribute('data-correo');
            const telefono = this.getAttribute('data-telefono');
            const ultimaReserva = this.getAttribute('data-ultimareserva');
            const estado = this.getAttribute('data-estado');
            const esFrecuente = this.getAttribute('data-esfrecuente') === 'True';

            // Llenar el modal con los datos
            document.getElementById('detalles-id-cliente').textContent = id;
            document.getElementById('detalles-nombre-cliente').textContent = nombre;
            document.getElementById('detalles-identificacion-cliente').textContent = identificacion;
            document.getElementById('detalles-correo-cliente').textContent = correo;
            document.getElementById('detalles-telefono-cliente').textContent = telefono;
            document.getElementById('detalles-ultimareserva-cliente').textContent = ultimaReserva;

            // Tipo de cliente
            const tipoClienteEl = document.querySelector('#detalles-tipo-cliente span');
            if (esFrecuente) {
                tipoClienteEl.textContent = 'Cliente Frecuente';
                tipoClienteEl.parentElement.style.display = 'flex';
            } else {
                tipoClienteEl.parentElement.style.display = 'none';
            }

            // Estado badge
            const estadoBadge = document.getElementById('detalles-estado-badge-cliente');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = 'var(--green)';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = 'var(--red)';
                estadoBadge.style.color = 'white';
            }
        });
    });

    // ============================================
    // MODAL ELIMINAR CLIENTE
    // ============================================
    const botonesEliminar = document.querySelectorAll('.btn-eliminar');

    botonesEliminar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const identificacion = this.getAttribute('data-identificacion');
            const correo = this.getAttribute('data-correo');
            const estado = this.getAttribute('data-estado');

            // Llenar el modal con los datos
            document.getElementById('eliminar-id-display-cliente').textContent = id;
            document.getElementById('eliminar-id-cliente').value = id;
            document.getElementById('eliminar-nombre-cliente').textContent = nombre;
            document.getElementById('eliminar-identificacion-cliente').textContent = identificacion;
            document.getElementById('eliminar-correo-cliente').textContent = correo;

            // Estado badge
            const estadoBadge = document.getElementById('eliminar-estado-badge-cliente');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = 'var(--green)';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = 'var(--red)';
            }

            // Actualizar acción del formulario
            const form = document.getElementById('formEliminarCliente');
            form.action = '/Cliente/EliminarCliente/' + id;
        });
    });
}

// ============================================
// VALIDACIÓN DE CONTRASEÑAS
// ============================================
function validarContrasenas() {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const contrasena = this.querySelector('input[name="Contrasena"]');
            const confirmarContrasena = this.querySelector('input[name="ConfirmarContrasena"]');
            const contrasenaNueva = this.querySelector('input[name="ContrasenaNueva"]');
            const confirmarContrasenaNueva = this.querySelector('input[name="ConfirmarContrasenaNueva"]');

            // Validar contraseña en crear
            if (contrasena && confirmarContrasena) {
                if (contrasena.value !== confirmarContrasena.value) {
                    e.preventDefault();
                    mostrarNotificacion('Las contraseñas no coinciden', 'error');
                    contrasena.style.borderColor = 'var(--red)';
                    confirmarContrasena.style.borderColor = 'var(--red)';
                    return;
                }

                if (contrasena.value.length < 8 && contrasena.value.length > 0) {
                    e.preventDefault();
                    mostrarNotificacion('La contraseña debe tener al menos 8 caracteres', 'error');
                    contrasena.style.borderColor = 'var(--red)';
                    return;
                }
            }

            // Validar contraseña en editar
            if (contrasenaNueva && confirmarContrasenaNueva) {
                if (contrasenaNueva.value !== confirmarContrasenaNueva.value) {
                    e.preventDefault();
                    mostrarNotificacion('Las contraseñas nuevas no coinciden', 'error');
                    contrasenaNueva.style.borderColor = 'var(--red)';
                    confirmarContrasenaNueva.style.borderColor = 'var(--red)';
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
    const passwordInputs = document.querySelectorAll('input[type="password"]');
    passwordInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.style.borderColor = '';
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
        'warning': '#f39c12',
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
    }, 3000);
}

// Animaciones para notificaciones
const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
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
    @keyframes tooltipFadeIn {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
    @keyframes tooltipFadeOut {
        from {
            opacity: 1;
            transform: translateY(0);
        }
        to {
            opacity: 0;
            transform: translateY(10px);
        }
    }
`;
document.head.appendChild(notifStyle);

console.log('%c✓ Script de clientes cargado correctamente', 'color: var(--green); font-weight: bold;');