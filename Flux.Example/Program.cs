using System;
using Flux;

try
{
    Console.WriteLine("Flux.NET Example");

    string modelDir = "flux-klein-model";

    if (!Directory.Exists(modelDir))
    {
        Console.WriteLine($"Error: Model directory '{modelDir}' not found.");
        Console.WriteLine("Please run 'python download_model.py' first.");
        return;
    }

    // Load the context
    Console.WriteLine("Loading model...");
    using var ctx = FluxContext.Load(modelDir);
    Console.WriteLine("Model loaded successfully!");
    Console.WriteLine($"Model Info: {ctx.GetModelInfo()}");

    // Configure parameters
    var paramsObj = new FluxParams
    {
        Width = 256,
        Height = 256,
        NumSteps = 4,
        Seed = 42
    };

    Console.WriteLine("Generating image with prompt: 'A beautiful sunset over the mountains'");
    using var img = ctx.Generate("A beautiful sunset over the mountains", paramsObj);

    Console.WriteLine($"Image generated: {img.Width}x{img.Height}x{img.Channels}");

    // Save the image
    img.Save("output.png");
    Console.WriteLine("Saved to output.png");

    Console.WriteLine("\nFlux.NET example completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Flux Error: {FluxContext.GetError()}");
}
