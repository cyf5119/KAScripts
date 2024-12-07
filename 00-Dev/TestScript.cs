using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace TestScript;

[ScriptType(guid: "C0C67BDA-B6B3-411D-8C7F-C31613633333", name: "TestScript1", territorys: [], version: "0.0.0.1", author: "YourName", note: "这是一条消息")]
public class TestScript1
{
    [ScriptMethod(name: "TestMethod1", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37010"])]
    public void TestMethod1(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.SendChat("/e test1");
    }
}

[ScriptType(guid: "C0C67BDA-B6B3-411D-8C7F-C31613633334", name: "TestScript2", territorys: [], version: "0.0.0.1", author: "YourName", note: "这是一条消息")]
public class TestScript2
{
    [ScriptMethod(name: "TestMethod2", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37013"])]
    public void TestMethod2(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.SendChat("/e test2");
    }
}

[ScriptType(guid: "C0C67BDA-B6B3-411D-8C7F-C31613633335", name: "TestScript3", territorys: [], version: "0.0.0.1", author: "YourName", note: "这是一条消息")]
public class TestScript3
{
    [ScriptMethod(name: "TestMethod3", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37030"])]
    public void TestMethod3(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.SendChat("/e test3");
    }
}

[ScriptType(guid: "C0C67BDA-B6B3-411D-8C7F-C31613633336", name: "TestScript4", territorys: [], version: "0.0.0.1", author: "YourName", note: "这是一条消息")]
public class TestScript4
{
    [ScriptMethod(name: "TestMethod4", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:24286"])]
    public void TestMethod4(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.SendChat("/e test4");
    }
}
