// ============================================
// INICIALIZACIÓN Y ANIMACIONES
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    mostrarPagina();
    initializeModals();
    initializeCalculoDias();
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
    const tabla = document.getElementById('laTablaDeSolicitudes');
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
    const tabla = document.getElementById('laTablaDeSolicitudes');
    const tbody = tabla ? tabla.querySelector('tbody') : null;
    if (!tbody) return;

    const todasLasFilas = tbody.querySelectorAll('tr');
    const totalRegistros = todasLasFilas.length;

    const inicio = (paginaActual - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;

    todasLasFilas.forEach((fila, index) => {
        if (index >= inicio && index < fin) {
            fila.style.display = '';
        } else {
            fila.style.display = 'none';
        }
    });

    const totalPaginas = Math.ceil(totalRegistros / registrosPorPagina);

    const startRecord = document.getElementById('startRecord');
    const endRecord = document.getElementById('endRecord');
    const totalRecordsEl = document.getElementById('totalRecords');

    if (startRecord) startRecord.textContent = totalRegistros > 0 ? inicio + 1 : 0;
    if (endRecord) endRecord.textContent = Math.min(fin, totalRegistros);
    if (totalRecordsEl) totalRecordsEl.textContent = totalRegistros;

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
// CÁLCULO DE DÍAS
// ============================================
function initializeCalculoDias() {
    const fechaInicio = document.querySelector('input[name="FechaInicio"]');
    const fechaFin = document.querySelector('input[name="FechaFin"]');
    const diasCalculados = document.getElementById('diasCalculados');

    if (fechaInicio && fechaFin && diasCalculados) {
        function calcularDias() {
            if (fechaInicio.value && fechaFin.value) {
                const inicio = new Date(fechaInicio.value);
                const fin = new Date(fechaFin.value);

                if (fin >= inicio) {
                    let dias = 0;
                    let fechaActual = new Date(inicio);

                    while (fechaActual <= fin) {
                        // Excluir domingos (0 = domingo)
                        if (fechaActual.getDay() !== 0) {
                            dias++;
                        }
                        fechaActual.setDate(fechaActual.getDate() + 1);
                    }

                    diasCalculados.textContent = dias;

                    // Actualizar campo hidden si existe
                    const diasSolicitadosInput = document.querySelector('input[name="DiasSolicitados"]');
                    if (diasSolicitadosInput) {
                        diasSolicitadosInput.value = dias;
                    }
                } else {
                    diasCalculados.textContent = '0';
                }
            }
        }

        fechaInicio.addEventListener('change', calcularDias);
        fechaFin.addEventListener('change', calcularDias);

        // Calcular al cargar si ya hay fechas
        calcularDias();
    }
}

// ============================================
// MODALES
// ============================================
function initializeModals() {
    // ============================================
    // MODAL DETALLES SOLICITUD
    // ============================================
    const botonesDetalles = document.querySelectorAll('.btn-detalles');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const identificacion = this.getAttribute('data-identificacion');
            const cargo = this.getAttribute('data-cargo');
            const fechaSolicitud = this.getAttribute('data-fechasolicitud');
            const fechaInicio = this.getAttribute('data-fechainicio');
            const fechaFin = this.getAttribute('data-fechafin');
            const diasSolicitados = this.getAttribute('data-diassolicitados');
            const diasDisponibles = this.getAttribute('data-diasdisponibles');
            const diasTemporales = this.getAttribute('data-diastemporales');
            const estado = this.getAttribute('data-estado');
            const fechaRespuesta = this.getAttribute('data-fecharespuesta');

            document.getElementById('detalles-id-solicitud').textContent = id;
            document.getElementById('detalles-nombre-empleado').textContent = nombre;
            document.getElementById('detalles-identificacion-empleado').textContent = identificacion;
            document.getElementById('detalles-cargo-empleado').textContent = cargo;
            document.getElementById('detalles-fechasolicitud').textContent = fechaSolicitud;
            document.getElementById('detalles-fechainicio').textContent = fechaInicio;
            document.getElementById('detalles-fechafin').textContent = fechaFin;
            document.getElementById('detalles-diassolicitados').textContent = diasSolicitados;
            document.getElementById('detalles-diasdisponibles').textContent = diasDisponibles;
            document.getElementById('detalles-diastemporales').textContent = diasTemporales;
            document.getElementById('detalles-fecharespuesta').textContent = fechaRespuesta;

            const estadoBadge = document.getElementById('detalles-estado-badge');
            estadoBadge.textContent = estado;

            if (estado === 'Aprobada') {
                estadoBadge.style.background = '#27ae60';
                estadoBadge.style.color = 'white';
            } else if (estado === 'Rechazada') {
                estadoBadge.style.background = '#e74c3c';
                estadoBadge.style.color = 'white';
            } else {
                estadoBadge.style.background = '#f39c12';
                estadoBadge.style.color = 'white';
            }
        });
    });

    // ============================================
    // MODAL ELIMINAR SOLICITUD
    // ============================================
    const botonesEliminar = document.querySelectorAll('.btn-eliminar');

    botonesEliminar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const periodo = this.getAttribute('data-periodo');
            const diasSolicitados = this.getAttribute('data-diassolicitados');
            const estado = this.getAttribute('data-estado');

            document.getElementById('eliminar-id-display').textContent = id;
            document.getElementById('eliminar-id-solicitud').value = id;
            document.getElementById('eliminar-nombre-empleado').textContent = nombre;
            document.getElementById('eliminar-periodo').textContent = periodo;
            document.getElementById('eliminar-diassolicitados').textContent = diasSolicitados + ' días';

            const estadoBadge = document.getElementById('eliminar-estado-badge');
            estadoBadge.textContent = estado;

            if (estado === 'Aprobada') {
                estadoBadge.style.background = '#27ae60';
            } else if (estado === 'Rechazada') {
                estadoBadge.style.background = '#e74c3c';
            } else {
                estadoBadge.style.background = '#f39c12';
            }

            const form = document.getElementById('formEliminarSolicitud');
            form.action = '/SolicitudVacaciones/EliminarSolicitud/' + id;
        });
    });

    // ============================================
    // MODAL APROBAR SOLICITUD
    // ============================================
    const botonesAprobar = document.querySelectorAll('.btn-aprobar');

    botonesAprobar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const periodo = this.getAttribute('data-periodo');
            const diasSolicitados = this.getAttribute('data-diassolicitados');

            document.getElementById('aprobar-id-solicitud').value = id;
            document.getElementById('aprobar-nombre-empleado').textContent = nombre;
            document.getElementById('aprobar-periodo').textContent = periodo;
            document.getElementById('aprobar-diassolicitados').textContent = diasSolicitados + ' días laborables';

            const form = document.getElementById('formAprobarSolicitud');
            form.action = '/SolicitudVacaciones/AprobarSolicitud/' + id;
        });
    });

    // ============================================
    // MODAL RECHAZAR SOLICITUD
    // ============================================
    const botonesRechazar = document.querySelectorAll('.btn-rechazar');

    botonesRechazar.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            const periodo = this.getAttribute('data-periodo');

            document.getElementById('rechazar-id-solicitud').value = id;
            document.getElementById('rechazar-nombre-empleado').textContent = nombre;
            document.getElementById('rechazar-periodo').textContent = periodo;

            const form = document.getElementById('formRechazarSolicitud');
            form.action = '/SolicitudVacaciones/RechazarSolicitud/' + id;
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

console.log('%c✓ Script de solicitudes de vacaciones cargado correctamente', 'color: #27ae60; font-weight: bold;');