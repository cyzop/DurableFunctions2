# Azure Durable Functions e Azure Pipelines
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

# Sobre o Projeto
O sistema foi desenvolvido em .NET Core 8, correspondente à FASE 2 do curso ARQUITETURA DE SISTEMAS .NET COM AZURE da FIAP.

Neste sistema foi criada uma Azure Functions, do tipo Durable Functions, para simular o processo de aprovação de um pedido. 
O start para esta durable functiona é do tipo HTTP e o processo de aprovação de pedido (order), quando válido, irá postar no Azure Service Bus que será consumida pela aplicação atualizando pedido no repositório.

Para gerar um pedido, foi criada uma WebApi onde é possível fazer um POST de uma pedido (order). Este pedido é armazenado no repositório com um status de "requested" e é acionado o gatilho de disparo da Azure Function.
Após o processo de validação/aprovação da Function, ao enviar uma mensagem pelo Azure Service Bus, um background process recebe a mensagem e atualiza o pedido.

# 📋 Tecnologias utilizadas

- Microsoft Azure
- Microsoft .Net Core 8 WebApi (Back-end)
- Azure Functions (Durable)
- MongoDB (Clould)

# 🔧 Como executar o projeto

Para executar o projeto, após baixar o código do Git e abrir a solution no Visual Studio (utilizado Community 2022).

No Visual Studio, selecione a inicialização de vários projetos e marque a ação de iniciar para:
- HttpDurableFuntion
- OrderApi

## Azure Functions (projeto HttpDurableFunction)

- Configurar uma variável de ambiente com nome SERVICEBUSCONNECTION contendo a string de conexão com o Azure Service Bus
- Configurar uma variável de ambiente com nome APPROVEDQUEUE contendo o nome da fila do Azure Service Bus a ser utilizada (será a mesma fila a ser configurada na WebApi)

## WebApi (projeto OrderApi na pasta Presenter)

- Configurar uma variável de ambiente com nome APPROVEAZFUNCTION contendo a url para o endpoint (gatilho) da Azure Function
- Configurar uma variável de ambiente com nome SERVICEBUSCONNECTION contendo a string de conexão com o Azure Service Bus
- Configurar uma variável de ambiente com nome APPROVEDQUEUE contendo o nome da fila do Azure Service Bus a ser utilizada (será a mesma fila a ser configurada na WebApi)
- Configurar uma varíavel de ambiente com nome MONGOCONNECTIONSTRING contendo a string de conexao para o banco de dados mongo (pode ser utilizado local ou em núvem)


