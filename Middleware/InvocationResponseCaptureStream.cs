using GSB.Test.Api.Services;

namespace GSB.Test.Api.Middleware;

/// <summary>Forwards bytes immediately; only a bounded copy is retained for logging.</summary>
internal sealed class InvocationResponseCaptureStream(Stream inner) : Stream
{
    private readonly MemoryStream _capture = new();
    private bool _overflow;
    public string? CapturedBody => _overflow
        ? new string(' ', InvocationLogSanitizer.BodyLimit + 1)
        : _capture.Length == 0 ? null : System.Text.Encoding.UTF8.GetString(_capture.ToArray());

    private void Capture(ReadOnlySpan<byte> buffer)
    {
        if (_overflow) return;
        if (_capture.Length + buffer.Length > InvocationLogSanitizer.BodyLimit)
        {
            _overflow = true;
            _capture.SetLength(0);
            return;
        }
        _capture.Write(buffer);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
        Capture(buffer.AsSpan(offset, count));
    }
    public override void Write(ReadOnlySpan<byte> buffer) { inner.Write(buffer); Capture(buffer); }
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        await inner.WriteAsync(buffer, offset, count, cancellationToken);
        Capture(buffer.AsSpan(offset, count));
    }
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await inner.WriteAsync(buffer, cancellationToken);
        Capture(buffer.Span);
    }
    public override void Flush() => inner.Flush();
    public override Task FlushAsync(CancellationToken cancellationToken) => inner.FlushAsync(cancellationToken);
    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => inner.CanSeek;
    public override bool CanWrite => inner.CanWrite;
    public override long Length => inner.Length;
    public override long Position { get => inner.Position; set => inner.Position = value; }
    public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);
    public override void SetLength(long value) => inner.SetLength(value);
    protected override void Dispose(bool disposing) { if (disposing) _capture.Dispose(); base.Dispose(disposing); }
}
