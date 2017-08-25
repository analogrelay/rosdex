// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// GLOBAL NAMESPACE! WOOOOO!

internal struct HashCode
{
    private long _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private HashCode(long value)
    {
        _value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(IEnumerable e)
    {
        if (e == null)
        {
            return Add(0);
        }
        else
        {
            var count = 0;
            var code = this;
            foreach (object o in e)
            {
                code = code.Add(o);
                count++;
            }
            return code.Add(count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(HashCode self) => self._value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(int i) => new HashCode(((_value << 5) + _value) ^ i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(string s) => Add((s != null) ? s.GetHashCode() : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add(object o) => Add((o != null) ? o.GetHashCode() : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashCode Add<TValue>(TValue value, IEqualityComparer<TValue> comparer) => Add(value != null ? comparer.GetHashCode(value) : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashCode New() => new HashCode(0x1505L);
}
