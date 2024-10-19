﻿using System;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.Draw;
using Dalamud.Utility.Numerics;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using Dalamud.Memory.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommons;
using System.Linq;
using ImGuiNET;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using KodakkuAssist.Module.GameOperate;

namespace Cyf5119Script
{
    [ScriptType(guid: "{C6AAF3DF-64BA-15C2-41F8-D24F7F4656DD}", name: "荒野秘境仙人刺谷", territorys: [1203], version: "0.0.0.1")]
    public class TenderValley
    {
        private uint 
            stack1id = 0,
            stack2id = 0,
            stack3id = 0;
        
        public void Init(ScriptAccessory accessory)
        {
            stack1id = 0;
            stack2id = 0;
            stack3id = 0;
        }
        
        private static bool ParseObjectId(string? idStr, out uint id)
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
        
        private static readonly (Vector3 Position, Vector2 Shape)[] AOEMap = 
        [(new(-112.5f, -167f, -486.5f), new(7.5f, 22f)), 
            (new(-147.5f, -167f, -471.5f), new(7.5f, 22f)),
            (new(-147.5f, -167f, -486.5f), new(7.5f, 12f)), 
            (new(-112.5f, -167f, -471.5f), new(7.5f, 12f))];

        private static Vector2 GetShape(Vector3 position)
        {
            foreach (var (pos, shape) in AOEMap)
                if (Math.Abs(position.X - pos.X) < 1 && Math.Abs(position.Z - pos.Z) < 1)
                    return shape;
            return new Vector2 (7.5f, 35f);
        }

        #region BOSS1

        [ScriptMethod(name: "老一AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37392"])]
        public void 老一AOE(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 5000);
        }
        
        [ScriptMethod(name: "老一死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39242"])]
        public void 老一死刑(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("死刑", duration: 5000);
        }
        
        [ScriptMethod(name: "老一分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37391"])]
        public void 老一分摊(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["TargetId"], out var tid))
            {
                dp.Owner = tid;
            }
            dp.Name = $"老一分摊";
            dp.Scale = new(6);
            dp.Color = new(0.2f, 0.8f, 0.2f, 0.4f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "老一扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37386"])]
        public void 老一扇形(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老一扇形";
            dp.Scale = new(36);
            dp.Radian = float.Pi / 180 * 50;
            dp.Color = new(0.2f, 0.2f, 0.8f, 1.6f);
            dp.DestoryAt = 6500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
        
        [ScriptMethod(name: "老一右扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39154"])]
        public void 老一右扇形(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老一右扇形";
            dp.Scale = new(36);
            dp.Radian = float.Pi / 180 * 330;
            dp.Rotation = float.Pi / -2;
            dp.Color = new(0.2f, 0.2f, 0.8f, 1.6f);
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
        
        [ScriptMethod(name: "老一左扇形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:5"])]
        public void 老一左扇形(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老一左扇形";
            dp.Scale = new(36);
            dp.Radian = float.Pi / 180 * 330;
            dp.Rotation = float.Pi / 2;
            dp.Color = new(0.2f, 0.2f, 0.8f, 1.6f);
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
        
        // 没发现actorspawn之类的东西 没法更早绘制
        [ScriptMethod(name: "老一大仙人球", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37389"])]
        public void 老一大仙人球(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老一大仙人球{sid:X}";
            dp.Scale = new(11);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        // [ScriptMethod(name: "老一大仙人球移除", eventType: EventTypeEnum.RemoveCombatant, eventCondition: ["DataId:2014193"],userControl:false)]
        // public void 老一大仙人球移除(Event @event, ScriptAccessory accessory)
        // {
        //     if (ParseObjectId(@event["SourceId"], out var id))
        //     {
        //         accessory.Method.RemoveDraw($"老一大仙人球{id:X}");
        //     }
        // }
        
        [ScriptMethod(name: "老一小仙人球", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37388"])]
        public void 老一小仙人球(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老一小仙人球{sid:X}";
            dp.Scale = new(6);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        // [ScriptMethod(name: "老一小仙人球移除", eventType: EventTypeEnum.RemoveCombatant, eventCondition: ["DataId:2014192"],userControl:false)]
        // public void 老一小仙人球移除(Event @event, ScriptAccessory accessory)
        // {
        //     if (ParseObjectId(@event["SourceId"], out var id))
        //     {
        //         accessory.Method.RemoveDraw($"老一小仙人球{id:X}");
        //     }
        // }
        
        [ScriptMethod(name: "老一击退预测", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37390"])]
        public void 老一击退预测(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "老一击退预测";
            dp.Scale = new(1.5f,20);
            dp.Color = new(0.2f, 0.8f, 0.8f, 1.6f);
            dp.Owner = accessory.Data.Me;
            dp.TargetPosition = new(-65f, -4f, 470f);
            dp.Rotation = float.Pi;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Displacement, dp);
        }

        #endregion

        #region BOSS2

        [ScriptMethod(name: "老二AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36557"])]
        public void 老二AOE(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 5000);
        }
        
        [ScriptMethod(name: "老二死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:38468"])]
        public void 老二死刑(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("死刑", duration: 5000);
        }
        
        [ScriptMethod(name: "老二分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36555"])]
        public void 老二分摊(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["TargetId"], out var tid))
            {
                dp.Owner = tid;
            }
            dp.Name = $"老二分摊";
            dp.Scale = new(6);
            dp.Color = new(0.2f, 0.8f, 0.2f, 0.4f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老二炸弹圆形一", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36401"])]
        public void 老二炸弹圆形一(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老二炸弹圆形一{sid:X}";
            dp.Scale = new(10);
            dp.Color = new(0.2f, 0.2f, 0.8f, 0.8f);
            dp.DestoryAt = 11500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老二炸弹圆形二", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36402"])]
        public void 老二炸弹圆形二(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老二炸弹圆形二{sid:X}";
            dp.Scale = new(10);
            dp.Color = new(0.2f, 0.2f, 0.8f, 0.8f);
            dp.DestoryAt = 8500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老二炸弹直线一", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36545"])]
        public void 老二炸弹直线一(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老二炸弹直线一{sid:X}";
            dp.Scale = new(6f, 40f);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 11500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        
        [ScriptMethod(name: "老二炸弹直线二", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36551"])]
        public void 老二炸弹直线二(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老二炸弹直线二{sid:X}";
            dp.Scale = new(6f, 40f);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 8500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        
        #endregion
        
        // 道中石像 log没有position 需要想办法拿
        [ScriptMethod(name: "道中石像", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0025"])]
        public void 道中石像(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var pos= JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
            dp.Owner = sid;
            dp.Name = $"道中石像";
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 8100;
            dp.Scale = GetShape(pos);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        #region BOSS3

        [ScriptMethod(name: "老三AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36748"])]
        public void 老三AOE(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 5000);
        }
        
        [ScriptMethod(name: "老三死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36744"])]
        public void 老三死刑(Event @event, ScriptAccessory accessory)
        {
            // 实际死刑是一个非读条技能 大约五秒多一些判定 约作5秒了
            accessory.Method.TextInfo("死刑", duration: 5000);
        }

        [ScriptMethod(name: "老三分摊id获取1", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:003E"], userControl: false)]
        public void 老三分摊id获取1(Event @event, ScriptAccessory accessory)
        {
            if (ParseObjectId(@event["TargetId"], out var tid))
            {
                stack1id = tid;
            }
        }
        
        [ScriptMethod(name: "老三分摊id获取2", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:021E"], userControl: false)]
        public void 老三分摊id获取2(Event @event, ScriptAccessory accessory)
        {
            if (ParseObjectId(@event["TargetId"], out var tid))
            {
                stack2id = tid;
            }
        }
        
        [ScriptMethod(name: "老三分摊id获取3", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:021F"], userControl: false)]
        public void 老三分摊id获取3(Event @event, ScriptAccessory accessory)
        {
            if (ParseObjectId(@event["TargetId"], out var tid))
            {
                stack3id = tid;
            }
        }
        
        // 这些分摊的判定也和死刑差不多
        [ScriptMethod(name: "老三分摊一", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36753"])]
        public void 老三分摊一(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if(stack1id == 0) return;
                dp.Owner = stack1id;
            dp.Name = $"老三分摊一";
            dp.Scale = new(6);
            dp.Color = new(0.2f, 0.8f, 0.2f, 0.4f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老三分摊二", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36752"])]
        public void 老三分摊二(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if(stack2id == 0) return;
            dp.Owner = stack2id;
            dp.Name = $"老三分摊二";
            dp.Scale = new(5);
            dp.Color = new(0.2f, 0.8f, 0.2f, 0.4f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老三分摊三", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36751"])]
        public void 老三分摊三(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if(stack3id == 0) return;
            dp.Owner = stack3id;
            dp.Name = $"老三分摊三";
            dp.Scale = new(4);
            dp.Color = new(0.2f, 0.8f, 0.2f, 0.4f);
            dp.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老三圆形", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36749"])]
        public void 老三圆形(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老三圆形{sid:X}";
            dp.Scale = new(9);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
        [ScriptMethod(name: "老三直线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36750"])]
        public void 老三直线(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.Owner = sid;
            }
            dp.Name = $"老三直线{sid:X}";
            dp.Scale = new(5f, 52f);
            dp.Color = new(0.8f, 0.2f, 0.2f, 0.8f);
            dp.DestoryAt = 7000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
        }
        
        [ScriptMethod(name: "老三击退预测", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:36756"])]
        public void 老三击退预测(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            if (ParseObjectId(@event["SourceId"], out var sid))
            {
                dp.TargetObject = sid;
            }
            dp.Name = "老三击退预测";
            dp.Scale = new(1.5f,15);
            dp.Color = new(0.2f, 0.8f, 0.8f, 1.6f);
            dp.Owner = accessory.Data.Me;
            dp.Rotation = float.Pi;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Displacement, dp);
        }
        
        [ScriptMethod(name: "老三迷宫安全区", eventType: EventTypeEnum.EnvControl, eventCondition: ["Index:00000001"])]
        public void 老三迷宫安全区(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            var id = @event["Id"];
            switch (id)
            {
                case "01000080":
                    dp.Position = new(-124f, -170f, -552f);
                    break;
                case "04000200":
                    dp.Position = new(-128f, -170f, -560f);
                    break;
                case "10000800":
                    dp.Position = new(-132f, -170f, -548f);
                    break;
                case "00020001":
                    dp.Position = new(-136f, -170f, -556f);
                    break;
                case "00100004" or "00200004" or "00400004" or "00080004":
                    accessory.Method.RemoveDraw("老三迷宫安全区");
                    return;
            }
            dp.Name = "老三迷宫安全区";
            dp.Scale = new(4);
            dp.Color = new(0.2f, 1f, 0.2f, 1.5f);
            dp.DestoryAt = 10000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
        }
        
        #endregion
        
    }
}
