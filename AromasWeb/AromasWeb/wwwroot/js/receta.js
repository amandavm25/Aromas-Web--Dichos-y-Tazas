// ============================================
// VARIABLES GLOBALES
// ============================================
let ingredienteIndex = 0;
let insumosDisponibles = [];

// ============================================
// INICIALIZACIÓN
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    console.log('Inicializando receta.js...');

    // ⭐ Cargar insumos desde window.insumosDisponibles (viene del servidor)
    if (window.insumosDisponibles && Array.isArray(window.insumosDisponibles)) {
        insumosDisponibles = window.insumosDisponibles;
        console.log('Insumos cargados:', insumosDisponibles.length);
    } else {
        console.warn('No se encontraron insumos en window.insumosDisponibles');
        insumosDisponibles = [];
    }

    // Configurar botón de agregar ingrediente
    const btnAgregar = document.getElementById('btnAgregarIngrediente');
    if (btnAgregar) {
        console.log('Botón agregar ingrediente encontrado');

        btnAgregar.addEventListener('click', function (e) {
            e.preventDefault();
            console.log('Click en agregar ingrediente');
            agregarIngrediente();
        });

        // Agregar primer ingrediente automáticamente si estamos en crear
        const container = document.getElementById('ingredientes-container');
        if (container && container.children.length === 0) {
            console.log('Agregando primer ingrediente automáticamente');
            agregarIngrediente();
        }
    } else {
        console.error('Botón agregar ingrediente NO encontrado');
    }

    // Event listener para precio de venta
    const inputPrecioVenta = document.querySelector('input[name="PrecioVenta"]');
    if (inputPrecioVenta) {
        inputPrecioVenta.addEventListener('input', calcularResumenCostos);
    }

    // Event listener para cantidad de porciones
    const inputPorciones = document.querySelector('input[name="CantidadPorciones"]');
    if (inputPorciones) {
        inputPorciones.addEventListener('input', calcularResumenCostos);
    }

    // Inicializar modales
    initializeModalsReceta();

    // Animar elementos al cargar
    animateOnLoadReceta();

    // Calcular costos iniciales
    calcularResumenCostos();

    console.log('receta.js inicializado correctamente');
});

// ============================================
// GESTIÓN DE INGREDIENTES
// ============================================
function agregarIngrediente() {
    console.log('Ejecutando agregarIngrediente()');

    const template = document.getElementById('ingrediente-template');
    const container = document.getElementById('ingredientes-container');

    if (!template) {
        console.error('Template de ingrediente no encontrado');
        return;
    }

    if (!container) {
        console.error('Container de ingredientes no encontrado');
        return;
    }

    if (!insumosDisponibles || insumosDisponibles.length === 0) {
        console.error('No hay insumos disponibles para agregar');
        mostrarNotificacion('No hay insumos disponibles. Por favor, agrega insumos primero.', 'error');
        return;
    }

    console.log('Template y container encontrados');
    console.log('Insumos disponibles:', insumosDisponibles.length);

    // Clonar template
    const clone = template.content.cloneNode(true);
    console.log('Template clonado');

    // Llenar select de insumos
    const selectInsumo = clone.querySelector('.select-insumo');

    if (!selectInsumo) {
        console.error('Select de insumo no encontrado en el template');
        return;
    }

    console.log('Llenando select con', insumosDisponibles.length, 'insumos');

    insumosDisponibles.forEach(insumo => {
        const option = document.createElement('option');
        option.value = insumo.id;
        option.textContent = `${insumo.nombre} (${insumo.unidadMedida})`;
        option.setAttribute('data-costo', insumo.costoUnitario);
        option.setAttribute('data-disponible', insumo.cantidadDisponible);
        option.setAttribute('data-unidad', insumo.unidadMedida);
        selectInsumo.appendChild(option);
    });

    console.log('Select llenado con opciones');

    // Event listeners para el nuevo ingrediente
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

    console.log('Ingrediente agregado. Total:', ingredienteIndex);

    // Animar entrada
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

    mostrarNotificacion('Ingrediente agregado', 'success');
}

function eliminarIngrediente(item) {
    if (!item) return;

    item.style.transition = 'all 0.3s ease';
    item.style.opacity = '0';
    item.style.transform = 'translateX(-20px)';

    setTimeout(() => {
        item.remove();
        calcularResumenCostos();

        // Verificar si no hay ingredientes
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
            item.style.borderLeft = '4px solid #e74c3c';
        }
    } else if (cantidad > 0) {
        if (alertaStock) {
            alertaStock.style.display = 'none';
            item.style.borderLeft = '';
        }
    }
}

// ============================================
// CÁLCULOS DE COSTOS Y GANANCIAS
// ============================================
function calcularResumenCostos() {
    const container = document.getElementById('ingredientes-container');
    if (!container) return;

    const items = container.querySelectorAll('.ingrediente-item');
    let costoTotal = 0;

    items.forEach(item => {
        const selectInsumo = item.querySelector('.select-insumo');
        const inputCantidad = item.querySelector('.input-cantidad');

        if (selectInsumo && selectInsumo.value && inputCantidad && inputCantidad.value) {
            const selectedOption = selectInsumo.options[selectInsumo.selectedIndex];
            const costoUnitario = parseFloat(selectedOption.getAttribute('data-costo') || 0);
            const cantidad = parseFloat(inputCantidad.value || 0);

            costoTotal += costoUnitario * cantidad;
        }
    });

    // Actualizar costo total
    const costoTotalElement = document.getElementById('costoTotalReceta');
    if (costoTotalElement) {
        costoTotalElement.textContent = '₡' + costoTotal.toLocaleString('es-CR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    // Calcular costo por porción
    const inputPorciones = document.querySelector('input[name="CantidadPorciones"]');
    const porciones = parseInt(inputPorciones?.value || 1);
    const costoPorcion = porciones > 0 ? costoTotal / porciones : 0;

    const costoPorPorcionElement = document.getElementById('costoPorPorcion');
    if (costoPorPorcionElement) {
        costoPorPorcionElement.textContent = '₡' + costoPorcion.toLocaleString('es-CR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    // Calcular ganancia si hay precio de venta
    const inputPrecioVenta = document.querySelector('input[name="PrecioVenta"]');
    const precioVenta = parseFloat(inputPrecioVenta?.value || 0);

    const resumenGananciaElement = document.getElementById('resumenGanancia');
    if (resumenGananciaElement) {
        if (precioVenta > 0) {
            const ganancia = precioVenta - costoTotal;
            const margen = costoTotal > 0 ? (ganancia / precioVenta) * 100 : 0;

            resumenGananciaElement.style.display = 'block';

            const precioVentaMostrarElement = document.getElementById('precioVentaMostrar');
            if (precioVentaMostrarElement) {
                precioVentaMostrarElement.textContent = '₡' + precioVenta.toLocaleString('es-CR', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });
            }

            const gananciaNetaElement = document.getElementById('gananciaNeta');
            if (gananciaNetaElement) {
                gananciaNetaElement.textContent = '₡' + ganancia.toLocaleString('es-CR', {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                });

                // Cambiar color según rentabilidad
                if (margen >= 50) {
                    gananciaNetaElement.style.color = '#27ae60';
                } else if (margen >= 30) {
                    gananciaNetaElement.style.color = '#f39c12';
                } else {
                    gananciaNetaElement.style.color = '#e74c3c';
                }
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
// MODAL DETALLES RECETA
// ============================================
function initializeModalsReceta() {
    const botonesDetalles = document.querySelectorAll('.btn-detalles-receta');

    botonesDetalles.forEach(boton => {
        boton.addEventListener('click', function () {
            const id = this.getAttribute('data-id');
            const nombre = this.getAttribute('data-nombre');
            console.log('Mostrando detalles de receta:', nombre);
        });
    });
}

// ============================================
// RANKING DE RENTABILIDAD
// ============================================
function ordenarPorRentabilidad() {
    const grid = document.getElementById('recetas-grid');
    if (!grid) return;

    const cards = Array.from(grid.querySelectorAll('.menu-card'));

    cards.sort((a, b) => {
        const margenA = parseFloat(a.getAttribute('data-margen'));
        const margenB = parseFloat(b.getAttribute('data-margen'));
        return margenB - margenA;
    });

    // Animar salida
    cards.forEach(card => {
        card.style.transition = 'all 0.3s ease';
        card.style.opacity = '0';
        card.style.transform = 'scale(0.9)';
    });

    setTimeout(() => {
        grid.innerHTML = '';
        cards.forEach((card, index) => {
            grid.appendChild(card);
            setTimeout(() => {
                card.style.opacity = '1';
                card.style.transform = 'scale(1)';
            }, index * 50);
        }, 300);
    }, 300);

    mostrarNotificacion('Recetas ordenadas por rentabilidad', 'success');
}

// ============================================
// VALIDACIÓN DE FORMULARIO
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form[action*="CrearReceta"], form[action*="EditarReceta"]');

    if (form) {
        form.addEventListener('submit', function (e) {
            const container = document.getElementById('ingredientes-container');

            if (!container || container.children.length === 0) {
                e.preventDefault();
                mostrarNotificacion('Debes agregar al menos un ingrediente a la receta', 'error');
                return false;
            }

            // Validar que todos los ingredientes tengan insumo y cantidad
            const items = container.querySelectorAll('.ingrediente-item');
            let todosCompletos = true;

            items.forEach(item => {
                const selectInsumo = item.querySelector('.select-insumo');
                const inputCantidad = item.querySelector('.input-cantidad');

                if (!selectInsumo.value || !inputCantidad.value || parseFloat(inputCantidad.value) <= 0) {
                    todosCompletos = false;
                    selectInsumo.style.borderColor = '#e74c3c';
                    inputCantidad.style.borderColor = '#e74c3c';
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

            // Validar disponibilidad de stock
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
// ANIMACIONES
// ============================================
function animateOnLoadReceta() {
    const cards = document.querySelectorAll('.menu-card');

    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';

        setTimeout(() => {
            card.style.transition = 'all 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
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
`;
document.head.appendChild(notifStyle);

console.log('%c✓ Script de recetas cargado correctamente', 'color: #27ae60; font-weight: bold;');