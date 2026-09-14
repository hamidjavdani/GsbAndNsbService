# MSB test checklist

1. Put the test API key in `MSB:ApiKey` in the runtime configuration.
2. Call local endpoint `POST /api/msb/document-verification-inquiry` with the documented 10-field request body.
3. The service sends JSON directly to MSB without the old GSB encryption wrapper.
4. MSB callback is accepted at `POST /document-ownership-verification/create`.
5. Successful callbacks return the documented MSB acknowledgement shape.
