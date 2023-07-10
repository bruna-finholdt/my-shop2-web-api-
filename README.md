# Introdução
Projeto MiniShop utilizando ASP.NET 7

# Pré requisistos

1. Docker;
2. DotNet 7;
3. SQL Server Management Studio (MSSQL) ou SQL Developer (Oracle), DBeaver e outros navegadores de banco de dados são suportados também;
4. Visual Studio 2022;

# Fluxo de trabalho

1. Você não poderá realizar alterações na branch `main` somente através de [Pull Requests](https://learn.microsoft.com/azure/devops/repos/git/about-pull-requests);

2. Você deverá criar branchs respeitando as nomenclaturas do GitFlow:

    - `feat/` ou `feature/`: Para implementações novas;
    - `bugfix/` ou `hotfix/`: Para correções de implementações já presentes na branch `main`;

3. Assim que sua implementação for concluída você deverá realizar o push e solicitar a entrega através de um Pull Request;

    - O [pull_request_template.md](.azuredevops/pull_request_template.md) é o seu template de Pull Requests, ele cria uma descrição orientando você e seus colegas de time como devem trabalhar;
    - O Pull Request é uma documentação do trabalho realizado e revisado pelo time portanto é muito importante que todos os membros do time o **REVISEM**.

4. A entrega final deve estar contida na branch `main`.

## Docker

As configurações para utilizar MSSQL ou Oracle estão no arquivo [docker-compose.yml](docker-compose.yml) que está na raíz do projeto, por padrão o BD escolhido é MSSQL para utilizar Oracle siga as instruções no arquivo.

Passos para executar o docker:

1. No nivel do arquivo `docker-compose.yml`, digitar:

    - No Windows:

    <br/>

    ```bash
    wsl docker compose up -d
    ```

    - Linux (Inclusive WSL)/Mac OS:

    <br/>

    ```bash
    docker compose up -d
    ```

2. Após isso precisamos criar nosso banco de dados, podemos fazer isso direto pela linha de comando do docker:

    - No Windows com SQL Server:

    <br />

    ```bash
    wsl docker compose exec -it <nome_service_compose> /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Cpc33UBI' -Q 'CREATE DATABASE minishop'
    ```

    - Linux/Mac OS com SQL Server:

    <br />

    ```bash
    docker compose exec -it <nome_service_compose> /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'Cpc33UBI' -Q 'CREATE DATABASE minishop'
    ```

> **Considerações:** Algumas considerações sobre docker com banco de dados:
>
> - Informações de acesso ao banco de dados no arquivo [appsettings.Development.json](Minishop/appsettings.Development.json)
> - Uma vez executado o comando `docker compose up -d` nas próximas vezes que precisar iniciar ou parar os contâiners de banco utilize os comandos `docker compose start` ou `docker compose stop`;
> - A VPN deve estar desconectada e os serviços de SQL instalados na máquina desativados para que a conexão aconteça com sucesso;

## Migrations

Para versionar nosso banco de dados estamos utilizando migrations através do [EFCore](https://learn.microsoft.com/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli).

> **Importante:** O projeto não possui nenhuma migration criada, porém é obrigatório utilizá-las, para isso você pode decidir por dois caminhos:
>
> - **DB First + Code First:** Esse caminho consitirá em que criar um banco de dados com os scripts fornecidos, gerar as entidades a partir do comando `scaffold` do EFCore e por fim criar migrations com um banco de dados vazio.
> - **Code First Executando Scripts:** Esse caminho consitirá em adicionar os scripts fornecidos como arquivos do projeto (que devem, inclusive, ser [copiados para a pasta de build](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#copytopublishdirectory)) criar uma migration vazia onde o conteúdo SQL dos arquivos seja lido e executado através de `migrationBuilder.Sql()`.

## Testes

Para os Testes podemos utilizar o banco de dados em memória ou adiciona o pacote `Moq` no projeto de testes para mockar as diversas camadas.

> **Atenção:** Configuramos a geração do relatório de cobertura utilizando o arquivo `coverlat.runsettings`, assim a pipeline poderá validar os dados de cobertura.

## Documentação Api

A documentação da api esta disponivel no swagger.

## HealthCheck

Você pode verificar conexão com o banco de dados através do endpoint: <http://localhost:53571/health>

> **Documentação:** Leia mais sobre [aqui](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0).

