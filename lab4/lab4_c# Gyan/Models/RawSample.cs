using System;

namespace DistanceMonitor.Models;

public readonly record struct RawSample(DateTime Timestamp, int RawValue, byte Ms5, byte Ls5);
