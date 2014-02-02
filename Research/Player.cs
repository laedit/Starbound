using System;
using System.Collections.Generic;
using System.IO;
//using System.Drawing;
using System.Linq;
using System.Text;

namespace StarboundEdit
{
    /// <summary>
    /// Class created by Ephemerality (http://community.playstarbound.com/index.php?members/ephemerality.12881/)
    /// </summary>
    public class Main
    {
        public Main()
        {
        }

        public static short ToInt16BigEndian(byte[] buf, int i)
        {
            return (short)((buf[i] << 8) | buf[i + 1]);
        }

        public static int ToInt32BigEndian(byte[] buf, int i)
        {
            return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
        }

        public static uint ToUInt32BigEndian(byte[] buf, int i)
        {
            return (uint)((buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3]);
        }

        public static UInt64 ToUInt64BigEndian(byte[] buf, int i)
        {
            return Convert.ToUInt64((buf[i] << 56) | (buf[i + 1] << 48) | (buf[i + 2] << 40) | (buf[i + 3] << 32) | (buf[i + 4] << 24) | (buf[i + 5] << 16) | (buf[i + 6] << 8) | buf[i + 7]);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string readCString(BinaryReader reader)
        {
            StringBuilder s = new StringBuilder();
            Int32 input;
            while ((input = reader.Read()) != -1)
            {
                Char c = (Char)input;
                if (c != '\0') s.Append(c);
                else break;
            }
            return s.ToString();
        }

        public static float readFloatBigEndian(BinaryReader reader)
        {
            byte[] floatBytes = reader.ReadBytes(4);
            Array.Reverse(floatBytes);
            float value = BitConverter.ToSingle(floatBytes, 0);
            return value;
        }

        public static void writeFloatBigEndian(BinaryWriter writer, float f)
        {
            byte[] floatBytes = BitConverter.GetBytes(f);
            Array.Reverse(floatBytes);
            writer.Write(floatBytes);
        }

        public static string readString(BinaryReader reader)
        {
            StringBuilder s = new StringBuilder();
            int size = VLQ.readVLQ(reader);
            s.Append(reader.ReadChars(size));
            return s.ToString();
        }

        public class Player
        {
            public UDID udid;
            public HumanoidIdentity identity;
            StatusEntityParameters statusParams;
            Status status;
            public string desc;
            public UInt64 playTime;
            public PlayerInventory inventory;
            public BlueprintLibrary blueprints;
            public TechLibrary techs;
            ItemDescriptor Helmet;
            ItemDescriptor Chest;
            ItemDescriptor Pants;
            ItemDescriptor VanityHelm; //maybe
            ItemDescriptor VanityPants; //maybe
            ItemDescriptor VanityChest;
            ItemDescriptor Unknown1;
            ItemDescriptor Unknown2;
            ItemDescriptor Unknown3;
            ItemDescriptor Unknown4;

            public Player(BinaryReader reader)
            {
                udid = new UDID(reader.ReadByte(), reader.ReadBytes(16));
                identity = new HumanoidIdentity(reader);
                statusParams = new StatusEntityParameters(reader);
                status = new Status(reader);
                desc = readString(reader);
                playTime = reader.ReadUInt64();
                int invSize = VLQ.readVLQ(reader);
                inventory = new PlayerInventory(reader);
                inventory.pixels = 40000;
                inventory.TileBagContents.addItem(new ItemDescriptor("plutoniumrod", new VLQ(2000), new Variant(7, new Dictionary<string, Variant>())));
                int bLibSize = VLQ.readVLQ(reader);
                blueprints = new BlueprintLibrary(reader);
                //int tLibSize = VLQ.readVLQ(reader);
                //byte[] techLib = reader.ReadBytes(tLibSize);
                techs = new TechLibrary(reader);//new BinaryReader(new MemoryStream(techLib)));
                Helmet = new ItemDescriptor(reader);
                Chest = new ItemDescriptor(reader);
                Pants = new ItemDescriptor(reader);
                VanityHelm = new ItemDescriptor(reader); //maybe
                VanityPants = new ItemDescriptor(reader); //maybe
                VanityChest = new ItemDescriptor(reader);
                Unknown1 = new ItemDescriptor(reader);
                Unknown2 = new ItemDescriptor(reader);
                Unknown3 = new ItemDescriptor(reader);
                Unknown4 = new ItemDescriptor(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(Encoding.ASCII.GetBytes("SBPFV1.1"));
                writer.Write(2801860608);
                MemoryStream m = new MemoryStream();
                BinaryWriter w = new BinaryWriter(m);
                w.Write(udid.initialized);
                w.Write(udid.data);
                identity.Write(w);
                statusParams.Write(w);
                status.Write(w);
                w.Write(strBytes(desc));
                w.Write(playTime);
                using (MemoryStream inv = new MemoryStream())
                {
                    BinaryWriter w2 = new BinaryWriter(inv);
                    inventory.Write(w2);
                    w.Write(VLQ.getVLQBytes(Convert.ToUInt64(inv.Length)));
                    inv.Flush();
                    inv.Seek(0, SeekOrigin.Begin);
                    byte[] test = new byte[inv.Length];
                    inv.Read(test, 0, (int)inv.Length);
                    w.Write(test);
                    w2.Close();
                }
                using (MemoryStream inv = new MemoryStream())
                {
                    BinaryWriter w2 = new BinaryWriter(inv);
                    blueprints.Write(w2);
                    w.Write(VLQ.getVLQBytes(Convert.ToUInt64(inv.Length)));
                    inv.Flush();
                    inv.Seek(0, SeekOrigin.Begin);
                    byte[] test = new byte[inv.Length];
                    inv.Read(test, 0, (int)inv.Length);
                    w.Write(test);
                    w2.Close();
                }
                using (MemoryStream inv = new MemoryStream())
                {
                    BinaryWriter w2 = new BinaryWriter(inv);
                    techs.Write(w2);
                    w.Write(VLQ.getVLQBytes(Convert.ToUInt64(inv.Length)));
                    inv.Flush();
                    inv.Seek(0, SeekOrigin.Begin);
                    byte[] test = new byte[inv.Length];
                    inv.Read(test, 0, (int)inv.Length);
                    w.Write(test);
                    w2.Close();
                }
                Helmet.Write(w);
                Chest.Write(w);
                Pants.Write(w);
                VanityHelm.Write(w);
                VanityPants.Write(w);
                VanityChest.Write(w);
                Unknown1.Write(w);
                Unknown2.Write(w);
                Unknown3.Write(w);
                Unknown4.Write(w);
                w.Write('\0');
                writer.Write(VLQ.getVLQBytes(Convert.ToUInt64(m.Length)));
                m.Flush();
                byte[] test2 = new byte[m.Length];
                m.Seek(0, SeekOrigin.Begin);
                m.Read(test2, 0, (int)m.Length);
                writer.Write(test2);
                w.Close();
            }
        }

        public struct Vec2F
        {
            public float X;
            public float Y;

            public Vec2F(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        public struct RGBAColor
        {
            byte R;
            byte G;
            byte B;
            byte A;

            public RGBAColor(BinaryReader reader)
            {
                R = reader.ReadByte();
                G = reader.ReadByte();
                B = reader.ReadByte();
                A = reader.ReadByte();
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(R);
                writer.Write(G);
                writer.Write(B);
                writer.Write(A);
            }
        }

        public struct RGB3F
        {
            public float R;
            public float G;
            public float B;

            public RGB3F(BinaryReader reader)
            {
                R = readFloatBigEndian(reader);
                G = readFloatBigEndian(reader);
                B = readFloatBigEndian(reader);
            }
        }

        public struct UDID
        {
            public bool initialized;
            public byte[] data;

            public UDID(byte init, byte[] indata)
            {
                initialized = (init > 0 ? true : false);
                data = indata;
            }

            public override string ToString()
            {
                return ByteArrayToString(data);
            }
        }

        public struct Personality
        {
            public string idle;
            public string armIdle;
            Vec2F headOffset;
            Vec2F armOffset;

            public Personality(BinaryReader reader)
            {
                idle = readString(reader);
                armIdle = readString(reader);
                headOffset = new Vec2F(readFloatBigEndian(reader), readFloatBigEndian(reader));
                armOffset = new Vec2F(readFloatBigEndian(reader), readFloatBigEndian(reader));
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(strBytes(idle));
                writer.Write(strBytes(armIdle));
                writeFloatBigEndian(writer, headOffset.X);
                writeFloatBigEndian(writer, headOffset.Y);
                writeFloatBigEndian(writer, armOffset.X);
                writeFloatBigEndian(writer, armOffset.Y);
            }
        }

        public class HumanoidIdentity
        {
            public string name;
            public string race;
            public byte gender;
            public string hairGroup;
            public string hairType;
            public string hairColors;
            public string bodyColors;
            public string facialHairGroup;
            public string facialHairType;
            public string facialHairColors;
            public string facialMaskGroup;
            public string facialMaskType;
            public string facialMaskColors;
            public Personality personality;
            public RGBAColor favColor;

            public HumanoidIdentity(BinaryReader reader)
            {
                name = reader.ReadString();
                race = readString(reader);
                gender = reader.ReadByte();
                hairGroup = readString(reader);
                hairType = readString(reader);
                hairColors = readString(reader);
                bodyColors = readString(reader);
                facialHairGroup = readString(reader);
                facialHairType = readString(reader);
                facialHairColors = readString(reader);
                facialMaskGroup = readString(reader);
                facialMaskType = readString(reader);
                facialMaskColors = readString(reader);
                personality = new Personality(reader);
                favColor = new RGBAColor(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(strBytes(name));
                writer.Write(strBytes(race));
                writer.Write(gender);
                writer.Write(strBytes(hairGroup));
                writer.Write(strBytes(hairType));
                writer.Write(strBytes(hairColors));
                writer.Write(strBytes(bodyColors));
                writer.Write(strBytes(facialHairGroup));
                writer.Write(strBytes(facialHairType));
                writer.Write(strBytes(facialHairColors));
                writer.Write(strBytes(facialMaskGroup));
                writer.Write(strBytes(facialMaskType));
                writer.Write(strBytes(facialMaskColors));
                personality.Write(writer);
                favColor.Write(writer);
            }
        }

        public struct StatusEntityParameters
        {
            byte[] firstCrap;
            string bodyMaterial;
            string damageConfig;

            public StatusEntityParameters(BinaryReader reader)
            {
                firstCrap = reader.ReadBytes(65);
                bodyMaterial = readString(reader);
                damageConfig = readString(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(firstCrap);
                writer.Write(strBytes(bodyMaterial));
                writer.Write(strBytes(damageConfig));
            }
        }

        public struct StatusValue
        {
            float Value;
            float Maximum;

            public StatusValue(BinaryReader reader)
            {
                Value = readFloatBigEndian(reader);
                Maximum = readFloatBigEndian(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writeFloatBigEndian(writer, Value);
                writeFloatBigEndian(writer, Maximum);
            }
        }

        public struct Status
        {
            public StatusValue health;
            public StatusValue energy;
            public StatusValue warmth;
            public StatusValue food;
            public StatusValue breath;
            public bool invulnerable;
            public RGB3F glowColor;
            public List<string> effects;
            public List<string> unk;

            public Status(BinaryReader reader)
            {
                health = new StatusValue(reader);
                energy = new StatusValue(reader);
                warmth = new StatusValue(reader);
                food = new StatusValue(reader);
                breath = new StatusValue(reader);
                invulnerable = Convert.ToBoolean(reader.ReadByte());
                glowColor = new RGB3F(reader);
                effects = new List<string>();
                for (int i = VLQ.readVLQ(reader); i > 0; i--)
                {
                    effects.Add(readString(reader));
                }
                unk = new List<string>();
                for (int i = VLQ.readVLQ(reader); i > 0; i--)
                {
                    unk.Add(readString(reader));
                }
            }

            public void Write(BinaryWriter writer)
            {
                health.Write(writer);
                energy.Write(writer);
                warmth.Write(writer);
                food.Write(writer);
                breath.Write(writer);
                writer.Write(invulnerable);
                writeFloatBigEndian(writer, glowColor.R);
                writeFloatBigEndian(writer, glowColor.G);
                writeFloatBigEndian(writer, glowColor.B);
                writeStringList(writer, effects);
                writeStringList(writer, unk);
            }
        }

        public class Variant
        {
            public byte type;
            object data;

            public Variant(byte t, object d)
            {
                type = t;
                data = d;
            }

            public Variant(BinaryReader reader)
            {
                type = reader.ReadByte();
                switch (type)
                {
                    case (2):
                        data = reader.ReadDouble();
                        break;
                    case (3):
                        data = reader.ReadBoolean();
                        break;
                    case (4):
                        data = VLQ.readVLQ(reader);
                        break;
                    case (5):
                        data = readString(reader);
                        break;
                    case (6):
                        List<Variant> variantlist = new List<Variant>();
                        for (int size = VLQ.readVLQ(reader); size > 0; size--)
                        {
                            variantlist.Add(new Variant(reader));
                        }
                        data = variantlist;
                        break;
                    case (7):
                        Dictionary<string, Variant> variantmap = new Dictionary<string, Variant>();
                        for (int size = VLQ.readVLQ(reader); size > 0; size--)
                        {
                            variantmap.Add(readString(reader), new Variant(reader));
                        }
                        data = variantmap;
                        break;
                }
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(type);
                switch (type)
                {
                    case (2):
                        writer.Write((double)data);
                        break;
                    case (3):
                        writer.Write((bool)data);
                        break;
                    case (4):
                        writer.Write(VLQ.getVLQBytes(Convert.ToUInt64((int)data)));
                        break;
                    case (5):
                        writer.Write(strBytes((string)data));
                        break;
                    case (6):
                        writer.Write(VLQ.getVLQBytes((ulong)((List<Variant>)data).Count));
                        foreach (Variant v in (List<Variant>)data)
                        {
                            v.Write(writer);
                        }
                        break;
                    case (7):
                        Dictionary<string, Variant> variantmap = (Dictionary<string, Variant>)data;
                        writer.Write(VLQ.getVLQBytes((ulong)(variantmap.Count)));
                        foreach (KeyValuePair<string, Variant> v in variantmap)
                        {
                            writer.Write(strBytes(v.Key));
                            v.Value.Write(writer);
                        }
                        break;
                }
            }
        }

        public struct ItemDescriptor
        {
            public string name;
            public VLQ count;
            public Variant itemVariant;

            public ItemDescriptor(string n, VLQ c, Variant v)
            {
                name = n;
                count = c;
                itemVariant = v;
            }

            public ItemDescriptor(BinaryReader reader)
            {
                name = readString(reader);
                count = new VLQ(reader);
                itemVariant = new Variant(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(strBytes(name));
                writer.Write(count.Bytes);
                itemVariant.Write(writer);
            }
        }

        public struct ItemBag
        {
            public List<ItemDescriptor> items;

            public ItemBag(BinaryReader reader)
            {
                VLQ size = new VLQ(reader);
                items = new List<ItemDescriptor>();
                for (int i = 0; i < size.Value; i++)
                {
                    items.Add(new ItemDescriptor(reader));
                }
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(VLQ.getVLQBytes(Convert.ToUInt64(items.Count)));
                foreach (ItemDescriptor i in items)
                {
                    i.Write(writer);
                }
            }

            public void addItem(ItemDescriptor newItem)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemDescriptor item = items[i];
                    if (item.name == "" && item.count.Value == 1 && item.itemVariant.type == 7)
                    {
                        items[i] = newItem;
                        break;
                    }
                }
            }
        }

        public enum BagType : byte
        {
            INVALID = 0, Bag = 1, TileBag = 2, ActionBar = 3, Equipment = 4, Wieldable
        }

        public struct InventorySlot
        {
            BagType bag;
            VLQ slotNum;

            public InventorySlot(BinaryReader reader)
            {
                bag = (BagType)reader.ReadByte();
                slotNum = new VLQ(reader);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write((byte)bag);
                writer.Write(slotNum.Bytes);
            }
        }

        public struct PlayerInventory
        {
            public UInt64 pixels;
            public ItemBag BagContents;
            public ItemBag TileBagContents;
            public ItemBag ActionBarContents;
            public ItemBag Equipment;
            public ItemBag Wieldable;
            public ItemDescriptor SwapActive_UNKNOWN;
            public InventorySlot PrimaryHeldSlot;
            public InventorySlot AltHeldSlot;

            public PlayerInventory(BinaryReader reader)
            {
                pixels = ToUInt64BigEndian(reader.ReadBytes(8), 0);
                BagContents = new ItemBag(reader);
                TileBagContents = new ItemBag(reader);
                ActionBarContents = new ItemBag(reader);
                Equipment = new ItemBag(reader);
                Wieldable = new ItemBag(reader);
                SwapActive_UNKNOWN = new ItemDescriptor(reader);
                PrimaryHeldSlot = new InventorySlot(reader);
                AltHeldSlot = new InventorySlot(reader);
            }

            public void Write(BinaryWriter writer)
            {
                byte[] pix = BitConverter.GetBytes(pixels);
                Array.Reverse(pix);
                writer.Write(pix);
                BagContents.Write(writer);
                TileBagContents.Write(writer);
                ActionBarContents.Write(writer);
                Equipment.Write(writer);
                Wieldable.Write(writer);
                SwapActive_UNKNOWN.Write(writer);
                PrimaryHeldSlot.Write(writer);
                AltHeldSlot.Write(writer);
            }
        }

        public struct BlueprintLibrary
        {
            VLQ count;
            List<ItemDescriptor> blueprints;

            public BlueprintLibrary(BinaryReader reader)
            {
                count = new VLQ(reader);
                blueprints = new List<ItemDescriptor>();
                for (int i = 0; i < count.Value; i++)
                {
                    blueprints.Add(new ItemDescriptor(reader));
                }
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(count.Bytes);
                foreach (ItemDescriptor i in blueprints)
                {
                    i.Write(writer);
                }
            }
        }

        public struct TechLibrary
        {
            VLQ count;
            List<ItemDescriptor> techs;
            byte[] placeholder;

            public TechLibrary(BinaryReader reader)
            {
                count = new VLQ(reader);
                /*techs = new List<ItemDescriptor>();
                for (int i = 0; i < count.Value; i++)
                {
                    techs.Add(new ItemDescriptor(reader));
                }*/
                techs = new List<ItemDescriptor>();
                placeholder = reader.ReadBytes((int)count.Value);
            }

            public void Write(BinaryWriter writer)
            {
                /*writer.Write(count.Bytes);
                foreach (ItemDescriptor i in techs)
                {
                    i.Write(writer);
                }*/
                writer.Write(placeholder);
            }
        }

        public void DuplicatePlayerSave(string playerFilePath)
        {
            FileStream stream = new FileStream(playerFilePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);
            string filever = new string(reader.ReadChars(8));
            //string blank = readCString(reader);
            UInt32 version = reader.ReadUInt32();
            VLQ fileSize = new VLQ(reader);
            Player player = new Player(reader);
            Console.WriteLine(player.ToString());
            int i = 0;
            reader.Close();
            stream.Close();
            stream = new FileStream("test.player", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            player.Write(writer);
            writer.Close();
            stream.Close();
            //Environment.Exit(0);
        }

        public PlayerSaveFile ReadPlayerSave(string playerFilePath)
        {
            return new PlayerSaveFile(playerFilePath);
        }

        public static byte[] strBytes(string str)
        {
            byte[] strLen = VLQ.getVLQBytes(Convert.ToUInt64(str.Length));
            byte[] strBytes = Encoding.ASCII.GetBytes(str.ToCharArray());
            byte[] output = new byte[strLen.Length + strBytes.Length];
            strLen.CopyTo(output, 0);
            strBytes.CopyTo(output, strLen.Length);
            return output;
        }

        public static void writeStringList(BinaryWriter writer, List<string> list)
        {
            writer.Write(VLQ.getVLQBytes(Convert.ToUInt64(list.Count)));
            foreach (string s in list)
            {
                writer.Write(strBytes(s));
            }
        }

        public class PlayerSaveFile
        {
            public string FileVersion { get; private set; }
            public uint SaveVersion { get; private set; }
            public VLQ FileSize { get; set; }
            public Player Player { get; private set; }

            public PlayerSaveFile(string playerFilePath)
            {
                using (FileStream stream = new FileStream(playerFilePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        FileVersion = new string(reader.ReadChars(8));
                        //string blank = readCString(reader);
                        SaveVersion = reader.ReadUInt32();
                        FileSize = new VLQ(reader);
                        Player = new Player(reader);
                    }
                }
            }
        }

        public class VLQ
        {
            //
            public VLQ(int num)
            {
                m_value = Convert.ToUInt32(num);
                m_numBytes = 0;
            }
            // Functions
            public VLQ(BinaryReader reader)//byte[] inVariableData, int inOffset)
            {
                // At least one byte is always used
                m_value = reader.ReadByte();
                m_numBytes = 1;
                if (((m_value & 0x80) != 0))
                {
                    UInt32 c;
                    m_value &= 0x7F;

                    do
                    {
                        c = reader.ReadByte();
                        m_value = (m_value << 7) + (c & 0x7F);
                        ++m_numBytes;
                    } while ((c & 0x80) != 0);
                }
            }

            public static int readVLQ(BinaryReader reader)
            {
                int result = 0;
                result = reader.ReadByte();
                if (((result & 0x80) != 0))
                {
                    byte c;
                    result &= 0x7F;

                    do
                    {
                        c = reader.ReadByte();
                        result = (result << 7) + (c & 0x7F);
                    } while ((c & 0x80) != 0);
                }
                return result;
            }

            public static byte[] getVLQBytes(ulong number) //17103 should be 0x81 0x85 0x4F (129, 133, 79)
            {
                List<byte> output = new List<byte>();
                int offset = 0;
                byte result = 0;
                int i;
                for (i = 9; i > 0; i--)
                {
                    if (((number >> (7 * i)) & 0x7F) > 0) break;
                }
                for (int j = i; j >= 0; j--)
                {
                    result = (byte)((number >> (7 * j)) & 0x7F);
                    result |= 0x80;
                    if (j == 0)
                        result ^= 0x80;
                    output.Add(result);
                }
                return output.ToArray();
            }

            public byte[] Bytes
            {
                get
                {
                    return VLQ.getVLQBytes(m_value);
                }
            }
            //
            public UInt32 Value
            {
                get
                {
                    return m_value;
                }
            }

            //
            public UInt32 NumBytes
            {
                get
                {
                    return m_numBytes;
                }
            }

            //
            // Attributes
            UInt32 m_value;
            UInt32 m_numBytes;
        }
    }
}
