# Projeto XYZ - .NET 8

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

## Configuração

1. No diretório `Properties`, você encontrará o arquivo `serviceAccountKey.dev.json`. Renomeie este arquivo para `serviceAccountKey.json`:

    ```bash
    mv Properties/serviceAccountKey.dev.json Properties/serviceAccountKey.json
    ```

2. **Opcional**: Se você possui sua própria credencial de conta de serviço, adicione-a ao diretório `Properties` com o nome `serviceAccountKey.json`.

## Executando o Projeto

Para compilar e executar o projeto, use os comandos abaixo:

```bash
dotnet build
dotnet run
