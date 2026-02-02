// receta-editar.js - JavaScript para el formulario de edición de recetas

document.addEventListener('DOMContentLoaded', function () {
    let ingredienteIndex = 0;
    const insumosData = {}; // Almacena información de insumos (costo, disponibilidad, etc.)

    // Cargar datos de insumos desde el servidor
    function cargarDatosInsumos() {
        const insumos = window.insumosDisponibles || [];

        insumos.forEach(insumo => {
            insumosData[insumo.idInsumo] = {
                nombreInsumo: insumo.nombreInsumo,
                costoUnitario: insumo.costoUnitario,
                cantidadDisponible: insumo.cantidadDisponible,
                unidadMedida: insumo.unidadMedida
            };
        });
    }

    // Agregar un nuevo ingrediente
    function agregarIngrediente(idInsumo = '', cantidad = '') {
        const template = document.getElementById('ingrediente-template');
        const container = document.getElementById('ingredientes-container');

        if (!template || !container) {
            console.error('Template o container no encontrado');
            return;
        }

        const clone = template.content.cloneNode(true);

        // Actualizar los nombres de los inputs para que sean arrays
        const selectInsumo = clone.querySelector('.select-insumo');
        const inputCantidad = clone.querySelector('.input-cantidad');

        selectInsumo.name = 'IngredientesIdInsumo';
        inputCantidad.name = 'IngredientesCantidad';

        // Cargar opciones de insumos
        const insumos = window.insumosDisponibles || [];
        insumos.forEach(insumo => {
            const option = document.createElement('option');
            option.value = insumo.idInsumo;
            option.textContent = `${insumo.nombreInsumo} (${insumo.unidadMedida})`;

            if (insumo.idInsumo == idInsumo) {
                option.selected = true;
            }

            selectInsumo.appendChild(option);
        });

        // Establecer cantidad si se proporcionó
        if (cantidad) {
            inputCantidad.value = cantidad;
        }

        // Agregar evento para calcular costo cuando cambie el insumo o cantidad
        selectInsumo.addEventListener('change', function () {
            calcularCostoIngrediente(this);
            calcularCostoTotal();
        });

        inputCantidad.addEventListener('input', function () {
            calcularCostoIngrediente(this);
            calcularCostoTotal();
        });

        // Botón eliminar
        const btnEliminar = clone.querySelector('.btn-eliminar-ingrediente');
        btnEliminar.addEventListener('click', function () {
            this.closest('.ingrediente-item').remove();
            calcularCostoTotal();
        });

        container.appendChild(clone);
        ingredienteIndex++;

        // Calcular costo si ya tiene datos
        if (idInsumo && cantidad) {
            setTimeout(() => {
                const items = container.querySelectorAll('.ingrediente-item');
                const ultimoItem = items[items.length - 1];
                calcularCostoIngrediente(ultimoItem.querySelector('.select-insumo'));
                calcularCostoTotal();
            }, 100);
        }
    }

    // Calcular el costo de un ingrediente individual
    function calcularCostoIngrediente(elemento) {
        const item = elemento.closest('.ingrediente-item');
        if (!item) return;

        const selectInsumo = item.querySelector('.select-insumo');
        const inputCantidad = item.querySelector('.input-cantidad');
        const inputCosto = item.querySelector('.costo-ingrediente');
        const alertaStock = item.querySelector('.alerta-stock');
        const mensajeAlerta = item.querySelector('.mensaje-alerta');

        const idInsumo = selectInsumo.value;
        const cantidad = parseFloat(inputCantidad.value) || 0;

        if (!idInsumo || cantidad <= 0) {
            inputCosto.value = '₡0.00';
            alertaStock.style.display = 'none';
            return;
        }

        // Buscar el insumo en los datos
        const insumos = window.insumosDisponibles || [];
        const insumo = insumos.find(i => i.idInsumo == idInsumo);

        if (insumo) {
            const costoTotal = cantidad * insumo.costoUnitario;
            inputCosto.value = `₡${costoTotal.toFixed(2)}`;

            // Verificar disponibilidad en stock
            if (cantidad > insumo.cantidadDisponible) {
                alertaStock.style.display = 'block';
                if (insumo.cantidadDisponible > 0) {
                    mensajeAlerta.textContent = `Stock insuficiente. Disponible: ${insumo.cantidadDisponible} ${insumo.unidadMedida}`;
                } else {
                    mensajeAlerta.textContent = 'Sin stock disponible';
                }
            } else {
                alertaStock.style.display = 'none';
            }
        }
    }

    // Calcular el costo total de la receta
    function calcularCostoTotal() {
        const items = document.querySelectorAll('.ingrediente-item');
        let costoTotal = 0;

        items.forEach(item => {
            const costoTexto = item.querySelector('.costo-ingrediente').value;
            const costo = parseFloat(costoTexto.replace('₡', '').replace(',', '')) || 0;
            costoTotal += costo;
        });

        // Actualizar el resumen
        document.getElementById('costoTotalReceta').textContent = `₡${costoTotal.toFixed(2)}`;

        // Calcular costo por porción
        const porciones = parseInt(document.querySelector('input[name="CantidadPorciones"]').value) || 1;
        const costoPorPorcion = costoTotal / porciones;
        document.getElementById('costoPorPorcion').textContent = `₡${costoPorPorcion.toFixed(2)}`;

        // Calcular ganancia si hay precio de venta
        const precioVentaInput = document.querySelector('input[name="PrecioVenta"]');
        const precioVenta = parseFloat(precioVentaInput.value) || 0;

        if (precioVenta > 0) {
            document.getElementById('resumenGanancia').style.display = 'block';
            document.getElementById('precioVentaMostrar').textContent = `₡${precioVenta.toFixed(2)}`;

            const ganancia = precioVenta - costoTotal;
            document.getElementById('gananciaNeta').textContent = `₡${ganancia.toFixed(2)}`;

            const margen = costoTotal > 0 ? ((ganancia / costoTotal) * 100) : 0;
            document.getElementById('margenGanancia').textContent = `${margen.toFixed(2)}%`;
        } else {
            document.getElementById('resumenGanancia').style.display = 'none';
        }
    }

    // Evento para el botón de agregar ingrediente
    const btnAgregar = document.getElementById('btnAgregarIngrediente');
    if (btnAgregar) {
        btnAgregar.addEventListener('click', function () {
            agregarIngrediente();
        });
    }

    // Evento para recalcular cuando cambie el precio de venta
    const precioVentaInput = document.querySelector('input[name="PrecioVenta"]');
    if (precioVentaInput) {
        precioVentaInput.addEventListener('input', calcularCostoTotal);
    }

    // Evento para recalcular cuando cambien las porciones
    const porcionesInput = document.querySelector('input[name="CantidadPorciones"]');
    if (porcionesInput) {
        porcionesInput.addEventListener('input', calcularCostoTotal);
    }

    // Cargar ingredientes existentes (para modo edición)
    function cargarIngredientesExistentes() {
        // Los ingredientes existentes deberían venir del modelo
        if (window.ingredientesExistentes && Array.isArray(window.ingredientesExistentes)) {
            window.ingredientesExistentes.forEach(ing => {
                agregarIngrediente(ing.idInsumo, ing.cantidadUtilizada);
            });
        }
    }

    // Validación antes de enviar el formulario
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            const items = document.querySelectorAll('.ingrediente-item');

            if (items.length === 0) {
                e.preventDefault();
                alert('Debes agregar al menos un ingrediente a la receta');
                return false;
            }

            // Verificar que todos los ingredientes tengan insumo y cantidad
            let hayErrores = false;
            items.forEach(item => {
                const select = item.querySelector('.select-insumo');
                const input = item.querySelector('.input-cantidad');

                if (!select.value || !input.value || parseFloat(input.value) <= 0) {
                    hayErrores = true;
                }
            });

            if (hayErrores) {
                e.preventDefault();
                alert('Todos los ingredientes deben tener un insumo seleccionado y una cantidad válida');
                return false;
            }
        });
    }

    // Inicialización
    cargarDatosInsumos();
    cargarIngredientesExistentes();
});