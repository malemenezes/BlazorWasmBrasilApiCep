Consulta de CEP com Blazor WebAssembly + BrasilAPI

Atividade prática da disciplina U, Dev Web, Mobile e Jogos (Microservice / POO), conduzida pelo professor Daniel Henrique Matos de Paiva.

O objetivo é colocar em prática o conceito de integração de sistemas: criar uma aplicação Blazor WebAssembly em C# que consome uma API pública (a BrasilAPI) e usar Inteligência Artificial como apoio para gerar a lógica de Program.cs e Home.razor.

Resultado esperado

Uma página com um campo para digitar o CEP e um botão Consultar. Ao consultar, a aplicação exibe:

CEP, logradouro, bairro, cidade (UF) e serviço utilizado
Dados do IBGE (código da cidade e do estado)

Exemplo: CEP 30130000 retorna Avenida Afonso Pena, Centro, Belo Horizonte (MG), serviço open-cep, cidade IBGE 3106200 e estado IBGE 31.

Tecnologias
.NET (SDK com suporte a blazorwasm)
C# e Blazor WebAssembly
Bootstrap (já incluso no template)
BrasilAPI (endpoint de CEP)
Visual Studio ou VS Code
Uma IA generativa (ex.: Gemini) para apoio na geração do código
Pré-requisitos
.NET SDK instalado (dotnet --version)
Visual Studio ou VS Code com a extensão C# Dev Kit
Navegador moderno
Git (para versionar o projeto)
Passo a passo
1. Criar o projeto

Pelo Visual Studio, escolha o template Blazor WebAssembly Standalone App com o nome BlazorWasmBrasilApiCep, ou use o terminal:

bash
cd Documents
dotnet new blazorwasm -n BlazorWasmBrasilApiCep
cd BlazorWasmBrasilApiCep
code .
2. Entender a API

Documentação: https://brasilapi.com.br/docs#tag/CEP

GET https://brasilapi.com.br/cep/v1/{cep}

Resposta de exemplo (200):

json
{
  "cep": "89010025",
  "state": "SC",
  "city": "Blumenau",
  "neighborhood": "Centro",
  "street": "Rua Doutor Luiz de Freitas Melro",
  "service": "viacep",
  "ibge": {
    "city": "4202404",
    "state": "42"
  }
}

A busca usa o OpenCep como fonte principal e, se não encontrar o CEP, tenta outros provedores. Se todos falharem, a API retorna 404.

3. Criar as classes de modelo

Cole o JSON de exemplo em um conversor como o JSON2CSharp para gerar as classes, e salve cada uma em um arquivo dentro da pasta Models/.

Models/Ibge.cs

csharp
namespace BlazorWasmBrasilApiCep.Models
{
    public class Ibge
    {
        public string city { get; set; }
        public string state { get; set; }
    }
}

Models/Cep.cs

csharp
namespace BlazorWasmBrasilApiCep.Models
{
    public class Cep
    {
        public string cep { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string neighborhood { get; set; }
        public string street { get; set; }
        public string service { get; set; }
        public Ibge ibge { get; set; }
    }
}
4. Usar a IA para gerar a lógica

Com o consumo de API já explicado em aula, o passo seguinte é pedir à IA o código de Program.cs e Home.razor. Quanto mais contexto no prompt, melhor o resultado. Exemplo de prompt:

text
Você é um desenvolvedor sênior em C#.
Criou uma aplicação Blazor Web Assembly.
Vai consumir essa url:

https://brasilapi.com.br/cep/v1/{cep}

Já criei as classes de modelo de resposta:
<cole aqui as classes Cep e Ibge>

Construa a lógica que será implementada na classe Program.cs.
Atualmente ela está assim:
<cole aqui o conteúdo atual do Home.razor>

Dica: passe sempre o que você já tem (modelos, URL, código atual). A IA acerta mais quando sabe o contexto do projeto.

5. Configurar o Program.cs

Registre um HttpClient apontando para a BrasilAPI (ela usa HTTPS e tem CORS liberado):

csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmBrasilApiCep;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registra o HttpClient para ser injetado nos componentes
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://brasilapi.com.br/")
});

await builder.Build().RunAsync();
6. Implementar o componente Pages/Home.razor

O componente injeta o HttpClient, consome a API usando os modelos Cep e Ibge e exibe o resultado de forma reativa. Uma implementação equivalente à gerada em aula:

razor
@page "/"
@using System.Net.Http.Json
@using BlazorWasmBrasilApiCep.Models
@inject HttpClient Http

<PageTitle>Consulta CEP - BrasilAPI</PageTitle>

<h1>Consulta de CEP com Blazor WebAssembly</h1>

<div class="row mb-3">
    <div class="col-md-4">
        <label for="cepInput" class="form-label">Digite o CEP:</label>
        <input id="cepInput" class="form-control" @bind="cepDigitado" placeholder="Ex: 01001000" />
    </div>
    <div class="col-md-2 d-flex align-items-end">
        <button class="btn btn-primary w-100" @onclick="ConsultarCep" disabled="@carregando">
            @(carregando ? "Buscando..." : "Consultar")
        </button>
    </div>
</div>

@if (!string.IsNullOrEmpty(mensagemErro))
{
    <div class="alert alert-danger">@mensagemErro</div>
}

@if (resultado is not null)
{
    <div class="card">
        <div class="card-body">
            <h4 class="text-success">Endereço Encontrado</h4>
            <p><strong>CEP:</strong> @resultado.cep</p>
            <p><strong>Logradouro:</strong> @resultado.street</p>
            <p><strong>Bairro:</strong> @resultado.neighborhood</p>
            <p><strong>Cidade:</strong> @resultado.city (@resultado.state)</p>
            <p><strong>Serviço:</strong> @resultado.service</p>

            @if (resultado.ibge is not null)
            {
                <hr />
                <h5 class="text-secondary">Dados do IBGE</h5>
                <p><strong>Cidade IBGE:</strong> @resultado.ibge.city</p>
                <p><strong>Estado IBGE:</strong> @resultado.ibge.state</p>
            }
        </div>
    </div>
}

@code {
    private string cepDigitado = string.Empty;
    private Cep? resultado;
    private string? mensagemErro;
    private bool carregando;

    private async Task ConsultarCep()
    {
        resultado = null;
        mensagemErro = null;

        var cepLimpo = new string(cepDigitado.Where(char.IsDigit).ToArray());

        if (cepLimpo.Length != 8)
        {
            mensagemErro = "Informe um CEP válido com 8 dígitos.";
            return;
        }

        carregando = true;
        try
        {
            resultado = await Http.GetFromJsonAsync<Cep>($"cep/v1/{cepLimpo}");
        }
        catch (HttpRequestException)
        {
            mensagemErro = "CEP não encontrado ou serviço indisponível.";
        }
        finally
        {
            carregando = false;
        }
    }
}

Observação: a IA pode gerar um código ligeiramente diferente. Isso é esperado. O importante é revisar, entender e ajustar o que ela devolve.

7. Conferir erros e avisos na IDE

Depois de ajustar o código, abra o painel Problems do VS Code. No nosso caso aparecem apenas avisos (CS8618), por causa das propriedades não anuláveis nos modelos:

Non-nullable property 'city' must contain a non-null value when exiting constructor.
Consider adding the 'required' modifier or declaring the property as nullable. (CS8618)

Eles não impedem a execução. Para removê-los, escolha uma das opções:

csharp
// Opção 1: modificador required
public required string city { get; set; }

// Opção 2: valor padrão
public string city { get; set; } = string.Empty;

// Opção 3: propriedade anulável
public string? city { get; set; }
8. Executar a aplicação
bash
dotnet run

Acesse a URL exibida no terminal (neste exemplo, http://localhost:5062), digite um CEP, como 30130000, e clique em Consultar. Para parar a aplicação, use Ctrl + C.

9. Limpar e subir para o GitHub
Pare a aplicação (Ctrl + C).
Apague as pastas bin e obj.
Crie um novo repositório chamado blazor-brasilapi-cep com o .gitignore do Visual Studio.
Envie o projeto:
bash
git init
git add .
git commit -m "Consulta de CEP com Blazor WebAssembly e BrasilAPI"
git branch -M main
git remote add origin https://github.com/<seu-usuario>/blazor-brasilapi-cep.git
git push -u origin main
Estrutura do projeto
BlazorWasmBrasilApiCep/
├── Layout/
├── Models/
│   ├── Cep.cs
│   └── Ibge.cs
├── Pages/
│   └── Home.razor
├── Properties/
├── wwwroot/
├── _Imports.razor
├── App.razor
├── BlazorWasmBrasilApiCep.csproj
└── Program.cs
