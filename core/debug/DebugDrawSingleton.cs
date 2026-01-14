using System;
using System.Collections.Generic;
using Godot;

namespace emergentgameplay.core.debug;

public sealed class DebugDrawSingleton
{
    private static readonly Lazy<DebugDrawSingleton> lazy =
        new Lazy<DebugDrawSingleton>(() => new DebugDrawSingleton());

    public static DebugDrawSingleton Instance { get { return lazy.Value; } }

    private DebugDrawSingleton()
    {
    }
    
    public Dictionary<string, VectorToDraw> vectorsToDraw = new();
    
    public void UpdateVectorToDraw(string key, Vector2 start, Vector2 end, Color? color = null)
    {
        vectorsToDraw[key] = new VectorToDraw(key, start, end, color);
    }
}