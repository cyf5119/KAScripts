using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using ECommons;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;

namespace Cyf5119Scripts.Endwalker.TheAetherfont;

[ScriptType(guid: "554C2F0D-3E58-14B1-237A-2CA662AC229E", name: "间歇灵泉哈姆岛", territorys: [1126], version: "0.0.0.1", author: "Cyf5119")]
public class TheAetherfont
{
    public void Init(ScriptAccessory sa)
    {
        sa.Method.RemoveDraw(".*");
    }

    #region BOSS1

    [ScriptMethod(name: "B1-AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(3333[58])$"])]
    public void Boss1AOE(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", (int)(evt.DurationMilliseconds() + 300));
    }
    
    [ScriptMethod(name: "B1-死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33345"])]
    public void Boss1Tankbuster(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("死刑", 5000);
    }
    
    [ScriptMethod(name: "B1-分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33343"])]
    public void Boss1Stack(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B1-分摊", evt.TargetId(), 5000, 6, true);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "B1-小冰", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33339"])]
    public void ResonantFrequency(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B1-小冰", evt.SourceId(), 5000, 8);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "B1-大冰", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33340"])]
    public void ExplosiveFrequency(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B1-大冰", evt.SourceId(), 10000, 15);
        if (sa.Data.Objects.Any(x => x is IBattleChara y && y.CastActionId == 33339))
        {
            dp.Delay = 5000;
            dp.DestoryAt = 5000;
        }

        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "B1-扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33344"])]
    public void TidalBreath(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B1-扇形", evt.SourceId(), 5000, 40);
        dp.Radian = float.Pi;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }
    
    #endregion


    #region BOOS2

    [ScriptMethod(name: "B2-AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(34605|33364|33615)$"])]
    public void Boss2AOE(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", 5000);
    }
    
    [ScriptMethod(name: "B2-死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33368"])]
    public void RipperClaw(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("死刑", 5000);
    }
    
    [ScriptMethod(name: "B2-直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33363"])]
    public void SpunLightning(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B2-直线", evt.SourceId(), 3500, new Vector2(8, 30));
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    
    private static readonly Dictionary<uint, (Vector2[] Start, Vector2[] End)> Patterns = new()
    {
        [4] = (new Vector2[]
        {
            new(419.293f, -445.661f), new(422.919f, -448.675f), new(419.359f, -445.715f), new(419.333f, -437.25f),
            new(432.791f, -434.82f), new(423.239f, -442.489f), new(426.377f, -437.596f), new(419.335f, -445.663f),
            new(417.162f, -442.421f), new(427.274f, -448.618f), new(430.839f, -441.877f), new(419.292f, -445.596f),
            new(427.482f, -448.548f), new(419.101f, -434.242f), new(424.274f, -433.427f), new(419.326f, -445.681f)
        }, new Vector2[]
        {
            new(420.035f, -454.124f), new(427.42f, -448.692f), new(412.039f, -447.562f), new(417.533f, -427.085f),
            new(433.860f, -427.97f), new(426.993f, -437.034f), new(433.646f, -433.433f), new(411.276f, -434.165f),
            new(419.394f, -436.118f), new(430.442f, -453.971f), new(439.872f, -438.59f), new(423.667f, -442.039f),
            new(431.815f, -441.032f), new(425.437f, -432.547f), new(428.824f, -425.528f), new(424.002f, -448.966f)
        }),
        [3] = (new Vector2[]
        {
            new(419.343f, -434.343f), new(416.325f, -437.829f), new(419.304f, -434.353f), new(427.97f, -434.253f),
            new(430.244f, -447.772f), new(422.523f, -438.223f), new(427.408f, -441.363f), new(419.274f, -434.245f),
            new(422.582f, -432.152f), new(416.35f, -442.222f), new(423.09f, -445.755f), new(419.412f, -434.285f),
            new(419.294f, -434.309f), new(416.47f, -442.448f), new(430.633f, -433.69f), new(431.389f, -439.114f)
        }, new Vector2[]
        {
            new(410.880f, -435.019f), new(416.312f, -442.557f), new(417.441f, -427.085f), new(437.949f, -432.547f),
            new(436.942f, -448.875f), new(428.031f, -442.039f), new(431.571f, -448.63f), new(430.9f, -426.261f),
            new(428.916f, -434.379f), new(411.032f, -445.457f), new(426.413f, -454.917f), new(422.934f, -438.59f),
            new(416.037f, -438.926f), new(423.941f, -446.738f), new(432.364f, -440.177f), new(439.475f, -443.809f)
        }),
        [2] = (new Vector2[]
        {
            new(430.635f, -434.592f), new(430.708f, -434.484f), new(434.518f, -440.005f), new(424.457f, -445.105f),
            new(430.834f, -434.374f), new(431.156f, -439.05f), new(430.599f, -434.383f), new(434.571f, -440.454f),
            new(423.033f, -437.371f), new(422.069f, -437.329f), new(419.287f, -441.464f), new(430.513f, -434.548f),
            new(417.501f, -435.027f), new(431.252f, -446.301f), new(430.458f, -434.36f), new(425.28f, -430.49f)
        }, new Vector2[]
        {
            new(439.139f, -435.325f), new(422.232f, -437.583f), new(431.083f, -446.983f), new(420.279f, -454.215f),
            new(429.831f, -440.299f), new(424.063f, -445.762f), new(434.379f, -440.269f), new(439.811f, -441.733f),
            new(411.612f, -433.28f), new(418.936f, -441.794f), new(412.07f, -447.532f), new(424.308f, -430.228f),
            new(410.025f, -440.269f), new(430.594f, -453.88f), new(429.587f, -425.834f), new(414.298f, -429.557f)
        }),
        [1] = (new Vector2[]
        {
            new(430.357f, -445.557f), new(434.507f, -440.159f), new(430.357f, -445.557f), new(424.561f, -449.554f),
            new(425.887f, -446.107f), new(423.516f, -434.294f), new(430.346f, -445.616f), new(419.902f, -439.485f),
            new(430.357f, -445.557f), new(430.404f, -445.54f), new(429.973f, -432.501f), new(427.648f, -437.101f),
            new(430.357f, -445.557f), new(427.648f, -438.04f), new(424.713f, -449.483f), new(418.756f, -446.251f)
        }, new Vector2[]
        {
            new(439.2f, -444.602f), new(435.416f, -429.313f), new(429.618f, -454.246f), new(423.24f, -454.887f),
            new(419.303f, -439.078f), new(417.441f, -427.054f), new(424.704f, -444.846f), new(410.757f, -435.294f),
            new(424.643f, -449.577f), new(427.451f, -437.247f), new(424.796f, -425.132f), new(423.148f, -433.951f),
            new(434.867f, -439.109f), new(431.693f, -426.627f), new(418.051f, -446.036f), new(411.063f, -445.579f)
        })
    };

    [ScriptMethod(name: "B2-雷线", eventType: EventTypeEnum.EnvControl, eventCondition: ["Id:regex:^(00020001|00200010)$"])]
    public void ForkedFissures(Event evt, ScriptAccessory sa)
    {
        if (Patterns.TryGetValue(evt.Index(), out var pattern))
        {
            var starts = pattern.Start;
            var ends = pattern.End;
            for (var i = starts.Length - 1; i >= 0; --i)
            {
                var start = starts[i];
                var end = ends[i];
                var dp = sa.FastDp("雷线", new Vector3(start.X, 20, start.Y), 6000, 4);
                dp.ScaleMode = ScaleMode.YByDistance;
                dp.TargetPosition = new Vector3(end.X, 20, end.Y);
                sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
        }
    }
    
    #endregion


    #region BOSS3

    [ScriptMethod(name: "B3-AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33356"])]
    public void Boss3AOE(Event evt, ScriptAccessory sa)
    {
        sa.Method.TextInfo("AOE", 5000);
    }
    
    [ScriptMethod(name: "B3-扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33348"])]
    public void Clearout(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B3-扇形", evt.SourceId(), 9000, 16);
        dp.Radian = float.Pi / 3 * 2;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }
    
    [ScriptMethod(name: "B3-小钢铁", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33353"])]
    public void SalineSpit(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B3-小钢铁", evt.SourceId(), 6000, 8);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "B3-大钢铁", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:33351"])]
    public void Telekinesis(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B3-大钢铁", evt.SourceId(), 10000, 12);
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
    
    [ScriptMethod(name: "B3-吐息", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(34551|33354)$"])]
    public void Breath(Event evt, ScriptAccessory sa)
    {
        var dp = sa.FastDp("B3-吐息", evt.SourceId(), evt.ActionId() > 33354 ? 16500u : 10000u, 35);
        dp.Radian = float.Pi;
        sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    #endregion
    
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

    public static uint ActionId(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["ActionId"]);
    }

    public static uint SourceId(this Event evt)
    {
        return ParseHexId(evt["SourceId"], out var id) ? id : 0;
    }

    public static uint TargetId(this Event evt)
    {
        return ParseHexId(evt["TargetId"], out var id) ? id : 0;
    }

    public static uint DurationMilliseconds(this Event evt)
    {
        return JsonConvert.DeserializeObject<uint>(evt["DurationMilliseconds"]);
    }

    public static uint Index(this Event evt)
    {
        return ParseHexId(evt["Index"], out var id) ? id : 0;
    }
}

public static class ScriptAccessoryExtensions
{
    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, uint owner, uint duration, float radius, bool safe = false)
    {
        return FastDp(sa, name, owner, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, uint owner, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, Vector3 pos, uint duration, float radius, bool safe = false)
    {
        return FastDp(sa, name, pos, duration, new Vector2(radius), safe);
    }

    public static DrawPropertiesEdit FastDp(this ScriptAccessory sa, string name, Vector3 pos, uint duration, Vector2 scale, bool safe = false)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = safe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.DestoryAt = duration;
        dp.Scale = scale;
        return dp;
    }
}