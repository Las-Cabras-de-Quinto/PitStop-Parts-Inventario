// Función para formatear fecha para mostrar
function formatearFecha(fecha) {
    if (!fecha) return '';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-ES');
}

// =======================================================
// CONFIGURACIÓN GLOBAL Y UTILIDADES
// =======================================================

const API_BASE_URL = '/api';

// Función para mostrar notificaciones toast
function showToast(message, type = 'success') {
    const toast = document.getElementById('successToast');
    if (toast) {
        const toastBody = toast.querySelector('.toast-body');
        if (toastBody) {
            toastBody.textContent = message;
        }
        
        // Cambiar colores según el tipo
        const toastHeader = toast.querySelector('.toast-header');
        if (toastHeader) {
            toastHeader.className = `toast-header bg-${type} text-white`;
        }

        // Cambiar el título según el tipo
        const toastTitle = toast.querySelector('.toast-title');
        if (toastTitle) {
            let titleText = '';
            switch (type) {
                case 'success':
                    titleText = 'Éxito';
                    break;
                case 'danger':
                    titleText = 'Error';
                    break;
                case 'warning':
                    titleText = 'Advertencia';
                    break;
                case 'info':
                    titleText = 'Información';
                    break;
                default:
                    titleText = 'Notificación';
            }
            toastTitle.textContent = titleText;
        }
        
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
    }
}

// Función genérica para hacer peticiones HTTP
async function makeRequest(url, method = 'GET', data = null) {
    try {
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        };

        if (data && (method === 'POST' || method === 'PUT')) {
            options.body = JSON.stringify(data);
        }

        const response = await fetch(url, options);
        const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || 'Error en la petición');
        }

        return result;
    } catch (error) {
        console.error('Error en la petición:', error);
        showToast(error.message || 'Error de conexión', 'danger');
        throw error;
    }
}

// Función para cargar datos en un modal de visualización
async function cargarDatosEnModal(entidad, id, callback) {
    try {
        const response = await makeRequest(`${API_BASE_URL}/${entidad}/${id}`);
        if (response.success && callback) {
            callback(response.data);
        }
    } catch (error) {
        console.error(`Error al cargar ${entidad}:`, error);
    }
}

// =======================================================
// FUNCIONES PARA CATEGORÍAS
// =======================================================

// Crear nueva categoría
async function guardarCategoria() {
    const form = document.getElementById('nuevaCategoriaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        Nombre: document.getElementById('nombreCategoria')?.value,
        Descripcion: document.getElementById('descripcionCategoria')?.value,
        IdEstado: 1 // Hardcodeado: nuevo estado activo
    };

    try {
        const response = await fetch('/Categoria/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaCategoriaModal'));
            modal.hide();
            form.reset();
            showToast('Categoría creada exitosamente');
            // Esperar 2 segundos antes de recargar para que se vea el toast
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear la categoría', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar categoría:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar categoría
async function actualizarCategoria() {
    const form = document.getElementById('editarCategoriaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const categoriaId = window.currentCategoriaId;
    if (!categoriaId) {
        showToast('Error: ID de la categoría no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdCategoria: categoriaId,
        Nombre: document.getElementById('editNombreCategoria')?.value,
        Descripcion: document.getElementById('editDescripcionCategoria')?.value,
        IdEstado: parseInt(document.getElementById('editEstadoCategoria')?.value)
    };

    try {
        const response = await fetch('/Categoria/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarCategoriaModal'));
            modal.hide();
            showToast('Categoría actualizada exitosamente');
            // Esperar 2 segundos antes de recargar para que se vea el toast
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar la categoría', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar categoría:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar categoría
async function eliminarCategoria() {
    const categoriaId = window.currentCategoriaId;
    if (!categoriaId) {
        showToast('Error: ID de la categoría no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Categoria/Eliminar?id=${categoriaId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarCategoriaModal'));
            modal.hide();
            showToast('Categoría eliminada exitosamente');
            // Esperar 2 segundos antes de recargar para que se vea el toast
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar la categoría', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar categoría:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar categoría para visualizar/editar
async function cargarCategoria(id, modo = 'visualizar') {
    window.currentCategoriaId = id;
    
    try {
        const response = await fetch(`/Categoria/ObtenerPorId?id=${id}`);
        const result = await response.json();
        
        if (result.success) {
            const categoria = result.data;
            mostrarDatosCategoria(categoria, modo);
        } else {
            showToast(result.message || 'Error al cargar la categoría', 'danger');
        }
    } catch (error) {
        console.error('Error al cargar categoría:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Función auxiliar para mostrar datos de categoría en modales
function mostrarDatosCategoria(categoria, modo) {
    if (modo === 'visualizar') {
        document.getElementById('visualizarNombre').textContent = categoria.nombre;
        document.getElementById('visualizarDescripcion').textContent = categoria.descripcion;
        document.getElementById('visualizarEstado').textContent = categoria.estado?.nombre || 'Sin Estado';
        document.getElementById('visualizarFecha').textContent = formatearFecha(categoria.fechaRegistro) || 'Sin Fecha';

        // Mostrar productos relacionados
        const visualizarProductos = document.getElementById('visualizarProductos');
        if (visualizarProductos) {
            const productos = categoria.CategoriaProductos || categoria.productos || [];
            visualizarProductos.textContent = `${productos.length} productos`;
        }
        
    } else if (modo === 'editar') {
        document.getElementById('editNombreCategoria').value = categoria.nombre;
        document.getElementById('editDescripcionCategoria').value = categoria.descripcion;

        // Cargar estados en el select y establecer el valor actual
        if (categoria.estado.idEstado) {
            setTimeout(() => {
                document.getElementById('editEstadoCategoria').value = categoria.estado.idEstado;
            }, 100);
        }
    } else if (modo === 'eliminar') {
        document.getElementById('eliminarNombre').textContent = categoria.nombre;
        document.getElementById('eliminarDescripcion').textContent = categoria.descripcion;
    }
}

// =======================================================
// FUNCIONES PARA PRODUCTOS
// =======================================================

// Crear nuevo producto
async function guardarProducto() {
    const form = document.getElementById('nuevoProductoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    // Obtener los proveedores seleccionados del multi-select
    const selectProveedores = document.getElementById('proveedorProducto');
    const proveedoresSeleccionados = Array.from(selectProveedores.selectedOptions).map(option => parseInt(option.value));

    // Obtener las categorías seleccionadas del multi-select
    const selectCategorias = document.getElementById('categoriaProducto');
    const categoriasSeleccionadas = Array.from(selectCategorias.selectedOptions).map(option => parseInt(option.value));

    const data = {
        Nombre: document.getElementById('nombreProducto')?.value,
        Descripcion: document.getElementById('descripcionProducto')?.value,
        SKU: parseInt(document.getElementById('skuProducto')?.value),
        PrecioVenta: parseFloat(document.getElementById('precioProducto')?.value) || 0,
        PrecioCompra: parseFloat(document.getElementById('precioProducto')?.value) || 0, // Usar el mismo campo por ahora
        StockMin: parseInt(document.getElementById('stockMinProducto')?.value) || 0,
        StockMax: parseInt(document.getElementById('stockMaxProducto')?.value) || 0,
        IdMarca: parseInt(document.getElementById('marcaProducto')?.value),
        IdEstado: 1, // Hardcodeado: nuevo estado activo
        IdsProveedores: proveedoresSeleccionados,
        IdsCategorias: categoriasSeleccionadas
    };

    try {
        const response = await fetch('/Producto/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoProductoModal'));
            modal.hide();
            form.reset();
            showToast('Producto creado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear el producto', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar producto:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar producto
async function actualizarProducto() {
    const form = document.getElementById('editarProductoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const productoId = window.currentProductoId;
    if (!productoId) {
        showToast('Error: ID del producto no encontrado', 'danger');
        return;
    }
    
    // Obtener los proveedores seleccionados del multi-select
    const selectProveedores = document.getElementById('editProveedorProducto');
    const proveedoresSeleccionados = Array.from(selectProveedores.selectedOptions).map(option => parseInt(option.value));

    // Obtener las categorías seleccionadas del multi-select
    const selectCategorias = document.getElementById('editCategoriaProducto');
    const categoriasSeleccionadas = Array.from(selectCategorias.selectedOptions).map(option => parseInt(option.value));

    const data = {
        IdProducto: productoId,
        Nombre: document.getElementById('editNombreProducto')?.value,
        Descripcion: document.getElementById('editDescripcionProducto')?.value,
        SKU: parseInt(document.getElementById('editSkuProducto')?.value) || 0,
        PrecioVenta: parseFloat(document.getElementById('editPrecioProducto')?.value) || 0,
        PrecioCompra: parseFloat(document.getElementById('editPrecioProducto')?.value) || 0, // Usar el mismo campo por ahora
        StockMin: parseInt(document.getElementById('editStockMinProducto')?.value) || 0,
        StockMax: parseInt(document.getElementById('editStockMaxProducto')?.value) || 0,
        IdMarca: parseInt(document.getElementById('editMarcaProducto')?.value),
        IdEstado: parseInt(document.getElementById('editEstadoProducto')?.value),
        IdsProveedores: proveedoresSeleccionados,
        IdsCategorias: categoriasSeleccionadas
    };

    try {
        const response = await fetch('/Producto/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarProductoModal'));
            modal.hide();
            showToast('Producto actualizado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar el producto', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar producto:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar producto
async function eliminarProducto() {
    const productoId = window.currentProductoId;
    if (!productoId) {
        showToast('Error: ID del producto no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Producto/Eliminar?id=${productoId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarProductoModal'));
            modal.hide();
            showToast('Producto eliminado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar el producto', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar producto:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar producto para visualizar/editar
async function cargarProducto(id, modo = 'visualizar') {
    window.currentProductoId = id;
    
    try {
        const response = await fetch(`/Producto/ObtenerPorId?id=${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) {
            throw new Error('Error al obtener producto');
        }
        
        const result = await response.json();
        
        if (result.success) {
            await mostrarDatosProducto(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar el producto', 'danger');
        }
        
    } catch (error) {
        console.error('Error al cargar producto:', error);
        showToast('Error al cargar los datos del producto', 'danger');
    }
}

// Función auxiliar para mostrar datos de producto en modales
async function mostrarDatosProducto(producto, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreProducto');
        const visualizarSku = document.getElementById('visualizarSkuProducto');
        const visualizarDescripcion = document.getElementById('visualizarDescripcionProducto');
        const visualizarPrecioVenta = document.getElementById('visualizarPrecioProducto');
        const visualizarMarca = document.getElementById('visualizarMarcaProducto');
        const visualizarEstado = document.getElementById('visualizarEstadoProducto');
        const visualizarStock = document.getElementById('visualizarStockProducto');
        const visualizarProveedores = document.getElementById('visualizarProveedoresProducto');
        const visualizarCategoria = document.getElementById('visualizarCategoriaProducto');
        const visualizarFecha = document.getElementById('visualizarFechaProducto');
        
        if (visualizarNombre) visualizarNombre.textContent = producto.Nombre || producto.nombre || '';
        if (visualizarSku) visualizarSku.textContent = producto.sku || '';
        if (visualizarDescripcion) visualizarDescripcion.textContent = producto.descripcion || '';
        if (visualizarPrecioVenta) visualizarPrecioVenta.textContent = `$${producto.precioVenta || 0}`;
        if (visualizarMarca) visualizarMarca.textContent = producto.marca?.nombre || '';
        if (visualizarEstado) visualizarEstado.textContent = producto.estado?.nombre || 'Sin Estado';
        if (visualizarStock) visualizarStock.textContent = `${producto.stockActual || 0} unidades`;
        if (visualizarFecha) {
            const fechaTexto = formatearFecha(producto.FechaRegistro || producto.fechaRegistro) || 'Sin fecha';
            visualizarFecha.textContent = fechaTexto;
        }
        
        // Mostrar proveedores relacionados como badges
        if (visualizarProveedores) {
            const proveedores = producto.Proveedores || producto.proveedores || [];
            if (proveedores.length > 0) {
                visualizarProveedores.innerHTML = proveedores.map(p => {
                    const nombreProveedor = p.Nombre || p.nombre || p.name || 'Proveedor sin nombre';
                    return `<span class="badge bg-primary me-1">${nombreProveedor}</span>`;
                }).join('');
            } else {
                visualizarProveedores.innerHTML = '<span class="text-muted">Sin proveedores asignados</span>';
            }
        }

        // Mostrar categorías relacionadas como badges
        if (visualizarCategoria) {
            const categorias = producto.Categorias || producto.categorias || [];
            if (categorias.length > 0) {
                visualizarCategoria.innerHTML = categorias.map(c => {
                    const nombreCategoria = c.Nombre || c.nombre || c.name || 'Categoría sin nombre';
                    return `<span class="badge bg-success me-1">${nombreCategoria}</span>`;
                }).join('');
            } else {
                visualizarCategoria.innerHTML = '<span class="text-muted">Sin categorías asignadas</span>';
            }
        }
        } else if (modo === 'editar') {        
        // Cargar las opciones de selects antes de establecer valores
        await cargarEstadosEnSelect('editEstadoProducto');
        await cargarMarcasEnSelect('editMarcaProducto');
        await cargarCategoriasEnSelect('editCategoriaProducto');
        await cargarProveedoresEnSelect('editProveedorProducto');
        
        const editNombre = document.getElementById('editNombreProducto');
        const editSku = document.getElementById('editSkuProducto');
        const editDescripcion = document.getElementById('editDescripcionProducto');
        const editPrecioVenta = document.getElementById('editPrecioProducto');
        const editPrecioCompra = document.getElementById('editPrecioProducto'); // Usar el mismo campo por ahora
        const editStockMin = document.getElementById('editStockMinProducto');
        const editStockMax = document.getElementById('editStockMaxProducto');
        const editMarca = document.getElementById('editMarcaProducto');
        const editEstado = document.getElementById('editEstadoProducto');
        const editProveedores = document.getElementById('editProveedorProducto');
        const editCategorias = document.getElementById('editCategoriaProducto');

        if (editNombre) editNombre.value = producto.nombre || '';
        if (editSku) editSku.value = producto.sku || '';
        if (editDescripcion) editDescripcion.value = producto.descripcion || '';
        if (editPrecioVenta) editPrecioVenta.value = producto.precioVenta || 0;
        if (editPrecioCompra) editPrecioCompra.value = producto.precioCompra || 0;
        if (editStockMin) editStockMin.value = producto.stockMin || 0;
        if (editStockMax) editStockMax.value = producto.stockMax || 0;
        if (editMarca) editMarca.value = producto.idMarca || '';
        if (editEstado) editEstado.value = producto.idEstado || '';

        // Marcar proveedores seleccionados en el multi-select
        if (editProveedores && (producto.Proveedores || producto.proveedores)) {
            const proveedoresIds = (producto.Proveedores || producto.proveedores).map(p => 
                (p.IdProveedor || p.idProveedor || p.id).toString()
            );
            
            Array.from(editProveedores.options).forEach(option => {
                option.selected = proveedoresIds.includes(option.value);
            });
        }

        // Marcar categorías seleccionadas en el multi-select
        if (editCategorias && (producto.Categorias || producto.categorias)) {
            const categoriasIds = (producto.Categorias || producto.categorias).map(c => 
                (c.IdCategoria || c.idCategoria || c.id).toString()
            );
            
            Array.from(editCategorias.options).forEach(option => {
                option.selected = categoriasIds.includes(option.value);
            });
        }
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreProducto');
        const eliminarDescripcion = document.getElementById('eliminarDescripcionProducto');
        const eliminarMarca = document.getElementById('eliminarMarcaProducto');
        const eliminarPrecio = document.getElementById('eliminarPrecioProducto');
        
        if (eliminarNombre) eliminarNombre.textContent = producto.nombre || '';
        if (eliminarDescripcion) eliminarDescripcion.textContent = producto.descripcion || '';
        if (eliminarMarca) eliminarMarca.textContent = producto.marca?.nombre || '';
        if (eliminarPrecio) eliminarPrecio.textContent = (producto.precioVenta || 0).toString();
    }
}

// =======================================================
// FUNCIONES PARA PROVEEDORES
// =======================================================

// Crear nuevo proveedor
async function guardarProveedor() {
    const form = document.getElementById('nuevoProveedorForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        Nombre: document.getElementById('nombreProveedor')?.value,
        Contacto: document.getElementById('contactoProveedor')?.value,
        Direccion: document.getElementById('direccionProveedor')?.value,
        IdEstado: 1 // Hardcodeado: nuevo estado activo
    };

    try {
        const response = await fetch('/Proveedor/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoProveedorModal'));
            modal.hide();
            form.reset();
            showToast('Proveedor creado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear el proveedor', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar proveedor:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar proveedor
async function actualizarProveedor() {
    const form = document.getElementById('editarProveedorForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const proveedorId = window.currentProveedorId;
    if (!proveedorId) {
        showToast('Error: ID del proveedor no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdProveedor: proveedorId,
        Nombre: document.getElementById('editNombreProveedor')?.value,
        Contacto: document.getElementById('editContactoProveedor')?.value,
        Direccion: document.getElementById('editDireccionProveedor')?.value,
        IdEstado: parseInt(document.getElementById('editEstadoProveedor')?.value)
    };

    try {
        const response = await fetch('/Proveedor/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarProveedorModal'));
            modal.hide();
            showToast('Proveedor actualizado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar el proveedor', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar proveedor:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar proveedor
async function eliminarProveedor() {
    const proveedorId = window.currentProveedorId;
    if (!proveedorId) {
        showToast('Error: ID del proveedor no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Proveedor/Eliminar?id=${proveedorId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarProveedorModal'));
            modal.hide();
            showToast('Proveedor eliminado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar el proveedor', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar proveedor:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar proveedor para visualizar/editar
async function cargarProveedor(id, modo = 'visualizar') {
    window.currentProveedorId = id;
    
    try {
        const response = await fetch(`/Proveedor/ObtenerPorId?id=${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) {
            throw new Error('Error al obtener proveedor');
        }
        
        const result = await response.json();
        
        if (result.success) {
            mostrarDatosProveedor(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar el proveedor', 'danger');
        }
        
    } catch (error) {
        console.error('Error al cargar proveedor:', error);
        showToast('Error al cargar los datos del proveedor', 'danger');
    }
}

// Función auxiliar para mostrar datos de proveedor en modales
function mostrarDatosProveedor(proveedor, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreProveedor');
        const visualizarContacto = document.getElementById('visualizarContactoProveedor');
        const visualizarDireccion = document.getElementById('visualizarDireccionProveedor');
        const visualizarFecha = document.getElementById('visualizarFechaProveedor');
        const visualizarEstado = document.getElementById('visualizarEstadoProveedor');
        const visualizarProductos = document.getElementById('visualizarProductosProveedor');

        if (visualizarNombre) visualizarNombre.textContent = proveedor.nombre || '';
        if (visualizarContacto) visualizarContacto.textContent = proveedor.contacto || '';
        if (visualizarDireccion) visualizarDireccion.textContent = proveedor.direccion || '';
        if (visualizarFecha) visualizarFecha.textContent = 'No disponible';
		if (visualizarEstado) visualizarEstado.textContent = proveedor.estado?.nombre || '';
        
        // Mostrar productos relacionados como badges
        if (visualizarProductos) {
            const productos = proveedor.Productos || proveedor.productos || [];
            if (productos.length > 0) {
                visualizarProductos.innerHTML = productos.map(p => 
                    `<span class="badge bg-success me-1">${p.Nombre || p.nombre}</span>`
                ).join('');
            } else {
                visualizarProductos.innerHTML = '<span class="text-muted">Sin productos asignados</span>';
            }
        }
        
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreProveedor');
        const editContacto = document.getElementById('editContactoProveedor');
        const editDireccion = document.getElementById('editDireccionProveedor');
        
        if (editNombre) editNombre.value = proveedor.nombre || '';
        if (editContacto) editContacto.value = proveedor.contacto || '';
        if (editDireccion) editDireccion.value = proveedor.direccion || '';

        // Cargar estados en el select y establecer el valor actual
        if (proveedor.idEstado) {
            setTimeout(() => {
                document.getElementById('editEstadoProveedor').value = proveedor.idEstado;
            }, 100);
        }
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreProveedor');
        const eliminarContacto = document.getElementById('eliminarContactoProveedor');

        if (eliminarNombre) eliminarNombre.textContent = proveedor.nombre;
        if (eliminarContacto) eliminarContacto.textContent = proveedor.contacto;
    }
}

// =======================================================
// FUNCIONES PARA BODEGAS
// =======================================================

// Crear nueva bodega
async function guardarBodega() {
    const form = document.getElementById('nuevaBodegaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        Nombre: document.getElementById('nombreBodega')?.value,
        Descripcion: document.getElementById('descripcionBodega')?.value,
        Ubicacion: document.getElementById('ubicacionBodega')?.value,
        IdEstado: 1 // Hardcodeado: nuevo estado activo
    };

    try {
        const response = await fetch('/Bodega/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaBodegaModal'));
            modal.hide();
            form.reset();
            showToast('Bodega creada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear la bodega', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar bodega:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar bodega
async function actualizarBodega() {
    const form = document.getElementById('editarBodegaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const bodegaId = window.currentBodegaId;
    if (!bodegaId) {
        showToast('Error: ID de la bodega no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdBodega: bodegaId,
        Nombre: document.getElementById('editNombreBodega')?.value,
        Descripcion: document.getElementById('editDescripcionBodega')?.value,
        Ubicacion: document.getElementById('editUbicacionBodega')?.value,
        IdEstado: parseInt(document.getElementById('editEstadoBodega')?.value)
    };

    try {
        const response = await fetch('/Bodega/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarBodegaModal'));
            modal.hide();
            showToast('Bodega actualizada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar la bodega', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar bodega:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar bodega
async function eliminarBodega() {
    const bodegaId = window.currentBodegaId;
    if (!bodegaId) {
        showToast('Error: ID de la bodega no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Bodega/Eliminar?id=${bodegaId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarBodegaModal'));
            modal.hide();
            showToast('Bodega eliminada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar la bodega', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar bodega:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar bodega para visualizar/editar
async function cargarBodega(id, modo = 'visualizar') {
    window.currentBodegaId = id;
    
    try {
        const response = await fetch(`/Bodega/ObtenerPorId?id=${id}`);
        const result = await response.json();
        
        if (result.success) {
            mostrarDatosBodega(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar la bodega', 'danger');
        }
    } catch (error) {
        console.error('Error al cargar bodega:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Función auxiliar para mostrar datos de bodega en modales
function mostrarDatosBodega(bodega, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreBodega');
        const visualizarDescripcion = document.getElementById('visualizarDescripcionBodega');
        const visualizarUbicacion = document.getElementById('visualizarUbicacionBodega');
        const visualizarProductos = document.getElementById('visualizarProductosBodega');
        const visualizarFecha = document.getElementById('visualizarFechaBodega');
		const visualizarEstado = document.getElementById('visualizarEstadoBodega');

        if (visualizarNombre) visualizarNombre.textContent = bodega.nombre;
        if (visualizarDescripcion) visualizarDescripcion.textContent = bodega.descripcion;
        if (visualizarUbicacion) visualizarUbicacion.textContent = bodega.ubicacion;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(bodega.fechaRegistro) || 'Sin Fecha';
		if (visualizarEstado) visualizarEstado.textContent = bodega.estado?.nombre || 'Sin Estado';
        
        // Mostrar productos con stock
        if (visualizarProductos) {
            const productos = bodega.bodegaProductos || [];
            
            if (productos && productos.length > 0) {
                const totalProductos = productos.length;
                const stockTotal = productos.reduce((sum, p) => {
                    const stock = p.stockTotal || 0;
                    return sum + stock;
                }, 0);
                visualizarProductos.textContent = `${totalProductos} productos (${stockTotal} unidades)`;
            } else {
                if (bodega.TotalProductos > 0) {
                    visualizarProductos.textContent = `${bodega.TotalProductos} productos (${bodega.TotalStock} unidades)`;
                } else {
                    visualizarProductos.textContent = "0 productos";
                }
            }
        }
        
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreBodega');
        const editDescripcion = document.getElementById('editDescripcionBodega');
        const editUbicacion = document.getElementById('editUbicacionBodega');
        
        if (editNombre) editNombre.value = bodega.Nombre || bodega.nombre;
        if (editDescripcion) editDescripcion.value = bodega.Descripcion || bodega.descripcion;
        if (editUbicacion) editUbicacion.value = bodega.Ubicacion || bodega.ubicacion;
        
        // Cargar estados en el select y establecer el valor actual
        if (bodega.estado.idEstado) {
            setTimeout(() => {
                document.getElementById('editEstadoBodega').value = bodega.estado.idEstado;
            }, 100);
        }
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreBodega');
        const eliminarUbicacion = document.getElementById('eliminarUbicacionBodega');

        if (eliminarNombre) eliminarNombre.textContent = bodega.nombre;
        if (eliminarUbicacion) eliminarUbicacion.textContent = bodega.ubicacion;
    }
}

// =======================================================
// FUNCIONES PARA MARCAS
// =======================================================

// Crear nueva marca
async function guardarMarca() {
    const form = document.getElementById('nuevaMarcaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        Nombre: document.getElementById('nombreMarca')?.value,
        Descripcion: document.getElementById('descripcionMarca')?.value,
        IdEstado: 1 // Hardcodeado: nuevo estado activo
    };

    try {
        const response = await fetch('/Marca/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaMarcaModal'));
            modal.hide();
            form.reset();
            showToast('Marca creada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear la marca', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar marca:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar marca
async function actualizarMarca() {
    const form = document.getElementById('editarMarcaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const marcaId = window.currentMarcaId;
    if (!marcaId) {
        showToast('Error: ID de la marca no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdMarca: marcaId,
        Nombre: document.getElementById('editNombreMarca')?.value,
        Descripcion: document.getElementById('editDescripcionMarca')?.value,
        IdEstado: parseInt(document.getElementById('editEstadoMarca')?.value)
    };

    try {
        const response = await fetch('/Marca/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarMarcaModal'));
            modal.hide();
            showToast('Marca actualizada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar la marca', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar marca:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar marca
async function eliminarMarca() {
    const marcaId = window.currentMarcaId;
    if (!marcaId) {
        showToast('Error: ID de la marca no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Marca/Eliminar?id=${marcaId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarMarcaModal'));
            modal.hide();
            showToast('Marca eliminada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar la marca', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar marca:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar marca para visualizar/editar
async function cargarMarca(id, modo = 'visualizar') {
    window.currentMarcaId = id;
    
    try {
        const response = await fetch(`/Marca/ObtenerPorId?id=${id}`);
        const result = await response.json();
        
        if (result.success) {
            mostrarDatosMarca(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar la marca', 'danger');
        }
    } catch (error) {
        console.error('Error al cargar marca:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Función auxiliar para mostrar datos de marca en modales
function mostrarDatosMarca(marca, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreMarca');
        const visualizarDescripcion = document.getElementById('visualizarDescripcionMarca');
        const visualizarProductos = document.getElementById('visualizarProductosMarca');
        const visualizarFecha = document.getElementById('visualizarFechaMarca');
		const visualizarEstado = document.getElementById('visualizarEstadoMarca');
        
        if (visualizarNombre) visualizarNombre.textContent = marca.nombre;
        if (visualizarDescripcion) visualizarDescripcion.textContent = marca.descripcion;
        if (visualizarProductos) {
            const productos = marca.productos || [];
            visualizarProductos.textContent = `${productos.length} productos`;
        }
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(marca.fechaRegistro) || 'Sin Fecha';
        if (visualizarEstado) visualizarEstado.textContent = marca.estado?.nombre || 'Sin Estado';
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreMarca');
        const editDescripcion = document.getElementById('editDescripcionMarca');

        if (editNombre) editNombre.value = marca.nombre;
        if (editDescripcion) editDescripcion.value = marca.descripcion;
        
        // Cargar estados en el select y establecer el valor actual
        if (marca.estado.idEstado) {
            setTimeout(() => {
                document.getElementById('editEstadoMarca').value = marca.estado.idEstado;
            }, 100);
        }
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreMarca');
        const eliminarDescripcion = document.getElementById('eliminarDescripcionMarca');

        if (eliminarNombre) eliminarNombre.textContent = marca.nombre;
        if (eliminarDescripcion) eliminarDescripcion.textContent = marca.descripcion;
    }
}

// =======================================================
// FUNCIONES PARA ESTADOS
// =======================================================

// Crear nuevo estado
async function guardarEstado() {
    const form = document.getElementById('nuevoEstadoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        Nombre: document.getElementById('nombreEstado')?.value,
        Funcion: document.getElementById('funcionEstado')?.value,
        Descripcion: document.getElementById('descripcionEstado')?.value
    };

    try {
        const response = await fetch('/Estado/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoEstadoModal'));
            modal.hide();
            form.reset();
            showToast('Estado creado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear el estado', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar estado:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar estado
async function actualizarEstado() {
    const form = document.getElementById('editarEstadoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const estadoId = window.currentEstadoId;
    if (!estadoId) {
        showToast('Error: ID del estado no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdEstado: estadoId,
        Nombre: document.getElementById('editNombreEstado')?.value,
        Funcion: document.getElementById('editFuncionEstado')?.value,
        Descripcion: document.getElementById('editDescripcionEstado')?.value
    };

    try {
        const response = await fetch('/Estado/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarEstadoModal'));
            modal.hide();
            showToast('Estado actualizado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar el estado', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar estado:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar estado
async function eliminarEstado() {
    const estadoId = window.currentEstadoId;
    if (!estadoId) {
        showToast('Error: ID del estado no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/Estado/Eliminar?id=${estadoId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarEstadoModal'));
            modal.hide();
            showToast('Estado eliminado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar el estado', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar estado:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar estado para visualizar/editar
async function cargarEstado(id, modo = 'visualizar') {
    window.currentEstadoId = id;
    
    try {
        const response = await fetch(`/Estado/ObtenerPorId?id=${id}`);
        const result = await response.json();
        
        if (result.success) {
            const estado = result.data;
            mostrarDatosEstado(estado, modo);
        } else {
            showToast(result.message || 'Error al cargar el estado', 'danger');
        }
    } catch (error) {
        console.error('Error al cargar estado:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Función auxiliar para mostrar datos de estado en modales
function mostrarDatosEstado(estado, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreEstado');
        const visualizarFuncion = document.getElementById('visualizarFuncionEstado');
        const visualizarDescripcion = document.getElementById('visualizarDescripcionEstado');
        const visualizarElementos = document.getElementById('visualizarElementosEstado');
        const visualizarFecha = document.getElementById('visualizarFechaEstado');

        if (visualizarNombre) visualizarNombre.textContent = estado.nombre;
        if (visualizarFuncion) visualizarFuncion.textContent = estado.funcion;
        if (visualizarDescripcion) visualizarDescripcion.textContent = estado.descripcion;
        if (visualizarElementos) visualizarElementos.textContent = `${estado.elementosCount || 0} elementos`;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(estado.fechaRegistro) || 'Sin Fecha';
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreEstado');
        const editFuncion = document.getElementById('editFuncionEstado');
        const editDescripcion = document.getElementById('editDescripcionEstado');

        if (editNombre) editNombre.value = estado.nombre;
        if (editFuncion) editFuncion.value = estado.funcion;
        if (editDescripcion) editDescripcion.value = estado.descripcion;
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreEstado');
        const eliminarFuncion = document.getElementById('eliminarFuncionEstado');

        if (eliminarNombre) eliminarNombre.textContent = estado.nombre;
        if (eliminarFuncion) eliminarFuncion.textContent = estado.funcion;
    }
}

// Funciones específicas para abrir modales de estado
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
    cargarEstado(id, 'eliminar');
    const modal = new bootstrap.Modal(document.getElementById('eliminarEstadoModal'));
    modal.show();
}

// =======================================================
// FUNCIONES PARA ENTRADAS
// =======================================================

// Crear nueva entrada
async function guardarEntrada() {
    const form = document.getElementById('nuevaEntradaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const data = {
        IdBodega: parseInt(document.getElementById('bodegaEntrada')?.value),
        IdProducto: parseInt(document.getElementById('productoEntrada')?.value),
        CantidadProducto: parseInt(document.getElementById('cantidadEntrada')?.value),
        Fecha: document.getElementById('fechaIngresoEntrada')?.value
    };

    try {
        const response = await fetch('/EntradaProducto/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaEntradaModal'));
            modal.hide();
            form.reset();
            showToast('Entrada creada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear la entrada', 'danger');
        }
    } catch (error) {
        console.error('Error al guardar entrada:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar entrada
async function actualizarEntrada() {
    const form = document.getElementById('editarEntradaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const entradaId = window.currentEntradaId;
    if (!entradaId) {
        showToast('Error: ID de la entrada no encontrado', 'danger');
        return;
    }
    
    const data = {
        IdEntrada: entradaId,
        IdBodega: parseInt(document.getElementById('editBodegaEntrada')?.value),
        IdProducto: parseInt(document.getElementById('editProductoEntrada')?.value),
        CantidadProducto: parseInt(document.getElementById('editCantidadEntrada')?.value),
        Fecha: document.getElementById('editFechaIngresoEntrada')?.value
    };

    try {
        const response = await fetch('/EntradaProducto/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarEntradaModal'));
            modal.hide();
            showToast('Entrada actualizada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar la entrada', 'danger');
        }
    } catch (error) {
        console.error('Error al actualizar entrada:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar entrada
async function eliminarEntrada() {
    const entradaId = window.currentEntradaId;
    if (!entradaId) {
        showToast('Error: ID de la entrada no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/EntradaProducto/Eliminar?id=${entradaId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarEntradaModal'));
            modal.hide();
            showToast('Entrada eliminada exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar la entrada', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar entrada:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar entrada para visualizar/editar
async function cargarEntrada(id, modo = 'visualizar') {
    window.currentEntradaId = id;
    
    try {
        // Si es modo editar, cargar primero los selects
        if (modo === 'editar') {
            await Promise.all([
                cargarProductosParaEntrada(),
                cargarBodegasParaEntrada()
            ]);
        }
        
        const response = await fetch(`/EntradaProducto/ObtenerPorId?id=${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) {
            throw new Error('Error al obtener entrada');
        }
        
        const result = await response.json();
        
        if (result.success) {
            mostrarDatosEntrada(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar la entrada', 'danger');
        }
        
    } catch (error) {
        console.error('Error al cargar entrada:', error);
        showToast('Error al cargar los datos de la entrada', 'danger');
    }
}

// Función auxiliar para cargar productos en el select de edición
async function cargarProductosParaEntrada() {
    try {
        const response = await fetch('/Producto/ObtenerParaSelect');
        const result = await response.json();
        
        if (result.success && result.data) {
            const select = document.getElementById('editProductoEntrada');
            if (select) {
                select.innerHTML = '<option value="">Seleccione un producto</option>';
                result.data.forEach(producto => {
                    const option = document.createElement('option');
                    option.value = producto.id;
                    option.textContent = producto.nombre;
                    select.appendChild(option);
                });
            }
        }
    } catch (error) {
        console.error('Error al cargar productos:', error);
    }
}

// Función auxiliar para cargar bodegas en el select de edición
async function cargarBodegasParaEntrada() {
    try {
        const response = await fetch('/Bodega/ObtenerParaSelect');
        const result = await response.json();
        
        if (result.success && result.data) {
            const select = document.getElementById('editBodegaEntrada');
            if (select) {
                select.innerHTML = '<option value="">Seleccione una bodega</option>';
                result.data.forEach(bodega => {
                    const option = document.createElement('option');
                    option.value = bodega.id;
                    option.textContent = bodega.nombre;
                    select.appendChild(option);
                });
            }
        }
    } catch (error) {
        console.error('Error al cargar bodegas:', error);
    }
}

// Función auxiliar para mostrar datos de entrada en modales
function mostrarDatosEntrada(entrada, modo) {
    if (modo === 'visualizar') {
        const visualizarProducto = document.getElementById('visualizarProductoEntrada');
        const visualizarBodega = document.getElementById('visualizarBodegaEntrada');
        const visualizarCantidad = document.getElementById('visualizarCantidadEntrada');
        const visualizarFecha = document.getElementById('visualizarFechaIngresoEntrada');
        const visualizarUsuario = document.getElementById('visualizarUsuarioEntrada');
        const visualizarEstado = document.getElementById('visualizarEstadoEntrada');
        const visualizarHora = document.getElementById('visualizarHoraEntrada');
        
        if (visualizarProducto) visualizarProducto.textContent = entrada.producto?.nombre || '';
        if (visualizarBodega) visualizarBodega.textContent = entrada.bodega?.nombre || '';
        if (visualizarCantidad) visualizarCantidad.textContent = `${entrada.cantidadProducto || 0} unidades`;
        if (visualizarFecha) {
            const fecha = new Date(entrada.fecha);
            if (!isNaN(fecha.getTime())) {
                visualizarFecha.textContent = fecha.toLocaleDateString('es-ES', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit'
                });
            } else {
                visualizarFecha.textContent = 'Fecha no disponible';
            }
        }
        if (visualizarUsuario) visualizarUsuario.textContent = entrada.usuario?.userName || '';
        if (visualizarEstado) visualizarEstado.textContent = entrada.producto?.estado?.nombre || '';
        if (visualizarHora) {
            const fecha = new Date(entrada.fecha);
            if (!isNaN(fecha.getTime())) {
                visualizarHora.textContent = fecha.toLocaleTimeString('es-ES', { 
                    hour: '2-digit', 
                    minute: '2-digit', 
                    second: '2-digit' 
                });
            } else {
                visualizarHora.textContent = 'Hora no disponible';
            }
        }
        
    } else if (modo === 'editar') {
        const editProducto = document.getElementById('editProductoEntrada');
        const editBodega = document.getElementById('editBodegaEntrada');
        const editCantidad = document.getElementById('editCantidadEntrada');
        const editFecha = document.getElementById('editFechaIngresoEntrada');
        
        setTimeout(() => {
            if (editProducto) editProducto.value = entrada.idProducto || '';
            if (editBodega) editBodega.value = entrada.idBodega || '';
        }, 100);

        if (editCantidad) editCantidad.value = entrada.cantidadProducto || 0;
        if (editFecha) {
            // Formatear la fecha para el input date (YYYY-MM-DD)
            let fechaFormateada = '';
            if (entrada.fecha) {
                const fecha = new Date(entrada.fecha);
                if (!isNaN(fecha.getTime())) {
                    fechaFormateada = fecha.toISOString().split('T')[0];
                }
            }
            editFecha.value = fechaFormateada;
        }
        
    } else if (modo === 'eliminar') {
        const eliminarProducto = document.getElementById('eliminarProductoEntrada');
        const eliminarBodega = document.getElementById('eliminarBodegaEntrada');
        const eliminarCantidad = document.getElementById('eliminarCantidadEntrada');

        if (eliminarProducto) eliminarProducto.textContent = entrada.producto?.nombre || '';
        if (eliminarBodega) eliminarBodega.textContent = entrada.bodega?.nombre || '';
        if (eliminarCantidad) eliminarCantidad.textContent = `${entrada.cantidadProducto || 0} unidades`;
    }
}

// =======================================================
// FUNCIONES PARA AJUSTES
// =======================================================

// Crear nuevo ajuste
async function guardarAjuste() {
    const form = document.getElementById('nuevoAjusteForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    // Recopilar productos de la tabla
    const productos = [];
    const filas = document.querySelectorAll('#tablaProductosAjuste tr');
    
    filas.forEach(fila => {
        const celdas = fila.querySelectorAll('td');
        // Buscar tanto badges como elementos p para mayor compatibilidad
        const cantidadElement = fila.querySelector('span.badge[data-field="cantidad"]') || fila.querySelector('p[data-field="cantidad"]');
        if (celdas.length > 1 && cantidadElement && fila.dataset.productoId) {
            productos.push({
                IdProducto: parseInt(fila.dataset.productoId),
                CantidadProducto: parseInt(cantidadElement.textContent) || 0,
                Motivo: "Ajuste de inventario"
            });
        }
    });

    // Formatear la fecha como DateTime ISO string
    const fechaInput = document.getElementById('fechaAjuste')?.value;
    let fechaISO = '';
    if (fechaInput) {
        // Crear fecha a las 12:00 PM para evitar problemas de zona horaria
        const fecha = new Date(fechaInput + 'T12:00:00');
        fechaISO = fecha.toISOString();
    }

    const data = {
        IdBodega: parseInt(document.getElementById('bodegaAjuste')?.value),
        Fecha: fechaISO,
        Productos: productos
    };

    try {
        const response = await fetch('/AjusteInventario/Crear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoAjusteModal'));
            modal.hide();
            form.reset();
            showToast('Ajuste creado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al crear el ajuste', 'danger');
            if (result.errors) {
                console.error('Errores de validación:', result.errors);
            }
        }
    } catch (error) {
        console.error('Error al guardar ajuste:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Actualizar ajuste
async function actualizarAjuste() {
    const form = document.getElementById('editarAjusteForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const ajusteId = window.currentAjusteId;
    if (!ajusteId) {
        showToast('Error: ID del ajuste no encontrado', 'danger');
        return;
    }
    
    // Recopilar productos de la tabla de edición
    const productos = [];
    const filas = document.querySelectorAll('#editTablaProductosAjuste tr');
    
    filas.forEach(fila => {
        const celdas = fila.querySelectorAll('td');
        // Buscar tanto badges como elementos p para mayor compatibilidad
        const cantidadElement = fila.querySelector('span.badge[data-field="cantidad"]') || fila.querySelector('p[data-field="cantidad"]');
        if (celdas.length > 1 && cantidadElement && fila.dataset.productoId) {
            productos.push({
                IdProducto: parseInt(fila.dataset.productoId),
                CantidadProducto: parseInt(cantidadElement.textContent) || 0,
                Motivo: "Ajuste de inventario"
            });
        }
    });

    // Formatear la fecha como DateTime ISO string
    const fechaInput = document.getElementById('editFechaAjuste')?.value;
    let fechaISO = '';
    if (fechaInput) {
        // Crear fecha a las 12:00 PM para evitar problemas de zona horaria
        const fecha = new Date(fechaInput + 'T12:00:00');
        fechaISO = fecha.toISOString();
    }

    const data = {
        IdAjusteInventario: ajusteId,
        IdBodega: parseInt(document.getElementById('editBodegaAjuste')?.value),
        Fecha: fechaISO,
        Productos: productos
    };

    try {
        const response = await fetch('/AjusteInventario/Editar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarAjusteModal'));
            modal.hide();
            showToast('Ajuste actualizado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al actualizar el ajuste', 'danger');
            if (result.errors) {
                console.error('Errores de validación:', result.errors);
            }
        }
    } catch (error) {
        console.error('Error al actualizar ajuste:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Eliminar ajuste
async function eliminarAjuste() {
    const ajusteId = window.currentAjusteId;
    if (!ajusteId) {
        showToast('Error: ID del ajuste no encontrado', 'danger');
        return;
    }

    try {
        const response = await fetch(`/AjusteInventario/Eliminar?id=${ajusteId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        const result = await response.json();
        
        if (result.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarAjusteModal'));
            modal.hide();
            showToast('Ajuste eliminado exitosamente');
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        } else {
            showToast(result.message || 'Error al eliminar el ajuste', 'danger');
        }
    } catch (error) {
        console.error('Error al eliminar ajuste:', error);
        showToast('Error de conexión al servidor', 'danger');
    }
}

// Cargar ajuste para visualizar/editar
async function cargarAjuste(id, modo = 'visualizar') {
    window.currentAjusteId = id;
    
    try {
        // Si es modo editar, cargar primero los selects
        if (modo === 'editar') {
            await cargarBodegasParaAjuste();
        }
        
        const response = await fetch(`/AjusteInventario/ObtenerPorId?id=${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        
        if (!response.ok) {
            throw new Error('Error al obtener ajuste');
        }
        
        const result = await response.json();
        
        if (result.success) {
            console.log('Datos del ajuste recibidos:', result.data); // Debug
            mostrarDatosAjuste(result.data, modo);
        } else {
            showToast(result.message || 'Error al cargar el ajuste', 'danger');
        }
        
    } catch (error) {
        console.error('Error al cargar ajuste:', error);
        showToast('Error al cargar los datos del ajuste', 'danger');
    }
}

// Función auxiliar para cargar bodegas en el select de edición de ajuste
async function cargarBodegasParaAjuste() {
    try {
        const response = await fetch('/Bodega/ObtenerParaSelect');
        const result = await response.json();
        
        if (result.success && result.data) {
            const select = document.getElementById('editBodegaAjuste');
            if (select) {
                select.innerHTML = '<option value="">Seleccione una bodega</option>';
                result.data.forEach(bodega => {
                    const option = document.createElement('option');
                    option.value = bodega.id;
                    option.textContent = bodega.nombre;
                    select.appendChild(option);
                });
            }
        }
    } catch (error) {
        console.error('Error al cargar bodegas:', error);
    }
}

// Función auxiliar para mostrar datos de ajuste en modales
function mostrarDatosAjuste(ajuste, modo) {
    if (modo === 'visualizar') {
        const visualizarFecha = document.getElementById('visualizarFechaAjuste');
        const visualizarBodega = document.getElementById('visualizarBodegaAjuste');
        const visualizarTotalProductos = document.getElementById('visualizarTotalProductosAjuste');
        const visualizarUsuario = document.getElementById('visualizarUsuarioAjuste');
        
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(ajuste.fecha);
        if (visualizarBodega) visualizarBodega.textContent = ajuste.bodega?.nombre || '';
        if (visualizarTotalProductos) visualizarTotalProductos.textContent = `${ajuste.ajusteInventarioProductos?.length || 0} productos`;
        if (visualizarUsuario) visualizarUsuario.textContent = ajuste.usuario?.userName || '';
        
        // Cargar productos en la tabla de visualización
        const tabla = document.getElementById('visualizarTablaProductosAjuste');
        if (tabla && ajuste.ajusteInventarioProductos) {
            tabla.innerHTML = '';
            const productos = ajuste.ajusteInventarioProductos || [];
            productos.forEach(producto => {
                const fila = document.createElement('tr');
                const diferencia = producto.cantidadProducto || 0;
                const tipoAjuste = diferencia > 0 ? 'Aumento' : diferencia < 0 ? 'Disminución' : 'Sin cambio';
                const badgeClass = diferencia > 0 ? 'bg-success' : diferencia < 0 ? 'bg-danger' : 'bg-secondary';
                const tipoBadgeClass = diferencia > 0 ? 'bg-success' : diferencia < 0 ? 'bg-warning' : 'bg-secondary';
                
                fila.innerHTML = `
                    <td>${producto.producto?.nombre || ''}</td>
                    <td>
                        <span class="badge ${badgeClass}">
                            ${diferencia > 0 ? '+' : ''}${diferencia}
                        </span>
                    </td>
                    <td>
                        <span class="badge ${tipoBadgeClass}">${tipoAjuste}</span>
                    </td>
                `;
                tabla.appendChild(fila);
            });
        }
    } else if (modo === 'editar') {
        const editBodega = document.getElementById('editBodegaAjuste');
        const editFecha = document.getElementById('editFechaAjuste');
        
        // Formatear la fecha para el input date (YYYY-MM-DD)
        if (editFecha) {
            let fechaFormateada = '';
            if (ajuste.fecha) {
                console.log('Fecha recibida:', ajuste.fecha); // Debug
                const fecha = new Date(ajuste.fecha);
                if (!isNaN(fecha.getTime())) {
                    fechaFormateada = fecha.toISOString().split('T')[0];
                    console.log('Fecha formateada:', fechaFormateada); // Debug
                }
            }
            editFecha.value = fechaFormateada;
        }
        
        // Establecer valores con delay para asegurar que los selects estén cargados
        setTimeout(() => {
            if (editBodega) editBodega.value = ajuste.idBodega || '';
            console.log('Bodega establecida:', ajuste.idBodega); // Debug
        }, 100);
        
        // Cargar productos en la tabla
        const tabla = document.getElementById('editTablaProductosAjuste');
        if (tabla && ajuste.ajusteInventarioProductos) {
            tabla.innerHTML = '';
            const productos = ajuste.ajusteInventarioProductos || [];
            
            productos.forEach(producto => {
                const fila = document.createElement('tr');
                fila.dataset.productoId = producto.idProducto;
                const cantidad = producto.cantidadProducto || 0;
                
                fila.innerHTML = `
                    <td><p data-field="nombre">${producto.producto?.nombre || ''}</p></td>
                    <td>
                        <span class="badge ${cantidad >= 0 ? 'bg-primary' : 'bg-warning'}" data-field="cantidad">
                            ${cantidad}
                        </span>
                    </td>
                    <td>
                        <button type="button" class="btn btn-danger btn-sm" onclick="editQuitarProductoAjuste(this)">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                `;
                tabla.appendChild(fila);
            });
        }
    } else if (modo === 'eliminar') {
        const eliminarBodega = document.getElementById('eliminarBodegaAjuste');
        const eliminarFecha = document.getElementById('eliminarFechaAjuste');
        const eliminarId = document.getElementById('eliminarIdAjuste');
        
        if (eliminarBodega) eliminarBodega.textContent = ajuste.bodega?.nombre || `Bodega ID: ${ajuste.idBodega}`;
        if (eliminarFecha) eliminarFecha.textContent = formatearFecha(ajuste.fecha);
        if (eliminarId) eliminarId.textContent = ajuste.idAjusteInventario;
    }
}

// =======================================================
// FUNCIONES AUXILIARES PARA AJUSTES
// =======================================================

// Función para agregar producto a la tabla de edición
function editAgregarProducto() {
    const selectProducto = document.getElementById('editSelectProducto');
    const inputCantidad = document.getElementById('editCantidadProducto');
    const tablaProductos = document.getElementById('editTablaProductos');
    
    if (!selectProducto.value || !inputCantidad.value || inputCantidad.value <= 0) {
        alert('Por favor seleccione un producto y especifique una cantidad válida');
        return;
    }
    
    // Verificar si el producto ya existe en la tabla
    const productosExistentes = tablaProductos.querySelectorAll('tr[data-producto-id="' + selectProducto.value + '"]');
    if (productosExistentes.length > 0) {
        alert('Este producto ya está en la lista');
        return;
    }
    
    // Obtener el nombre del producto seleccionado
    const nombreProducto = selectProducto.options[selectProducto.selectedIndex].text;
    
    // Crear nueva fila
    const nuevaFila = document.createElement('tr');
    nuevaFila.dataset.productoId = selectProducto.value;
    nuevaFila.innerHTML = `
        <td>${nombreProducto}</td>
        <td><span class="badge bg-primary">${inputCantidad.value}</span></td>
        <td>
            <button type="button" class="btn btn-danger btn-sm" onclick="editQuitarProducto(this)">
                <i class="fas fa-trash"></i>
            </button>
        </td>
    `;
    
    // Agregar la fila a la tabla
    tablaProductos.appendChild(nuevaFila);
    
    // Limpiar campos
    selectProducto.value = '';
    inputCantidad.value = '';
}

// Función para quitar producto de la tabla de edición
function editQuitarProducto(boton) {
    const fila = boton.closest('tr');
    if (fila) {
        fila.remove();
    }
}

// Función específica para quitar productos en la edición de ajustes
function editQuitarProductoAjuste(boton) {
    const fila = boton.closest('tr');
    const tbody = fila.parentNode;
    
    if (fila) {
        fila.remove();
    }
    
    // Si no quedan productos, mostrar mensaje
    if (tbody.children.length === 0) {
        tbody.innerHTML = '<tr class="text-center"><td colspan="3" class="text-white">No hay productos agregados</td></tr>';
    }
}

// =======================================================
// FUNCIONES DE INICIALIZACIÓN Y EVENTOS
// =======================================================

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Configurar validaciones en tiempo real para todos los formularios
    const formularios = document.querySelectorAll('form');
    formularios.forEach(form => {
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                if (this.checkValidity()) {
                    this.classList.remove('is-invalid');
                    this.classList.add('is-valid');
                } else {
                    this.classList.remove('is-valid');
                    this.classList.add('is-invalid');
                }
            });
        });
    });

    // Limpiar formularios cuando se abren los modales de creación
    const modalesCreacion = [
        'nuevaCategoriaModal', 'nuevoProductoModal', 'nuevoProveedorModal',
        'nuevaBodegaModal', 'nuevaMarcaModal', 'nuevoEstadoModal',
        'nuevaEntradaModal', 'nuevoAjusteModal'
    ];

    modalesCreacion.forEach(modalId => {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.addEventListener('show.bs.modal', function() {
                const form = this.querySelector('form');
                if (form) {
                    form.reset();
                    // Limpiar clases de validación
                    const inputs = form.querySelectorAll('input, textarea, select');
                    inputs.forEach(input => {
                        input.classList.remove('is-valid', 'is-invalid');
                    });
                }
            });
        }
    });
});

// Validación en tiempo real
document.addEventListener('DOMContentLoaded', function() {
	const forms = document.querySelectorAll('form');
	forms.forEach(form => {
		// Prevenir envío automático de formularios
		form.addEventListener('submit', function(e) {
			e.preventDefault();
		});
		
		form.addEventListener('input', function(e) {
			if (e.target.checkValidity()) {
				e.target.classList.remove('is-invalid');
				e.target.classList.add('is-valid');
			} else {
				e.target.classList.remove('is-valid');
				e.target.classList.add('is-invalid');
			}
		});
	});
});

// Función auxiliar para formatear fecha para inputs date
function formatearFechaInput(fecha) {
    if (!fecha) return '';
    
    const date = new Date(fecha);
    if (isNaN(date.getTime())) return '';
    
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
}