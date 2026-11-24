# Manual de usuario — Property Manager

> Guía práctica de la aplicación **Property Manager**: inmuebles, unidades, inquilinos, contratos de arrendamiento, rentas, pagos, gastos, documentos, mensajes, agenda y administración.
>
> Destinado a usuarios finales y administradores. No se necesitan conocimientos de programación.
>
> 💡 Este manual está disponible **dentro de la aplicación**: haga clic en el botón `?` de la **barra superior** para abrirlo en un diálogo sin abandonar la página en la que se encuentra.

---

## Índice

- [1. Primeros pasos](#primeiros-passos)
- [2. Visión general y navegación](#navegacao)
- [3. Panel de control](#dashboard)
- [4. Inmuebles](#imoveis)
- [5. Unidades](#fracoes)
- [6. Inquilinos](#inquilinos)
- [7. Arrendamientos](#arrendamentos)
- [8. Rentas y pagos](#rendas-pagamentos)
- [9. Gastos](#despesas)
- [10. Contactos, propietario y documentos](#contactos-documentos)
- [11. Mensajes, notificaciones y agenda](#mensagens)
- [12. Plantillas de cartas](#modelos-cartas)
- [13. Calculadoras y mapas](#calculadoras)
- [14. Administración](#administracao)
- [15. Resolución de problemas](#resolucao)
- [16. Preguntas frecuentes](#faq)
- [Apéndice A — Rutas principales](#rotas)

---

## 1. Primeros pasos {#primeiros-passos}

### 1.1 Iniciar sesión

1. Abra la dirección de la aplicación en el navegador.
2. Introduzca su **correo electrónico** y su **contraseña**.
3. Haga clic en **Iniciar sesión**.
4. Si todavía no tiene cuenta, use el enlace de **registro** en la pantalla de autenticación (en algunas instalaciones la cuenta la crea el administrador).

### 1.2 Elegir el idioma

1. En la barra superior, utilice el **selector de idioma**.
2. Elija entre **Portugués**, **Inglés**, **Español** y **Francés**.
3. La elección se guarda en la configuración de la aplicación y recarga la interfaz.

> 💡 El contenido que introduce (nombres, direcciones, descripciones) **no se traduce**: solo se traduce la interfaz. Puede tener la interfaz en español y sus datos en portugués.

### 1.3 Crear el primer inmueble

1. En el menú lateral, abra **Inmuebles** (`/imoveis`).
2. Haga clic en **Nuevo** (el botón verde con `+`).
3. Rellene los datos de identificación y la **dirección** del inmueble.
4. Haga clic en **Guardar**.

### 1.4 Crear la primera unidad

1. Abra **Inmuebles** y seleccione el inmueble deseado.
2. Añada una **unidad**, por ejemplo un apartamento o un local.
3. Rellene la identificación de la unidad, la **renta**, el **tipo** y la **situación**.
4. Puede cargar **imágenes** de la unidad en el gestor de imágenes (`/fracoes/imagesmanager`).

### 1.5 Crear el primer inquilino

1. En el menú lateral, abra **Inquilinos** (`/inquilinos`).
2. Haga clic en **Nuevo** y rellene los datos personales y de contacto.
3. Si procede, asocie un **garante**.
4. Guarde.

### 1.6 Crear el primer arrendamiento

1. Abra **Arrendamientos** (`/arrendamentos`).
2. Haga clic en **Nuevo**.
3. Asocie la **unidad** y el **inquilino**, e indique la fecha de inicio, el plazo y el importe de la renta.
4. Guarde. A partir de aquí podrá generar el contrato y las cartas relacionadas.

### 1.7 Tabla «quiero hacer X → voy a Y»

| Quiero… | Voy a… | Ruta |
|---|---|---|
| Ver el resumen general | Inicio | `/` |
| Gestionar inmuebles | Inmuebles | `/imoveis` |
| Gestionar unidades e imágenes | Inmuebles → unidad | `/fracoes/viewunits` |
| Gestionar inquilinos | Inquilinos | `/inquilinos` |
| Gestionar contratos | Arrendamientos | `/arrendamentos` |
| Procesar las rentas mensuales | Pagos → Procesar rentas | `/monthlyrentprocessing` |
| Registrar pagos | Pagos → Mantenimiento | `/recebimentos/manutencao` |
| Gestionar gastos | Gastos → Mantenimiento | `/despesas/manutencao` |
| Editar cartas | Administración → Plantillas de cartas | `/LetterTemplates` |
| Ver notificaciones | Campana en la barra superior | `/notifications` |
| Abrir la agenda | Agenda | `/scheduler` |
| Ajustes y copias de seguridad | Mantenimiento | `/admin/appsettings` |

---

## 2. Visión general y navegación {#navegacao}

### 2.1 Barra superior

Contiene:

- El botón para **abrir/cerrar el menú lateral**.
- El nombre de la aplicación.
- El botón de **ayuda** (`?`) — abre este manual sin salir de la página.
- La **campana de notificaciones**.
- El **selector de idioma**.
- El **nombre de usuario** (enlace para gestionar la cuenta).
- El botón de **tema claro/oscuro**.
- La opción de **cerrar sesión**.

### 2.2 Menú lateral

El menú está organizado en grupos:

- **General** — Inicio, Inmuebles, Inquilinos, Arrendamientos, Gastos, Pagos, Calculadoras, Contactos, Documentos, Mensajes y Agenda.
- **Administración / mantenimiento** — Registros, Ajustes, Copia de seguridad de la base de datos y Copia de seguridad de archivos.

Los grupos con subelementos se expanden o contraen al hacer clic. El elemento activo aparece resaltado.

### 2.3 Rejillas

Casi todas las listas usan una rejilla con barra de herramientas. Las operaciones más habituales son:

| Acción | Cómo |
|---|---|
| Buscar | Cuadro de búsqueda en la barra de la rejilla |
| Ordenar | Hacer clic en el encabezado de una columna |
| Exportar | `PdfExport` o `ExcelExport` en la barra de herramientas |
| Expandir/contraer filas | Botones de expandir/contraer en la barra |
| Crear | Botón **Nuevo** |
| Editar / eliminar | Botones en la fila o menú contextual |

> 💡 En algunas rejillas, el clic derecho abre un **menú contextual** con acciones rápidas (cartas de retraso, actualización de rentas, etc.).

---

## 3. Panel de control {#dashboard}

La página de inicio (`/`) presenta los indicadores generales: resumen de inmuebles, inquilinos, arrendamientos, rentas y gastos, además de alertas y actividad reciente.

- Use las tarjetas para saltar al área correspondiente.
- Si una cifra está desactualizada, recargue la página (F5).
- Las **alertas** del panel son las mismas que muestra la campana de notificaciones: no son avisos distintos por página.

---

## 4. Inmuebles {#imoveis}

Página: **Inmuebles** (`/imoveis`). También existe una vista alternativa en `/properties`.

### 4.1 Qué es un inmueble

El **inmueble** es el nivel superior: un edificio, una casa o un conjunto. Dentro de él se encuentran las **unidades**.

### 4.2 Crear y editar

1. Haga clic en **Nuevo**.
2. Rellene la identificación y la dirección.
3. Guarde.
4. Para modificarlo, use el botón de edición en la fila de la rejilla.

### 4.3 Exportar y buscar

La rejilla permite buscar, ordenar y exportar la lista a PDF.

---

## 5. Unidades {#fracoes}

Páginas: **Unidades** (`/fracoes/viewunits`) y **Gestor de imágenes** (`/fracoes/imagesmanager`).

### 5.1 Qué es una unidad

Una **unidad** es una división dentro del inmueble, por ejemplo un apartamento, un local o un garaje. El **arrendamiento** y la **renta** se asocian a la unidad.

### 5.2 Datos principales

- Identificación y descripción.
- **Tipo de unidad** y **situación** (por ejemplo, disponible o arrendada).
- **Renta** y valor de **IMI** (impuesto municipal sobre inmuebles).
- **Seguro** asociado.
- **Activa / inactiva**.

### 5.3 Imágenes

1. Abra el gestor de imágenes (`/fracoes/imagesmanager`).
2. Seleccione la unidad.
3. Cargue una o más imágenes.
4. Confirme la carga; las imágenes estarán disponibles en la vista de la unidad.

> 💡 Si la carga falla con `Access to the path ... is denied`, la carpeta de carga del servidor no tiene permisos de escritura: pida al administrador que corrija los permisos en IIS.

---

## 6. Inquilinos {#inquilinos}

Página: **Inquilinos** (`/inquilinos`).

### 6.1 Crear un inquilino

1. Haga clic en **Nuevo**.
2. Rellene el **nombre**, los **contactos** y los datos de identificación.
3. Si procede, asocie un **garante**.
4. Guarde.

### 6.2 La ficha del inquilino

La ficha reúne:

- Datos personales y de contacto.
- **Documentos** cargados (`DocumentosInquilino`).
- **Saldo corriente** (`SaldoCorrenteInquilino`): lo que se debe o lo que hay a favor.
- Contratos asociados.

### 6.3 Actualizar el importe de la renta

Hay dos formas:

- **Manualmente**: en el menú contextual del inquilino, elija **Actualizar importe de la renta** (`/updaterentvalue/{id}`).
- **Mediante carta**: en **Arrendamientos → Actualización de rentas** (`/updaterentsletter`).

### 6.4 Cartas rápidas

El menú contextual del inquilino permite generar directamente:

- **Carta de retraso en el pago**.
- **Carta de revocación / oposición al contrato**.
- **Carta de actualización de rentas**.

> ⚠️ Antes de generar una carta, compruebe que los datos del contrato y del inquilino son correctos: la carta usa los valores guardados.

---

## 7. Arrendamientos {#arrendamentos}

Página: **Arrendamientos** (`/arrendamentos`).

### 7.1 Qué es un arrendamiento

Un arrendamiento vincula un **inquilino** a una **unidad**, con fecha de inicio, plazo e importe de la renta. La aplicación avisa cuando un contrato está próximo a terminar o cuando hay rentas atrasadas.

### 7.2 Crear un contrato

1. Haga clic en **Nuevo**.
2. Seleccione la **unidad** y el **inquilino**.
3. Introduzca la **fecha de inicio**, el **plazo** y el **importe de la renta**.
4. Guarde.

### 7.3 Acciones en la rejilla

La barra de herramientas y el menú contextual permiten:

- **Ver el contrato** (`Ver contrato`).
- **Ampliar el plazo del contrato**.
- **Actualizar rentas** y generar la carta correspondiente.
- Consultar el **saldo corriente del inquilino** (`/saldocorrenteinquilino`).

### 7.4 Coeficientes de actualización

En **Arrendamientos → Coeficientes** (`/updatecoefficients`) puede gestionar los coeficientes utilizados en el cálculo de las actualizaciones anuales de la renta.

> 💡 Según la configuración de la instalación, el menú *Arrendamientos* puede aparecer con submenús (Mantenimiento y Actualización de rentas) en lugar de un único elemento.

---

## 8. Rentas y pagos {#rendas-pagamentos}

Páginas: **Pagos → Mantenimiento** (`/recebimentos/manutencao`), **Procesar rentas** (`/monthlyrentprocessing`) y **Panel de pagos** (`/payments/dashboard`).

### 8.1 Registrar un pago

1. Abra **Pagos → Mantenimiento**.
2. Haga clic en **Nuevo** (o equivalente).
3. Seleccione el **inquilino**, el **concepto**, el **importe** y la **fecha**.
4. Guarde.

La rejilla permite buscar, ordenar y exportar a Excel.

### 8.2 Procesamiento mensual de rentas

1. Abra **Pagos → Procesar rentas** (`/monthlyrentprocessing`).
2. Confirme el **mes/año** y los criterios.
3. Revise la vista previa de las rentas que se van a procesar.
4. Confirme el procesamiento.

> ⚠️ El procesamiento por lotes crea varios registros a la vez. Revise siempre la lista antes de confirmar.

### 8.3 Panel de pagos

El panel (`/payments/dashboard`) resume los pagos por período y permite seguir lo que se ha pagado y lo que está pendiente.

---

## 9. Gastos {#despesas}

Páginas: **Gastos → Mantenimiento** (`/despesas/manutencao`), **Tipos de gasto** (`/despesas/tipodespesas`) y **Panel de gastos** (`/expenses/dashboard`).

### 9.1 Registrar un gasto

1. Abra **Gastos → Mantenimiento**.
2. Haga clic en **Nuevo**.
3. Rellene la **descripción**, el **importe**, la **fecha** y el **tipo de gasto**.
4. Guarde.

### 9.2 Tipos de gasto

En **Tipos de gasto** puede crear, editar y organizar las categorías usadas al registrar gastos.

### 9.3 Gráficos

El panel muestra gráficos de gastos por tipo y a lo largo del tiempo (líneas y sectores). Pase el cursor sobre un punto para ver el valor exacto.

---

## 10. Contactos, propietario y documentos {#contactos-documentos}

### 10.1 Contactos

Página: `/contactos`. Guarda contactos útiles (administradores, comunidades, proveedores, etc.).

### 10.2 Propietario

Páginas: `/Proprietario` y `/ProprietarioPage`. Reúnen los datos del propietario del inmueble.

### 10.3 Documentos

Página: **Documentos** (`/documentos`). Permite registrar y cargar documentos generales de la aplicación. Para los documentos de un inquilino, use su ficha.

---

## 11. Mensajes, notificaciones y agenda {#mensagens}

### 11.1 Notificaciones

La **campana** de la barra superior abre el centro de notificaciones. También puede abrir la página completa en `/notifications`.

- Las notificaciones incluyen avisos operativos (citas, rentas, cartas, contratos).
- Los mensajes sin leer aparecen destacados.
- Al abrir una notificación, se marca como leída.

### 11.2 Mensajes

Página: **Mensajes** (`/messages-modern`; la versión anterior está en `/messages`). Permite enviar y consultar mensajes.

### 11.3 Agenda

Página: **Agenda** (`/scheduler`). Permite registrar citas y consultarlas en un calendario, con búsqueda de eventos (`/scheduler/search-events`).

---

## 12. Plantillas de cartas {#modelos-cartas}

Página: **Plantillas de cartas** (`/LetterTemplates`).

### 12.1 Qué son las plantillas

Son archivos `.dotx` usados para generar automáticamente cartas y contratos (avisos de retraso, actualizaciones de renta, revocaciones, contratos de arrendamiento, etc.).

### 12.2 Editar una plantilla

1. Abra **Plantillas de cartas**.
2. Seleccione la plantilla en la lista.
3. Edite el documento en el editor de texto.
4. Para ver el resultado con valores de ejemplo, use **Vista previa**.
5. **Guarde** solo cuando esté satisfecho.

### 12.3 Campos de combinación

Las plantillas contienen **campos de combinación** (por ejemplo `«NomeInquilino»`) que se rellenan con los datos del contrato o del inquilino al generar la carta. No elimine estos campos sin necesidad.

> ⚠️ Eliminar una plantilla es permanente. Confirme siempre antes de borrar.

---

## 13. Calculadoras y mapas {#calculadoras}

### 13.1 Calculadoras

El menú **Calculadoras** incluye:

- **IMI** (`/CalculadoraIMI`) — cálculo del impuesto municipal sobre inmuebles.
- **VPT** (`/CalculadoraVPT`) — cálculo del valor patrimonial tributario.
- **Gestión de coeficientes IMI** (`/ManutencaoCoeficientesIMI`).

### 13.2 Mapas

Las páginas `/maps` y `/gmap` muestran la ubicación de los inmuebles en un mapa.

---

## 14. Administración {#administracao}

### 14.1 Ajustes

Página: **Ajustes** (`/admin/appsettings`). Permite configurar parámetros generales como el idioma predeterminado y el ajuste automático de rentas.

### 14.2 Copias de seguridad

- **Base de datos** (`/BackupDatabase`) — crea una copia de la base de datos.
- **Archivos / cargas** (`/backupuploads/1`) — crea copias de las carpetas de archivos cargados.

> 💡 Haga copias de seguridad con regularidad y guárdelas en un lugar distinto del servidor.

### 14.3 Registros

- **Registros de la aplicación** (`/admin/viewapplogs`) — errores, avisos y eventos.
- **Información de inicios de sesión** (`/admin/logininfo`) — registros de acceso a la aplicación.

### 14.4 Tablas y paneles

- **Panel de administración** (`/admin/adminpanel`) y **configuración de tablas** (`/admin/configapptables`).
- **Panel de resumen** (`/summarydashboard`).

---

## 15. Resolución de problemas {#resolucao}

| Síntoma | Causa probable | Qué hacer |
|---|---|---|
| «Acceso denegado a la ruta…» al cargar archivos | La carpeta de carga del servidor no tiene permisos de escritura | Pida al administrador que corrija los permisos en IIS |
| La rejilla no carga | API no disponible o error de conexión con la base de datos | Revise los registros (`/admin/viewapplogs`) e inténtelo de nuevo |
| El manual no se abre | Falta el archivo `docs/Help*.md` en el servidor | Compruebe que la carpeta `docs` se publicó con la aplicación |
| Los datos «desaparecieron» | Filtro activo en la rejilla o usuario diferente | Limpie los filtros y confirme con qué cuenta inició sesión |
| Los botones no responden | La página se cargó antes de que la conexión Blazor estuviera lista | Espere un segundo e inténtelo de nuevo; recargue si persiste |

---

## 16. Preguntas frecuentes {#faq}

**¿Necesito saber programar para usar la aplicación?**
No. Todo se hace mediante formularios y rejillas.

**¿Puedo usar la aplicación en otro idioma?**
Sí. Portugués, inglés, español y francés. Elija el idioma en el selector de la barra superior.

**¿Se borran mis datos al cambiar de idioma?**
No. Solo cambia la interfaz.

**¿Qué ocurre si elimino un inquilino con contratos?**
La aplicación avisa o impide la eliminación, según las reglas configuradas. Resuelva primero los contratos asociados.

**¿Cómo sé si una renta está pagada?**
Consulte el **saldo corriente del inquilino** y el **panel de pagos**.

**¿El manual se actualiza sin reiniciar la aplicación?**
Sí. El archivo se lee del disco cada vez que abre el diálogo.

---

## Apéndice A — Rutas principales {#rotas}

| Área | Ruta |
|---|---|
| Inicio | `/` |
| Inmuebles | `/imoveis`, `/properties` |
| Unidades | `/fracoes/viewunits`, `/fracoes/imagesmanager` |
| Inquilinos | `/inquilinos`, `/Inquilinos/Detalhe` |
| Arrendamientos | `/arrendamentos`, `/updaterentsletter`, `/updatecoefficients` |
| Rentas | `/monthlyrentprocessing`, `/saldocorrenteinquilino` |
| Pagos | `/recebimentos/manutencao`, `/payments/dashboard` |
| Gastos | `/despesas/manutencao`, `/despesas/tipodespesas`, `/expenses/dashboard` |
| Contactos | `/contactos` |
| Documentos | `/documentos` |
| Mensajes | `/messages-modern`, `/messages` |
| Notificaciones | `/notifications` |
| Agenda | `/scheduler` |
| Plantillas de cartas | `/LetterTemplates` |
| Calculadoras | `/CalculadoraIMI`, `/CalculadoraVPT`, `/ManutencaoCoeficientesIMI` |
| Mapas | `/maps`, `/gmap` |
| Ajustes | `/admin/appsettings` |
| Copias de seguridad | `/BackupDatabase`, `/backupuploads/1` |
| Registros | `/admin/viewapplogs`, `/admin/logininfo` |
