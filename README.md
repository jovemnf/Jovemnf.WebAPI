# Jovemnf.WebAPI 💎

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Uma biblioteca .NET moderna, elegante e de alta performance para consumo de APIs HTTP. Inspirada na simplicidade, mas construída com o poder e a tipagem do **.NET 9**.

## ✨ Destaques

- 🚀 **Arquitetura Facade**: Interface simplificada que esconde uma engenharia robusta.
- 🏗️ **Padrão Builder**: Construção de requisições modular e segura.
- 🛡️ **Tipagem Forte**: Suporte total a Generics e Nullable Reference Types.
- 🧩 **Zero Config**: Funciona "out of the box", mas é totalmente extensível.
- 🚦 **HttpClient Optimized**: Gerenciamento inteligente de instâncias do HttpClient.

---

## 🚀 Como Usar: API Estática (Estilo Axios)

A forma mais rápida e moderna de consumir APIs. Sem instanciamento, sem complicações. Todos os métodos estáticos retornam `WebApiResponse<T>` (veja [Tipo de retorno](#-tipo-de-retorno-webapiresponse) abaixo).

### GET Simples
```csharp
var response = await Api.Get<Post>("https://api.example.com/posts/1");
Console.WriteLine(response.Content?.Title);
// response.StatusCode, response.Exception também disponíveis
```

### POST com JSON
```csharp
var data = new { title = "Meu Post", body = "Conteúdo legal", userId = 1 };
var result = await Api.Post<Post>("https://api.example.com/posts", data);
var post = result.Content;
```

### Outros Verbos (PUT, PATCH, DELETE)
```csharp
// Atualização parcial
var patchResponse = await Api.Patch("https://api.example.com/posts/1", new { title = "Novo Título" });

// Exclusão
var deleteResponse = await Api.Delete<bool>("https://api.example.com/posts/1");
```

---

## 🛠️ Modo Avançado (Fluente)

Para quando você precisa de controle granular sobre headers, timeouts e instâncias.

```csharp
// Timeout em milissegundos (ex.: 15 segundos)
using var api = new Api("https://api.example.com", 15000)
    .WithHeaders(new Dictionary<string, string> { 
        { "Authorization", "Bearer my-token" },
        { "X-Custom-Header", "Value" }
    });

api.SetJson(new { filter = "active" });
var response = await api.Send<List<Data>>();
// response.Content, response.StatusCode, response.Exception
```

---

## 🔐 Autenticação

### Basic Auth
```csharp
using var api = new Api("https://api.exemplo.com")
    .WithBasicAuth("usuario", "senha");

var dados = await api.Send<Modelo>();
```

### Certificados e mTLS
Para chamadas que exigem autenticação mTLS (Certificado Digital), você pode usar o método fluente `.WithCertificate()`.

```csharp
using var cert = new X509Certificate2("path/to/certificate.pfx", "password");

using var api = new Api("https://api.secure.com")
    .WithCertificate(cert);

var response = await api.Send<SecureData>();
```

Você também pode injetar seu próprio `HttpClient` se desejar:
```csharp
using var api = new Api("https://api.example.com")
    .WithHttpClient(myCustomHttpClient);
```

---

## 📦 Tipo de retorno WebApiResponse

Todas as chamadas (estáticas e fluentes) retornam `WebApiResponse<T>`:

| Propriedade   | Descrição |
|---------------|-----------|
| `Content`     | Corpo deserializado (tipo `T`) ou `null` em caso de erro/timeout. |
| `StatusCode`  | Código HTTP da resposta (ex.: `HttpStatusCode.OK`). |
| `Exception`   | Preenchido em timeout ou quando a resposta não foi sucesso (ex.: `TimeoutException`). |

Em respostas de sucesso (2xx), use `response.Content`. Em falhas, a biblioteca lança exceções semânticas (veja abaixo); em timeout, o resultado vem em `response.Exception` sem lançar.

---

## 🛰️ Tratamento de Erros Inteligente

O `Jovemnf.WebApi` elimina a necessidade de verificar códigos de status manualmente o tempo todo. Ele mapeia automaticamente **todos** os códigos 4xx e 5xx para exceções específicas e semânticas.

**Exemplos de Exceções Inclusas (> 40 tipos):**
- `BadRequestException` (400)
- `UnauthorizedAccessException` (401)
- `ForbiddenException` (403)
- `NotFoundException` (404)
- `ConflictException` (409)
- `UnprocessableEntityException` (422)
- `InternalServerError` (500)
- `BadGatewayException` (502)
- `ProxyProhibited` (quando a requisição é bloqueada por proxy, via `WebException`)

Para converter um `HttpResponseMessage` ou `WebException` em exceção semântica sem fazer a requisição, use `Api.CheckException(response)` ou `Api.CheckException(webException)`.

```csharp
try 
{
    var response = await Api.Get<User>(url);
    var user = response.Content;
}
catch (NotFoundException)
{
    // Lógica para usuário não encontrado
}
catch (UnauthorizedAccessException) 
{
    // Lógica para erro de login
}
```

---

## 📦 Instalação e Requisitos

- **Runtime**: .NET 9.0+
- **Dependências**: 
  - `Newtonsoft.Json`: Para alta flexibilidade na serialização.

```bash
dotnet add package Jovemnf.WebAPI
```

### Rodar os testes

Na raiz do repositório:

```bash
dotnet test
```

---

## 🏛️ Design Patterns Aplicados

Esta biblioteca foi refatorada seguindo princípios **SOLID** e padrões de projeto para garantir manutenibilidade:
- **Facade Pattern**: Classe principal `Api`.
- **Builder Pattern**: Internamente via `WebRequestBuilder`.
- **Strategy Pattern**: Validação de respostas e lançamento de exceções.
- **Engine Pattern**: Processamento centralizado via `WebApiEngine`.

---

© 2026 Wallace Silva - Desenvolvido com ❤️ e foco em qualidade.
