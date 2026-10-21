# BlazorWasmBrasilApiCep

Aplicação Blazor WebAssembly (.NET 10) que consulta CEPs na [BrasilAPI](https://brasilapi.com.br/docs#tag/CEP).

Digite um CEP (com ou sem hífen), clique em **Consultar** ou aperte Enter, e a página mostra o endereço
retornado pelo endpoint `https://brasilapi.com.br/api/cep/v1/{cep}`:

```
CEP: 30130000
Logradouro: Avenida Afonso Pena
Bairro: Centro
Cidade: Belo Horizonte (MG)
Serviço: open-cep

Cidade IBGE: 3106200
Estado IBGE: 31
```

Se o CEP não existir, a API responde 404 e a página mostra "CEP não encontrado".

## Como executar

```bash
dotnet run
```

Depois é só abrir o endereço que aparece no terminal.

## Estrutura

- `Models/Cep.cs` e `Models/Ibge.cs`: classes que mapeiam o JSON da API
- `Pages/Home.razor`: formulário de consulta, chamada à API e exibição do endereço
- `Program.cs`: registro do `HttpClient` com o endereço base da BrasilAPI
