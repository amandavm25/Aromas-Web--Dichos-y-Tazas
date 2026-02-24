// receta.js - Gestión de recetas

// ============================================
// VARIABLES GLOBALES
// ============================================
let ingredienteIndex = 0;
let insumosDisponibles = [];

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (usando función general de site.js)
    // ============================================
    if (document.getElementById('laTablaDeRecetas')) {
        initTablePagination({
            tableId: 'laTablaDeRecetas',
            recordsPerPage: 10,
            prevButtonId: 'btnAnterior',
            nextButtonId: 'btnSiguiente',
            startRecordId: 'startRecord',
            endRecordId: 'endRecord',
            totalRecordsId: 'totalRecords'
        });
    }

    // Paginación panel Margen Bajo
    if (document.getElementById('margenBajoLista')) {
        initCardsPagination({
            containerId: 'margenBajoLista',
            cardsPerPage: 2,
            prevButtonId: 'btnMargenAnterior',
            nextButtonId: 'btnMargenSiguiente',
            startCardId: 'margenStart',
            endCardId: 'margenEnd',
            totalCardsId: 'margenTotal'
        });
    }

    // ============================================
    // HOVER EFFECTS EN TABLA
    // ============================================
    const tabla = document.getElementById('laTablaDeRecetas');
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
    // FORMULARIO: GESTIÓN DE INGREDIENTES
    // ============================================
    if (window.insumosDisponibles && Array.isArray(window.insumosDisponibles)) {
        insumosDisponibles = window.insumosDisponibles;
    }

    const btnAgregar = document.getElementById('btnAgregarIngrediente');
    if (btnAgregar) {
        btnAgregar.addEventListener('click', function (e) {
            e.preventDefault();
            agregarIngrediente();
        });

        const container = document.getElementById('ingredientes-container');
        if (container && container.children.length === 0) {
            agregarIngrediente();
        }
    }

    const inputPrecioVenta = document.querySelector('input[name="PrecioVenta"]');
    if (inputPrecioVenta) {
        inputPrecioVenta.addEventListener('input', calcularResumenCostos);
    }

    const inputPorciones = document.querySelector('input[name="CantidadPorciones"]');
    if (inputPorciones) {
        inputPorciones.addEventListener('input', calcularResumenCostos);
    }

    calcularResumenCostos();

    // ============================================
    // VALIDACIÓN DEL FORMULARIO
    // ============================================
    const form = document.querySelector('form[action*="CrearReceta"], form[action*="EditarReceta"]');
    if (form) {
        form.addEventListener('submit', function (e) {
            const container = document.getElementById('ingredientes-container');

            if (!container || container.children.length === 0) {
                e.preventDefault();
                mostrarNotificacion('Debes agregar al menos un ingrediente a la receta', 'error');
                return false;
            }

            const items = container.querySelectorAll('.ingrediente-item');
            let todosCompletos = true;

            items.forEach(item => {
                const selectInsumo = item.querySelector('.select-insumo');
                const inputCantidad = item.querySelector('.input-cantidad');

                if (!selectInsumo.value || !inputCantidad.value || parseFloat(inputCantidad.value) <= 0) {
                    todosCompletos = false;
                    selectInsumo.style.borderColor = 'var(--red)';
                    inputCantidad.style.borderColor = 'var(--red)';
                } else {
                    selectInsumo.style.borderColor = '';
                    inputCantidad.style.borderColor = '';
                }
            });

            if (!todosCompletos) {
                e.preventDefault();
                mostrarNotificacion('Completa todos los campos de los ingredientes', 'error');
                return false;
            }

            let hayStockInsuficiente = false;
            items.forEach(item => {
                const alertaStock = item.querySelector('.alerta-stock');
                if (alertaStock && alertaStock.style.display === 'block') {
                    hayStockInsuficiente = true;
                }
            });

            if (hayStockInsuficiente) {
                const confirmar = confirm('Algunos ingredientes tienen stock insuficiente. ¿Deseas continuar de todos modos?');
                if (!confirmar) {
                    e.preventDefault();
                    return false;
                }
            }
        });
    }

});

// ============================================
// GESTIÓN DE INGREDIENTES
// ============================================
function agregarIngrediente() {
    const template = document.getElementById('ingrediente-template');
    const container = document.getElementById('ingredientes-container');

    if (!template || !container) return;

    if (!insumosDisponibles || insumosDisponibles.length === 0) {
        mostrarNotificacion('No hay insumos disponibles. Por favor, agrega insumos primero.', 'error');
        return;
    }

    const clone = template.content.cloneNode(true);
    const selectInsumo = clone.querySelector('.select-insumo');

    if (!selectInsumo) return;

    insumosDisponibles.forEach(insumo => {
        const option = document.createElement('option');
        option.value = insumo.idInsumo;
        option.textContent = `${insumo.nombreInsumo} (${insumo.unidadMedida})`;
        option.setAttribute('data-costo', insumo.costoUnitario);
        option.setAttribute('data-disponible', insumo.cantidadDisponible);
        option.setAttribute('data-unidad', insumo.unidadMedida);
        selectInsumo.appendChild(option);
    });

    const selectInsumoElement = clone.querySelector('.select-insumo');
    const inputCantidad = clone.querySelector('.input-cantidad');
    const btnEliminar = clone.querySelector('.btn-eliminar-ingrediente');

    if (selectInsumoElement) {
        selectInsumoElement.addEventListener('change', function () {
            actualizarCostoIngrediente(this.closest('.ingrediente-item'));
            validarDisponibilidad(this.closest('.ingrediente-item'));
        });
    }

    if (inputCantidad) {
        inputCantidad.addEventListener('input', function () {
            actualizarCostoIngrediente(this.closest('.ingrediente-item'));
            validarDisponibilidad(this.closest('.ingrediente-item'));
        });
    }

    if (btnEliminar) {
        btnEliminar.addEventListener('click', function () {
            eliminarIngrediente(this.closest('.ingrediente-item'));
        });
    }

    container.appendChild(clone);
    ingredienteIndex++;

    const items = container.querySelectorAll('.ingrediente-item');
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

function eliminarIngrediente(item) {
    if (!item) return;

    item.style.transition = 'all 0.3s ease';
    item.style.opacity = '0';
    item.style.transform = 'translateX(-20px)';

    setTimeout(() => {
        item.remove();
        calcularResumenCostos();

        const container = document.getElementById('ingredientes-container');
        if (container && container.children.length === 0) {
            mostrarNotificacion('Debes agregar al menos un ingrediente', 'warning');
        }
    }, 300);
}

function actualizarCostoIngrediente(item) {
    if (!item) return;

    const selectInsumo = item.querySelector('.select-insumo');
    const inputCantidad = item.querySelector('.input-cantidad');
    const inputCosto = item.querySelector('.costo-ingrediente');

    if (!selectInsumo || !inputCantidad || !inputCosto) return;

    const selectedOption = selectInsumo.options[selectInsumo.selectedIndex];
    const costoUnitario = parseFloat(selectedOption.getAttribute('data-costo') || 0);
    const cantidad = parseFloat(inputCantidad.value || 0);
    const costoTotal = costoUnitario * cantidad;

    inputCosto.value = '₡' + costoTotal.toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    calcularResumenCostos();
}

function validarDisponibilidad(item) {
    if (!item) return;

    const selectInsumo = item.querySelector('.select-insumo');
    const inputCantidad = item.querySelector('.input-cantidad');
    const alertaStock = item.querySelector('.alerta-stock');
    const mensajeAlerta = item.querySelector('.mensaje-alerta');

    if (!selectInsumo || !selectInsumo.value) return;

    const selectedOption = selectInsumo.options[selectInsumo.selectedIndex];
    const disponible = parseFloat(selectedOption.getAttribute('data-disponible') || 0);
    const cantidad = parseFloat(inputCantidad.value || 0);
    const unidad = selectedOption.getAttribute('data-unidad') || '';

    if (cantidad > disponible) {
        if (alertaStock && mensajeAlerta) {
            alertaStock.style.display = 'block';
            mensajeAlerta.textContent = `Stock insuficiente. Disponible: ${disponible} ${unidad}`;
            item.style.borderLeft = '4px solid var(--red)';
        }
    } else {
        if (alertaStock) alertaStock.style.display = 'none';
        item.style.borderLeft = '';
    }
}

function calcularResumenCostos() {
    const container = document.getElementById('ingredientes-container');
    if (!container) return;

    let costoTotal = 0;
    const items = container.querySelectorAll('.ingrediente-item');

    items.forEach(item => {
        const inputCosto = item.querySelector('.costo-ingrediente');
        if (inputCosto) {
            const costo = parseFloat(inputCosto.value.replace(/[^0-9.-]+/g, '') || 0);
            costoTotal += costo;
        }
    });

    const costoTotalElement = document.getElementById('costoTotalReceta');
    if (costoTotalElement) {
        costoTotalElement.textContent = '₡' + costoTotal.toLocaleString('es-CR', { minimumFractionDigits: 2 });
    }

    const inputPorciones = document.querySelector('input[name="CantidadPorciones"]');
    const porciones = inputPorciones ? parseFloat(inputPorciones.value || 1) : 1;

    const costoPorPorcionElement = document.getElementById('costoPorPorcion');
    if (costoPorPorcionElement) {
        costoPorPorcionElement.textContent = '₡' + (costoTotal / porciones).toLocaleString('es-CR', { minimumFractionDigits: 2 });
    }

    const inputPrecioVenta = document.querySelector('input[name="PrecioVenta"]');
    const precioVenta = inputPrecioVenta ? parseFloat(inputPrecioVenta.value || 0) : 0;

    const resumenGananciaElement = document.getElementById('resumenGanancia');
    if (resumenGananciaElement) {
        if (precioVenta > 0) {
            resumenGananciaElement.style.display = 'block';

            const precioVentaMostrarElement = document.getElementById('precioVentaMostrar');
            if (precioVentaMostrarElement) {
                precioVentaMostrarElement.textContent = '₡' + precioVenta.toLocaleString('es-CR', { minimumFractionDigits: 2 });
            }

            const gananciaNeta = precioVenta - costoTotal;
            const margen = precioVenta > 0 ? (gananciaNeta / precioVenta) * 100 : 0;

            const gananciaNetaElement = document.getElementById('gananciaNeta');
            if (gananciaNetaElement) {
                gananciaNetaElement.textContent = '₡' + gananciaNeta.toLocaleString('es-CR', { minimumFractionDigits: 2 });
                if (margen >= 50) gananciaNetaElement.style.color = 'var(--green)';
                else if (margen >= 30) gananciaNetaElement.style.color = 'var(--yellow)';
                else gananciaNetaElement.style.color = 'var(--red)';
            }

            const margenGananciaElement = document.getElementById('margenGanancia');
            if (margenGananciaElement) {
                margenGananciaElement.textContent = margen.toFixed(2) + '%';
            }
        } else {
            resumenGananciaElement.style.display = 'none';
        }
    }
}

// ============================================
// NOTIFICACIONES
// ============================================
function mostrarNotificacion(mensaje, tipo) {
    const iconos = { 'success': 'fa-check-circle', 'error': 'fa-exclamation-triangle', 'warning': 'fa-exclamation-triangle', 'info': 'fa-info-circle' };
    const colores = { 'success': 'var(--green)', 'error': 'var(--red)', 'warning': 'var(--yellow)', 'info': 'var(--gold)' };

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
    @keyframes slideIn { from { transform: translateX(400px); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
    @keyframes slideOut { from { transform: translateX(0); opacity: 1; } to { transform: translateX(400px); opacity: 0; } }
`;
document.head.appendChild(notifStyle);