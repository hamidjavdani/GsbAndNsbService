# Made14 Cancellation

## Overview

- Direction: Outbound
- Provider: MSB
- Document Version: 1.0
- Document Date: 1405/07/05 (Solar Hijri)
- MSB Service ID: `S01-OUT-MWD-1405/07/05-V1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir`
- Endpoint: `/made14/ebtal/v1/response`
- Authentication/Header: official `X-MSB-Api-Key`; service sends configured `MSB:ApiKeyHeaderName`, locally `msb-identifier`, with `MSB:ApiKey`.
- Content-Type: `application/json` in the PDF header table and implementation; the PDF curl example instead uses `text/plain`.

## Contract

PDF p. 5 requires string `organId`, object `data` containing string `cancelReason` and `actionId`, string `owTrackingCode`, and string `ruleId`. Optional boolean `noToken` has the documented constant `true`. Keep `owTrackingCode` spelling distinct from other services' `owTrakingCode`.

Response (p. 6): integer `code` and string `msg`. Examples show `200`/`"OK"` for success and `201`/`"token not found"` for failure; the status table also lists `202` for invalid authorization. Registration completion is returned through the separate inbound Registration Response service.

## Fixed Values

- RuleId without a registration tracking code: `mgrjz6mo`.
- RuleId with a registration tracking code: `mgw2bnhz`.
- noToken: `true` (documented fixed value; optional request field).
- elzam14: Not specified in official document

## Implementation

- Controller: none active for this outbound service.
- Service: [Made14CancellationService](Services/Made14CancellationService.cs), `IMade14CancellationService.CancelMade14Async`, registered in `Program.cs`. Legacy `Services/MsbService.cs` remains separate and is not the registered cancellation implementation.
- Request/Response Models: [Made14CancellationRequest](Models/Made14CancellationRequest.cs), `Made14CancellationData`, `Made14CancellationRuleIds`, [Made14CancellationResponse](Models/Made14CancellationResponse.cs).
- Config keys: `MSB:BaseUrl`, `MSB:CancellationEndpoint`, `MSB:ApiKeyHeaderName`, `MSB:ApiKey`; logger persistence uses `UseInMemoryDatabase` or `ConnectionStrings:DefaultConnection`.
- Database persistence: no cancellation business record; outbound invocation records are persisted in `MsbInvocationLogs`.
- Invocation logging status: `Made14Cancellation` outbound begin/completion, sanitized bodies, identifiers, HTTP result and response code/message; logging failures do not block the HTTP operation. `IsSuccess` reflects HTTP success without a processing exception, not an independent check that business `code = 200`.

## Official Documentation

[MSB-Made14-Cancellation-OUT-v1.0-1405-07-05.pdf](Docs/MSB-Made14-Cancellation-OUT-v1.0-1405-07-05.pdf)

## Verification Status

Implementation requires final verification

The model constants match the official rule IDs and the service rejects other rules before sending. `noToken` defaults to true but false is not rejected. The PDF's curl content type conflicts with its header table; the implementation follows the table. Its sample also has malformed JSON punctuation, so it should not be copied as executable input.

The checked-in invocation regression suite instantiates legacy `MsbService` with `mo6mgrjz`/`bnhz2mgw`, not the official values in the current models. Earlier local PASS results therefore do not establish final verification of the registered service with the current official rules. Verify those rules, noToken behavior and the configured header separately; this documentation change does not modify code or tests.
