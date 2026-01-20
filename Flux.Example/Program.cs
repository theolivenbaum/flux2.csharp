using System;
using Flux;

try
{
    Console.WriteLine("Flux.NET Example");

    // In a real scenario, you would have the model directory
    string modelDir = "flux-klein-model";

    if (!Directory.Exists(modelDir))
    {
        Console.WriteLine($"Warning: Model directory '{modelDir}' not found.");
        Console.WriteLine("This example will likely fail to load the model, but demonstrates the API usage.");
    }

    // Load the context
    // using var ctx = FluxContext.Load(modelDir);
    // Console.WriteLine("Model loaded successfully!");
    // Console.WriteLine($"Model Info: {ctx.GetModelInfo()}");

    // Configure parameters
    var paramsObj = new FluxParams
    {
        Width = 256,
        Height = 256,
        NumSteps = 4,
        Seed = 42
    };

    Console.WriteLine("Generating image with prompt: 'A beautiful sunset over the mountains'");
    // using var img = ctx.Generate("A beautiful sunset over the mountains", paramsObj);

    // Console.WriteLine($"Image generated: {img.Width}x{img.Height}x{img.Channels}");

    // Save the image
    // img.Save("output.png");
    // Console.WriteLine("Saved to output.png");

    Console.WriteLine($"Current Flux Error: {FluxContext.GetError()}");

    Console.WriteLine("\nAPI usage demonstration completed.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
