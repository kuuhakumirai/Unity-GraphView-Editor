using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace GraphEditor.Core
{
    public static class NodeExtensions
    {
        public static string GetSpeakText(this SpeakNode node, GraphInstance instance)
        {
            SlotData slot = node.Slots.First(x => x.Name == "Text");
            SlotData vSlot = instance.Compiler.GetSourceFromInput(slot).Value.FirstOrDefault(x => x.Name == "V" && x.IsInputSlot);
            return vSlot.Value;
        }

        public static string GetSpeaker(this SpeakNode node, GraphInstance instance)
        {
            SlotData slot = node.Slots.First(x => x.Name == "Name");
            SlotData vSlot = instance.Compiler.GetSourceFromInput(slot).Value.FirstOrDefault(x => x.Name == "V" && x.IsInputSlot);
            return vSlot.Value;
        }

        public static string GetSpeakerHead(this SpeakNode node, GraphInstance instance)
        {
            SlotData slot = node.Slots.First(x => x.Name == "Head");
            SlotData vSlot = instance.Compiler.GetSourceFromInput(slot).Value.FirstOrDefault(x => x.Name == "V" && x.IsInputSlot);
            return vSlot.Value;
        }

        public static List<SlotData> GetAvailableSelections(this SelectionNode node)
        {
            List<SlotData> slots = new();
            var outputs = node.Slots.Where(x => x.Name.Contains("Out")).ToList();
            var inputs = node.Slots.Where(x => !x.IsActionSlot && x.IsInputSlot).ToList();
            for (int i = 0; i < inputs.Count; i++)
            {
                if (outputs[i].Connections.Any())
                {
                    slots.Add(inputs[i]);
                }
            }
            return slots;
        }

        public static List<string> GetSelectionText(this SelectionNode node, List<SlotData> inputs, GraphInstance instance)
        {
            List<string> strings = new();
            foreach (var slot in inputs)
            {
                SlotData vSlot = instance.Compiler.GetSourceFromInput(slot).Value.FirstOrDefault(x => x.Name == "V" && x.IsInputSlot);
                strings.Add(vSlot.Value);
            }
            return strings;
        }
    }
}
