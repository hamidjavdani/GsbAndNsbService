# POA Inquiry

## Overview

- Direction: Outbound
- Provider: MSB
- Document Version: 1.1
- Document Date: 1405/05/10 (Solar Hijri)
- MSB Service ID: `S01-OUT-made14-IIP-1405/04/21-1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir`
- Endpoint: `/interagency/poa/G2GInquery`
- Authentication/Header: official `X-MSB-Api-Key`; service sends configured `MSB:ApiKeyHeaderName`, locally `msb-identifier`, with `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF p. 4 requires string `nationalRegisterNo`, `secretNo`, `requesterName`, `requesterFamily`, `requesterNationalCode`, `requesterOfficProvinceName`, `requesterOfficeCode`, `requesterOfficNumber`, `ruleId`, and `requestUniqueId`.

Response (p. 5): integer `code`, string `msg`, and string `owTrakingCode` on success. Examples show `200` (accepted), `201` (API key failure), `203` (validation failure). The `201` sample has a trailing space in `msg `; the table and model use `msg`.

## Fixed Values

- RuleId: `mhlu20po` (official fixed value).
- noToken, elzam14: Not specified in official document

## Implementation

- Controller: none active for this service.
- Service: [PoaInquiryService](Services/PoaInquiryService.cs), `IPoaInquiryService.SendAsync`.
- Request/Response Models: [PoaInquiryRequest](Models/PoaInquiryRequest.cs), [PoaInquiryResponse](Models/PoaInquiryResponse.cs).
- Config keys: `MSB:BaseUrl`, `MSB:PoaInquiryEndpoint`, `MSB:PoaInquiryRuleId`, `MSB:ApiKeyHeaderName`, `MSB:ApiKey`.
- Database persistence: none in this service.
- Invocation logging status: no outbound invocation logging in `PoaInquiryService`.

## Official Documentation

[MSB-Poa-Inquiry-OUT-v1.1-1405-05-10.pdf](Docs/MSB-Poa-Inquiry-OUT-v1.1-1405-05-10.pdf)

## Verification Status

Implementation requires final verification

The service defaults an empty rule from configuration but does not enforce the official rule on nonempty input or validate every required field. Confirm the configured header and complete request/response behavior with MSB. HTTP success alone is not confirmation of business success; the returned `code` must be checked by the caller. The trailing-space message sample requires clarification if returned literally.
