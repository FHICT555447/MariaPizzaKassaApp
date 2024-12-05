using System.Runtime.InteropServices;
using System.Text;

namespace dotnet_pizza_protocol
{
    static class Constants
    {
        public const byte LEFT_FOUR_BITS = 0b11110000;
        public const byte RIGHT_FOUR_BITS = 0b00001111;
        public const byte SizeShift = 5;
        public const byte SizeBits = 0b11100000;
        public const byte CountBits = 0b00011111;
        public const byte ModLengthBits = 0b01111111;
        public const byte ModBit = 0b10000000;
        public const byte ModTypeBit = 0b00000001;
        public const byte ModTypeShift = 7;
    }

    public enum Opcode
    {
        PizzaOrderMinimized = 0b0000,
        PizzaOrderMinimizedModifications = 0b0001,
        PizzaOrderExpanded = 0b0010,
        IdsUnavailable = 0b0011,
        IdsAvailable = 0b0100
    }

    public enum ModificationType { Add, Remove }

    public class PizzaOrderMinimized : IPizzaSerde<PizzaOrderMinimized>
    {
        public static readonly byte opcode = (byte)Opcode.PizzaOrderMinimized;
        private readonly byte PizzaID;
        private readonly byte SizeCount;

        public PizzaOrderMinimized(byte pizzaID, byte sizeCount) {
            PizzaID = pizzaID;
            SizeCount = sizeCount;
        }

        public PizzaOrderMinimized(byte pizzaID, byte size, byte count) {
            PizzaID = pizzaID;
            SizeCount = (byte)(size << Constants.SizeShift & Constants.SizeBits
                | count & Constants.CountBits);
        }

        public byte GetPizzaId() { return PizzaID; }
        public byte GetSizeId() { return (byte)(SizeCount >> Constants.SizeShift); }
        public byte GetCount() { return (byte)(SizeCount & Constants.CountBits); }
        public byte GetSizeCount() { return SizeCount; }

        public static PizzaOrderMinimized? Deserialize(byte[] bytes, int length)
        {
            if (length < 3 || (byte)(bytes[0] >> 4 & Constants.RIGHT_FOUR_BITS) != opcode)
            {
                return null;
            }

            byte pizzaID = (byte)((bytes[0] << 4 & Constants.LEFT_FOUR_BITS)
                | (bytes[1] >> 4 & Constants.RIGHT_FOUR_BITS));
            byte sizeCount = (byte)((bytes[1] << 4 & Constants.LEFT_FOUR_BITS)
                | (bytes[2] >> 4 & Constants.RIGHT_FOUR_BITS));

            return new PizzaOrderMinimized(pizzaID, sizeCount);
        }

        public byte[] Serialize()
        {
            List<byte> res = [0, 0, 0];

            res[0] |= (byte)((opcode << 4) & Constants.LEFT_FOUR_BITS);
            res[0] |= (byte)((PizzaID >> 4) & Constants.RIGHT_FOUR_BITS);
            res[1] |= (byte)((PizzaID << 4) & Constants.LEFT_FOUR_BITS);
            res[1] |= (byte)((SizeCount >> 4) & Constants.RIGHT_FOUR_BITS);
            res[2] |= (byte)((SizeCount << 4) & Constants.LEFT_FOUR_BITS);

            return [.. res];
        }
    }

    public readonly struct MinimizedModification(byte mod)
    {
        private readonly byte Mod = mod;

        public ModificationType GetModType() { return (ModificationType)(Mod >> Constants.ModTypeShift); }
        public byte GetModID() { return (byte)(Mod & Constants.ModLengthBits); }
        public byte GetMod() { return Mod; }
    }

    public class PizzaOrderMinimizedModifications(
        PizzaOrderMinimized minimized,
        List<MinimizedModification> mods
    ) : IPizzaSerde<PizzaOrderMinimizedModifications>
    {
        public static readonly byte opcode = (byte)Opcode.PizzaOrderMinimizedModifications;

        private readonly PizzaOrderMinimized Minimized = minimized;
        private readonly List<MinimizedModification> Mods = mods;

        public byte GetPizzaId() { return Minimized.GetPizzaId(); }
        public byte GetSizeId() { return Minimized.GetSizeId(); }
        public byte GetCount() { return Minimized.GetCount(); }
        public byte GetSizeCount() { return Minimized.GetSizeCount(); }
        public byte GetModCount() { return (byte)Mods.Count; }

        public MinimizedModification this[int i]
        {
            get => Mods[i];
            set => Mods[i] = value;
        }

        public static PizzaOrderMinimizedModifications? Deserialize(byte[] bytes, int length)
        {
            if (length < 3 ||
                (bytes[0] >> 4 & Constants.RIGHT_FOUR_BITS) != opcode ||
                (length < 3 + (bytes[0] & Constants.RIGHT_FOUR_BITS)))
            {
                return null;
            }

            byte modCount = (byte)(bytes[0] & Constants.RIGHT_FOUR_BITS);
            byte pizzaID = bytes[1];
            byte sizeCount = bytes[2];

            List<MinimizedModification> mods = new(modCount);

            for (int i = 0; i < modCount; ++i)
            {
                mods.Add(new MinimizedModification(bytes[i + 3]));
            }

            var minimized = new PizzaOrderMinimized(pizzaID, sizeCount);
            return new PizzaOrderMinimizedModifications(minimized, mods);
        }

        public byte[] Serialize()
        {
            List<byte> res = new(3 + GetModCount());

            byte b1 = (byte)(((opcode << 4) & Constants.LEFT_FOUR_BITS)
                | (GetModCount() & Constants.RIGHT_FOUR_BITS));
            
            res.Add(b1);
            res.Add(GetPizzaId());
            res.Add(GetSizeCount());

            for (int i = 0; i < GetModCount(); ++i)
            {
                res.Add(this[i].GetMod());
            }

            return [.. res];
        }
    }

    public class ExpandedModification(
        ModificationType mod,
        string modName
    ) : IPizzaSerde<ExpandedModification>
    {
        private readonly ModificationType Mod = mod;
        private readonly string ModName = modName;

        public ModificationType GetModType() { return Mod; }
        // public byte GetModNameLength() { return (byte)(ModName.Length); }
        public string GetModname() { return ModName; }

        public static ExpandedModification? Deserialize(byte[] bytes, int length)
        {
            if (length == 0)
            {
                return null;
            }

            ModificationType mod = (ModificationType)(bytes[0] >> Constants.ModTypeShift
                & Constants.ModTypeBit);
            byte len = (byte)(bytes[0] & Constants.ModLengthBits);

            List<byte> utf8ModName = new(len);

            for (int i = 1; i < len + 1; ++i)
            {
                utf8ModName.Add(bytes[i]);
            }

            ReadOnlySpan<byte> utf8Span = CollectionsMarshal.AsSpan(utf8ModName);
            return new ExpandedModification(mod, Encoding.UTF8.GetString(utf8Span));
        }

        public byte[] Serialize()
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(ModName);

            byte length = (byte)utf8Bytes.Length;

            byte b1 = (byte)(length | ((int)GetModType() << Constants.ModTypeShift
                & Constants.ModBit));

            List<byte> res = new(length + 1) { b1 };
            res.AddRange(utf8Bytes);

            // for (int i = 0; i < length; ++i)
            // {
            //     res.Add(utf8Bytes[i]);
            // }

            return [.. res];
        }
    }

    public class PizzaOrderExpanded(
        byte pizzaCount,
        string pizzaName,
        string pizzaSizeName,
        List<ExpandedModification> mods
    ) : IPizzaSerde<PizzaOrderExpanded>
    {
        public static readonly byte opcode = (byte)Opcode.PizzaOrderExpanded;
        private readonly byte PizzaCount = pizzaCount;
        private readonly string PizzaName = pizzaName;
        private readonly string PizzaSizeName = pizzaSizeName;
        private readonly List<ExpandedModification> Mods = mods;

        public string GetPizzaName() { return PizzaName; }
        public string GetSizeName() { return PizzaSizeName; }
        public byte GetCount() { return PizzaCount; }
        public byte GetModCount() { return (byte)Mods.Count; }

        public ExpandedModification this[int i]
        {
            get => Mods[i];
            set => Mods[i] = value;
        }

        public static PizzaOrderExpanded? Deserialize(byte[] bytes, int length)
        {
            if (length < 4 || (bytes[0] >> 4 & Constants.RIGHT_FOUR_BITS) != opcode)
            {
                return null;
            }

            int mod_count = bytes[0] & Constants.RIGHT_FOUR_BITS;
            int pizza_name_len = bytes[1];

            int offset = 2;
            List<byte> utf8Name = [];

            for (int i = offset; i < offset + pizza_name_len; ++i)
            {
                utf8Name.Add(bytes[i]);
            }

            int pizza_size_len = bytes[offset + pizza_name_len];
            offset += pizza_name_len + 1;
            List<byte> utf8Size = [];

            for (int i = offset; i < offset + pizza_size_len; ++i)
            {
                utf8Size.Add(bytes[i]);
            }

            byte pizza_count = bytes[offset + pizza_size_len];
            offset += pizza_size_len + 1;

            List<ExpandedModification> mods = [];

            for (int i = 0; i < mod_count; ++i)
            {
                byte len = (byte)((bytes[offset] & Constants.ModLengthBits) + 1);
                byte[] modBytes = bytes[offset..(offset + len)];

                var maybeMod = ExpandedModification.Deserialize(modBytes, (byte)len);
                if (maybeMod is not null)
                {
                    mods.Add(maybeMod);
                }
                offset += len;
            }

            ReadOnlySpan<byte> utf8NameSpan = CollectionsMarshal.AsSpan(utf8Name);
            ReadOnlySpan<byte> utf8SizeSpan = CollectionsMarshal.AsSpan(utf8Size);

            string pizzaName = Encoding.UTF8.GetString(utf8NameSpan);
            string pizzaSize = Encoding.UTF8.GetString(utf8SizeSpan);

            return new PizzaOrderExpanded(pizza_count, pizzaName, pizzaSize, mods);
        }

        public byte[] Serialize()
        {
            List<byte> res = [];

            byte opc = (byte)(opcode << 4 & Constants.LEFT_FOUR_BITS);
            byte mod_count = (byte)Mods.Count;

            res.Add((byte)(opc | mod_count));

            byte[] utf8Name = Encoding.UTF8.GetBytes(PizzaName);
            byte[] utf8Size = Encoding.UTF8.GetBytes(PizzaSizeName);

            res.Add((byte)utf8Name.Length);
            res.AddRange(utf8Name);

            res.Add((byte)utf8Size.Length);
            res.AddRange(utf8Size);

            res.Add(GetCount());

            foreach (var mod in Mods)
            {
                var bytes = mod.Serialize();
                res.AddRange(bytes);
            }

            return [.. res];
        }
    }

    public abstract record PizzaMessage
    {
        public record OrderMinimized(PizzaOrderMinimized Order) : PizzaMessage;
        public record OrderMinimizedModifications(PizzaOrderMinimizedModifications Order) : PizzaMessage;
        public record OrderExpanded(PizzaOrderExpanded Order) : PizzaMessage;
        public record IdsUnavailable : PizzaMessage;
        public record IdsAvailable : PizzaMessage;
        public record InvalidOrder : PizzaMessage;

        public static PizzaMessage Receive(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return new InvalidOrder();
            }

            var opcode = (Opcode)(bytes[0] >> 4 & Constants.RIGHT_FOUR_BITS);

            switch (opcode)
            {
                case Opcode.PizzaOrderMinimized:
                    var maybePOM = PizzaOrderMinimized.Deserialize(bytes, bytes.Length);
                    return maybePOM is not null ? new OrderMinimized(maybePOM) : new InvalidOrder();
                case Opcode.PizzaOrderMinimizedModifications:
                    var maybePOMM = PizzaOrderMinimizedModifications.Deserialize(bytes, bytes.Length);
                    return maybePOMM is not null ? new OrderMinimizedModifications(maybePOMM) : new InvalidOrder();
                case Opcode.PizzaOrderExpanded:
                    var maybePOE = PizzaOrderExpanded.Deserialize(bytes, bytes.Length);
                    return maybePOE is not null ? new OrderExpanded(maybePOE) : new InvalidOrder();
                case Opcode.IdsUnavailable:
                    return new IdsUnavailable();
                case Opcode.IdsAvailable:
                    return new IdsAvailable();
                default:
                    return new InvalidOrder();
            }
        }
    }
}