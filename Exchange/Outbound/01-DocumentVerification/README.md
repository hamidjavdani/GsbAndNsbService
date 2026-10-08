# Document Verification Inquiry

## Overview

- Direction: Outbound
- Provider: MSB
- Document Version: 1.1
- Document Date: 1405/06/10 (Solar Hijri)
- MSB Service ID: `S01-OUT-made14-IDV-1405/04/20-1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir`
- Endpoint: `/interagency/document-verification-inquiry/G2GInquery`
- Authentication/Header: official `X-MSB-Api-Key`; service sends configured `MSB:ApiKeyHeaderName`, locally `msb-identifier`, with `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF p. 4 requires string fields `requesterName`, `requesterFamily`, `requesterNationalCode`, `requesterOfficProvinceName`, `requesterOfficeCode`, `requesterOfficNumber`, `ruleId`, `requestUniqueId`, `ElectronicEstateNoteNo`, and `nationalitycode`. Field names retain the PDF's spelling and casing.

Response (p. 5): integer `code`, string `msg`, and string `owTrakingCode` on success. Examples show `200` (accepted), `201` (API key failure), `203` (validation failure). The `201` sample spells the message key `msg ` with a trailing space; the table and model use `msg`.

## Fixed Values

- RuleId: `mhrne7iv` (official fixed value).
- noToken, elzam14: Not specified in official document

## Implementation

- Controller: none active for this outbound service; historical controller text is not an active route.
- Service: [DocumentVerificationService](Services/DocumentVerificationService.cs), `IDocumentVerificationService.G2GInquiryAsync`.
- Request/Response Models: [G2GInquiryRequest](Models/G2GInquiryRequest.cs), [G2GInquiryResponse](Models/G2GInquiryResponse.cs).
- Config keys: `MSB:BaseUrl`, `MSB:InquiryEndpoint`, `MSB:ApiKeyHeaderName`, `MSB:ApiKey`; the official rule is enforced independently of `MSB:RuleId`.
- Database persistence: no business record; sanitized outbound invocation records use `MsbInvocationLogs`.
- Invocation logging status: shared `MsbOutboundInvocation` captures `DocumentVerification` begin/completion, identifiers, masked bodies and HTTP/response metadata; logging failures do not block sending.

## Official Documentation

[MSB-Document-Verification-OUT-v1.1-1405-06-10.pdf](Docs/MSB-Document-Verification-OUT-v1.1-1405-06-10.pdf)

## Verification Status

Implementation requires final verification

All required request strings are validated before sending. An empty rule becomes the official `mhrne7iv`; other values are rejected even if configuration contains a different rule. Fake-handler regression tests cover validation, the exact URL/header/media type, deserialization, sanitized persisted logs and failure completion. No real MSB call is made. Confirm runtime configuration and the header difference with MSB; the trailing-space message sample needs clarification if emitted literally. HTTP success is not an independent check of business success.
