# GPT Integrated test - Companion API

Projeto desenvolvido para testar integração e criação de assistentes integrados com AI.
Constituído de uma arquitetura MVC simples.

Este repositório contém o código do projeto Companion utilizando .NET 8. Siga as instruções abaixo para configurar e executar o projeto.

## Pré-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou superior (opcional)

## Instalação

1. Clone este repositório:
2. Acesse o diretório do projeto:
3. Restaure os pacotes NuGet:
## Configuração do Firebase

Este projeto utiliza o Firebase para autenticação ou outros serviços. Para configurar corretamente, é necessário adicionar uma chave de conta de serviço ao projeto.

1. No diretório `Properties`, você encontrará o arquivo `serviceAccountKey.dev.json`. Este é um arquivo de exemplo para desenvolvimento. Renomeie este arquivo para `serviceAccountKey.json`:
2. **Opcional**: Se você possui sua própria chave de conta de serviço do Firebase, adicione-a ao diretório `Properties` com o nome `serviceAccountKey.json`.

   - Você pode obter uma nova chave de conta de serviço acessando o console do Firebase:
     - Vá para **Configurações do Projeto** > **Contas de Serviço**.
     - Clique em **Gerar nova chave privada** e salve o arquivo JSON no diretório `Properties` com o nome `serviceAccountKey.json`.

## Configuração do appsettings

Para configurar o arquivo `appsettings.Development.example.json`, siga os passos abaixo:

1. No diretório `CompanionAPI`, você encontrará o arquivo `appsettings.Development.example.json`. Renomeie este arquivo para `appsettings.Development.json`:
2. **Opcional**: Se você possui suas próprias configurações, edite o arquivo `appsettings.Development.json` conforme necessário.

## Executando o Projeto

Para compilar e executar o projeto, use os comandos abaixo:
