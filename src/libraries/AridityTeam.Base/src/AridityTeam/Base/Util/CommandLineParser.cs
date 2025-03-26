/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 */

namespace AridityTeam.Base.Util;

public class CommandLineParser
{
    private string[] _args;

    public CommandLineParser(string[] args)
    {
        _args = args;
    }

    public bool FindParm(string parm)
    {
        foreach (var arg in _args)
            return arg.Equals(parm);
        return false;
    }
    public int GetParm(string parm)
    {
        foreach (var arg in _args)
            return arg.IndexOf(parm);
        return -1;
    }

    public object? GetParmValue(int index)
    {
        if (index < 0 || index >= _args.Length) return null;
        string input = _args[index].Substring(0).Trim();
        string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2);
        return parts[0].Trim();
    }
}