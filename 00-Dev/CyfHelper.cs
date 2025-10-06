using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Data;
using KodakkuAssist.Extensions;

namespace Cyf5119Script.Dev.CyfHelper;

[ScriptType(guid: "FFA80AC9-E88E-437F-B303-5776423CFCD5", name: "CyfHelper", territorys: [], version: "0.0.0.2", author: "Cyf5119", note: "")]
public class CyfHelper
{
    // [PluginService] public static IClientState ClientState { get; private set; }
    public void Init(ScriptAccessory accessory)
    {
    }
    
    [ScriptMethod(name: "测试方法", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo"])]
    public void Test(Event evt, ScriptAccessory sa)
    {
        if (evt["Message"] == "clear")
        {
            sa.Method.RemoveDraw(".*");
        }
        else if (evt["Message"] == "test")
        {
            // sa.Method.SendChat($"ClientState.TerritoryType is {ClientState.TerritoryType}");
        }
        else if (evt["Message"] == "uninit")
        {
        }
    }
}
