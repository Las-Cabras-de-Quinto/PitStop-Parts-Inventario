// =======================================================
// FUNCIONES AUXILIARES PARA CARGA DE DATOS EN MODALES
// =======================================================

/**
 * Funciones para abrir modales con datos específicos
 * Estas funciones se llaman desde los botones de acción en las tablas
 */

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE CATEGORÍAS
// =======================================================

function abrirModalVisualizarCategoria(id) {
    cargarCategoria(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarCategoriaModal'));
    modal.show();
}

function abrirModalEditarCategoria(id) {
    cargarCategoria(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarCategoriaModal'));
    modal.show();
}

function abrirModalEliminarCategoria(id) {
    window.currentCategoriaId = id;
    // Cargar datos para mostrar en el modal de confirmación
    cargarCategoria(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarCategoriaModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE PRODUCTOS
// =======================================================

function abrirModalVisualizarProducto(id) {
    cargarProducto(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarProductoModal'));
    modal.show();
}

function abrirModalEditarProducto(id) {
    cargarProducto(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarProductoModal'));
    modal.show();
}

function abrirModalEliminarProducto(id) {
    window.currentProductoId = id;
    cargarProducto(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarProductoModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE PROVEEDORES
// =======================================================

function abrirModalVisualizarProveedor(id) {
    cargarProveedor(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarProveedorModal'));
    modal.show();
}

function abrirModalEditarProveedor(id) {
    cargarProveedor(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarProveedorModal'));
    modal.show();
}

function abrirModalEliminarProveedor(id) {
    window.currentProveedorId = id;
    cargarProveedor(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarProveedorModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE BODEGAS
// =======================================================

function abrirModalVisualizarBodega(id) {
    cargarBodega(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarBodegaModal'));
    modal.show();
}

function abrirModalEditarBodega(id) {
    cargarBodega(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarBodegaModal'));
    modal.show();
}

function abrirModalEliminarBodega(id) {
    window.currentBodegaId = id;
    cargarBodega(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarBodegaModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE MARCAS
// =======================================================

function abrirModalVisualizarMarca(id) {
    cargarMarca(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarMarcaModal'));
    modal.show();
}

function abrirModalEditarMarca(id) {
    cargarMarca(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarMarcaModal'));
    modal.show();
}

function abrirModalEliminarMarca(id) {
    window.currentMarcaId = id;
    cargarMarca(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarMarcaModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE ESTADOS
// =======================================================

function abrirModalVisualizarEstado(id) {
    cargarEstado(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarEstadoModal'));
    modal.show();
}

function abrirModalEditarEstado(id) {
    cargarEstado(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarEstadoModal'));
    modal.show();
}

function abrirModalEliminarEstado(id) {
    window.currentEstadoId = id;
    cargarEstado(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarEstadoModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE ENTRADAS
// =======================================================

function abrirModalVisualizarEntrada(id) {
    cargarEntrada(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarEntradaModal'));
    modal.show();
}

function abrirModalEditarEntrada(id) {
    cargarEntrada(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarEntradaModal'));
    modal.show();
}

function abrirModalEliminarEntrada(id) {
    window.currentEntradaId = id;
    cargarEntrada(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarEntradaModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ABRIR MODALES DE AJUSTES
// =======================================================

function abrirModalVisualizarAjuste(id) {
    cargarAjuste(id, 'visualizar');
    const modal = new bootstrap.Modal(document.getElementById('visualizarAjusteModal'));
    modal.show();
}

function abrirModalEditarAjuste(id) {
    cargarAjuste(id, 'editar');
    const modal = new bootstrap.Modal(document.getElementById('editarAjusteModal'));
    modal.show();
}

function abrirModalEliminarAjuste(id) {
    window.currentAjusteId = id;
    cargarAjuste(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarAjusteModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA CARGAR LISTAS DESPLEGABLES
// =======================================================

/**
 * Cargar categorías en un select
 */
async function cargarCategoriasEnSelect(selectId) {
    try {
        const response = await fetch('/Categoria/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    // Limpiar opciones existentes excepto la primera
                    select.innerHTML = '<option value="">Seleccionar categoría</option>';
                    
                    result.data.forEach(categoria => {
                        const option = document.createElement('option');
                        option.value = categoria.id;
                        option.textContent = categoria.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar categorías:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar categoría</option>';
            Object.values(datosDemo.categorias).forEach(categoria => {
                const option = document.createElement('option');
                option.value = categoria.id;
                option.textContent = categoria.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar marcas en un select
 */
async function cargarMarcasEnSelect(selectId) {
    try {
        const response = await fetch('/Marca/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    select.innerHTML = '<option value="">Seleccionar marca</option>';
                    
                    result.data.forEach(marca => {
                        const option = document.createElement('option');
                        option.value = marca.id;
                        option.textContent = marca.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar marcas:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar marca</option>';
            Object.values(datosDemo.marcas).forEach(marca => {
                const option = document.createElement('option');
                option.value = marca.id;
                option.textContent = marca.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar bodegas en un select
 */
async function cargarBodegasEnSelect(selectId) {
    try {
        const response = await fetch('/Bodega/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    select.innerHTML = '<option value="">Seleccionar bodega</option>';
                    
                    result.data.forEach(bodega => {
                        const option = document.createElement('option');
                        option.value = bodega.id;
                        option.textContent = bodega.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar bodegas:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar bodega</option>';
            Object.values(datosDemo.bodegas).forEach(bodega => {
                const option = document.createElement('option');
                option.value = bodega.id;
                option.textContent = bodega.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar productos en un select
 */
async function cargarProductosEnSelect(selectId) {
    try {
        const response = await fetch('/Producto/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    select.innerHTML = '<option value="">Seleccionar producto</option>';
                    
                    result.data.forEach(producto => {
                        const option = document.createElement('option');
                        option.value = producto.id;
                        option.textContent = producto.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar productos:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar producto</option>';
            Object.values(datosDemo.productos).forEach(producto => {
                const option = document.createElement('option');
                option.value = producto.id;
                option.textContent = producto.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar proveedores en un select
 */
async function cargarProveedoresEnSelect(selectId) {
    try {
        const response = await fetch('/Proveedor/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    select.innerHTML = '<option value="">Seleccionar proveedor</option>';
                    
                    result.data.forEach(proveedor => {
                        const option = document.createElement('option');
                        option.value = proveedor.id;
                        option.textContent = proveedor.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar proveedores:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar proveedor</option>';
            Object.values(datosDemo.proveedores).forEach(proveedor => {
                const option = document.createElement('option');
                option.value = proveedor.id;
                option.textContent = proveedor.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar estados en un select
 */
async function cargarEstadosEnSelect(selectId) {
    try {
        const response = await fetch('/Estado/ObtenerParaSelect');
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                const select = document.getElementById(selectId);
                if (select) {
                    select.innerHTML = '<option value="">Seleccionar estado</option>';
                    
                    result.data.forEach(estado => {
                        const option = document.createElement('option');
                        option.value = estado.id;
                        option.textContent = estado.nombre;
                        select.appendChild(option);
                    });
                }
            }
        }
    } catch (error) {
        console.error('Error al cargar estados:', error);
        // Usar datos demo si falla la API
        const datosDemo = cargarDatosDemo();
        const select = document.getElementById(selectId);
        if (select) {
            select.innerHTML = '<option value="">Seleccionar estado</option>';
            Object.values(datosDemo.estados).forEach(estado => {
                const option = document.createElement('option');
                option.value = estado.id;
                option.textContent = estado.nombre;
                select.appendChild(option);
            });
        }
    }
}

/**
 * Cargar todos los estados disponibles en múltiples selects
 */
async function cargarEstadosEnMultiplesSelects(selectIds) {
    if (Array.isArray(selectIds)) {
        selectIds.forEach(selectId => {
            cargarEstadosEnSelect(selectId);
        });
    }
}

/**
 * Inicializar estados en todos los selects de estados de una página
 */
function inicializarEstadosEnPagina() {
    // Buscar todos los selects que contengan "estado" en su ID
    const selectsEstados = document.querySelectorAll('select[id*="estado"], select[id*="Estado"]');
    selectsEstados.forEach(select => {
        if (select.id) {
            cargarEstadosEnSelect(select.id);
        }
    });
}

// =======================================================
// FUNCIONES PARA MANEJO DE TABLAS DINÁMICAS
// =======================================================

/**
 * Agregar producto a la tabla de ajustes
 */
function agregarProductoAjuste(tipo = 'agregar') {
    let selectProducto, inputCantidad, tbody;
    
    if (tipo === 'agregar') {
        selectProducto = document.getElementById('productoAjuste');
        inputCantidad = document.getElementById('cantidadAjuste');
        tbody = document.getElementById('tablaProductosAjuste');
    } else if (tipo === 'editar') {
        selectProducto = document.getElementById('editProductoAjuste');
        inputCantidad = document.getElementById('editCantidadAjuste');
        tbody = document.getElementById('editTablaProductosAjuste');
    }

    // Validar que se seleccionó un producto y se especificó la cantidad
    if (!selectProducto.value || !inputCantidad.value) {
        showToast('Seleccione un producto y especifique la cantidad', 'warning');
        return;
    }
    
    // Verificar si el producto ya está en la tabla
    const productoExistente = tbody.querySelector(`tr[data-producto-id="${selectProducto.value}"]`);

    if (productoExistente) {
        showToast('El producto ya está agregado a la lista', 'warning');
        return;
    }

    // Crear nueva fila
    const fila = document.createElement('tr');
    fila.dataset.productoId = selectProducto.value;
    fila.innerHTML = `
        <td><p data-field="nombre">${selectProducto.options[selectProducto.selectedIndex].text}</p></td>
        <td><p data-field="cantidad">${inputCantidad.value}</p></td>
        <td>
            <button type="button" class="btn btn-danger btn-sm" onclick="quitarProductoAjuste(this)">
                <i class="fas fa-trash"></i>
            </button>
        </td>
    `;
    
    // Si es la primera fila, quitar el mensaje "No hay productos"
    if (tbody.children.length === 1 && tbody.children[0].textContent.includes('No hay productos')) {
        tbody.innerHTML = '';
    }
    
    tbody.appendChild(fila);
    
    // Limpiar campos
    selectProducto.value = '';
    inputCantidad.value = '';
}

/**
 * Quitar producto de la tabla de ajustes
 */
function quitarProductoAjuste(boton) {
    const fila = boton.closest('tr');
    const tbody = fila.parentNode;
    
    fila.remove();
    
    // Si no quedan productos, mostrar mensaje
    if (tbody.children.length === 0) {
        tbody.innerHTML = '<tr class="text-center"><td colspan="3" class="text-white">No hay productos agregados</td></tr>';
    }
}

// =======================================================
// FUNCIONES DE INICIALIZACIÓN ESPECÍFICAS
// =======================================================

/**
 * Inicializar datos cuando se abre un modal que requiere listas
 */
function inicializarDatosModal(modalId) {
    switch (modalId) {
        case 'nuevoProductoModal':
            cargarCategoriasEnSelect('categoriaProducto');
            cargarMarcasEnSelect('marcaProducto');
            cargarProveedoresEnSelect('proveedorProducto');
            cargarEstadosEnSelect('estadoProducto');
            break;
            
        case 'editarProductoModal':
            cargarCategoriasEnSelect('editCategoriaProducto');
            cargarMarcasEnSelect('editMarcaProducto');
            cargarProveedoresEnSelect('editProveedorProducto');
            cargarEstadosEnSelect('editEstadoProducto');
            break;
            
        case 'nuevoProveedorModal':
            cargarEstadosEnSelect('estadoProveedor');
            break;
            
        case 'editarProveedorModal':
            cargarEstadosEnSelect('editEstadoProveedor');
            break;
            
        case 'nuevaBodegaModal':
            cargarEstadosEnSelect('estadoBodega');
            break;
            
        case 'editarBodegaModal':
            cargarEstadosEnSelect('editEstadoBodega');
            break;
            
        case 'nuevaMarcaModal':
            cargarEstadosEnSelect('estadoMarca');
            break;
            
        case 'editarMarcaModal':
            cargarEstadosEnSelect('editEstadoMarca');
            break;
            
        case 'nuevaCategoriaModal':
            cargarEstadosEnSelect('estadoCategoria');
            break;
            
        case 'editarCategoriaModal':
            cargarEstadosEnSelect('editEstadoCategoria');
            break;
            
        case 'nuevaEntradaModal':
            cargarProductosEnSelect('productoEntrada');
            cargarBodegasEnSelect('bodegaEntrada');
            cargarProveedoresEnSelect('proveedorEntrada');
            cargarEstadosEnSelect('estadoEntrada');
            break;
            
        case 'editarEntradaModal':
            cargarProductosEnSelect('editProductoEntrada');
            cargarBodegasEnSelect('editBodegaEntrada');
            cargarProveedoresEnSelect('editProveedorEntrada');
            cargarEstadosEnSelect('editEstadoEntrada');
            break;
            
        case 'nuevoAjusteModal':
            cargarBodegasEnSelect('bodegaAjuste');
            cargarProductosEnSelect('productoAjuste');
            cargarEstadosEnSelect('estadoAjuste');
            break;
            
        case 'editarAjusteModal':
            cargarBodegasEnSelect('editBodegaAjuste');
            cargarProductosEnSelect('editProductoAjuste');
            cargarEstadosEnSelect('editEstadoAjuste');
            break;
    }
}

// =======================================================
// EVENT LISTENERS ADICIONALES
// =======================================================

document.addEventListener('DOMContentLoaded', function() {
    // Configurar event listeners para inicializar datos en modales
    const modalesConDatos = [
        'nuevoProductoModal', 'editarProductoModal',
        'nuevoProveedorModal', 'editarProveedorModal',
        'nuevaBodegaModal', 'editarBodegaModal',
        'nuevaMarcaModal', 'editarMarcaModal',
        'nuevaCategoriaModal', 'editarCategoriaModal',
        'nuevoEstadoModal', 'editarEstadoModal',
        'nuevaEntradaModal', 'editarEntradaModal',
        'nuevoAjusteModal', 'editarAjusteModal'
    ];
    
    modalesConDatos.forEach(modalId => {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.addEventListener('show.bs.modal', function() {
                inicializarDatosModal(modalId);
            });
        }
    });
    
    // Configurar búsqueda en tiempo real en selects (si se implementa)
    const selectsBuscables = document.querySelectorAll('select[data-searchable="true"]');
    selectsBuscables.forEach(select => {
        // Implementar funcionalidad de búsqueda si es necesario
    });
});

// =======================================================
// FUNCIONES DE UTILIDAD PARA FORMATO
// =======================================================

/**
 * Formatear fecha para mostrar
 */
function formatearFecha(fecha) {
    if (!fecha) return '';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-ES');
}

/**
 * Formatear número como moneda
 */
function formatearMoneda(cantidad) {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: 'EUR'
    }).format(cantidad);
}

/**
 * Formatear número entero
 */
function formatearNumero(numero) {
    return new Intl.NumberFormat('es-ES').format(numero);
}

// Inicializar datos demo al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    // Inicializar estados en todos los selects de estados de la página
    inicializarEstadosEnPagina();
    
    // También inicializar otros selects comunes
    setTimeout(() => {
        // Dar tiempo para que se carguen los elementos del DOM
        if (document.getElementById('categoriaProducto')) cargarCategoriasEnSelect('categoriaProducto');
        if (document.getElementById('marcaProducto')) cargarMarcasEnSelect('marcaProducto');
        if (document.getElementById('proveedorProducto')) cargarProveedoresEnSelect('proveedorProducto');
        if (document.getElementById('bodegaAjuste')) cargarBodegasEnSelect('bodegaAjuste');
        if (document.getElementById('productoAjuste')) cargarProductosEnSelect('productoAjuste');
    }, 100);
});