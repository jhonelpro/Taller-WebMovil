# Taller-WebMovil


Antes de ejecutar el programa se debe crear un archivo en la raiz del proyecto y llamarlo ".env".
En este archivo se debe pegar estas contrasenas:

DATA_BASE_URL = Data Source=database.db
CLOUDINARY_NAME = dd0ljsgtp
CLOUDINARY_API_KEY = 348851711395649
CLOUDINARY_API_SECRET = 7Sfr7qrZ2_1pVDLkqnnLONYzgbY
JWT_ISSUER = http://localhost:5000
JWT_AUDIENCE = http://localhost:500
JWT_SIGNING_KEY = EstaEsUnaClaveDeFirmaSuficientementeLargaQueSuperaLos64Bytes1234567890


Cuanto se ejecute el programa se creara la base de datos, pero no se podra ejecutar.
Para solucionar esto actualiza la base de datos con el comando "dotnet ef database update"
