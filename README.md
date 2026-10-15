# EventBank Accounts

The Accounts microservice of [EventBank](https://github.com/marcelo-roman/eventbank-platform). It owns bank accounts: opening, deposits, withdrawals, closing, its side of every transfer, and the charges of card purchases. Each account is an event-sourced aggregate; no balance is stored anywhere.

## Domain

[`BankAccount`](src/EventBank.Accounts.Domain/BankAccount.cs) changes state only by raising events, and `Apply` rebuilds it from them.

| Event | Raised when |
| --- | --- |
| `AccountOpened` | An account is opened for an approved customer, with a holder, a currency and a branch/number |
| `FundsDeposited` / `FundsWithdrawn` | Money moves in or out; a withdrawal never takes the balance below zero |
| `TransferDebited` / `TransferDebitDeclined` | The account pays for a transfer, or records why it could not |
| `TransferCredited` / `TransferCreditDeclined` | The account receives a transfer, or records why it could not |
| `TransferRefunded` | A debited transfer is compensated |
| `AccountCharged` / `AccountChargeDeclined` | A card purchase is charged, or declined with a reason |
| `AccountClosed` | The account is closed; only a zero balance can be closed |

A declined debit is a fact, not an exception, and every transfer reply is idempotent: a redelivered `DebitAccount` for a transfer the account already answered changes nothing.

`Money`, `Currency` and `AccountHolder` validate input on the way in; events carry primitives so the stored history never depends on value-object rules.

Account numbers come from a PostgreSQL sequence and carry a modulo-11 check digit (`0001/100001-2`). The checking account that opens when a customer is approved gets a deterministic id derived from the customer, so a redelivered `CustomerApproved` finds it instead of opening a second one.

## Who can do what

Every endpoint needs a Keycloak token. Only the owner moves money; operators read every account, including its events and replays, but cannot move money. `AccountAccess` in the application layer enforces it and answers 403.

## Layers

| Project | Holds |
| --- | --- |
| `EventBank.Accounts.Domain` | The aggregate, value objects and events |
| `EventBank.Accounts.Application` | Commands, queries, their handlers, and the `IAccountReadStore` port |
| `EventBank.Accounts.Infrastructure` | EF Core read models and migrations, the projection, and the domain-to-integration event mapping |
| `EventBank.Accounts.Api` | Minimal API endpoints and the RabbitMQ consumers, both only translating input into commands and queries |

[Architecture tests](tests/EventBank.Accounts.Architecture.Tests/CleanArchitectureTests.cs) keep the dependencies pointing inward and keep the Transfers bounded context out.

## API

| Method | Path | Side |
| --- | --- | --- |
| `POST` | `/accounts` | command; opens another account for an approved customer |
| `POST` | `/accounts/{id}/deposits`, `/withdrawals`, `/close` | command |
| `GET` | `/accounts`, `/accounts/{id}`, `/accounts/{id}/transactions` | read model |
| `GET` | `/accounts/{id}/events` | event stream |
| `GET` | `/accounts/{id}/state?version=n` | the aggregate as it was at version n |
| `GET` | `/ops/subscriptions` | checkpoint and lag of each subscription |
| `POST` | `/ops/subscriptions/read-models/rebuild` | drop the read models and replay every event |

Commands answer with the new stream version, so a client can wait until the read model reaches it. OpenAPI is at `/openapi/v1.json` and Scalar at `/scalar`.

## Messages

Consumes `CustomerApproved`, `DebitAccount`, `CreditAccount`, `RefundAccount` and `ChargeAccount`. Publishes `AccountOpened`, `AccountClosed`, `AccountDebited`, `AccountDebitDeclined`, `AccountCredited`, `AccountCreditDeclined`, `AccountRefunded`, `AccountCharged` and `AccountChargeDeclined`, all from [eventbank-contracts](https://github.com/marcelo-roman/eventbank-contracts). Integration messages are produced by a subscription on the event store, so a message goes out if and only if its event was stored.

## Running

The service depends on the `EventBank.BuildingBlocks.*` and `EventBank.Contracts` packages. Restore them either from GitHub Packages (add the source with a token that can read packages) or from a local feed: clone the EventBank repositories side by side and run `eventbank-platform/scripts/local-feed.sh`; `Directory.Build.props` picks up `../.feed` when it exists.

```bash
docker compose up -d            # PostgreSQL on 55432 and RabbitMQ on 5672
dotnet run --project src/EventBank.Accounts.Api   # http://localhost:5101
dotnet test
```

To run it together with everything else, use [eventbank-platform](https://github.com/marcelo-roman/eventbank-platform).

## License

[MIT](LICENSE)
