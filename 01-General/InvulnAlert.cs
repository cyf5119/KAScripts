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

namespace Cyf5119Script.General.InvulnAlert;

[ScriptType(guid: "FCDCA220-DA96-48B5-909B-C1E336671D54", name: "无敌提示宏", territorys: [], version: "0.0.0.2", author: "Cyf5119", note: "自身无敌倒计时提示。")]
public class InvulnAlert
{
    [UserSetting("频道")] public string channel { get; set; } = "e";
    [UserSetting("提示音")] public string se { get; set; } = "<se.1><se.1>";

    private uint LastRunTime = 0;
    
    [ScriptMethod(name: "神圣领域", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:82"])]
    public void HallowedGround(Event @event, ScriptAccessory accessory)
    {
        if(@event.SourceId() != accessory.Data.Me) return;
        Alert(accessory, "神圣领域");
    }
    
    [ScriptMethod(name: "死斗", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:409"])]
    public void Holmgang(Event @event, ScriptAccessory accessory)
    {
        if(@event.SourceId() != accessory.Data.Me) return;
        Alert(accessory, "死斗");
    }
    
    [ScriptMethod(name: "行尸走肉", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:810"])]
    public void LivingDead(Event @event, ScriptAccessory accessory)
    {
        if(@event.SourceId() != accessory.Data.Me) return;
        accessory.Method.SendChat($"/e 行尸走肉已开，请适当放生！{se}");
    }
        
    [ScriptMethod(name: "死而不僵", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:811"])]
    public void WalkingDead(Event @event, ScriptAccessory accessory)
    {
        if(@event.SourceId() != accessory.Data.Me) return;
        Alert(accessory, "死而不僵");
    }
    
    [ScriptMethod(name: "超火流星", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:1836"])]
    public void Superbolide(Event @event, ScriptAccessory accessory)
    {
        if(@event.SourceId() != accessory.Data.Me) return;
        Alert(accessory, "超火流星");
    }

    private void Alert(ScriptAccessory accessory, string str)
    {
        accessory.Method.SendChat($"/{channel} {str}已生效！{se}");
        Thread.Sleep(7000);
        accessory.Method.SendChat($"/{channel} {str}剩余三秒！");
        Thread.Sleep(1000);
        accessory.Method.SendChat($"/{channel} {str}剩余二秒！");
        Thread.Sleep(1000);
        accessory.Method.SendChat($"/{channel} {str}剩余一秒！");
        Thread.Sleep(1000);
        accessory.Method.SendChat($"/{channel} {str}已结束！{se}");
    }
}

public static class EventExtensions
{
    private static bool ParseHexId(string? idStr, out uint id)
    {
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;
        try
        {
            var idStr2 = idStr.Replace("0x", "");
            id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static uint SourceId(this Event @event)
    {
        return ParseHexId(@event["SourceId"], out var id) ? id : 0;
    }
}