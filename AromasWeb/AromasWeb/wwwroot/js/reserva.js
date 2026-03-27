// ============================================
// CONSTANTE DE POLÍTICA
// ============================================
const DIAS_MINIMOS_CANCELACION = 2;

// ============================================
// INICIALIZACIÓN
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    animateOnLoad();
    initializeTooltips();
    initializeTableHoverEffects();
    initializeModals();

    // Paginación de tabla principal (ListadoReservas)
    if (document.getElementById('laTablaDeReservas')) {
        initTablePagination({
            tableId: 'laTablaDeReservas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación de tabla historial (HistorialReservas usa id distinto)
    if (document.getElementById('laTablaDeHistorialReservas')) {
        initTablePagination({
            tableId: 'laTablaDeHistorialReservas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación de cards (MisReservas)
    if (document.getElementById('historial-reservas-timeline')) {
        initCardsPagination({
            containerId: 'historial-reservas-timeline',
            cardsPerPage: 3,
            prevButtonId: 'btnAnteriorCards',
            nextButtonId: 'btnSiguienteCards',
            startCardId: 'startCard',
            endCardId: 'endCard',
            totalCardsId: 'totalCards'
        });
    }
});

// ============================================
// ANIMACIONES AL CARGAR
// ============================================
function animateOnLoad() {
    document.querySelectorAll('[style*="linear-gradient(135deg,"]').forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px) scale(0.9)';
        setTimeout(() => {
            card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0) scale(1)';
        }, index * 100);
    });
}

// ============================================
// EFECTOS HOVER EN TABLA
// ============================================
function initializeTableHoverEffects() {
    document.querySelectorAll('table tbody tr').forEach(row => {
        row.addEventListener('mouseenter', function () {
            this.style.background = 'linear-gradient(90deg, rgba(143, 142, 106, 0.05) 0%, transparent 100%)';
            this.style.transform = 'translateX(5px)';
            this.style.boxShadow = '0 4px 12px rgba(0,0,0,0.08)';
        });
        row.addEventListener('mouseleave', function () {
            this.style.background = '';
            this.style.transform = '';
            this.style.boxShadow = '';
        });
    });
}

// ============================================
// TOOLTIPS
// ============================================
function initializeTooltips() {
    document.querySelectorAll('[title]').forEach(element => {
        const title = element.getAttribute('title');
        element.removeAttribute('title');
        element.setAttribute('data-tooltip', title);
        element.addEventListener('mouseenter', (e) => showTooltip(e, title));
        element.addEventListener('mouseleave', hideTooltip);
    });
}

let tooltipElement = null;

function showTooltip(event, text) {
    hideTooltip();
    tooltipElement = document.createElement('div');
    tooltipElement.textContent = text;
    tooltipElement.style.cssText = `
        position: fixed;
        background: linear-gradient(135deg, var(--dark-green), var(--olive-green));
        color: white; padding: 0.7rem 1.2rem; border-radius: 12px;
        font-size: 0.9rem; z-index: 10000;
        box-shadow: 0 8px 24px rgba(0,0,0,0.2);
        pointer-events: none; white-space: nowrap;
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
            if (tooltipElement && tooltipElement.parentNode) tooltipElement.remove();
            tooltipElement = null;
        }, 200);
    }
}

// ============================================
// MODALES
// ============================================
function initializeModals() {

    // ── Modal de detalles ──────────────────────────────────────────────
    document.querySelectorAll('.btn-detalles-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };

            // Usar getAttribute para evitar colisión de 'id' con propiedad DOM del elemento
            const idReserva = this.getAttribute('data-id');

            set('detalles-cliente-reserva', this.dataset.cliente);
            set('detalles-telefono-text-reserva', this.dataset.telefono); // id correcto del HTML del modal
            set('detalles-id-reserva', '#' + idReserva);
            set('detalles-fecha-reserva', this.dataset.fecha);
            set('detalles-hora-reserva', this.dataset.hora);
            set('detalles-personas-reserva', this.dataset.personas + ' persona(s)');

            // Observaciones
            const observaciones = this.dataset.observaciones;
            const elObs = document.getElementById('detalles-observaciones-reserva');
            const elObsCont = document.getElementById('detalles-observaciones-container');
            if (elObs) {
                elObs.textContent = observaciones || 'Sin observaciones';
                elObs.style.opacity = observaciones ? '1' : '0.5';
            }
            if (elObsCont) {
                elObsCont.style.display = observaciones ? 'block' : 'none';
            }

            // Badge de estado
            const estadoBadge = document.getElementById('detalles-estado-badge-reserva');
            if (estadoBadge) {
                const estado = this.dataset.estado;
                const config = {
                    'Pendiente': { icon: 'fa-clock', bg: 'var(--yellow)' },
                    'Confirmada': { icon: 'fa-check-circle', bg: 'var(--green)' },
                    'Completada': { icon: 'fa-check-double', bg: 'var(--gold)' },
                    'Cancelada': { icon: 'fa-ban', bg: 'var(--red)' }
                };
                const c = config[estado] || { icon: 'fa-question-circle', bg: 'var(--gray)' };
                estadoBadge.innerHTML = `<i class="fas ${c.icon}"></i> ${estado}`;
                estadoBadge.style.cssText = `
                    background: ${c.bg}; color: white; font-size: 1rem;
                    padding: 0.6rem 1.5rem; border-radius: 50px;
                    display: inline-flex; align-items: center; gap: 0.5rem; font-weight: 600;
                `;
            }
        });
    });

    // ── Modal cancelar EMPLEADO (ListadoReservas y HistorialReservas) ──
    document.querySelectorAll('.btn-cancelar-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const diasRestantes = parseInt(this.getAttribute('data-dias-restantes') ?? '99');
            const set = (elId, val) => { const el = document.getElementById(elId); if (el) el.textContent = val; };

            set('cancelar-id-display-reserva', '#' + id);
            set('cancelar-cliente-reserva', this.dataset.cliente);
            set('cancelar-fecha-reserva', this.dataset.fecha);
            set('cancelar-hora-reserva', this.dataset.hora);
            set('cancelar-personas-reserva', this.dataset.personas + ' persona(s)');

            const inputId = document.getElementById('cancelar-id-reserva');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formCancelarReserva');
            if (form) form.action = '/Reserva/CancelarReserva/' + id;

            mostrarAviso3Dias('aviso-plazo-reserva', diasRestantes);
        });
    });

    // ── Modal cancelar CLIENTE (MisReservas) ──────────────────────────
    document.querySelectorAll('.btn-cancelar-mi-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const diasRestantes = parseInt(this.getAttribute('data-dias-restantes') ?? '99');
            const set = (elId, val) => { const el = document.getElementById(elId); if (el) el.textContent = val; };

            set('mi-cancelar-id-display', '#' + id);
            set('mi-cancelar-cliente', this.dataset.cliente);
            set('mi-cancelar-fecha', this.dataset.fecha);
            set('mi-cancelar-hora', this.dataset.hora);
            set('mi-cancelar-personas', this.dataset.personas + ' persona(s)');

            const inputId = document.getElementById('mi-cancelar-id-input');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formCancelarMiReserva');
            if (form) form.action = '/Reserva/CancelarMiReserva/' + id;

            mostrarAviso3Dias('aviso-plazo-mi-reserva', diasRestantes);
        });
    });

    // ── Modal confirmar reserva ──────────────────────────────────────────
    document.querySelectorAll('.btn-confirmar-reserva').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const set = (elId, val) => { const el = document.getElementById(elId); if (el) el.textContent = val; };

            set('confirmar-id-display-reserva', '#' + id);
            set('confirmar-cliente-reserva', this.dataset.cliente);
            set('confirmar-fecha-reserva', this.dataset.fecha);
            set('confirmar-hora-reserva', this.dataset.hora);
            set('confirmar-personas-reserva', this.dataset.personas + ' persona(s)');

            const inputId = document.getElementById('confirmar-id-reserva');
            if (inputId) inputId.value = id;

            const form = document.getElementById('formConfirmarReserva');
            if (form) form.action = '/Reserva/ConfirmarReserva/' + id;
        });
    });
}

// ============================================
// AVISO DE PLAZO DE 3 DÍAS
// ============================================
function mostrarAviso3Dias(elementId, diasRestantes) {
    const aviso = document.getElementById(elementId);
    if (!aviso) return;

    if (diasRestantes < DIAS_MINIMOS_CANCELACION) {
        aviso.style.display = 'flex';
        aviso.innerHTML = `
            <i class="fas fa-exclamation-triangle"></i>
            <span><strong>Atención:</strong> Quedan <strong>${diasRestantes} día(s)</strong>
            para la reserva. La política permite cancelar solo con al menos
            <strong>${DIAS_MINIMOS_CANCELACION} días</strong> de anticipación.</span>
        `;
    } else {
        aviso.style.display = 'none';
    }
}

// ============================================
// HORARIOS DINÁMICOS POR DÍA
// ============================================
const HORARIOS_POR_DIA = {
    semana: [ // Miércoles (3), Jueves (4), Viernes (5)
        { value: "12:00:00", label: "12:00 PM" },
        { value: "12:30:00", label: "12:30 PM" },
        { value: "13:00:00", label: "1:00 PM" },
        { value: "13:30:00", label: "1:30 PM" },
        { value: "14:00:00", label: "2:00 PM" },
        { value: "14:30:00", label: "2:30 PM" },
        { value: "18:00:00", label: "6:00 PM" },
        { value: "18:30:00", label: "6:30 PM" },
        { value: "19:00:00", label: "7:00 PM" },
        { value: "19:30:00", label: "7:30 PM" },
        { value: "20:00:00", label: "8:00 PM" },
        { value: "20:30:00", label: "8:30 PM" },
        { value: "21:00:00", label: "9:00 PM" },
        { value: "21:30:00", label: "9:30 PM" }
    ],
    sabado: [ // Sábado (6)
        { value: "07:30:00", label: "7:30 AM" },
        { value: "08:00:00", label: "8:00 AM" },
        { value: "08:30:00", label: "8:30 AM" },
        { value: "09:00:00", label: "9:00 AM" },
        { value: "09:30:00", label: "9:30 AM" },
        { value: "10:00:00", label: "10:00 AM" },
        { value: "10:30:00", label: "10:30 AM" },
        { value: "11:00:00", label: "11:00 AM" },
        { value: "11:30:00", label: "11:30 AM" },
        { value: "12:00:00", label: "12:00 PM" },
        { value: "12:30:00", label: "12:30 PM" },
        { value: "13:00:00", label: "1:00 PM" },
        { value: "13:30:00", label: "1:30 PM" },
        { value: "14:00:00", label: "2:00 PM" },
        { value: "14:30:00", label: "2:30 PM" },
        { value: "18:00:00", label: "6:00 PM" },
        { value: "18:30:00", label: "6:30 PM" },
        { value: "19:00:00", label: "7:00 PM" },
        { value: "19:30:00", label: "7:30 PM" },
        { value: "20:00:00", label: "8:00 PM" },
        { value: "20:30:00", label: "8:30 PM" },
        { value: "21:00:00", label: "9:00 PM" },
        { value: "21:30:00", label: "9:30 PM" }
    ],
    domingo: [ // Domingo (0)
        { value: "07:30:00", label: "7:30 AM" },
        { value: "08:00:00", label: "8:00 AM" },
        { value: "08:30:00", label: "8:30 AM" },
        { value: "09:00:00", label: "9:00 AM" },
        { value: "09:30:00", label: "9:30 AM" },
        { value: "10:00:00", label: "10:00 AM" },
        { value: "10:30:00", label: "10:30 AM" },
        { value: "11:00:00", label: "11:00 AM" },
        { value: "11:30:00", label: "11:30 AM" },
        { value: "12:00:00", label: "12:00 PM" },
        { value: "12:30:00", label: "12:30 PM" },
        { value: "13:00:00", label: "1:00 PM" },
        { value: "13:30:00", label: "1:30 PM" },
        { value: "14:00:00", label: "2:00 PM" },
        { value: "14:30:00", label: "2:30 PM" },
        { value: "18:00:00", label: "6:00 PM" },
        { value: "18:30:00", label: "6:30 PM" },
        { value: "19:00:00", label: "7:00 PM" },
        { value: "19:30:00", label: "7:30 PM" }
        // Domingo cierra 8:00 PM → última entrada 7:30 PM
    ]
};

function initHorariosDinamicos(fechaInputId, horaSelectId, avisoCerradoId, horaPreseleccionada = '') {
    const inputFecha = document.getElementById(fechaInputId);
    const selectHora = document.getElementById(horaSelectId);
    const avisoCerrado = document.getElementById(avisoCerradoId);

    if (!inputFecha || !selectHora) return;

    // Calcula el primer día válido desde hoy (salta lunes y martes)
    function proximaFechaValida() {
        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0);
        while (hoy.getDay() === 1 || hoy.getDay() === 2) {
            hoy.setDate(hoy.getDate() + 1);
        }
        return hoy.toISOString().split('T')[0];
    }

    // En vistas de creación poner el min al primer día válido;
    // en edición no forzar min para no bloquear la fecha existente
    if (!horaPreseleccionada) {
        inputFecha.min = proximaFechaValida();
    }

    function cargarHoras(preseleccionar) {
        const fecha = inputFecha.value;
        selectHora.innerHTML = '';

        if (!fecha) {
            selectHora.innerHTML = '<option value="">-- Primero selecciona una fecha --</option>';
            selectHora.disabled = true;
            if (avisoCerrado) avisoCerrado.style.display = 'none';
            return;
        }

        const dia = new Date(fecha + 'T00:00').getDay();
        // 0=Dom, 1=Lun, 2=Mar, 3=Mié, 4=Jue, 5=Vie, 6=Sáb

        // Rechazar lunes y martes aunque el usuario escriba la fecha a mano
        if (dia === 1 || dia === 2) {
            selectHora.innerHTML = '<option value="">-- No disponible este día --</option>';
            selectHora.disabled = true;
            inputFecha.value = '';
            if (avisoCerrado) {
                avisoCerrado.style.display = 'block';
                avisoCerrado.innerHTML = '<i class="fas fa-times-circle"></i> El restaurante no abre los lunes ni martes.';
            }
            return;
        }

        let horarios = null;
        if (dia === 3 || dia === 4 || dia === 5) horarios = HORARIOS_POR_DIA.semana;
        else if (dia === 6) horarios = HORARIOS_POR_DIA.sabado;
        else if (dia === 0) horarios = HORARIOS_POR_DIA.domingo;

        if (avisoCerrado) avisoCerrado.style.display = 'none';
        selectHora.disabled = false;
        selectHora.innerHTML = '<option value="">-- Selecciona una hora --</option>';

        horarios.forEach(h => {
            const opt = document.createElement('option');
            opt.value = h.value;
            opt.textContent = h.label;
            if (preseleccionar && h.value === preseleccionar) opt.selected = true;
            selectHora.appendChild(opt);
        });
    }

    inputFecha.addEventListener('change', () => cargarHoras(''));
    cargarHoras(horaPreseleccionada);
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = { success: 'fa-check-circle', error: 'fa-exclamation-triangle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
    const colores = { success: 'var(--green)', error: 'var(--red)', warning: 'var(--yellow)', info: 'var(--gold)' };

    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed; top: 100px; right: 20px;
        background: ${colores[tipo]}; color: white;
        padding: 1.5rem 2rem; border-radius: 10px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001; animation: slideInNotif 0.3s ease;
        font-weight: 600; display: flex; align-items: center;
        gap: 1rem; min-width: 300px;
    `;
    notification.innerHTML = `<i class="fas ${iconos[tipo]}" style="font-size:1.5rem;"></i><span>${mensaje}</span>`;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOutNotif 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// ============================================
// ESTILOS DE ANIMACIÓN
// ============================================
if (!document.getElementById('reserva-styles')) {
    const s = document.createElement('style');
    s.id = 'reserva-styles';
    s.innerHTML = `
        @keyframes slideInNotif  { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
        @keyframes slideOutNotif { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
        @keyframes tooltipFadeIn  { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
        @keyframes tooltipFadeOut { from { opacity: 1; transform: translateY(0); }   to { opacity: 0; transform: translateY(10px); } }
    `;
    document.head.appendChild(s);
}