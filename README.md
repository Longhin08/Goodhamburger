# 🍔 Good Hamburger API

Sistema de pedidos para a lanchonete **Good Hamburger**, construído com ASP.NET Core 8, Blazor WebAssembly e xUnit.

---

## Estrutura do projeto

```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Domain/        # Entidades e interfaces (sem dependências externas)
│   ├── GoodHamburger.Application/   # Serviços, regras de negócio e DTOs
│   ├── GoodHamburger.API/           # ASP.NET Core Web API + repositório in-memory
│   └── GoodHamburger.BlazorUI/      # Frontend Blazor WebAssembly
└── tests/
    └── GoodHamburger.Tests/         # Testes xUnit com FluentAssertions
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Como executar

### 1. Clonar / extrair o projeto

```bash
cd GoodHamburger
```

### 2. Rodar a API

```bash
cd src/GoodHamburger.API
dotnet run
```

A API sobe em `http://localhost:5100`.  
Swagger disponível em `http://localhost:5100/swagger`.

### 3. Rodar o frontend Blazor (opcional)

Em outro terminal:

```bash
cd src/GoodHamburger.BlazorUI
dotnet run
```

Acesse `http://localhost:5200`.

### 4. Rodar os testes

```bash
cd tests/GoodHamburger.Tests
dotnet test
```

---

## Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/menu` | Lista o cardápio completo |
| `GET` | `/api/orders` | Lista todos os pedidos |
| `GET` | `/api/orders/{id}` | Busca pedido por ID |
| `POST` | `/api/orders` | Cria novo pedido |
| `PUT` | `/api/orders/{id}` | Atualiza pedido existente |
| `DELETE` | `/api/orders/{id}` | Remove pedido |

### Exemplo de body para criar/atualizar pedido

```json
{
  "sandwichId": "x-burger",
  "sideDishId": "fries",
  "drinkId": "soda"
}
```

Todos os campos são opcionais, mas pelo menos um deve ser informado.  
Valores aceitos: `"x-burger"`, `"x-egg"`, `"x-bacon"`, `"fries"`, `"soda"`.

---

## Regras de desconto

| Combinação | Desconto |
|-----------|---------|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Apenas sanduíche | 0% |

---

## Decisões de arquitetura

- **Camadas separadas (Domain / Application / API):** facilita testes unitários e mantém regras de negócio desacopladas do framework.
- **In-memory com `ConcurrentDictionary`:** atende ao requisito sem banco de dados real; substituível por qualquer implementação de `IOrderRepository` (EF Core, Dapper etc.) sem tocar na lógica.
- **DiscountService isolado:** toda a lógica de desconto fica em um único lugar — fácil de testar e de estender.
- **Blazor WebAssembly:** roda no browser, consome a API via `HttpClient`, sem server-side rendering adicional.

## O que ficou de fora

- Persistência em banco de dados (SQLite/SQL Server) — o repositório in-memory perde dados ao reiniciar.
- Autenticação/autorização.
- Paginação na listagem de pedidos.
- CI/CD (GitHub Actions).
