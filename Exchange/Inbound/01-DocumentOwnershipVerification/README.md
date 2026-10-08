# Document Ownership Verification Response

## Overview

- Direction: Inbound
- Provider: MSB
- Document Version: 1.1
- Document Date: 1405/05/10 (Solar Hijri)
- MSB Service ID: `S01-IN-made14-DOV-1405/04/06-1.0` (PDF p. 3; distinct from document version)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir` is printed in the PDF; the receiving application's deployed base URL is not specified.
- Endpoint: official `/document-ownership-verification/create`; implementation `/document-ownership-verification/v1/create`.
- Authentication/Header: official API key header `X-MSB-Api-Key`; implementation reads `MSB:ApiKeyHeaderName`, currently configured locally as `msb-identifier`. No key value is documented here.
- Content-Type: `application/json`

## Contract

PDF pp. 4–11 define required `organId` (string), `owTrakingCode` (string) and `code` (integer). For `code = 200`, `data` contains document verification results: `GetKmlPolygonInfo`, `ConfirmDocumentByElectronicInfo` and `ConfirmDocumentInfo`. For `code = 201`, `error` contains `errorMessage` and `errorCode`.

Electronic verification includes document, owner, cadastral, share and restriction fields, with boolean flags and numeric areas/shares. `ConfirmDocumentInfo` uses string representations for those flags and numbers. The PDF's polygon table describes an object, while its sample sends `GetKmlPolygonInfo` as a JSON string; the implementation's `JsonElement` can retain either form.

Acknowledgment: `msbTrackingCode`, string `code`, `message`, `description`, and Persian `timestamp`; examples use `"200"`/`"OK"` for success and `"000"`/`"INVALID_DATA"` for failure.

## Fixed Values

- RuleId, noToken, elzam14: Not specified in official document
- Result codes: `200` (successful inquiry), `201` (inquiry error).
- Polygon type in the documented polygon payload: `Polygon`.

## Implementation

- Controller: [DocumentOwnershipVerificationController](Controllers/DocumentOwnershipVerificationController.cs).
- Service: [MsbCallbackService](Services/MsbCallbackService.cs), `IMsbCallbackService`.
- Request/Response Models: `MsbCallbackRequest`, `MsbCallbackData`, `MsbCallbackError`, `MsbCallbackAckResponse` under `Models/`; shared `ConfirmDocumentByElectronicInfo` and `ConfirmDocumentInfo` under repository `Models/Callback/`.
- Config keys: `MSB:ApiKeyHeaderName`, `MSB:ApiKey`; persistence uses `UseInMemoryDatabase` or `ConnectionStrings:DefaultConnection`.
- Database persistence: `ApplicationDbContext.SanadCallbacks`; stores result code, reserialized request JSON and creation time.
- Invocation logging status: inbound middleware captures this runtime route as `DocumentOwnershipVerification`, with sanitized request/response and completion metadata in `MsbInvocationLogs`.

## Official Documentation

[MSB-Document-Ownership-Verification-IN-v1.1-1405-05-10.pdf](Docs/MSB-Document-Ownership-Verification-IN-v1.1-1405-05-10.pdf)

## Verification Status

Implemented with documented intentional difference

The additional `/v1` segment is an intentional runtime endpoint difference coordinated with MSB, as confirmed by the project owner. It is not the endpoint printed in the PDF. Local verification of the callback/logging exists; this README does not claim a new external verification. The runtime header name also differs from the PDF and is configuration-dependent. The PDF's polygon table/sample representation difference remains explicit above.
