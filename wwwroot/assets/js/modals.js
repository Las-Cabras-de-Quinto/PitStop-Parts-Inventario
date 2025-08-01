// Función para cargar datos de demostración hasta que se implemente el backend
function cargarDatosDemo() {
    return {
        categorias: {
            1: { id: 1, nombre: 'Electrónica', descripcion: 'Productos electrónicos y tecnológicos de última generación', estado: 'Activa', estado_id: 1, productos_count: 47, fecha_registro: '2024-03-15' },
            2: { id: 2, nombre: 'Ropa', descripcion: 'Vestimenta y accesorios de moda', estado: 'Activa', estado_id: 1, productos_count: 23, fecha_registro: '2024-03-20' }
        },
        productos: {
            1: { 
                id: 1, 
                nombre: 'iPhone 14 Pro', 
                sku: 'IPH14PRO128', 
                descripcion: 'Smartphone Apple con pantalla ProMotion de 6.1 pulgadas, chip A16 Bionic, cámara principal de 48MP y almacenamiento de 128GB', 
                precio: 999.99, 
                categoria_id: 1, 
                categoria: 'Electrónica', 
                marca_id: 1, 
                marca: 'Apple', 
                estado: 'Activo', 
                estado_id: 1, 
                stock: 25, 
                proveedores: [1, 2], 
                fecha_registro: '2024-03-15' 
            },
            2: { 
                id: 2, 
                nombre: 'Samsung Galaxy S23', 
                sku: 'SGS23-256', 
                descripcion: 'Smartphone Samsung con pantalla Dynamic AMOLED 2X de 6.1 pulgadas, procesador Snapdragon 8 Gen 2, cámara de 50MP y 256GB de almacenamiento', 
                precio: 899.99, 
                categoria_id: 1, 
                categoria: 'Electrónica', 
                marca_id: 2, 
                marca: 'Samsung', 
                estado: 'Activo', 
                estado_id: 1, 
                stock: 15, 
                proveedores: [2, 3], 
                fecha_registro: '2024-03-16' 
            },
            3: { 
                id: 3, 
                nombre: 'MacBook Air M2', 
                sku: 'MBA-M2-256', 
                descripcion: 'Laptop Apple con chip M2, pantalla Liquid Retina de 13.6 pulgadas, 8GB de memoria unificada y SSD de 256GB', 
                precio: 1199.99, 
                categoria_id: 1, 
                categoria: 'Electrónica', 
                marca_id: 1, 
                marca: 'Apple', 
                estado: 'Activo', 
                estado_id: 1, 
                stock: 8, 
                proveedores: [1, 4], 
                fecha_registro: '2024-03-17' 
            },
            4: { 
                id: 4, 
                nombre: 'iPad Pro 11"', 
                sku: 'IPADPRO11-128', 
                descripcion: 'Tablet Apple con pantalla Liquid Retina de 11 pulgadas, chip M2, cámara posterior de 12MP y almacenamiento de 128GB', 
                precio: 799.99, 
                categoria_id: 1, 
                categoria: 'Electrónica', 
                marca_id: 1, 
                marca: 'Apple', 
                estado: 'Activo', 
                estado_id: 1, 
                stock: 12, 
                proveedores: [1, 3], 
                fecha_registro: '2024-03-18' 
            }
        },
        proveedores: {
            1: { id: 1, nombre: 'Tech Solutions S.A.', contacto: 'ventas@techsolutions.com', direccion: 'Av. Tecnológica 123, Ciudad Tech', estado: 'Activo', estado_id: 1, fecha_registro: '2024-03-15' },
            2: { id: 2, nombre: 'Distribuidora Global', contacto: 'comercial@distribuidoraglobal.com', direccion: 'Calle Principal 456, Zona Comercial', estado: 'Activo', estado_id: 1, fecha_registro: '2024-03-16' },
            3: { id: 3, nombre: 'Importadora Internacional', contacto: 'info@importadoraint.com', direccion: 'Boulevard Comercial 789, Centro', estado: 'Activo', estado_id: 1, fecha_registro: '2024-03-17' },
            4: { id: 4, nombre: 'Suministros Empresariales', contacto: 'pedidos@suministrosempr.com', direccion: 'Zona Industrial 321, Sector Norte', estado: 'Inactivo', estado_id: 2, fecha_registro: '2024-03-18' }
        },
        bodegas: {
            1: { id: 1, nombre: 'Bodega Central', descripcion: 'Bodega principal ubicada en zona estratégica para distribución', ubicacion: 'Av. Central 123, Ciudad Ejemplo', estado: 'Activa', estado_id: 1, productos_count: 250, fecha_registro: '2024-03-15' }
        },
        marcas: {
            1: { id: 1, nombre: 'Apple', descripcion: 'Tecnología innovadora', estado: 'Activa', estado_id: 1, productos_count: 15, fecha_registro: '2024-03-15' },
            2: { id: 2, nombre: 'Samsung', descripcion: 'Innovación tecnológica global', estado: 'Activa', estado_id: 1, productos_count: 12, fecha_registro: '2024-03-16' }
        },
        estados: {
            1: { id: 1, nombre: 'Activo', funcion: 'Habilitar funcionalidades', descripcion: 'Estado que indica que el elemento se encuentra operativo y disponible para su uso normal', elementos_count: 32, fecha_registro: '2024-03-15' },
            2: { id: 2, nombre: 'Inactivo', funcion: 'Deshabilitar funcionalidades', descripcion: 'Estado que indica que el elemento se encuentra deshabilitado temporalmente', elementos_count: 8, fecha_registro: '2024-03-15' },
            3: { id: 3, nombre: 'Suspendido', funcion: 'Suspender operaciones', descripcion: 'Estado que indica que el elemento ha sido suspendido por motivos administrativos', elementos_count: 3, fecha_registro: '2024-03-15' }
        },
        entradas: {
            1: { id: 1, producto_id: 1, producto: 'iPhone 14 Pro', bodega_id: 1, bodega: 'Bodega Principal', cantidad: 50, fecha_ingreso: '2024-03-15', estado: 'Completada', estado_id: 1, usuario: 'Admin', hora_registro: '10:30 AM' }
        },
        ajustes: {
            1: { 
                id: 1, 
                fecha: '2024-03-15', 
                bodega_id: 1, 
                bodega: 'Bodega Principal', 
                productos: [
                    { producto_id: 1, producto: 'iPhone 14 Pro', cantidad_anterior: 20, cantidad_nueva: 25, diferencia: 5 }
                ],
                total_productos: 3, 
                usuario: 'Admin', 
				estado: 'Completada',
				estado_id: 1,
            }
        }
    };
}

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

    const formData = new FormData(form);
    const data = {
        nombre: formData.get('nombreCategoria') || document.getElementById('nombreCategoria').value,
        descripcion: formData.get('descripcion') || document.getElementById('descripcion').value
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/categorias`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaCategoriaModal'));
            modal.hide();
            form.reset();
            showToast('Categoría creada exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar categoría:', error);
    }
}

// Actualizar categoría
async function actualizarCategoria() {
    const form = document.getElementById('editarCategoriaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    // Obtener ID desde un campo oculto o variable global
    const categoriaId = window.currentCategoriaId || 1; // Temporal
    
    const data = {
        nombre: document.getElementById('editNombreCategoria').value,
        descripcion: document.getElementById('editDescripcion').value,
        estado_id: parseInt(document.getElementById('editEstadoCategoria').value)
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/categorias/${categoriaId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarCategoriaModal'));
            modal.hide();
            showToast('Categoría actualizada exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar categoría:', error);
    }
}

// Eliminar categoría
async function eliminarCategoria() {
    const categoriaId = window.currentCategoriaId || 1; // Temporal

    try {
        const response = await makeRequest(`${API_BASE_URL}/categorias/${categoriaId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarCategoriaModal'));
            modal.hide();
            showToast('Categoría eliminada exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar categoría:', error);
    }
}

// Cargar categoría para visualizar/editar
async function cargarCategoria(id, modo = 'visualizar') {
    window.currentCategoriaId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/categorias/${id}`);
        
        if (response.success) {
            const categoria = response.data;
            mostrarDatosCategoria(categoria, modo);
        }
    } catch (error) {
        console.error('Error al cargar categoría:', error);
        // Si hay error, usar datos de demostración
        const datosDemo = cargarDatosDemo();
        const categoria = datosDemo.categorias[id] || datosDemo.categorias[1];
        mostrarDatosCategoria(categoria, modo);
    }
}

// Función auxiliar para mostrar datos de categoría en modales
function mostrarDatosCategoria(categoria, modo) {
    if (modo === 'visualizar') {
        document.getElementById('visualizarNombre').textContent = categoria.nombre;
        document.getElementById('visualizarDescripcion').textContent = categoria.descripcion;
        document.getElementById('visualizarEstado').textContent = categoria.estado;
        document.getElementById('visualizarProductos').textContent = `${categoria.productos_count} productos`;
        document.getElementById('visualizarFecha').textContent = formatearFecha(categoria.fecha_registro);
    } else if (modo === 'editar') {
        document.getElementById('editNombreCategoria').value = categoria.nombre;
        document.getElementById('editDescripcion').value = categoria.descripcion;
        
        // Cargar estados en el select y establecer el valor actual
        if (categoria.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoCategoria').value = categoria.estado_id;
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

    const formData = new FormData(form);
    const selectProveedores = document.getElementById('proveedorProducto');
    const proveedoresSeleccionados = Array.from(selectProveedores.selectedOptions).map(option => parseInt(option.value));
    
    const data = {
        nombre: formData.get('nombre') || document.getElementById('nombreProducto')?.value,
        descripcion: formData.get('descripcion') || document.getElementById('descripcionProducto')?.value,
        sku: formData.get('sku') || document.getElementById('skuProducto')?.value,
		precio: parseFloat(formData.get('precio') || document.getElementById('precioProducto')?.value) || 0,
        categoria_id: parseInt(formData.get('categoria_id') || document.getElementById('categoriaProducto')?.value),
        marca_id: parseInt(formData.get('marca_id') || document.getElementById('marcaProducto')?.value),
        proveedores: proveedoresSeleccionados
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/productos`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoProductoModal'));
            modal.hide();
            form.reset();
            showToast('Producto creado exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar producto:', error);
    }
}

// Actualizar producto
async function actualizarProducto() {
    const form = document.getElementById('editarProductoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const productoId = window.currentProductoId || 1;
    const selectProveedores = document.getElementById('editProveedorProducto');
    const proveedoresSeleccionados = Array.from(selectProveedores.selectedOptions).map(option => parseInt(option.value));
    
    const data = {
        nombre: document.getElementById('editNombreProducto')?.value,
        descripcion: document.getElementById('editDescripcionProducto')?.value,
        precio: parseFloat(document.getElementById('editPrecioProducto')?.value),
		sku: document.getElementById('editSkuProducto')?.value,
        categoria_id: parseInt(document.getElementById('editCategoriaProducto')?.value),
        marca_id: parseInt(document.getElementById('editMarcaProducto')?.value),
        estado_id: parseInt(document.getElementById('editEstadoProducto')?.value),
        proveedores: proveedoresSeleccionados
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/productos/${productoId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarProductoModal'));
            modal.hide();
            showToast('Producto actualizado exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar producto:', error);
    }
}

// Eliminar producto
async function eliminarProducto() {
    const productoId = window.currentProductoId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/productos/${productoId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarProductoModal'));
            modal.hide();
            showToast('Producto eliminado exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar producto:', error);
    }
}

// Cargar producto para visualizar/editar
async function cargarProducto(id, modo = 'visualizar') {
    window.currentProductoId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/productos/${id}`);
        if (response.success) {
            mostrarDatosProducto(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar producto:', error);
        const datosDemo = cargarDatosDemo();
        const producto = datosDemo.productos[id] || datosDemo.productos[1];
        mostrarDatosProducto(producto, modo);
    }
}

// Función auxiliar para mostrar datos de producto en modales
function mostrarDatosProducto(producto, modo) {
    if (modo === 'visualizar') {
        const visualizarNombre = document.getElementById('visualizarNombreProducto');
        const visualizarSku = document.getElementById('visualizarSkuProducto');
        const visualizarDescripcion = document.getElementById('visualizarDescripcionProducto');
        const visualizarPrecio = document.getElementById('visualizarPrecioProducto');
        const visualizarCategoria = document.getElementById('visualizarCategoriaProducto');
        const visualizarMarca = document.getElementById('visualizarMarcaProducto');
        const visualizarEstado = document.getElementById('visualizarEstadoProducto');
        const visualizarStock = document.getElementById('visualizarStockProducto');
        const visualizarFecha = document.getElementById('visualizarFechaProducto');
        
        if (visualizarNombre) visualizarNombre.textContent = producto.nombre;
        if (visualizarSku) visualizarSku.textContent = producto.sku;
        if (visualizarDescripcion) visualizarDescripcion.textContent = producto.descripcion;
        if (visualizarPrecio) visualizarPrecio.textContent = `$${producto.precio}`;
        if (visualizarCategoria) visualizarCategoria.textContent = producto.categoria;
        if (visualizarMarca) visualizarMarca.textContent = producto.marca;
        if (visualizarEstado) visualizarEstado.textContent = producto.estado;
        if (visualizarStock) visualizarStock.textContent = `${producto.stock} unidades`;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(producto.fecha_registro);
        
        // Mostrar proveedores
        const visualizarProveedores = document.getElementById('visualizarProveedoresProducto');
        if (visualizarProveedores) {
            const datosDemo = cargarDatosDemo();
            if (producto.proveedores && producto.proveedores.length > 0) {
                const nombresProveedores = producto.proveedores.map(id => {
                    const proveedor = datosDemo.proveedores[id];
                    return proveedor ? proveedor.nombre : `Proveedor ${id}`;
                }).join(', ');
                visualizarProveedores.textContent = nombresProveedores;
            } else {
                visualizarProveedores.textContent = 'No hay proveedores asignados';
            }
        }
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreProducto');
        const editSku = document.getElementById('editSkuProducto');
        const editDescripcion = document.getElementById('editDescripcionProducto');
        const editPrecio = document.getElementById('editPrecioProducto');
        const editCategoria = document.getElementById('editCategoriaProducto');
        const editMarca = document.getElementById('editMarcaProducto');
        
        if (editNombre) editNombre.value = producto.nombre;
        if (editSku) editSku.value = producto.sku;
        if (editDescripcion) editDescripcion.value = producto.descripcion;
        if (editPrecio) editPrecio.value = producto.precio;
        if (editCategoria) editCategoria.value = producto.categoria_id;
        if (editMarca) editMarca.value = producto.marca_id;
        
        // Cargar estados en el select y establecer el valor actual
        if (producto.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoProducto').value = producto.estado_id;
            }, 100);
        }

		if (producto.marca_id) {
            setTimeout(() => {
                document.getElementById('editMarcaProducto').value = producto.marca_id;
            }, 100);
        }

		if (producto.categoria_id) {
			setTimeout(() => {
				document.getElementById('editCategoriaProducto').value = producto.categoria_id;
			}, 100);
		}
		
		// Cargar y seleccionar proveedores
		if (producto.proveedores && producto.proveedores.length > 0) {
            setTimeout(() => {
                const selectProveedores = document.getElementById('editProveedorProducto');
                if (selectProveedores) {
                    // Limpiar selección actual
                    Array.from(selectProveedores.options).forEach(option => {
                        option.selected = false;
                    });
                    // Seleccionar proveedores del producto
                    producto.proveedores.forEach(proveedorId => {
                        const option = selectProveedores.querySelector(`option[value="${proveedorId}"]`);
                        if (option) {
                            option.selected = true;
                        }
                    });
                }
            }, 200);
        } else {
            // Si no hay proveedores, limpiar la selección
            setTimeout(() => {
                const selectProveedores = document.getElementById('editProveedorProducto');
                if (selectProveedores) {
                    Array.from(selectProveedores.options).forEach(option => {
                        option.selected = false;
                    });
                }
            }, 200);
        }
    } else if (modo === 'eliminar') {
        const eliminarNombre = document.getElementById('eliminarNombreProducto');
        const eliminarDescripcion = document.getElementById('eliminarDescripcionProducto');
        const eliminarMarca = document.getElementById('eliminarMarcaProducto');
        const eliminarPrecio = document.getElementById('eliminarPrecioProducto');
        
        if (eliminarNombre) eliminarNombre.textContent = producto.nombre;
        if (eliminarDescripcion) eliminarDescripcion.textContent = producto.descripcion;
        if (eliminarMarca) eliminarMarca.textContent = producto.marca;
        if (eliminarPrecio) eliminarPrecio.textContent = producto.precio;
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
        nombre: document.getElementById('nombreProveedor')?.value,
        contacto: document.getElementById('contactoProveedor')?.value,
        direccion: document.getElementById('direccionProveedor')?.value,
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/proveedores`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoProveedorModal'));
            modal.hide();
            form.reset();
            showToast('Proveedor creado exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar proveedor:', error);
    }
}

// Actualizar proveedor
async function actualizarProveedor() {
    const form = document.getElementById('editarProveedorForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const proveedorId = window.currentProveedorId || 1;
    
    const data = {
        nombre: document.getElementById('editNombreProveedor')?.value,
        contacto: document.getElementById('editContacto')?.value,
        direccion: document.getElementById('editDireccion')?.value,
        estado_id: parseInt(document.getElementById('editEstadoProveedor')?.value)
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/proveedores/${proveedorId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarProveedorModal'));
            modal.hide();
            showToast('Proveedor actualizado exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar proveedor:', error);
    }
}

// Eliminar proveedor
async function eliminarProveedor() {
    const proveedorId = window.currentProveedorId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/proveedores/${proveedorId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarProveedorModal'));
            modal.hide();
            showToast('Proveedor eliminado exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar proveedor:', error);
    }
}

// Cargar proveedor para visualizar/editar
async function cargarProveedor(id, modo = 'visualizar') {
    window.currentProveedorId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/proveedores/${id}`);
        if (response.success) {
            mostrarDatosProveedor(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar proveedor:', error);
        const datosDemo = cargarDatosDemo();
        const proveedor = datosDemo.proveedores[id] || datosDemo.proveedores[1];
        mostrarDatosProveedor(proveedor, modo);
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

        if (visualizarNombre) visualizarNombre.textContent = proveedor.nombre;
        if (visualizarContacto) visualizarContacto.textContent = proveedor.contacto;
        if (visualizarDireccion) visualizarDireccion.textContent = proveedor.direccion;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(proveedor.fecha_registro);
		if (visualizarEstado) visualizarEstado.textContent = proveedor.estado;
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreProveedor');
        const editContacto = document.getElementById('editContacto');
        const editDireccion = document.getElementById('editDireccion');
        
        if (editNombre) editNombre.value = proveedor.nombre;
        if (editContacto) editContacto.value = proveedor.contacto;
        if (editDireccion) editDireccion.value = proveedor.direccion;
        
        // Cargar estados en el select y establecer el valor actual
        if (proveedor.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoProveedor').value = proveedor.estado_id;
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
        nombre: document.getElementById('nombreBodega')?.value,
        descripcion: document.getElementById('descripcionBodega')?.value,
        ubicacion: document.getElementById('ubicacionBodega')?.value
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/bodegas`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaBodegaModal'));
            modal.hide();
            form.reset();
            showToast('Bodega creada exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar bodega:', error);
    }
}

// Actualizar bodega
async function actualizarBodega() {
    const form = document.getElementById('editarBodegaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const bodegaId = window.currentBodegaId || 1;
    
    const data = {
        nombre: document.getElementById('editNombreBodega')?.value,
        descripcion: document.getElementById('editDescripcionBodega')?.value,
        ubicacion: document.getElementById('editUbicacionBodega')?.value,
        estado_id: parseInt(document.getElementById('editEstadoBodega')?.value)
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/bodegas/${bodegaId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarBodegaModal'));
            modal.hide();
            showToast('Bodega actualizada exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar bodega:', error);
    }
}

// Eliminar bodega
async function eliminarBodega() {
    const bodegaId = window.currentBodegaId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/bodegas/${bodegaId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarBodegaModal'));
            modal.hide();
            showToast('Bodega eliminada exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar bodega:', error);
    }
}

// Cargar bodega para visualizar/editar
async function cargarBodega(id, modo = 'visualizar') {
    window.currentBodegaId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/bodegas/${id}`);
        if (response.success) {
            mostrarDatosBodega(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar bodega:', error);
        const datosDemo = cargarDatosDemo();
        const bodega = datosDemo.bodegas[id] || datosDemo.bodegas[1];
        mostrarDatosBodega(bodega, modo);
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
        if (visualizarProductos) visualizarProductos.textContent = `${bodega.productos_count} productos`;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(bodega.fecha_registro);
		if (visualizarEstado) visualizarEstado.textContent = bodega.estado;
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreBodega');
        const editDescripcion = document.getElementById('editDescripcionBodega');
        const editUbicacion = document.getElementById('editUbicacionBodega');
        
        if (editNombre) editNombre.value = bodega.nombre;
        if (editDescripcion) editDescripcion.value = bodega.descripcion;
        if (editUbicacion) editUbicacion.value = bodega.ubicacion;
        
        // Cargar estados en el select y establecer el valor actual
        if (bodega.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoBodega').value = bodega.estado_id;
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
        nombre: document.getElementById('nombreMarca')?.value,
        descripcion: document.getElementById('descripcionMarca')?.value,
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/marcas`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaMarcaModal'));
            modal.hide();
            form.reset();
            showToast('Marca creada exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar marca:', error);
    }
}

// Actualizar marca
async function actualizarMarca() {
    const form = document.getElementById('editarMarcaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const marcaId = window.currentMarcaId || 1;
    
    const data = {
        nombre: document.getElementById('editNombreMarca')?.value,
        descripcion: document.getElementById('editDescripcionMarca')?.value,
        estado_id: parseInt(document.getElementById('editEstadoMarca')?.value)
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/marcas/${marcaId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarMarcaModal'));
            modal.hide();
            showToast('Marca actualizada exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar marca:', error);
    }
}

// Eliminar marca
async function eliminarMarca() {
    const marcaId = window.currentMarcaId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/marcas/${marcaId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarMarcaModal'));
            modal.hide();
            showToast('Marca eliminada exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar marca:', error);
    }
}

// Cargar marca para visualizar/editar
async function cargarMarca(id, modo = 'visualizar') {
    window.currentMarcaId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/marcas/${id}`);
        if (response.success) {
            mostrarDatosMarca(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar marca:', error);
        const datosDemo = cargarDatosDemo();
        const marca = datosDemo.marcas[id] || datosDemo.marcas[1];
        mostrarDatosMarca(marca, modo);
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
        if (visualizarProductos) visualizarProductos.textContent = `${marca.productos_count} productos`;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(marca.fecha_registro);
        if (visualizarEstado) visualizarEstado.textContent = marca.estado;
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreMarca');
        const editDescripcion = document.getElementById('editDescripcionMarca');
        
        if (editNombre) editNombre.value = marca.nombre;
        if (editDescripcion) editDescripcion.value = marca.descripcion;
        
        // Cargar estados en el select y establecer el valor actual
        if (marca.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoMarca').value = marca.estado_id;
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
        nombre: document.getElementById('nombreEstado')?.value,
        funcion: document.getElementById('funcion')?.value,
        descripcion: document.getElementById('descripcion')?.value
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/estados`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoEstadoModal'));
            modal.hide();
            form.reset();
            showToast('Estado creado exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar estado:', error);
    }
}

// Actualizar estado
async function actualizarEstado() {
    const form = document.getElementById('editarEstadoForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const estadoId = window.currentEstadoId || 1;
    
    const data = {
        nombre: document.getElementById('editNombreEstado')?.value,
        funcion: document.getElementById('editFuncion')?.value,
        descripcion: document.getElementById('editDescripcion')?.value
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/estados/${estadoId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarEstadoModal'));
            modal.hide();
            showToast('Estado actualizado exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar estado:', error);
    }
}

// Eliminar estado
async function eliminarEstado() {
    const estadoId = window.currentEstadoId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/estados/${estadoId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarEstadoModal'));
            modal.hide();
            showToast('Estado eliminado exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar estado:', error);
    }
}

// Cargar estado para visualizar/editar
async function cargarEstado(id, modo = 'visualizar') {
    window.currentEstadoId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/estados/${id}`);
        if (response.success) {
            mostrarDatosEstado(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar estado:', error);
        const datosDemo = cargarDatosDemo();
        const estado = datosDemo.estados[id] || datosDemo.estados[1];
        mostrarDatosEstado(estado, modo);
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
        if (visualizarElementos) visualizarElementos.textContent = `${estado.elementos_count} elementos`;
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(estado.fecha_registro);
    } else if (modo === 'editar') {
        const editNombre = document.getElementById('editNombreEstado');
        const editFuncion = document.getElementById('editFuncion');
        const editDescripcion = document.getElementById('editDescripcion');
        
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
        producto_id: parseInt(document.getElementById('productoEntrada')?.value),
        bodega_id: parseInt(document.getElementById('bodegaEntrada')?.value),
        cantidad: parseInt(document.getElementById('cantidadEntrada')?.value),
        fecha_ingreso: document.getElementById('fechaIngresoEntrada')?.value
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/entradas`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevaEntradaModal'));
            modal.hide();
            form.reset();
            showToast('Entrada creada exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar entrada:', error);
    }
}

// Actualizar entrada
async function actualizarEntrada() {
    const form = document.getElementById('editarEntradaForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const entradaId = window.currentEntradaId || 1;
    
    const data = {
        producto_id: parseInt(document.getElementById('editProductoEntrada')?.value),
        bodega_id: parseInt(document.getElementById('editBodegaEntrada')?.value),
        cantidad: parseInt(document.getElementById('editCantidadEntrada')?.value),
        fecha_ingreso: document.getElementById('editFechaIngresoEntrada')?.value,
        estado_id: parseInt(document.getElementById('editEstadoEntrada')?.value)
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/entradas/${entradaId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarEntradaModal'));
            modal.hide();
            showToast('Entrada actualizada exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar entrada:', error);
    }
}

// Eliminar entrada
async function eliminarEntrada() {
    const entradaId = window.currentEntradaId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/entradas/${entradaId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarEntradaModal'));
            modal.hide();
            showToast('Entrada eliminada exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar entrada:', error);
    }
}

// Cargar entrada para visualizar/editar
async function cargarEntrada(id, modo = 'visualizar') {
    window.currentEntradaId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/entradas/${id}`);
        if (response.success) {
            mostrarDatosEntrada(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar entrada:', error);
        const datosDemo = cargarDatosDemo();
        const entrada = datosDemo.entradas[id] || datosDemo.entradas[1];
        mostrarDatosEntrada(entrada, modo);
    }
}

// Función auxiliar para mostrar datos de entrada en modales
function mostrarDatosEntrada(entrada, modo) {
    if (modo === 'visualizar') {
        const visualizarProducto = document.getElementById('visualizarProductoEntrada');
        const visualizarBodega = document.getElementById('visualizarBodegaEntrada');
        const visualizarCantidad = document.getElementById('visualizarCantidadEntrada');
        const visualizarFechaIngreso = document.getElementById('visualizarFechaIngresoEntrada');
        const visualizarEstado = document.getElementById('visualizarEstadoEntrada');
        const visualizarUsuario = document.getElementById('visualizarUsuarioEntrada');
        const visualizarHora = document.getElementById('visualizarHoraEntrada');
        
        if (visualizarProducto) visualizarProducto.textContent = entrada.producto;
        if (visualizarBodega) visualizarBodega.textContent = entrada.bodega;
        if (visualizarCantidad) visualizarCantidad.textContent = `${entrada.cantidad} unidades`;
        if (visualizarFechaIngreso) visualizarFechaIngreso.textContent = formatearFecha(entrada.fecha_ingreso);
        if (visualizarEstado) visualizarEstado.textContent = entrada.estado;
        if (visualizarUsuario) visualizarUsuario.textContent = entrada.usuario;
        if (visualizarHora) visualizarHora.textContent = entrada.hora_registro;
    } else if (modo === 'editar') {
        const editProducto = document.getElementById('editProductoEntrada');
        const editBodega = document.getElementById('editBodegaEntrada');
        const editCantidad = document.getElementById('editCantidadEntrada');
        const editFechaIngreso = document.getElementById('editFechaIngresoEntrada');
        
        if (editProducto) editProducto.value = entrada.producto_id;
        if (editBodega) editBodega.value = entrada.bodega_id;
        if (editCantidad) editCantidad.value = entrada.cantidad;
        if (editFechaIngreso) editFechaIngreso.value = entrada.fecha_ingreso;
        
        // Cargar estados en el select y establecer el valor actual
        if (entrada.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoEntrada').value = entrada.estado_id;
            }, 100);
        }

		if (entrada.producto_id) {
			setTimeout(() => {
				document.getElementById('editProductoEntrada').value = entrada.producto_id;
			}, 100);
		}

		if (entrada.bodega_id) {
			setTimeout(() => {
				document.getElementById('editBodegaEntrada').value = entrada.bodega_id;
			}, 100);
		}
    } else if (modo === 'eliminar') {
        const eliminarProducto = document.getElementById('eliminarProductoEntrada');
        const eliminarCantidad = document.getElementById('eliminarCantidadEntrada');
        
        if (eliminarProducto) eliminarProducto.textContent = entrada.producto;
        if (eliminarCantidad) eliminarCantidad.textContent = `${entrada.cantidad} unidades`;
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
    const filas = document.querySelectorAll('#tablaProductos tr:not(:first-child)');
    
    filas.forEach(fila => {
        const celdas = fila.querySelectorAll('td');
        if (celdas.length > 1) {
            productos.push({
                producto_id: parseInt(fila.dataset.productoId),
                cantidad_anterior: parseInt(celdas[1].textContent),
                cantidad_nueva: parseInt(celdas[2].textContent)
            });
        }
    });

    const data = {
        bodega_id: parseInt(document.getElementById('bodegaAjuste')?.value),
        fecha: document.getElementById('fechaAjuste')?.value,
        productos: productos
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/ajustes`, 'POST', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('nuevoAjusteModal'));
            modal.hide();
            form.reset();
            showToast('Ajuste creado exitosamente');
        }
    } catch (error) {
        console.error('Error al guardar ajuste:', error);
    }
}

// Actualizar ajuste
async function actualizarAjuste() {
    const form = document.getElementById('editarAjusteForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const ajusteId = window.currentAjusteId || 1;
    
    // Recopilar productos de la tabla de edición
    const productos = [];
    const filas = document.querySelectorAll('#editTablaProductos tr:not(:first-child)');
    
    filas.forEach(fila => {
        const celdas = fila.querySelectorAll('td');
        if (celdas.length > 1) {
            productos.push({
                producto_id: parseInt(fila.dataset.productoId),
                cantidad_anterior: parseInt(celdas[1].textContent),
                cantidad_nueva: parseInt(celdas[2].textContent)
            });
        }
    });

    const data = {
        bodega_id: parseInt(document.getElementById('editBodegaAjuste')?.value),
        fecha: document.getElementById('editFechaAjuste')?.value,
        estado_id: parseInt(document.getElementById('editEstadoAjuste')?.value),
        productos: productos
    };

    try {
        const response = await makeRequest(`${API_BASE_URL}/ajustes/${ajusteId}`, 'PUT', data);
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editarAjusteModal'));
            modal.hide();
            showToast('Ajuste actualizado exitosamente');
        }
    } catch (error) {
        console.error('Error al actualizar ajuste:', error);
    }
}

// Eliminar ajuste
async function eliminarAjuste() {
    const ajusteId = window.currentAjusteId || 1;

    try {
        const response = await makeRequest(`${API_BASE_URL}/ajustes/${ajusteId}`, 'DELETE');
        
        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('eliminarAjusteModal'));
            modal.hide();
            showToast('Ajuste eliminado exitosamente');
        }
    } catch (error) {
        console.error('Error al eliminar ajuste:', error);
    }
}

// Cargar ajuste para visualizar/editar
async function cargarAjuste(id, modo = 'visualizar') {
    window.currentAjusteId = id;
    
    try {
        const response = await makeRequest(`${API_BASE_URL}/ajustes/${id}`);
        if (response.success) {
            mostrarDatosAjuste(response.data, modo);
        }
    } catch (error) {
        console.error('Error al cargar ajuste:', error);
        const datosDemo = cargarDatosDemo();
        const ajuste = datosDemo.ajustes[id] || datosDemo.ajustes[1];
        mostrarDatosAjuste(ajuste, modo);
    }
}

// Función auxiliar para mostrar datos de ajuste en modales
function mostrarDatosAjuste(ajuste, modo) {
    if (modo === 'visualizar') {
        const visualizarFecha = document.getElementById('visualizarFechaAjuste');
        const visualizarBodega = document.getElementById('visualizarBodegaAjuste');
        const visualizarTotalProductos = document.getElementById('visualizarTotalProductosAjuste');
        const visualizarUsuario = document.getElementById('visualizarUsuarioAjuste');
        const visualizarEstado = document.getElementById('visualizarEstadoAjuste');
        
        if (visualizarFecha) visualizarFecha.textContent = formatearFecha(ajuste.fecha);
        if (visualizarBodega) visualizarBodega.textContent = ajuste.bodega;
        if (visualizarTotalProductos) visualizarTotalProductos.textContent = `${ajuste.total_productos} productos`;
        if (visualizarUsuario) visualizarUsuario.textContent = ajuste.usuario;
        if (visualizarEstado) visualizarEstado.textContent = ajuste.estado;
        
        // Cargar productos en la tabla de visualización
        const tabla = document.getElementById('visualizarTablaProductosAjuste');
        if (tabla && ajuste.productos) {
            tabla.innerHTML = '';
            ajuste.productos.forEach(producto => {
                const fila = document.createElement('tr');
                fila.innerHTML = `
                    <td>${producto.producto}</td>
                    <td><span class="badge bg-warning">${producto.cantidad_anterior}</span></td>
                    <td><span class="badge bg-success">${producto.cantidad_nueva}</span></td>
                    <td><span class="badge ${producto.diferencia >= 0 ? 'bg-success' : 'bg-danger'}">${producto.diferencia >= 0 ? '+' : ''}${producto.diferencia}</span></td>
                `;
                tabla.appendChild(fila);
            });
        }
    } else if (modo === 'editar') {
        const editBodega = document.getElementById('editBodegaAjuste');
        const editFecha = document.getElementById('editFechaAjuste');
        
        if (editBodega) editBodega.value = ajuste.bodega_id;
        if (editFecha) editFecha.value = ajuste.fecha;
        
        // Cargar estados en el select y establecer el valor actual
        if (ajuste.estado_id) {
            setTimeout(() => {
                document.getElementById('editEstadoAjuste').value = ajuste.estado_id;
            }, 100);
        }

		if (ajuste.bodega_id) {
			setTimeout(() => {
				document.getElementById('editBodegaAjuste').value = ajuste.bodega_id;
			}, 100);
		}

        // Cargar productos en la tabla
        const tabla = document.getElementById('editTablaProductosAjuste');
        if (tabla && ajuste.productos) {
            tabla.innerHTML = '';
            
            ajuste.productos.forEach(producto => {
                const fila = document.createElement('tr');
                fila.dataset.productoId = producto.producto_id;
                fila.innerHTML = `
                    <td>${producto.producto}</td>
                    <td><span class="badge bg-primary">${producto.cantidad_nueva || producto.cantidad}</span></td>
                    <td>
                        <button type="button" class="btn btn-danger btn-sm" onclick="editQuitarProducto(this)">
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
        
        if (eliminarBodega) eliminarBodega.textContent = ajuste.bodega;
        if (eliminarFecha) eliminarFecha.textContent = formatearFecha(ajuste.fecha);
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