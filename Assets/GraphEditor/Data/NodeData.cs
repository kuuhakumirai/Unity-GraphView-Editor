using System;
using System.Text.Json.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GraphEditor
{
    [JsonDerivedType(typeof(NodeData), typeDiscriminator: nameof(NodeData))]
    public partial class NodeData : IData
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("position")]
        public float[] Position { get; set; }

        private List<SlotData> m_Slots;

        [JsonPropertyName("expanded")]
        public bool PreviewExpanded { get; set; } = true;

        [JsonPropertyName("slots")]
        public IReadOnlyList<SlotData> Slots
        {
            get => m_Slots;
            init => m_Slots = value.ToList();
        }

        [JsonIgnore]
        public GraphData Owner { get; set; }

        public virtual void UpdateNodeAfterDeserialization()
        {
        }
        public NodeData()
        {
            m_Slots ??= new List<SlotData>();
            Position = new float[2];
        }

        public void SetPosition(Vector2 position)
        {
            Position[0] = position.x;
            Position[1] = position.y;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(Position[0], Position[1]);
        }

        public SlotData GetSlotFromId(string slotId)
        {
            return m_Slots.FirstOrDefault(x => x.Guid == slotId);
        }

        public SlotData AddSlot(SlotData slot)
        {
            SlotData foundSlot = FindSlot<SlotData>(slot.Id);
            if (foundSlot == slot)
            {
                return foundSlot;
            }
            int firstIndex = m_Slots.FindIndex(s => s.Id == slot.Id);
            if (firstIndex >= 0)
            {
                m_Slots[firstIndex] = slot;
                m_Slots.RemoveAllFromRange(s => s.Id == slot.Id, firstIndex + 1, m_Slots.Count - (firstIndex + 1));
            }
            else
            {
                m_Slots.Add(slot);
            }
            slot.Owner = this;
            if (slot.Owner == null)
            {
                Debug.Log(slot.Name);
            }
            if (foundSlot == null)
            {
                return slot;
            }
            return slot;
        }

        public void RemoveSlot(SlotData slot)
        {
            m_Slots.RemoveAll(x => x.Id == slot.Id);
        }

        public T FindSlot<T>(int slotId) where T : SlotData
        {
            foreach (var slot in m_Slots)
            {
                if (slot.Id == slotId && slot is T t)
                {
                    return t;
                }
            }
            return default;
        }

        public void SetupSlots()
        {
            foreach (var s in m_Slots)
            {
                s.Owner = this;
            }
        }
    }

    public static class ListUtilities
    {
        // Ideally, we should build a non-yield return, struct version of Slice
        public static IEnumerable<T> Slice<T>(this List<T> list, int start, int end)
        {
            for (int i = start; i < end; i++)
                yield return list[i];
        }

        public static int RemoveAllFromRange<T>(this List<T> list, Predicate<T> match, int startIndex, int count)
        {
            // match behaviour of RemoveRange
            if ((startIndex < 0) || (count < 0))
                throw new ArgumentOutOfRangeException();

            int endIndex = startIndex + count;
            if (endIndex > list.Count)
                throw new ArgumentException();

            int readIndex = startIndex;
            int writeIndex = startIndex;
            while (readIndex < endIndex)
            {
                T element = list[readIndex];
                bool remove = match(element);
                if (!remove)
                {
                    // skip some work if nothing removed (especially if T is a large struct)
                    if (writeIndex < readIndex)
                        list[writeIndex] = element;
                    writeIndex++;
                }
                readIndex++;
            }

            // once we're done, we can remove the entries at the end in one operation
            int numberRemoved = readIndex - writeIndex;
            if (numberRemoved > 0)
            {
                list.RemoveRange(writeIndex, numberRemoved);
            }

            return numberRemoved;
        }
    }

}
