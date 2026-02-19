// ============================================
// VARIABLES GLOBALES
// ============================================
let recetaIndex = 0;
let recetasDisponibles = [];

// Datos de demo de recetas (en producción vendrían del servidor)
const recetasDemoData = [
    { id: 1, nombre: "Torta de Chocolate", categoria: "Pasteles", precioVenta: 28000.00 },
    { id: 2, nombre: "Café Capuchino", categoria: "Bebidas Calientes", precioVenta: 2500.00 },
    { id: 3, nombre: "Croissant de Mantequilla", categoria: "Panadería", precioVenta: 12000.00 },
    { id: 4, nombre: "Galletas de Avena", categoria: "Galletas", precioVenta: 9000.00 },
    { id: 5, nombre: "Muffin de Arándanos", categoria: "Panadería", precioVenta: 13200.00 },
    { id: 6, nombre: "Brownie de Chocolate", categoria: "Postres", precioVenta: 16000.00 },
    { id: 7, nombre: "Smoothie de Fresa", categoria: "Bebidas Frías", precioVenta: 4000.00 },
    { id: 8, nombre: "Cheesecake de Vainilla", categoria: "Pasteles", precioVenta: 25000.00 }
];

// ============================================
// INICIALIZACIÓN
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    // Cargar recetas disponibles
    recetasDisponibles = recetasDemoData;

    // Agregar primera receta automáticamente si estamos en crear
    const btnAgregar = document.getElementById('btnAgregarReceta');
    if (btnAgregar) {
        // Solo agregar automáticamente si no hay recetas cargadas
        const container = document.getElementById('recetas-container');
        if (container && container.children.length === 0) {
            agregarReceta();
        }

        btnAgregar.addEventListener('click', agregarReceta);
    }

    // Event listeners para fechas
    const inputFechaInicio = document.querySelector('input[name="FechaInicio"]');
    const inputFechaFin = document.querySelector('input[name="FechaFin"]');

    if (inputFechaInicio && inputFechaFin) {
        inputFechaInicio.addEventListener('change', calcularDuracion);
        inputFechaFin.addEventListener('change', calcularDuracion);
    }

    // Event listener para descuento
    const inputDescuento = document.querySelector('input[name="PorcentajeDescuento"]');
    if (inputDescuento) {
        inputDescuento.addEventListener('input', actualizarResumen);
    }

    // Calcular duración inicial si hay fechas
    calcularDuracion();
    actualizarResumen();
});

// ============================================
// GESTIÓN DE RECETAS
// ============================================
function agregarReceta() {
    const template = document.getElementById('receta-template');
    const container = document.getElementById('recetas-container');

    if (!template || !container) return;

    // Clonar template
    const clone = template.content.cloneNode(true);

    // Actualizar índices
    const inputs = clone.querySelectorAll('input, select');
    inputs.forEach(input => {
        if (input.name) {
            input.name = input.name.replace('INDEX', recetaIndex);
        }
    });

    // Llenar select de recetas
    const selectReceta = clone.querySelector('.select-receta');
    recetasDisponibles.forEach(receta => {
        const option = document.createElement('option');
        option.value = receta.id;
        option.textContent = `${receta.nombre} (${receta.categoria})`;
        option.setAttribute('data-precio', receta.precioVenta);
        option.setAttribute('data-categoria', receta.categoria);
        selectReceta.appendChild(option);
    });

    // Event listeners para la nueva receta
    const selectRecetaElement = clone.querySelector('.select-receta');
    const btnEliminar = clone.querySelector('.btn-eliminar-receta');

    selectRecetaElement.addEventListener('change', function () {
        actualizarPreciosReceta(this.closest('.receta-item'));
    });

    btnEliminar.addEventListener('click', function () {
        eliminarReceta(this.closest('.receta-item'));
    });

    container.appendChild(clone);
    recetaIndex++;

    // Animar entrada
    const items = container.querySelectorAll('.receta-item');
    const lastItem = items[items.length - 1];
    lastItem.style.opacity = '0';
    lastItem.style.transform = 'translateY(20px)';
    setTimeout(() => {
        lastItem.style.transition = 'all 0.3s ease';
        lastItem.style.opacity = '1';
        lastItem.style.transform = 'translateY(0)';
    }, 10);

    actualizarResumen();
}

function eliminarReceta(item) {
    item.style.transition = 'all 0.3s ease';
    item.style.opacity = '0';
    item.style.transform = 'translateX(-20px)';

    setTimeout(() => {
        item.remove();
        actualizarResumen();

        // Verificar si no hay recetas
        const container = document.getElementById('recetas-container');
        if (container && container.children.length === 0) {
            mostrarNotificacion('Debes agregar al menos una receta', 'warning');
        }
    }, 300);
}

function actualizarPreciosReceta(item) {
    const selectReceta = item.querySelector('.select-receta');
    const inputPrecioOriginal = item.querySelector('.precio-original');
    const inputPrecioDescuento = item.querySelector('.precio-descuento');
    const infoReceta = item.querySelector('.info-receta');
    const categoriaReceta = item.querySelector('.categoria-receta');
    const ahorroReceta = item.querySelector('.ahorro-receta');

    if (!selectReceta.value) {
        inputPrecioOriginal.value = '₡0.00';
        inputPrecioDescuento.value = '₡0.00';
        infoReceta.style.display = 'none';
        return;
    }

    const selectedOption = selectReceta.options[selectReceta.selectedIndex];
    const precioOriginal = parseFloat(selectedOption.getAttribute('data-precio') || 0);
    const categoria = selectedOption.getAttribute('data-categoria') || '';

    // Obtener descuento global
    const inputDescuento = document.querySelector('input[name="PorcentajeDescuento"]');
    const descuento = parseFloat(inputDescuento?.value || 0);

    const precioConDescuento = precioOriginal * (1 - descuento / 100);
    const ahorro = precioOriginal - precioConDescuento;

    inputPrecioOriginal.value = '₡' + precioOriginal.toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    inputPrecioDescuento.value = '₡' + precioConDescuento.toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    categoriaReceta.textContent = categoria;
    ahorroReceta.textContent = '₡' + ahorro.toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    infoReceta.style.display = 'block';
}

// ============================================
// CÁLCULOS Y ACTUALIZACIONES
// ============================================
function calcularDuracion() {
    const inputFechaInicio = document.querySelector('input[name="FechaInicio"]');
    const inputFechaFin = document.querySelector('input[name="FechaFin"]');
    const divDuracion = document.getElementById('duracionPromocion');
    const textoDuracion = document.getElementById('textoDuracion');

    if (!inputFechaInicio || !inputFechaFin || !divDuracion) return;

    const fechaInicio = inputFechaInicio.value;
    const fechaFin = inputFechaFin.value;

    if (fechaInicio && fechaFin) {
        const inicio = new Date(fechaInicio);
        const fin = new Date(fechaFin);

        if (fin < inicio) {
            divDuracion.style.display = 'block';
            divDuracion.style.background = 'linear-gradient(135deg, rgba(231, 76, 60, 0.1), rgba(192, 57, 43, 0.1))';
            textoDuracion.textContent = 'La fecha de fin no puede ser anterior a la fecha de inicio';
            textoDuracion.style.color = 'var(--red)';
            return;
        }

        const diferenciaDias = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24)) + 1;

        divDuracion.style.display = 'flex';
        divDuracion.style.background = 'linear-gradient(135deg, rgba(32, 116, 118, 0.1), rgba(98, 170, 118, 0.1))';

        let textoCompleto = `${diferenciaDias} día${diferenciaDias !== 1 ? 's' : ''}`;

        if (diferenciaDias === 1) {
            textoCompleto += ' (Promoción de un solo día)';
        } else if (diferenciaDias <= 7) {
            textoCompleto += ' (Promoción de una semana)';
        } else if (diferenciaDias <= 30) {
            textoCompleto += ' (Promoción mensual)';
        } else {
            const meses = Math.floor(diferenciaDias / 30);
            textoCompleto += ` (Aproximadamente ${meses} mes${meses !== 1 ? 'es' : ''})`;
        }

        textoDuracion.textContent = textoCompleto;
        textoDuracion.style.color = 'var(--gray)';

        actualizarResumen();
    } else {
        divDuracion.style.display = 'none';
    }
}

function actualizarResumen() {
    // Actualizar descuento
    const inputDescuento = document.querySelector('input[name="PorcentajeDescuento"]');
    const descuento = parseFloat(inputDescuento?.value || 0);

    const resumenDescuento = document.getElementById('resumenDescuento');
    if (resumenDescuento) {
        resumenDescuento.textContent = descuento.toFixed(2) + '%';
    }

    // Actualizar cantidad de recetas
    const container = document.getElementById('recetas-container');
    const cantidadRecetas = container ? container.children.length : 0;

    const resumenRecetas = document.getElementById('resumenRecetas');
    if (resumenRecetas) {
        resumenRecetas.textContent = cantidadRecetas;
    }

    // Actualizar duración
    const inputFechaInicio = document.querySelector('input[name="FechaInicio"]');
    const inputFechaFin = document.querySelector('input[name="FechaFin"]');
    const resumenDuracion = document.getElementById('resumenDuracion');

    if (resumenDuracion && inputFechaInicio?.value && inputFechaFin?.value) {
        const inicio = new Date(inputFechaInicio.value);
        const fin = new Date(inputFechaFin.value);
        const diferenciaDias = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24)) + 1;

        if (diferenciaDias > 0) {
            resumenDuracion.textContent = `${diferenciaDias} día${diferenciaDias !== 1 ? 's' : ''}`;
        } else {
            resumenDuracion.textContent = '-';
        }
    }

    // Actualizar precios de todas las recetas si hay cambio en el descuento
    if (container) {
        const items = container.querySelectorAll('.receta-item');
        items.forEach(item => {
            const select = item.querySelector('.select-receta');
            if (select && select.value) {
                actualizarPreciosReceta(item);
            }
        });
    }
}

// ============================================
// VALIDACIÓN DE FORMULARIO
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form[action*="CrearPromocion"], form[action*="EditarPromocion"]');

    if (form) {
        form.addEventListener('submit', function (e) {
            // Validar fechas
            const inputFechaInicio = document.querySelector('input[name="FechaInicio"]');
            const inputFechaFin = document.querySelector('input[name="FechaFin"]');

            if (inputFechaInicio && inputFechaFin) {
                const fechaInicio = new Date(inputFechaInicio.value);
                const fechaFin = new Date(inputFechaFin.value);

                if (fechaFin < fechaInicio) {
                    e.preventDefault();
                    mostrarNotificacion('La fecha de fin no puede ser anterior a la fecha de inicio', 'error');
                    inputFechaFin.style.borderColor = 'var(--red)';
                    return false;
                }
            }

            // Validar que haya al menos una receta
            const container = document.getElementById('recetas-container');

            if (!container || container.children.length === 0) {
                e.preventDefault();
                mostrarNotificacion('Debes agregar al menos una receta a la promoción', 'error');
                return false;
            }

            // Validar que todas las recetas tengan una receta seleccionada
            const items = container.querySelectorAll('.receta-item');
            let todasCompletas = true;

            items.forEach(item => {
                const selectReceta = item.querySelector('.select-receta');

                if (!selectReceta.value) {
                    todasCompletas = false;
                    selectReceta.style.borderColor = 'var(--red)';
                } else {
                    selectReceta.style.borderColor = '';
                }
            });

            if (!todasCompletas) {
                e.preventDefault();
                mostrarNotificacion('Selecciona una receta para cada entrada', 'error');
                return false;
            }

            // Validar descuento
            const inputDescuento = document.querySelector('input[name="PorcentajeDescuento"]');
            const descuento = parseFloat(inputDescuento?.value || 0);

            if (descuento <= 0 || descuento > 100) {
                e.preventDefault();
                mostrarNotificacion('El descuento debe estar entre 0 y 100', 'error');
                inputDescuento.style.borderColor = 'var(--red)';
                return false;
            }
        });
    }
});

// ============================================
// ANIMACIONES
// ============================================
function animateOnLoadPromocion() {
    const cards = document.querySelectorAll('.feature-card');

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

// Ejecutar animaciones al cargar
document.addEventListener('DOMContentLoaded', animateOnLoadPromocion);

console.log('%c✓ Script de promociones cargado correctamente', 'color: var(--green); font-weight: bold;');