# Registration Response

## Overview

- Direction: Inbound
- Provider: MSB
- Document Version: 1.0
- Document Date: 1405/06/11 (Solar Hijri)
- MSB Service ID: Not specified in official document
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir` is printed in the PDF; the receiving application's deployed base URL is not specified.
- Endpoint: Not specified in official document
- Implementation endpoint: `/made-14/registration-response/v1/create` (not an official endpoint claim).
- Authentication/Header: official `X-MSB-Api-Key`; implementation uses `MSB:ApiKeyHeaderName`, locally `msb-identifier`, and `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF p. 4 requires `organId` and `owTrakingCode` (strings), `status` (integer), and `result` with integer `code` and string `msg`. `sabtTrackingCode` is an optional string. These describe the latest Article 14 registration status.

Acknowledgment (pp. 4–5): `msbTrackingCode`, string `code`, `message`, `description`, `timestamp`; examples use `"200"`/`"OK"` or `"000"`/`"INVALID_DATA"`. The request example leaves `result.code` blank; it is not executable JSON or a source of a fixed status value.

## Fixed Values

- RuleId, noToken, elzam14: Not specified in official document
- Allowed registration `status`/`result.code` values: Not specified in official document (sample `status = 2` is not declared fixed).

## Implementation

- Controller: [RegistrationResponseController](Controllers/RegistrationResponseController.cs).
- Service: [RegistrationCallbackService](Services/RegistrationCallbackService.cs), `IRegistrationCallbackService.SaveRegistrationStatusCallbackAsync`.
- Request/Response Models: `RegistrationStatusCallbackRequest`, `RegistrationStatusResult` in [RegistrationStatusCallbackRequest.cs](Models/RegistrationStatusCallbackRequest.cs); acknowledgment is an anonymous controller object with a generated tracking code.
- Config keys: `MSB:ApiKeyHeaderName`, `MSB:ApiKey`, `UseInMemoryDatabase`, `ConnectionStrings:DefaultConnection`.
- Database persistence: `SanadCallbacks` stores `result.code`, reserialized request JSON and creation time.
- Invocation logging status: inbound middleware captures the implementation route as `RegistrationResponse` in sanitized `MsbInvocationLogs`.

## Official Documentation

[MSB-Registration-Response-IN-v1.0-1405-06-11.pdf](Docs/MSB-Registration-Response-IN-v1.0-1405-06-11.pdf)

## Verification Status

Implementation requires final verification

The PDF leaves both endpoint and service ID empty. Existing local callback/logging checks do not establish an official endpoint. Confirm the receiving route and configured header with MSB; no endpoint or status enumeration has been inferred from another service.
