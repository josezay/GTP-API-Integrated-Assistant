# Companion API

Este repositório contém o código do projeto Companion utilizando .NET 8. Siga as instruções abaixo para configurar e executar o projeto.

## Pré-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou superior (opcional)

## Instalação

1. Clone este repositório:

    ```bash
    git clone https://github.com/usuario/projeto-xyz.git
    ```

2. Acesse o diretório do projeto:

    ```bash
    cd projeto-xyz
    ```

3. Restaure os pacotes NuGet:

    ```bash
    dotnet restore
    ```

## Configuração do Firebase

Este projeto utiliza o Firebase para autenticação ou outros serviços. Para configurar corretamente, é necessário adicionar uma chave de conta de serviço ao projeto.

1. No diretório `Properties`, você encontrará o arquivo `serviceAccountKey.dev.json`. Este é um arquivo de exemplo para desenvolvimento. Renomeie este arquivo para `serviceAccountKey.json`:

    ```bash
    mv Properties/serviceAccountKey.dev.json Properties/serviceAccountKey.json
    ```

2. **Opcional**: Se você possui sua própria chave de conta de serviço do Firebase, adicione-a ao diretório `Properties` com o nome `serviceAccountKey.json`.

   - Você pode obter uma nova chave de conta de serviço acessando o console do Firebase:
     - Vá para **Configurações do Projeto** > **Contas de Serviço**.
     - Clique em **Gerar nova chave privada** e salve o arquivo JSON no diretório `Properties` com o nome `serviceAccountKey.json`.

## Executando o Projeto

Para compilar e executar o projeto, use os comandos abaixo:

```bash
dotnet build
dotnet run
