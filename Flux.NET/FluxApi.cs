using System;
using System.Runtime.InteropServices;

namespace Flux;

internal static partial class FluxApi
{
    private const string LibName = "flux";

    [StructLayout(LayoutKind.Sequential)]
    public struct FluxImageNative
    {
        public int Width;
        public int Height;
        public int Channels;
        public IntPtr Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FluxParams
    {
        public int Width;
        public int Height;
        public int NumSteps;
        public float GuidanceScale;
        public long Seed;
        public float Strength;

        public static FluxParams Default => new FluxParams
        {
            Width = 256,
            Height = 256,
            NumSteps = 4,
            GuidanceScale = 1.0f,
            Seed = -1,
            Strength = 0.75f
        };
    }

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr flux_load_dir(string modelDir);

    [LibraryImport(LibName)]
    public static partial void flux_free(IntPtr ctx);

    [LibraryImport(LibName)]
    public static partial void flux_release_text_encoder(IntPtr ctx);

    [LibraryImport(LibName)]
    public static partial void flux_set_mmap(IntPtr ctx, int enable);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr flux_generate(IntPtr ctx, string prompt, in FluxParams params_ptr);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr flux_img2img(IntPtr ctx, string prompt, IntPtr input, in FluxParams params_ptr);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr flux_image_load(string path);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int flux_image_save(IntPtr img, string path);

    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int flux_image_save_with_seed(IntPtr img, string path, long seed);

    [LibraryImport(LibName)]
    public static partial IntPtr flux_image_create(int width, int height, int channels);

    [LibraryImport(LibName)]
    public static partial void flux_image_free(IntPtr img);

    [LibraryImport(LibName)]
    public static partial IntPtr flux_image_resize(IntPtr img, int newWidth, int newHeight);

    [LibraryImport(LibName)]
    public static partial void flux_set_seed(long seed);

    [LibraryImport(LibName)]
    public static partial IntPtr flux_model_info(IntPtr ctx);

    [LibraryImport(LibName)]
    public static partial IntPtr flux_get_error();
}
