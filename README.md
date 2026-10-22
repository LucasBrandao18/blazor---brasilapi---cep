# BlazorWasmBrasilApiCep

Prática em C# com **Blazor WebAssembly e .NET 10**, baseada no material `net_blazor_vscode.pdf`, do professor Daniel Henrique Matos de Paiva. A aplicação consulta um CEP na BrasilAPI e apresenta o endereço e os dados do IBGE retornados pelo serviço.

## O que o material solicita

O PDF apresenta uma prática guiada, com estas etapas:

1. Criar um aplicativo Blazor WebAssembly chamado `BlazorWasmBrasilApiCep` e abrir a pasta no VS Code (páginas 11–13).
2. Analisar a documentação da BrasilAPI e a resposta JSON da consulta de CEP (páginas 14–17).
3. Criar os modelos `Cep` e `Ibge` na pasta `Models` (páginas 18–20).
4. Configurar o `HttpClient` em `Program.cs` e implementar a consulta em `Pages/Home.razor` (páginas 21–26).
5. Verificar a compilação, executar com `dotnet run` e testar a consulta, exibindo CEP, logradouro, bairro, cidade/UF, serviço e códigos de cidade e estado do IBGE (páginas 27–32).
6. Encerrar a aplicação e preparar o código para publicação com `.gitignore` (página 33).

Todas as etapas locais foram implementadas. A criação e publicação do repositório no GitHub e a entrega no Google Classroom ficam a cargo do aluno, conforme solicitado. O PDF sugere o nome de repositório `blazor-brasilapi-cep`.

## Como executar

Requisitos: SDK do **.NET 10**, navegador atualizado e acesso à internet para restaurar os pacotes e consultar a BrasilAPI. Não é necessária chave de API ou banco de dados.

No PowerShell:

```powershell
cd "C:\Users\lucas\OneDrive\Documentos\vs code uni\pratica_BlazorWasmBrasilApiCep"
dotnet restore
dotnet run
```

Abra [http://localhost:5274](http://localhost:5274), digite um CEP e clique em **Consultar**, ou pressione **Enter** dentro do campo. O endereço local está configurado em `Properties/launchSettings.json`.

Exemplos de entrada: `30130000`, `30130-000` ou `01001000`. Os zeros à esquerda são preservados porque o CEP é tratado como texto.

Para encerrar o servidor, pressione `Ctrl+C` no terminal. Para executar novamente, use `dotnet run`.

No VS Code, abra esta pasta raiz e escolha **Terminal > Executar Tarefa > executar BlazorWasmBrasilApiCep**. A pasta `.vscode` inclui essa tarefa, uma tarefa de compilação e recomendações das extensões C# e C# Dev Kit.

Se a porta estiver ocupada, encerre a execução anterior ou use:

```powershell
dotnet run --urls http://localhost:5275
```

Nesse caso, abra a porta 5275 no navegador.

## Funcionamento do código

### Cliente HTTP

`Program.cs` inicializa o Blazor e registra o `HttpClient` na injeção de dependências, usando `https://brasilapi.com.br/` como endereço base e um tempo limite de 15 segundos. O componente recebe esse cliente com `@inject HttpClient Http`.

O endpoint utilizado é:

```text
GET https://brasilapi.com.br/api/cep/v1/{cep}
```

Algumas imagens e o exemplo de prompt do PDF apresentam a URL sem `/api`. O projeto utiliza o caminho acima, validado com uma requisição real. Não há endereços ou códigos do IBGE fixados no aplicativo.

### Modelos e JSON

`Models/Cep.cs` representa a resposta completa necessária à tela. Os atributos `JsonPropertyName` associam os campos JSON às propriedades C#:

| JSON | Propriedade C# | Informação |
| --- | --- | --- |
| `cep` | `Codigo` | CEP consultado |
| `state` | `Estado` | Sigla da UF |
| `city` | `Cidade` | Município |
| `neighborhood` | `Bairro` | Bairro |
| `street` | `Logradouro` | Rua, avenida ou outro logradouro |
| `service` | `Servico` | Provedor utilizado pela BrasilAPI |
| `ibge` | `Ibge` | Objeto com códigos do IBGE |

`Models/Ibge.cs` representa os campos `city` e `state` do objeto `ibge`. Eles permanecem como texto, conforme o JSON do material. As propriedades podem ser nulas para tratar respostas que omitam campos e evitar os avisos de inicialização mostrados nos slides.

### Consulta e exibição

`Pages/Home.razor` contém o formulário, a apresentação do resultado e a lógica assíncrona:

1. O vínculo `@bind` com evento `oninput` atualiza o CEP conforme o usuário digita.
2. Ao enviar o formulário, `ConsultarCep` limpa o resultado anterior e valida os formatos `00000000` e `00000-000`, desconsiderando espaços nas extremidades.
3. A função remove apenas o hífen do formato válido. Letras, números em excesso e entradas incompletas são rejeitados antes de qualquer requisição.
4. `GetAsync` faz a consulta HTTP e verifica o código de resposta.
5. `ReadFromJsonAsync<Cep>` desserializa o JSON, incluindo o objeto aninhado do IBGE.
6. O resultado é validado e apresentado no cartão **Endereço encontrado**. A tela mostra os valores recebidos da API.

A primeira abertura da página não consulta um CEP automaticamente: o usuário informa o CEP e envia o formulário, como nas imagens da aula. O campo e o botão ficam desabilitados enquanto a consulta está em andamento, evitando consultas simultâneas. Uma requisição pendente é cancelada quando o componente é encerrado.

### Tratamento de situações de erro

- CEP com formato inválido: mensagem de validação, sem chamar a API.
- HTTP 404: CEP não encontrado.
- HTTP 400: CEP recusado pela API.
- Outros erros HTTP, incluindo indisponibilidade: mensagem para tentar novamente mais tarde.
- Falha de conexão ou tempo limite: mensagem correspondente e liberação de nova tentativa.
- JSON inválido, resposta nula, CEP divergente ou cidade/UF ausentes: o resultado não é apresentado como um endereço válido.
- Bairro, logradouro, serviço ou dados do IBGE ausentes: o campo mostra **Não informado pela API**.

Os dados do IBGE são exibidos quando a resposta os contém. O projeto não inventa códigos nem faz consultas adicionais a outros serviços para preencher campos ausentes.

## Organização

```text
BlazorWasmBrasilApiCep.csproj   Configuração e dependências do aplicativo
Program.cs                    Inicialização e cliente HTTP
Models/Cep.cs                 Modelo do endereço
Models/Ibge.cs                Modelo dos códigos do IBGE
Pages/Home.razor               Formulário, consulta e resultado
Pages/Home.razor.css           Estilos específicos da consulta
Layout/                       Menu e estrutura visual
App.razor                     Roteamento
_Imports.razor                Importações comuns dos componentes
Properties/launchSettings.json Configuração de execução local
wwwroot/                      HTML, Bootstrap e arquivos estáticos
.vscode/                      Tarefas e sugestões de extensões
tests/                        Testes automatizados
.gitignore                    Exclusão de arquivos gerados
```

As páginas demonstrativas Counter e Weather foram removidas para manter o projeto concentrado na prática de CEP. O layout mantém a estrutura padrão do Blazor, com textos em português. O C# roda no navegador via WebAssembly; `dotnet run` inicia o servidor local que entrega os arquivos do aplicativo.

## Compilação e testes

Na raiz:

```powershell
dotnet build
dotnet test "tests\BlazorWasmBrasilApiCep.Tests\BlazorWasmBrasilApiCep.Tests.csproj"
```

Há **18 testes automatizados com bUnit e xUnit**, abrangendo CEP com e sem hífen, preservação do zero inicial, entradas inválidas, desserialização dos dados do IBGE, campos opcionais ausentes, erros HTTP/rede, tempo limite, JSON inválido, nova tentativa e bloqueio de consultas duplicadas.

Os testes usam respostas HTTP simuladas, sem depender da disponibilidade da BrasilAPI. A pasta `tests` é excluída dos arquivos do aplicativo pelo `.csproj` principal. A consulta real de `30130-000`, a validação de `123` e a resposta de CEP não encontrado para `00000000` também foram verificadas no navegador.

Na verificação real, `30130-000` retornou Avenida Afonso Pena, Centro, Belo Horizonte/MG, serviço `open-cep`, cidade IBGE `3106200` e estado IBGE `31`. Esses valores são apenas o registro da verificação e podem mudar conforme o serviço.

## Preparação para entrega manual

O `.gitignore` inclui `bin/`, `obj/` e outros arquivos gerados. Essas pastas são recriadas pelo .NET ao compilar e executar; não precisam ser enviadas ao GitHub. Se quiser removê-las antes da entrega, encerre a aplicação e remova apenas essas pastas geradas, incluindo as do projeto de testes. Preserve o código-fonte e os arquivos `.csproj`.

Nenhum repositório remoto foi criado e nenhum arquivo foi enviado ao GitHub ou ao Classroom.

## Referências

- Material fornecido: `net_blazor_vscode.pdf`.
- [Documentação da BrasilAPI](https://brasilapi.com.br/docs#tag/CEP).
- [Consulta utilizada na validação](https://brasilapi.com.br/api/cep/v1/30130000).
- [Documentação do Blazor](https://learn.microsoft.com/aspnet/core/blazor/).
