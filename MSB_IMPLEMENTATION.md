# MSB document verification implementation

Implemented on branch `feature/msb-document-verification`.

- Outbound inquiry uses the MSB contract and sends the request body directly as JSON.
- The API key header name is configurable and defaults to `X-MSB-Api-Key`.
- Callback endpoint is exposed at `/document-ownership-verification/create` while the legacy callback route remains available.
- Callback root supports `organId`, `owTrakingCode`, `code`, `data`, and `error`.
- Callback error supports `errorMessage` and `errorCode`.
- `GetKmlPolygonInfo` is accepted as flexible JSON so both object and string forms can be received.
- Callback acknowledgement follows the documented `msbTrackingCode`, `code`, `message`, `description`, `timestamp` shape.

The test API key is intentionally not duplicated in this documentation file. Configure `MSB:ApiKey` in the runtime `appsettings.json`.
