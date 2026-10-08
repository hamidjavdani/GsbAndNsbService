# Unique Identifier Response

## Overview

- Direction: Inbound
- Provider: MSB
- Document Version: 1.0
- Document Date: 1405/06/24 (Solar Hijri)
- MSB Service ID: `made14-SUI-1405/6/24-1.0` (PDF p. 3)
- HTTP Method: `POST`
- Base URL: Not specified in official document (the curl example uses `http://{shahrdari-BaseURL}` as a placeholder).
- Endpoint: `/made14/send-unique-identifier-response/v1/create`
- Authentication/Header: official `X-MSB-Api-Key`; implementation uses `MSB:ApiKeyHeaderName`, locally `msb-identifier`, and `MSB:ApiKey`.
- Content-Type: `application/json`

## Contract

PDF pp. 5–6 require `organId`, `data` (`actionId`, `ruleId`, `activityId`, `requestId`, `code`, `token`, `taskId`), `mapConfirmationTrackingCode`, `landData` and `userInfo`.

Land entries include `uniqueIdentifier`, `unitBlockNumber`, `unitNumber`, numeric `warehouseNumberList`/`parkingNumberList`, and `otherAttachmentsList` with `attachmentsType` and an attachment number. User entries contain `name`, `family`, `usertype`, identity fields, and numeric `totalShare`, `shareOf`, `landArseh`, `landAyan`.

Response (p. 7): required `status[]` containing `code` and `message`; optional `data.msbTrackingCode` and integer `data.timestamp`. Codes `100`–`107` cover success, missing tracking code, duplicate tracking code, invalid/incomplete/mismatched data, inability to record a result, and server error.

## Fixed Values

- Fixed RuleId, noToken, elzam14: Not specified in official document
- `data.ruleId` is required workflow input; the sample's value is not declared a fixed service rule.

## Implementation

- Controller: [UniqueIdentifierResponseController](Controllers/UniqueIdentifierResponseController.cs).
- Service: [UniqueIdentifierCallbackService](Services/UniqueIdentifierCallbackService.cs), `IUniqueIdentifierCallbackService`.
- Request/Response Models: `UniqueIdentifierResponseRequest`, workflow/land/attachment/user models, `UniqueIdentifierResponseEnvelope`, status/data models in [UniqueIdentifierResponseModels.cs](Models/UniqueIdentifierResponseModels.cs).
- Config keys: `MSB:ApiKeyHeaderName`, `MSB:ApiKey`, `UseInMemoryDatabase`, `ConnectionStrings:DefaultConnection`.
- Database persistence: `UniqueIdentifierCallbacks` stores workflow identifiers, map tracking code, reserialized request JSON and creation time.
- Invocation logging status: inbound middleware captures this route as `UniqueIdentifierResponse` in sanitized `MsbInvocationLogs`; tokens are masked in invocation logs.

## Official Documentation

[MSB-Unique-Identifier-Response-IN-v1.0-1405-06-24.pdf](Docs/MSB-Unique-Identifier-Response-IN-v1.0-1405-06-24.pdf)

## Verification Status

Implementation requires final verification

The PDF marks both `nationalCode` and `nationalId` mandatory, but its samples supply one according to user type; the controller accepts either. The PDF table uses `AttachmentsNumber`, its sample and model use `attachmentsNumber`. Per-land `mapConfirmationTrackingCode` appears only in the sample and is optional in the model. The response table labels status values as JSON, while the implementation emits integer codes and string messages.

The controller emits codes `100`, `103`, `104`, `107` and does not implement tracking-code lookup, duplicate detection or mismatch checks for `101`, `102`, `105`, `106`. Confirm these gaps and the runtime header with MSB. Masking applies to invocation logs; business `RawJson` is stored separately and is not passed through the invocation sanitizer.
