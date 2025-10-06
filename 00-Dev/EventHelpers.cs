using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
// using ECommons;
// using ECommons.DalamudServices;
// using ECommons.GameFunctions;
// using ECommons.MathHelpers;
using FFXIVClientStructs;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Scripts.Dev.EventHelpers;

// [ScriptType(guid: "75CBEA39-966A-F902-B085-46D6E4A98E4B", name: "EventHelpers", territorys: [], version: "0.0.0.1", author: "Cyf5119")]
public class EventHelpersTests
{
    public void Init(ScriptAccessory accessory)
    {
    }

    [ScriptMethod(name: "Test1", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(37010|133)$"])]
    public void Test1(Event @event, ScriptAccessory accessory)
    {
        Events.StartCastingEvent e = new(@event);
        e.ActionId();
    }
    
    [ScriptMethod(name: "Test2", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(24286)$"])]
    public void Test2(Event @event, ScriptAccessory accessory)
    {
    }
}

public class Events
{
    public class StartCastingEvent
    {
        // public uint ActionId;
        private Event @event;
        
        public StartCastingEvent(Event @event)
        {
             // ActionId = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
             this.@event = @event;
        }
        
        public uint ActionId() => GetActionId(this.@event);
    }
    
    
    private static uint GetActionId(Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
    }
}
