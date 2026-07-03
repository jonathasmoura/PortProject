# PortProject

Solução .NET 8 para cadastro e autenticação de usuários com envio de e‑mail. Projeto estruturado em camadas (API → Application → Domain → Infra) com padrões de mercado (DI, Repository, UnitOfWork, JWT, MailKit).

## Objetivo
Fornecer uma aplicação backend para:
- Registro de usuário (hash de senha, atribuição de role).
- Login com JWT (token de acesso) + refresh token (persistência em cookie HTTP‑only).
- Envio de e‑mail (MailKit) — ambiente de desenvolvimento com Mailpit via Docker.

## Arquitetura e camadas
- `PP.API` — Controllers / endpoints REST (ex.: `UserController`).
- `PP.Application` — Serviços de aplicação (UserService, EmailSendService), DTOs e contratos.
- `PP.Domain` — Entidades e interfaces (contratos de repositório).
- `PP.Infra` — Implementações EF Core, repositórios, migrations e extensões de DI (`InfraExtension`).

Padrões aplicados: Dependency Injection, Repository, UnitOfWork, separação por interfaces (facilita testes e substituição de implementações).

## Funcionalidades principais
- Endpoints:
  - `POST /v1/user/register` — registra usuário e envia e‑mail de boas‑vindas.
  - `POST /v1/user/login` — autentica e retorna JWT de acesso; grava refresh token em cookie HTTP‑only.
  - `POST /v1/user/logout` — limpa cookie de refresh token.
  - `POST /v1/user/refresh` — (ponto preparado para refresh token flow).
- Envio de e‑mail usando MailKit.
- Persistência em SQL Server via EF Core.

## Métricas e indicadores (reais / observáveis no repositório)
- Tokens:
  - Tempo de vida do access token: ~5 minutos (definido no serviço de geração de token).
  - Tempo de vida do refresh token: ~20 minutos (cookie expirado em 20min).
- Endpoints expostos: 4 (register, login, logout, refresh).
- Ports (docker):
  - App: `8080` / `8081`
  - SQL Server: `1433`
  - Mailpit UI: `8025` (SMTP `1025`)
- Organização: separação em 4 projetos principais (API, Application, Domain, Infra).
- Dependências críticas: EF Core, MailKit, JWT packages.

> Observação: métricas como cobertura de testes, latência e throughput não estão implementadas — recomenda-se adicionar CI com testes e métricas (Prometheus/Zipkin/Application Insights).

## Explicação do `docker-compose.yaml`
O `docker-compose.yaml` orquestra três serviços para ambiente de desenvolvimento:
- `app`
  - Constrói a imagem a partir do `Dockerfile` do repositório.
  - Variáveis de ambiente para desenvolvimento: `ASPNETCORE_ENVIRONMENT=Development` e `EmailSettings__*` para apontar o SMTP para o container `mailpit`.
  - Depende de `database` e `mailpit` (start order).
  - Expõe portas `8080:8080` e `8081:8081`.
- `database`
  - Imagem oficial do SQL Server 2022.
  - Volume `sqlvolume` para persistência dos dados.
  - Porta mapeada `1433:1433`.
- `mailpit`
  - Ferramenta para capturar e visualizar e‑mails localmente (UI em `8025`, SMTP em `1025`).
  - Volume `mailpit-data` para persistência de mensagens.
  
Propósito: permitir desenvolvimento e testes locais (registro/login + envio de e‑mail) sem dependências externas.

## Como executar (dev)
1. Ajuste segredos locais: defina `ConnectionStrings:DefaultConnection`, `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` (use __dotnet user-secrets__ ou variáveis de ambiente).
2. No diretório do projeto:
   - docker compose up --build
3. Acesse:
   - API: http://localhost:8080 (ou 8081 conforme configuração)
   - Mailpit UI: http://localhost:8025
4. Rotas principais: `/v1/user/register`, `/v1/user/login`

## Pontos fortes
- Estrutura modular e clara, com separação de responsabilidades.
- Fluxo de autenticação baseado em JWT + refresh token.
- Ambiente reproducível via Docker (SQL Server + Mailpit).
- Uso de padrões que facilitam testes e manutenção.

## Riscos e melhorias recomendadas (prioridade)
1. Segurança de segredos: mover JWT key, DB password para vaults/secret store (__User Secrets__ em dev, Azure Key Vault em produção).
2. Não enviar senha em texto por e‑mail — usar flow de confirmação/ativação com token ou link de reset.
3. Persistência e rotação de refresh tokens: salvar no banco, validar expiração e revogação.
4. HTTPS e cookie `Secure`: cookie atual usa `Secure=true` — requer HTTPS em produção; permitir configuração por ambiente.
5. Logging e observabilidade: adicionar `ILogger`, correlação de requests, métricas e tracing.
6. Testes automatizados: unit tests para serviços e integração para fluxo de autenticação.
7. Tratamento de exceções: evitar expor mensagens de erro cruas; padronizar responses de erro.

## SOLID — exemplos aplicados no projeto
- Single Responsibility: controllers apenas orquestram; services cuidam da lógica de aplicação; repositórios cuidam do acesso a dados.
- Open/Closed: serviços consomem interfaces; novas implementações podem ser adicionadas sem alterar consumidores.
- Liskov: implementações seguem contratos de interface (substituição possível por mocks).
- Interface Segregation: interfaces específicas para e‑mail, token, usuários.
- Dependency Inversion: camadas superiores dependem de abstrações; infraestrutura fornece implementações via DI (`InfraExtension`).

## Status atual e nível de maturidade
- Maturidade: arquitetura madura para desenvolvimento (intermediária). Bom design, porém precisa de hardening para produção (segurança, testes, observabilidade, validação e tratamento robusto de erros).
- Recomendações iniciais de roadmap: 1) segredos e segurança 2) persistência/validação de refresh tokens 3) testes automáticos 4) logging/monitoramento.

