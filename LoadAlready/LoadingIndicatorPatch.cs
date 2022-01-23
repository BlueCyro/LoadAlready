using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;

namespace LoadAlready
{
    public class LoadingIndicatorPatcher : NeosMod
    {
        public override string Author => "Cyro";
        public override string Name => "LoadAlready!";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.cyro.LoadAlready!");
            harmony.PatchAll();
        }
        [HarmonyPatch(typeof(LoadingIndicator), "OnAttach")]
        class LoadingPatcher
        {


            static void IndicatorPress(IButton button, ButtonEventData eventData, ref double lastPress, LoadingIndicator instance, TextRenderer Text, ref double num)
            {
                float pressDelay = 0.35f;
                num = instance.Time.WorldTime - lastPress;
                if (num > pressDelay)
                {
                    Text.Color.TweenFromTo(color.Red, color.White, pressDelay);
                    lastPress = instance.Time.WorldTime;
                }
                else
                {
                    instance.Slot.Destroy();
                }
                
            }

            static void Postfix(LoadingIndicator __instance)
            {
                double num = 0;
                double lastPress = 0;
                Slot VisualSlot = __instance.Slot.FindChild((s) => s.Name == "LoadingText",2);
                var Text = VisualSlot.GetComponent<TextRenderer>();
                var TextCol = VisualSlot.AttachComponent<BoxCollider>();
                var TextButton = VisualSlot.AttachComponent<TouchButton>();
                var BoxDriver = VisualSlot.AttachComponent<BoundingBoxDriver>();

                BoxDriver.BoundedSource.Target = Text;
                BoxDriver.Center.Target = TextCol.Offset;
                BoxDriver.Size.Target = TextCol.Size;

                TextButton.LocalPressed += (IButton b, ButtonEventData d) => IndicatorPress(b, d, ref lastPress, __instance, Text, ref num);
            }
        }
    }
}
