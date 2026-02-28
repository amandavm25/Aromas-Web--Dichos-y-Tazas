// promocion.js - Gestión de promociones

// ============================================
// VARIABLES GLOBALES
// ============================================
let recetaIndex = 0;
let recetasDisponibles = [];

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (usando función general de site.js)
    // ============================================
    if (document.getElementById('laTablaDeProm')) {
        initTablePagination({
            tableId: 'laTablaDeProm',
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
    const tabla = document.getElementById('laTablaDeProm');
    if (tabla) {
        const filas = tabla.querySelectorAll('tbody tr');

        filas.forEach(fila => {
            fila.addEventListener('mouseenter', function () {
                this.style.background = 'linear-gradient(90deg, rgba(32, 116, 118, 0.05) 0%, transparent 100%)';
                this.style.transform = 'translateX(5px)';
                this.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.08)';
            });

            fila.addEventListener('mouseleave', function () {
                this.style.background = '';
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        });
    }

    // ============================================
    // ANIMACIÓN DE ENTRADA (feature-cards)
    // ============================================
    const cards = document.querySelectorAll('.feature-card, .admin-form-wrapper');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.6s ease';
        observer.observe(card);
    });

    // ============================================
    // FORMULARIO: GESTIÓN DE RECETAS ASOCIADAS
    // ============================================
    if (window.recetasDisponibles && Array.isArray(window.recetasDisponibles)) {
        recetasDisponibles = window.recetasDisponibles;
    }

    const btnAgregar = document.getElementById('btnAgregarReceta');
    if (btnAgregar) {
        btnAgregar.addEventListener('click', function (e) {
            e.preventDefault();
            agregarReceta();
        });

        // Cargar recetas existentes (edición) o agregar una fila vacía (creación)
        const container = document.getElementById('recetas-container');
        if (window.recetasExistentes && window.recetasExistentes.length > 0) {
            window.recetasExistentes.forEach(function (rec) {
                agregarReceta();
                const items = container.querySelectorAll('.receta-item');
                const last = items[items.length - 1];
                const select = last.querySelector('.select-receta');
                if (select) {
                    select.value = rec.idReceta;
                    select.dispatchEvent(new Event('change'));
                }
            });
        } else if (container && container.children.length === 0) {
            agregarReceta();
        }
    }

    // Recalcular resumen cuando cambia el descuento
    const inputDescuento = document.querySelector('input[name="PorcentajeDescuento"]');
    if (inputDescuento) {
        inputDescuento.addEventListener('input', function () {
            actualizarResumenDescuento();
            recalcularTodosLosPrecios();
        });
        actualizarResumenDescuento();
    }

    // Recalcular duración cuando cambian las fechas
    const fechaInicioInput = document.querySelector('input[name="FechaInicio"]');
    const fechaFinInput = document.querySelector('input[name="FechaFin"]');

    if (fechaInicioInput) fechaInicioInput.addEventListener('change', calcularDuracion);
    if (fechaFinInput) fechaFinInput.addEventListener('change', calcularDuracion);

    calcularDuracion();

    // ============================================
    // VALIDACIÓN DEL FORMULARIO AL ENVIAR
    // ============================================
    const form = document.querySelector('form[action*="CrearPromocion"], form[action*="EditarPromocion"]');
    if (form) {
        form.addEventListener('submit', function (e) {
            const container = document.getElementById('recetas-container');

            if (!container || container.children.length === 0) {
                e.preventDefault();
                mostrarNotificacion('Debes agregar al menos una receta a la promoción', 'error');
                return false;
            }

            const items = container.querySelectorAll('.receta-item');
            let todosCompletos = true;

            items.forEach(item => {
                const selectReceta = item.querySelector('.select-receta');
                if (!selectReceta || !selectReceta.value) {
                    todosCompletos = false;
                    if (selectReceta) selectReceta.style.borderColor = 'var(--red)';
                } else {
                    if (selectReceta) selectReceta.style.borderColor = '';
                }
            });

            if (!todosCompletos) {
                e.preventDefault();
                mostrarNotificacion('Selecciona una receta en cada fila o elimina las que estén vacías', 'error');
                return false;
            }

            const descuento = parseFloat(document.querySelector('input[name="PorcentajeDescuento"]')?.value || 0);
            if (descuento < 0 || descuento > 100) {
                e.preventDefault();
                mostrarNotificacion('El porcentaje de descuento debe estar entre 0 y 100', 'error');
                return false;
            }

            const inicio = document.querySelector('input[name="FechaInicio"]')?.value;
            const fin = document.querySelector('input[name="FechaFin"]')?.value;
            if (inicio && fin && new Date(fin) < new Date(inicio)) {
                e.preventDefault();
                mostrarNotificacion('La fecha de fin no puede ser anterior a la fecha de inicio', 'error');
                return false;
            }
        });
    }

});

// ============================================
// GESTIÓN DE RECETAS ASOCIADAS
// ============================================
function agregarReceta() {
    const template = document.getElementById('receta-template');
    const container = document.getElementById('recetas-container');

    if (!template || !container) return;

    const clone = template.content.cloneNode(true);

    // Actualizar índices en los atributos name
    clone.querySelectorAll('[name*="INDEX"]').forEach(el => {
        el.name = el.name.replace('INDEX', recetaIndex);
    });

    // Poblar el select con las recetas disponibles
    const selectReceta = clone.querySelector('.select-receta');
    if (selectReceta) {
        recetasDisponibles.forEach(r => {
            const opt = document.createElement('option');
            opt.value = r.idReceta ?? r.id;
            opt.textContent = r.nombre;
            opt.dataset.precio = r.precioVenta ?? r.precio ?? 0;
            opt.dataset.categoria = r.nombreCategoria ?? r.categoria ?? '';
            selectReceta.appendChild(opt);
        });

        selectReceta.addEventListener('change', function () {
            actualizarPreciosReceta(this.closest('.receta-item'));
        });
    }

    // Botón eliminar
    const btnEliminar = clone.querySelector('.btn-eliminar-receta');
    if (btnEliminar) {
        btnEliminar.addEventListener('click', function () {
            eliminarReceta(this.closest('.receta-item'));
        });
    }

    container.appendChild(clone);
    recetaIndex++;
    actualizarContadorRecetas();

    // Animación de entrada (igual que receta.js)
    const items = container.querySelectorAll('.receta-item');
    const lastItem = items[items.length - 1];
    if (lastItem) {
        lastItem.style.opacity = '0';
        lastItem.style.transform = 'translateY(20px)';
        setTimeout(() => {
            lastItem.style.transition = 'all 0.3s ease';
            lastItem.style.opacity = '1';
            lastItem.style.transform = 'translateY(0)';
        }, 10);
    }
}

function eliminarReceta(item) {
    if (!item) return;

    item.style.transition = 'all 0.3s ease';
    item.style.opacity = '0';
    item.style.transform = 'translateX(-20px)';

    setTimeout(() => {
        item.remove();
        actualizarContadorRecetas();

        const container = document.getElementById('recetas-container');
        if (container && container.children.length === 0) {
            mostrarNotificacion('Debes agregar al menos una receta a la promoción', 'warning');
        }
    }, 300);
}

function actualizarPreciosReceta(item) {
    if (!item) return;

    const selectReceta = item.querySelector('.select-receta');
    const precioOriginalEl = item.querySelector('.precio-original');
    const precioDescuentoEl = item.querySelector('.precio-descuento');
    const infoReceta = item.querySelector('.info-receta');
    const categoriaEl = item.querySelector('.categoria-receta');
    const ahorroEl = item.querySelector('.ahorro-receta');

    if (!selectReceta || !selectReceta.value) {
        if (precioOriginalEl) precioOriginalEl.value = '₡0.00';
        if (precioDescuentoEl) precioDescuentoEl.value = '₡0.00';
        if (infoReceta) infoReceta.style.display = 'none';
        return;
    }

    const opcion = selectReceta.options[selectReceta.selectedIndex];
    const precio = parseFloat(opcion.dataset.precio || 0);
    const categoria = opcion.dataset.categoria || '';
    const descuento = parseFloat(document.querySelector('input[name="PorcentajeDescuento"]')?.value || 0);
    const precioFinal = precio - (precio * descuento / 100);
    const ahorro = precio - precioFinal;

    if (precioOriginalEl) precioOriginalEl.value = '₡' + precio.toLocaleString('es-CR', { minimumFractionDigits: 2 });
    if (precioDescuentoEl) precioDescuentoEl.value = '₡' + precioFinal.toLocaleString('es-CR', { minimumFractionDigits: 2 });
    if (infoReceta) infoReceta.style.display = 'block';
    if (categoriaEl) categoriaEl.textContent = categoria;
    if (ahorroEl) ahorroEl.textContent = '₡' + ahorro.toLocaleString('es-CR', { minimumFractionDigits: 2 });
}

function recalcularTodosLosPrecios() {
    document.querySelectorAll('.receta-item').forEach(item => {
        actualizarPreciosReceta(item);
    });
}

function actualizarContadorRecetas() {
    const container = document.getElementById('recetas-container');
    const resumenRecetas = document.getElementById('resumenRecetas');
    if (resumenRecetas && container) {
        resumenRecetas.textContent = container.querySelectorAll('.receta-item').length;
    }
}

function actualizarResumenDescuento() {
    const input = document.querySelector('input[name="PorcentajeDescuento"]');
    const resumen = document.getElementById('resumenDescuento');
    if (input && resumen) {
        resumen.textContent = (parseFloat(input.value) || 0) + '%';
    }
}

function calcularDuracion() {
    const inicioInput = document.querySelector('input[name="FechaInicio"]');
    const finInput = document.querySelector('input[name="FechaFin"]');
    const divDuracion = document.getElementById('duracionPromocion');
    const txtDuracion = document.getElementById('textoDuracion');
    const resumenDuracion = document.getElementById('resumenDuracion');

    if (!inicioInput || !finInput) return;

    const inicio = new Date(inicioInput.value);
    const fin = new Date(finInput.value);

    if (inicioInput.value && finInput.value && fin >= inicio) {
        const dias = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24)) + 1;
        if (divDuracion) divDuracion.style.display = 'flex';
        if (txtDuracion) txtDuracion.textContent = dias + ' días de vigencia';
        if (resumenDuracion) resumenDuracion.textContent = dias + ' días';
    } else {
        if (divDuracion) divDuracion.style.display = 'none';
        if (resumenDuracion) resumenDuracion.textContent = '-';
    }
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

const notifStyle = document.createElement('style');
notifStyle.innerHTML = `
    @keyframes slideIn  { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0);    opacity: 1; } }
    @keyframes slideOut { from { transform: translateX(0);    opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
`;
document.head.appendChild(notifStyle);