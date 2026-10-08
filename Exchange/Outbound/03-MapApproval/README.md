# Map Approval

## Overview

- Direction: Outbound
- Provider: MSB
- Document Version: 1.0
- Document Date: 1405/03/31 (cover); revision history also records a Base URL change on 1405/04/01.
- MSB Service ID: `made14-SEW-1405/04/01-1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: `http://pmsbgw.shahr-bank.ir`
- Endpoint: `/made14/map-approval/G2GInquery`
- Authentication/Header: official `X-MSB-Api-Key`; service sends configured `MSB:ApiKeyHeaderName`, locally `msb-identifier`, with `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF pp. 4–12 define `organId`, `requestUniqueId`, `elzam14`, issue number/date/time, action-reference and cadastral fields, `Address`, `Area`, optional `PostalCode`, and `TotalBlockNo`. Nested `Block[].Class[]` contains `EstateUnits` and `Joint`; `OwnersInfo` supplies owner identity, address and contact information. `MainPlan.EstateMap` and `UnitPlans[].UnitMap` carry map geometry; plan file metadata is also described.

`IssueDate` is Solar Hijri; `IssueTime` is `xx:xx`. Required/conditional fields, lengths and code tables are specified in the PDF. Main-plan image types are `image/png`/`image/jpeg`; unit-plan types also allow `application/pdf`.

Response fields, success/error codes and response examples: Not specified in official document. The implementation returns a raw `JsonElement`; no acknowledgment contract is inferred from other outbound services.

## Fixed Values

- elzam14: `true` is mandatory.
- External map CRS: `EPSG:4326` (PDF pp. 9, 11 and examples).
- RuleId, noToken: Not specified in official document
- `ActionReferenceType` and `StructureType` are enumerations in the appendix, not single fixed values.

## Implementation

- Controller: none active for this service.
- Service: [MapApprovalService](Services/MapApprovalService.cs), `IMapApprovalService.SendAsync`.
- Request/Response Models: [MapApprovalRequest](Models/MapApprovalRequest.cs) and nested block/class/unit/joint/owner/plan models; response is `JsonElement`, with no typed response model.
- Config keys: `MSB:BaseUrl`, `MSB:MapApprovalEndpoint`, `MSB:ApiKeyHeaderName`, `MSB:ApiKey`.
- Database persistence: none in this service.
- Invocation logging status: no outbound invocation logging in `MapApprovalService`.

## Official Documentation

[MSB-Map-Approval-OUT-v1.0-1405-03-31.pdf](Docs/MSB-Map-Approval-OUT-v1.0-1405-03-31.pdf)

## Verification Status

Implementation requires final verification

The service rejects `elzam14 = false` but does not validate the remaining documented field lengths, enumerations or CRS; geometry is passed through as `JsonElement` without CRS transformation. The PDF table spells `ActionRefrenceNo` and unit `Limitaion`, while the sample and models use `ActionReferenceNo` and `Limitation`. Geometry examples include both FeatureCollection and direct Polygon forms, and unit-plan identifiers appear as strings in the sample but numbers in the table/models. Confirm the accepted representation and runtime header before final verification; no response schema is invented.
