# Taller Web Movil

El objetivo de este documento es proporcionar una guía clara y precisa para desarrolladores que deseen configurar rápidamente un entorno de trabajo para levantar este proyecto desde GitHub. A lo largo de este tutorial, se cubrirán los pasos esenciales, desde la clonación del repositorio hasta la configuración del entorno y la ejecución del proyecto en Visual Studio.

## Clonar el repositorio

Para clonar un repositorio debes acceder a la direccion de github donde esta alojado el repositorio. 

<img src="https://drive.google.com/uc?export=view&id=11EZ8iK_pIHX_Sx66Ju_mUNHQaVL4hLyF" alt="Repositorio GitHub" width="850"/>

Apretar en el boton verde que dice codigo, y copiar el enlace https.

<img src="https://drive.google.com/uc?export=view&id=1FSJeRAGrD8goeLLIzMbxLQgEIOlmbmBR" alt="Repositorio/Codigo GitHub" width="450"/>

Ahora, es necesario crear una carpeta en cualquier direccion que deseas almacenar el proyecto.

Despues, haz click derecho sobre la carpeta  y selecciona abrir en terminal.

<img src="https://drive.google.com/uc?export=view&id=1svstl1CkoRq30wl20-3LR4lxb5cB9NXh" alt="Abrir con terminal" width="450"/>

Para clonar el repositorio en la carpeta que creaste, en la terminal ingresa el comando "git clone" seguido de un espacio y la direccion del repositorio (La cual copiaste anteriormente). 

````
git clone https://github.com/jhonelpro/Taller-WebMovil.git
````

> [!NOTE]
> Presiona enter, y espera a que termine de clonar el repositorio.

Luego, ingresa el comando "code ." para abrir Visual Studio Code.

````
code .
````

> [!NOTE]
> Presiona enter, y espera a que abra la aplicacion de vscode.

## Instalar dependencias

Una vez que ya abrio vscode, verifica si no te creo un archivo llamado "Taller-WebMovil.sln", si es asi debes eliminar este archivo para evitar errores posteriormente.

> [!CAUTION]
> Eliminar archivo "Taller-WebMovil.sln".

Abrir una terminal en vscode, presionando las teclas "Ctrl + Shift + `", o tambien puedes ir a la parte superior de vscode, seleccionar los 3 puntitos "...", luego "Terminal", y "New Terminal".

<img src="https://drive.google.com/uc?export=view&id=1vlOWIl9QsAydtAeePWJHbUGt18aAzoES" alt="Abrir terminal en vscode" width="550"/>

Ahora en la terminal accede a la raiz del proyecto con el siguiente comando:

````
cd Taller-WebMovil
````

Verifica si esta en la rama main del proyecto, a travez del siguiente codigo en la terminal.

````
git checkout main
````

Si no estuvieras en la rama main del proyecto, el codigo anterior te llevara a la rama.


Para restaurar las dependencias del proyecto, ingresa el siguiente comando en la terminal:

````
dotnet restore
````

## Configurar las variables de entorno

Crear un archivo llamado ".env" en la raiz del proyecto.

<img src="https://drive.google.com/uc?export=view&id=1-R4qGHr7wm1etIBZALhPO5Q5g5rod4C8" alt="Creacion de archivo .env" width="500"/>

Copiar el formulario del archivo ".env.example" y pegarlo en el archivo ".env" que se creo.

<img src="https://drive.google.com/uc?export=view&id=1iAlt4F9JHseLHWQegQR5E6c-jNDseAiW" alt="Archivo .env.example" width="550"/>

Ahora, rellenar el formulario con las claves correspondientes en el archivo ".env".

<img src="https://drive.google.com/uc?export=view&id=1Z6Hl_ieyn7Y5S3mwaK2RbebDaqwmJXoN" alt="Archivo .env" width="700"/>

> [!IMPORTANT]
> Guarda los cambios con el comando "Ctrl+s".

## Configurar la base de datos

En la terminal ejecuta el siguiente codigo para realizar las migraciones pertinentes a la base de datos.

````
dotnet ef database update
````

<img src="https://drive.google.com/uc?export=view&id=1KiWQFRrB5AWmY26fTx2pB5YJ8h9dtCSw" alt="Archivo .env" width="300"/>

> [!NOTE]
> Se te creara la base de datos.

## Ejecutar la aplicacion

Finalmente para ejecutar la aplicacion, debes ingresar el siguiente comando en la terminal:

````
dotnet run
````
