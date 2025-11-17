# FlightWatch

Sistema profissional de rastreamento de voos que se conecta à API do AviationStack para fornecer dados em tempo real de aeronaves, incluindo posição GPS, informações de voo e detalhes da companhia aérea.

## Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades em camadas:

### Camadas

#### 1. Domain Layer (`FlightWatch.Domain`)
- Entidades de domínio
- Exceções customizadas
- Interfaces de domínio
- Lógica de negócio pura

**Estrutura:**
```
Domain/
├── Entities/
│   └── Flight.cs
└── Exceptions/
    ├── FlightWatchException.cs
    ├── ExternalServiceException.cs
    ├── AviationStackException.cs
    └── InvalidFlightSearchException.cs
```

#### 2. Application Layer (`FlightWatch.Application`)
- DTOs para transferência de dados
- Interfaces de aplicação
- Result Pattern para tratamento de retornos
- Lógica de aplicação

**Estrutura:**
```
Application/
├── Common/
│   ├── Result.cs
│   ├── Error.cs
│   ├── ApiResponse.cs
│   └── ProblemDetailsResponse.cs
├── DTOs/
│   └── FlightDto.cs
└── Interfaces/
    └── IAviationStackClient.cs
```

#### 3. Infrastructure Layer (`FlightWatch.Infrastructure`)
- Implementação de clientes externos
- Configurações
- Políticas de resiliência
- Logging estruturado
- Validações

**Estrutura:**
```
Infrastructure/
├── Configuration/
│   ├── AviationStackSettings.cs
│   └── ResiliencePolicies.cs
├── Extensions/
│   └── ActivityExtensions.cs
└── ExternalServices/
    └── AviationStack/
        ├── AviationStackClient.cs
        ├── Logging/
        │   └── LogMessages.cs
        ├── Validators/
        │   └── FlightSearchValidator.cs
        └── Models/
            ├── AviationStackResponse.cs
            ├── FlightData.cs
            ├── FlightInfo.cs
            ├── AirlineInfo.cs
            ├── LiveData.cs
            ├── DepartureInfo.cs
            └── ArrivalInfo.cs
```

#### 4. API Layer (`FlightWatch.Api`)
- Controllers
- Middlewares
- Configurações de telemetria
- Ponto de entrada da aplicação

**Estrutura:**
```
Api/
├── Configuration/
│   └── TelemetryConfiguration.cs
├── Extensions/
│   └── MiddlewareExtensions.cs
├── Middleware/
│   ├── CorrelationIdMiddleware.cs
│   └── ExceptionHandlingMiddleware.cs
└── Program.cs
```

## Funcionalidades Implementadas

### 1. Client do AviationStack
- Busca de voos por origem e destino (códigos IATA)
- Retorna dados em tempo real incluindo:
  - Número do voo
  - Companhia aérea
  - Latitude e Longitude
  - Direção do voo
  - Origem e destino

### 2. Exception Handling
- Hierarquia de exceções customizadas
- Middleware global de tratamento de erros
- Respostas padronizadas usando Problem Details (RFC 7807)
- Mapeamento automático de exceções para HTTP status codes apropriados

### 3. Logging Profissional
- Logs estruturados usando LoggerMessage Source Generator
- Diferentes níveis de log (Information, Warning, Error, Critical)
- Context enriquecido com Correlation ID
- Eventos de log específicos para cada operação

### 4. Tracking e Correlação
- Middleware de Correlation ID
- Headers customizados (X-Correlation-ID, X-Request-ID)
- Propagação de contexto entre serviços
- Logs associados por correlation ID

### 5. Telemetria (OpenTelemetry)
- Instrumentação automática de ASP.NET Core
- Instrumentação de HttpClient
- Traces distribuídos
- Tags customizadas para operações de negócio
- Export para console (desenvolvimento)

### 6. Resiliência (Polly)
- **Retry Policy**: 3 tentativas com backoff exponencial
- **Circuit Breaker**: Proteção contra falhas em cascata
- **Timeout Policy**: Timeout de 30 segundos
- Tratamento de erros transientes HTTP

### 7. Result Pattern
- Retornos explícitos de sucesso/falha
- Eliminação de exceções para controle de fluxo
- Type-safe error handling
- Composição funcional

### 8. Validação
- Validação de códigos IATA (3 letras)
- Validação de parâmetros obrigatórios
- Validação de lógica de negócio (origem ≠ destino)

## Configuração

### User Secrets (Desenvolvimento)

A API key do AviationStack é armazenada de forma segura usando User Secrets:

```bash
cd src/FlightWatch.Api
dotnet user-secrets set "AviationStack:ApiKey" "SUA_API_KEY_AQUI"
```

### appsettings.json

```json
{
  "AviationStack": {
    "BaseUrl": "http://api.aviationstack.com/v1"
  }
}
```

## Tecnologias e Pacotes

### Pacotes NuGet Principais
- **OpenTelemetry**: Telemetria e observabilidade
- **Polly**: Resiliência e políticas de retry
- **Microsoft.Extensions.Http.Polly**: Integração HttpClient + Polly
- **System.Text.Json**: Serialização/deserialização JSON

### Frameworks
- **.NET 9.0**
- **ASP.NET Core**
- **Minimal APIs** (pronto para controllers)

## Padrões de Design Implementados

1. **Clean Architecture**: Separação em camadas com dependências unidirecionais
2. **Result Pattern**: Tratamento explícito de sucesso/falha
3. **Repository Pattern**: Abstração de acesso a dados externos
4. **Factory Pattern**: Criação de objetos complexos
5. **Strategy Pattern**: Políticas de resiliência configuráveis
6. **Middleware Pattern**: Pipeline de processamento de requisições
7. **Options Pattern**: Configurações tipadas e validadas

## Middleware Pipeline

```
Request
  ↓
CorrelationIdMiddleware (adiciona correlation ID)
  ↓
ExceptionHandlingMiddleware (captura exceções)
  ↓
OpenTelemetry Instrumentation (traces)
  ↓
Controllers/Endpoints
  ↓
Response
```

## Logging Events

| Event ID | Nível | Descrição |
|----------|-------|-----------|
| 1001 | Information | Busca de voo iniciada |
| 1002 | Information | Busca completada com sucesso |
| 1003 | Warning | Nenhum voo encontrado |
| 2001 | Error | Erro na resposta da API |
| 2002 | Error | Erro de deserialização |
| 2003 | Error | Falha na requisição HTTP |
| 3001 | Error | Parâmetros inválidos |
| 4001 | Critical | Falha crítica no cliente |

## Próximos Passos

- [ ] Implementar Controllers para expor endpoints
- [ ] Adicionar cache para otimizar requisições
- [ ] Implementar paginação de resultados
- [ ] Adicionar autenticação/autorização
- [ ] Configurar exporters de telemetria (Jaeger, Zipkin)
- [ ] Implementar health checks
- [ ] Adicionar testes unitários e de integração
- [ ] Documentação Swagger/OpenAPI

## Segurança

- API keys armazenadas em User Secrets (desenvolvimento)
- Preparado para Azure Key Vault (produção)
- HTTPS obrigatório
- Headers de segurança configuráveis
- Validação de entrada rigorosa

## Observabilidade

O sistema está preparado para observabilidade completa:

- **Logs**: Estruturados com contexto rico
- **Traces**: Distribuídos com OpenTelemetry
- **Metrics**: Prontos para implementação
- **Correlation**: IDs propagados através de toda a stack

## Contribuição

Este projeto segue as melhores práticas de desenvolvimento:
- Clean Code
- SOLID Principles
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- YAGNI (You Aren't Gonna Need It)

