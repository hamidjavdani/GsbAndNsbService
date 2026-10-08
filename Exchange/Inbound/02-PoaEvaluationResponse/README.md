# POA Evaluation Response

## Overview

- Direction: Inbound
- Provider: MSB
- Document Version: 1.1
- Document Date: 1405/05/10 (Solar Hijri)
- MSB Service ID: `S01-IN-made14-RIP-1405/04/01-1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir` is printed in the PDF; the receiving application's deployed base URL is not specified.
- Endpoint: official `/interagency-inquiry/poa-evaluation-response/create`; implementation `/interagency-inquiry/poa-evaluation-response/v1/create`.
- Authentication/Header: official `X-MSB-Api-Key`; implementation uses `MSB:ApiKeyHeaderName`, locally `msb-identifier`, and `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF pp. 4–7 define `organId`, `owTrakingCode`, integer `code`, `data` for success (`200`) or `error.errorMessage`/`error.errorCode` for failure (`201`). Success data includes `RegCases`, `BaseDocuments`, `FollowerDocuments`, `succseed`, `NationalRegisterNo`, document type and permissions, document images/dates, and `lstFindPersonInQuery` with agent/principal identity and role details.

Acknowledgment: `msbTrackingCode`, string `code`, `message`, `description`, `timestamp`; success uses `"200"`/`"OK"`, failure uses `"000"`.

## Fixed Values

- RuleId, noToken, elzam14: Not specified in official document
- Result codes: `200` (success), `201` (error).

## Implementation

- Controller: [PoaEvaluationResponseController](Controllers/PoaEvaluationResponseController.cs).
- Service: [PoaEvaluationCallbackService](Services/PoaEvaluationCallbackService.cs), `IPoaEvaluationCallbackService`.
- Request/Response Models: `PoaEvaluationResponseRequest`, `PoaEvaluationData`, `PoaEvaluationError`, `PoaEvaluationPerson` in [PoaEvaluationResponseModels.cs](Models/PoaEvaluationResponseModels.cs); acknowledgment is an anonymous controller object.
- Config keys: `MSB:ApiKeyHeaderName`, `MSB:ApiKey`, `UseInMemoryDatabase`, `ConnectionStrings:DefaultConnection`.
- Database persistence: `PoaEvaluationCallbacks` stores selected result/error fields, original body JSON and creation time.
- Invocation logging status: inbound middleware captures the runtime route as `PoaEvaluationResponse` in sanitized `MsbInvocationLogs`.

## Official Documentation

[MSB-Poa-Evaluation-Response-IN-v1.1-1405-05-10.pdf](Docs/MSB-Poa-Evaluation-Response-IN-v1.1-1405-05-10.pdf)

## Verification Status

Implementation requires final verification

MSB confirmation of the implementation's additional `/v1` segment is pending; it must not be treated as an approved endpoint difference. The configured header also differs from the printed header.

The PDF's spellings were audited; the two code-field mappings now follow the matching table and sample. No undocumented alias is assumed:

| Field | PDF table | PDF sample | Current model JSON name |
|---|---|---|---|
| Document type code | `DocType_code` | `DocType_code` | `DocType_code` |
| POA expiry | `ADVOCACYENDDATE` | `advocacyEndDate` | `advocacyEndDate` |
| Person role code | `Person_RoleType_code` | `Person_RoleType_code` | `Person_RoleType_code` |

Case-insensitive deserialization handles the expiry capitalization difference. The table and sample both use `DocImage_Base64` and `PersonType_code`, which the model already matches; `64Base_DocImage` and `code_PersonType` are not assumed aliases. `ImpotrtantAnnexText` is spelled that way in both PDF and model. Fake local tests verify the documented mappings. Endpoint confirmation remains outstanding.
