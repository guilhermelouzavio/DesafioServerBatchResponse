**Como Testar**

**RABBIT MQ**
baixar docker Desktop e rodar o comando abaixo:
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4.0-management
ou instalar rabbitMq na maquina.

**BATCH**
Startar o batch, no intervalo curto ele busca novas mensagens na Fila de Envio e Responde na fila de resposta.

**SERVER**
Subir o server e realizar as chamadas pelo endpoint /messages.

