using System;

namespace AridityTeam.Base;

public static class BaseFunctions
{
    public static void ImmediateCrash()
    {
        Environment.FailFast("Immediate crash.");
    }
    public static void ImmediateCrash(string msg)
    {
        Environment.FailFast("Immediate crash: " + msg);
    }
}