# Jovemnf.WebAPI 💎

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Uma biblioteca .NET moderna, elegante e de alta performance para consumo de APIs HTTP. Inspirada na simplicidade do **Axios**, mas construída com o poder e a tipagem do **.NET 9**.

## ✨ Destaques

- 🚀 **Arquitetura Facade**: Interface simplificada que esconde uma engenharia robusta.
- 🏗️ **Padrão Builder**: Construção de requisições modular e segura.
- 🛡️ **Tipagem Forte**: Suporte total a Generics e Nullable Reference Types.
- 🧩 **Zero Config**: Funciona "out of the box", mas é totalmente extensível.
- 🚦 **HttpClient Optimized**: Gerenciamento inteligente de instâncias do HttpClient.

---

## 🚀 Como Usar: API Estática (Estilo Axios)

A forma mais rápida e moderna de consumir APIs. Sem instanciamento, sem complicações.

### GET Simples
```csharp
var post = await WebAPI.Get<Post>("https://api.example.com/posts/1");
Console.WriteLine(post.Title);
```

### POST com JSON
```csharp
var data = new { title = "Meu Post", body = "Conteúdo legal", userId = 1 };
var result = await WebAPI.Post<Post>("https://api.example.com/posts", data);
```

### Outros Verbos (PUT, PATCH, DELETE)
```csharp
// Atualização parcial
await WebAPI.Patch("https://api.example.com/posts/1", new { title = "Novo Título" });

// Exclusão
await WebAPI.Delete<bool>("https://api.example.com/posts/1");
```

---

## 🛠️ Modo Avançado (Fluente)

Para quando você precisa de controle granular sobre headers, timeouts e instâncias.

```csharp
using var api = new WebAPI("https://api.example.com")
    .WithHeaders(new Dictionary<string, string> { 
        { "Authorization", "Bearer my-token" },
        { "X-Custom-Header", "Value" }
    });

api.SetJson(new { filter = "active" });

// Define timeout customizado (em ms)
var response = await api.Send<List<Data>>();
```

---

## 🔐 Autenticação

### Basic Auth
```csharp
using var api = new WebAPI("https://api.exemplo.com")
    .WithBasicAuth("usuario", "senha");

var dados = await api.Get<Modelo>();
```

### Certificados e mTLS
Para chamadas que exigem autenticação mTLS (Certificado Digital), você pode usar o método fluente `.WithCertificate()`.

```csharp
using var cert = new X509Certificate2("path/to/certificate.pfx", "password");

using var api = new WebAPI("https://api.secure.com")
    .WithCertificate(cert);

var response = await api.Get<SecureData>();
```

Você também pode injetar seu próprio `HttpClient` se desejar:
```csharp
using var api = new WebAPI("https://api.example.com")
    .WithHttpClient(myCustomHttpClient);
```

---

## 🛰️ Tratamento de Erros Inteligente

O `Jovemnf.WebAPI` elimina a necessidade de verificar códigos de status manualmente o tempo todo. Ele mapeia automaticamente **todos** os códigos 4xx e 5xx para exceções específicas e semânticas.

**Exemplos de Exceções Inclusas (> 40 tipos):**
- `BadRequestException` (400)
- `UnauthorizedAccessException` (401)
- `ForbiddenException` (403)
- `NotFoundException` (404)
- `ConflictException` (409)
- `UnprocessableEntityException` (422)
- `InternalServerError` (500)
- `BadGatewayException` (502)

```csharp
try 
{
    var user = await WebAPI.Get<User>(url);
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

---

## 🏛️ Design Patterns Aplicados

Esta biblioteca foi refatorada seguindo princípios **SOLID** e padrões de projeto para garantir manutenibilidade:
- **Facade Pattern**: Classe principal `WebAPI`.
- **Builder Pattern**: Internamente via `WebRequestBuilder`.
- **Strategy Pattern**: Validação de respostas e lançamento de exceções.
- **Engine Pattern**: Processamento centralizado via `WebAPIEngine`.

---

© 2026 Wallace Silva - Desenvolvido com ❤️ e foco em qualidade.
