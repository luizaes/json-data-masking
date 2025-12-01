using System;

namespace JsonDataMasking.Test.MockData
{
    public class BasicTypesMock
    {
        public bool Bool { get; set; } = true;
        public byte Byte { get; set; } = 1;
        public sbyte Sbyte { get; set; } = 1;
        public short Short { get; set; } = 1;
        public ushort Ushort { get; set; } = 1;
        public int Int { get; set; } = 1;
        public uint Uint { get; set; } = 1;
        public long Long { get; set; } = 1;
        public ulong Ulong { get; set; } = 1;
        public float Float { get; set; } = 1;
        public double Double { get; set; } = 1;
        public decimal Decimal { get; set; } = 1;
        public char Char { get; set; } = '1';
        public DateTime DateTime { get; set; } = new DateTime().AddYears(1);
        public DateTimeOffset DateTimeOffset { get; set; } = new DateTimeOffset().AddYears(1);
        public Guid Guid { get; set; } = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    }
}
