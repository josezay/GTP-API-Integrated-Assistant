# Companion API

Este reposit�rio cont�m o c�digo do projeto Companion utilizando .NET 8. Siga as instru��es abaixo para configurar e executar o projeto.

## Pr�-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou superior (opcional)

## Instala��o

1. Clone este reposit�rio:
2. Acesse o diret�rio do projeto:
3. Restaure os pacotes NuGet:
## Configura��o do Firebase

Este projeto utiliza o Firebase para autentica��o ou outros servi�os. Para configurar corretamente, � necess�rio adicionar uma chave de conta de servi�o ao projeto.

1. No diret�rio `Properties`, voc� encontrar� o arquivo `serviceAccountKey.dev.json`. Este � um arquivo de exemplo para desenvolvimento. Renomeie este arquivo para `serviceAccountKey.json`:
2. **Opcional**: Se voc� possui sua pr�pria chave de conta de servi�o do Firebase, adicione-a ao diret�rio `Properties` com o nome `serviceAccountKey.json`.

   - Voc� pode obter uma nova chave de conta de servi�o acessando o console do Firebase:
     - V� para **Configura��es do Projeto** > **Contas de Servi�o**.
     - Clique em **Gerar nova chave privada** e salve o arquivo JSON no diret�rio `Properties` com o nome `serviceAccountKey.json`.

## Configura��o do appsettings

Para configurar o arquivo `appsettings.Development.example.json`, siga os passos abaixo:

1. No diret�rio `CompanionAPI`, voc� encontrar� o arquivo `appsettings.Development.example.json`. Renomeie este arquivo para `appsettings.Development.json`:
2. **Opcional**: Se voc� possui suas pr�prias configura��es, edite o arquivo `appsettings.Development.json` conforme necess�rio.

## Executando o Projeto

Para compilar e executar o projeto, use os comandos abaixo:
