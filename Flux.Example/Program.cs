using System;
using System.IO;
using Flux;

try
{
    Console.WriteLine("Flux.NET Batch Image Generation");

    string modelDir = "flux-klein-model";
    string samplesDir = "samples";

    if (!Directory.Exists(samplesDir))
    {
        Directory.CreateDirectory(samplesDir);
    }

    if (!Directory.Exists(modelDir))
    {
        Console.WriteLine($"Error: Model directory '{modelDir}' not found.");
        Console.WriteLine("Please run 'python download_model.py' first.");
        return;
    }

    Console.WriteLine("Loading model...");
    using var ctx = FluxContext.Load(modelDir);
    Console.WriteLine("Model loaded successfully!");

    // Enable mmap for low memory systems
    Console.WriteLine("Enabling mmap mode...");
    ctx.SetMmap(true);

    string[] prompts = new[]
    {
        "A futuristic city with flying cars and neon lights",
        "A peaceful forest with a small stream and sunlight filtering through trees",
        "An astronaut riding a horse on Mars",
        "A cozy cabin in the snow with a warm glow from the windows",
        "A cyberpunk cat wearing high-tech goggles",
        "A majestic dragon perched on a mountain peak",
        "A surreal landscape with floating islands and purple skies",
        "A vintage portrait of a robot in Victorian clothing",
        "A colorful coral reef with exotic fish and clear blue water",
        "A steam-powered locomotive traveling through a desert at sunset"
    };

    var paramsObj = new FluxParams
    {
        Width = 256,
        Height = 256,
        NumSteps = 4
    };

    for (int i = 0; i < prompts.Length; i++)
    {
        string prompt = prompts[i];
        string filename = Path.Combine(samplesDir, $"sample_{i + 1}.png");

        Console.WriteLine($"[{i + 1}/{prompts.Length}] Generating: {prompt}");

        // Use a different seed for each image
        var currentParams = paramsObj with { Seed = 1000 + i };

        try
        {
            using var img = ctx.Generate(prompt, currentParams);
            img.Save(filename);
            Console.WriteLine($"Saved to {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate image {i + 1}: {ex.Message}");
            Console.WriteLine($"Flux Error: {FluxContext.GetError()}");

            // If we hit OOM, it's better to stop
            if (ex.Message.Contains("Out of memory") || FluxContext.GetError().Contains("memory"))
            {
                break;
            }
        }
    }

    Console.WriteLine("\nBatch generation completed.");
}
catch (Exception ex)
{
    Console.WriteLine($"Critical Error: {ex.Message}");
    Console.WriteLine($"Flux Error: {FluxContext.GetError()}");
}
