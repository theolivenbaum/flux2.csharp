using System;
using System.Runtime.InteropServices;

namespace Flux;

public class FluxImage : IDisposable
{
    internal IntPtr Handle { get; private set; }
    private bool _disposed;

    internal FluxImage(IntPtr handle)
    {
        Handle = handle;
    }

    public static FluxImage Load(string path)
    {
        IntPtr handle = FluxApi.flux_image_load(path);
        if (handle == IntPtr.Zero)
        {
            throw new Exception($"Failed to load image: {path}");
        }
        return new FluxImage(handle);
    }

    public static FluxImage Create(int width, int height, int channels)
    {
        IntPtr handle = FluxApi.flux_image_create(width, height, channels);
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Failed to create image");
        }
        return new FluxImage(handle);
    }

    public void Save(string path)
    {
        CheckDisposed();
        if (FluxApi.flux_image_save(Handle, path) != 0)
        {
            throw new Exception($"Failed to save image to {path}");
        }
    }

    public void SaveWithSeed(string path, long seed)
    {
        CheckDisposed();
        if (FluxApi.flux_image_save_with_seed(Handle, path, seed) != 0)
        {
            throw new Exception($"Failed to save image to {path} with seed");
        }
    }

    public FluxImage Resize(int newWidth, int newHeight)
    {
        CheckDisposed();
        IntPtr newHandle = FluxApi.flux_image_resize(Handle, newWidth, newHeight);
        if (newHandle == IntPtr.Zero)
        {
            throw new Exception("Failed to resize image");
        }
        return new FluxImage(newHandle);
    }

    public int Width
    {
        get
        {
            CheckDisposed();
            return Marshal.PtrToStructure<FluxApi.FluxImageNative>(Handle).Width;
        }
    }

    public int Height
    {
        get
        {
            CheckDisposed();
            return Marshal.PtrToStructure<FluxApi.FluxImageNative>(Handle).Height;
        }
    }

    public int Channels
    {
        get
        {
            CheckDisposed();
            return Marshal.PtrToStructure<FluxApi.FluxImageNative>(Handle).Channels;
        }
    }

    public Span<byte> GetData()
    {
        CheckDisposed();
        var native = Marshal.PtrToStructure<FluxApi.FluxImageNative>(Handle);
        unsafe
        {
            return new Span<byte>((void*)native.Data, native.Width * native.Height * native.Channels);
        }
    }

    private void CheckDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(FluxImage));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (Handle != IntPtr.Zero)
            {
                FluxApi.flux_image_free(Handle);
                Handle = IntPtr.Zero;
            }
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~FluxImage()
    {
        Dispose();
    }
}
