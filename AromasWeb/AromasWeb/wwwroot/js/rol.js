// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    mostrarPagina();
    initializeModals();
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
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
    const tabla = document.getElementById('laTablaDeRoles');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const todasLasFilas = tbody.querySelectorAll('tr');
    const totalPaginas = Math.ceil(todasLasFilas.length / registrosPorPagina);

    if (paginaActual < totalPaginas) {
        paginaActual++;
        mostrarPagina();
    }
}

function mostrarPagina() {
    const tabla = document.getElementById('laTablaDeRoles');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const todasLasFilas = tbody.querySelectorAll('tr');
    const totalRegistros = todasLasFilas.length;

    const inicio = (paginaActual - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;

    // Mostrar/ocultar filas según la página actual
    todasLasFilas.forEach((fila, index) => {
        if (index >= inicio && index < fin) {
            fila.style.display = '';
        } else {
            fila.style.display = 'none';
        }
    });

    // Actualizar información de paginación
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
// MODAL DETALLES ROL
// ============================================
function initializeModals() {
    const botonesDetalles = document.querySelectorAll('.btn-detalles');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const cantidadEmpleados = this.getAttribute('data-cantidadempleados');
            const fechaRegistro = this.getAttribute('data-fecharegist ro');
            const estado = this.getAttribute('data-estado');

            document.getElementById('detalles-id-rol').textContent = id;
            document.getElementById('detalles-nombre-rol').textContent = nombre;
            document.getElementById('detalles-descripcion-rol').textContent = descripcion;
            document.getElementById('detalles-cantidadempleados-rol').textContent = cantidadEmpleados + ' empleado(s)';
            document.getElementById('detalles-fecharegist ro-rol').textContent = fechaRegistro;

            const estadoBadge = document.getElementById('detalles-estado-badge-rol');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = '#27ae60';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = '#e74c3c';
                estadoBadge.style.color = 'white';
            }
        });
    });

    // ============================================
    // MODAL ELIMINAR ROL
    // ============================================
    const botonesEliminar = document.querySelectorAll('.btn-eliminar');

    botonesEliminar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const descripcion = this.getAttribute('data-descripcion');
            const cantidadEmpleados = this.getAttribute('data-cantidadempleados');
            const estado = this.getAttribute('data-estado');

            document.getElementById('eliminar-id-display-rol').textContent = id;
            document.getElementById('eliminar-id-rol').value = id;
            document.getElementById('eliminar-nombre-rol').textContent = nombre;
            document.getElementById('eliminar-descripcion-rol').textContent = descripcion;
            document.getElementById('eliminar-cantidadempleados-rol').textContent = cantidadEmpleados + ' empleado(s)';

            const estadoBadge = document.getElementById('eliminar-estado-badge-rol');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = '#27ae60';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = '#e74c3c';
            }

            const form = document.getElementById('formEliminarRol');
            form.action = '/Rol/EliminarRol/' + id;

            // Validar si tiene empleados asignados
            if (parseInt(cantidadEmpleados) > 0) {
                const submitButton = form.querySelector('button[type="submit"]');
                submitButton.disabled = true;
                submitButton.style.opacity = '0.5';
                submitButton.style.cursor = 'not-allowed';
                mostrarNotificacion('No se puede eliminar un rol con empleados asignados', 'warning');
            } else {
                const submitButton = form.querySelector('button[type="submit"]');
                submitButton.disabled = false;
                submitButton.style.opacity = '1';
                submitButton.style.cursor = 'pointer';
            }
        });
    });

    // ============================================
    // MODAL CAMBIAR ESTADO
    // ============================================
    const botonesEstado = document.querySelectorAll('.btn-estado');

    botonesEstado.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const estado = this.getAttribute('data-estado');

            document.getElementById('estado-id-rol').value = id;
            document.getElementById('estado-nombre-rol').textContent = nombre;

            const estadoBadge = document.getElementById('estado-actual-badge');
            if (estado === 'Activo') {
                estadoBadge.textContent = 'Activo';
                estadoBadge.style.background = '#27ae60';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.textContent = 'Inactivo';
                estadoBadge.style.background = '#e74c3c';
                estadoBadge.style.color = 'white';
            }

            const form = document.getElementById('formCambiarEstado');
            form.action = '/Rol/CambiarEstado/' + id;
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
        'success': '#27ae60',
        'error': '#e74c3c',
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

console.log('%c✓ Script de roles cargado correctamente', 'color: #27ae60; font-weight: bold;');