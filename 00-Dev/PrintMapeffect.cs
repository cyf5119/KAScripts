using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace Cyf5119Script.Dev.PrintMapeffect;

[ScriptType(guid: "3339DE09-2AEA-4D40-A579-400364F87A73", name: "PrintMapeffect", territorys: [], version: "0.0.0.1", author: "Cyf5119")]
public class PrintMapeffect
{
    [ScriptMethod(name: "Mapeffect", eventType: EventTypeEnum.EnvControl)]
    public void Mapeffect(Event @event, ScriptAccessory accessory)
    {
        var index = @event["Index"];
        var id = @event["Id"];
        accessory.Method.SendChat($"/e Mapeffect: Index: {index}, Id: {id}. ");
    }
}