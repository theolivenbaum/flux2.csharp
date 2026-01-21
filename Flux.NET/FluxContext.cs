using System;
using System.Runtime.InteropServices;

namespace Flux;

public class FluxContext : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    private FluxContext(IntPtr handle)
    {
        _handle = handle;
    }

    public static FluxContext Load(string modelDir)
    {
        IntPtr handle = FluxApi.flux_load_dir(modelDir);
        if (handle == IntPtr.Zero)
        {
            throw new Exception($"Failed to load model: {GetError()}");
        }
        return new FluxContext(handle);
    }

    public void ReleaseTextEncoder()
    {
        CheckDisposed();
        FluxApi.flux_release_text_encoder(_handle);
    }

    public void SetMmap(bool enable)
    {
        CheckDisposed();
        FluxApi.flux_set_mmap(_handle, enable ? 1 : 0);
    }

    public FluxImage Generate(string prompt, FluxParams? parameters = null)
    {
        CheckDisposed();
        var p = parameters ?? FluxParams.Default;
        var nativeParams = p.ToNative();
        IntPtr imgHandle = FluxApi.flux_generate(_handle, prompt, in nativeParams);
        if (imgHandle == IntPtr.Zero)
        {
            throw new Exception($"Generation failed: {GetError()}");
        }
        return new FluxImage(imgHandle);
    }

    public FluxImage Img2Img(string prompt, FluxImage input, FluxParams? parameters = null)
    {
        CheckDisposed();
        var p = parameters ?? FluxParams.Default;
        var nativeParams = p.ToNative();
        IntPtr imgHandle = FluxApi.flux_img2img(_handle, prompt, input.Handle, in nativeParams);
        if (imgHandle == IntPtr.Zero)
        {
            throw new Exception($"Img2Img failed: {GetError()}");
        }
        return new FluxImage(imgHandle);
    }

    public string GetModelInfo()
    {
        CheckDisposed();
        IntPtr ptr = FluxApi.flux_model_info(_handle);
        return Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
    }

    public static void SetSeed(long seed)
    {
        FluxApi.flux_set_seed(seed);
    }

    public static string GetError()
    {
        IntPtr ptr = FluxApi.flux_get_error();
        return Marshal.PtrToStringUTF8(ptr) ?? "Unknown error";
    }

    private void CheckDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(FluxContext));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_handle != IntPtr.Zero)
            {
                FluxApi.flux_free(_handle);
                _handle = IntPtr.Zero;
            }
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~FluxContext()
    {
        Dispose();
    }
}

public record struct FluxParams
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int NumSteps { get; init; }
    public float GuidanceScale { get; init; }
    public long Seed { get; init; }
    public float Strength { get; init; }

    public FluxParams()
    {
        Width = 256;
        Height = 256;
        NumSteps = 4;
        GuidanceScale = 1.0f;
        Seed = -1;
        Strength = 0.75f;
    }

    public static FluxParams Default => new FluxParams();

    internal FluxApi.FluxParams ToNative() => new FluxApi.FluxParams
    {
        Width = Width,
        Height = Height,
        NumSteps = NumSteps,
        GuidanceScale = GuidanceScale,
        Seed = Seed,
        Strength = Strength
    };
}
