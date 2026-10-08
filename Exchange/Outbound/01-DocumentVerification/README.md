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
- Config keys: `MSB:BaseUrl`, `MSB:InquiryEndpoint`, `MSB:RuleId`, `MSB:ApiKeyHeaderName`, `MSB:ApiKey`.
- Database persistence: none in this service.
- Invocation logging status: no outbound invocation logging in `DocumentVerificationService`; inbound middleware does not capture this HTTP client call.

## Official Documentation

[MSB-Document-Verification-OUT-v1.1-1405-06-10.pdf](Docs/MSB-Document-Verification-OUT-v1.1-1405-06-10.pdf)

## Verification Status

Implementation requires final verification

The service defaults an empty rule to configuration but does not reject a non-official rule or enforce all required request fields. It checks HTTP status, then deserializes the business code. Confirm runtime configuration and the header difference with MSB; a prior callback test does not verify this outbound inquiry. The trailing-space message sample also needs clarification if MSB emits it literally.
