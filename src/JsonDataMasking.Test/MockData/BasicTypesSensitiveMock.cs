using JsonDataMasking.Attributes;
using System;

namespace JsonDataMasking.Test.MockData
{
    public class BasicTypesSensitiveMock
    {
        [SensitiveData]
        public bool Bool { get; set; } = true;
        [SensitiveData]
        public byte Byte { get; set; } = 1;
        [SensitiveData]
        public sbyte Sbyte { get; set; } = 1;
        [SensitiveData]
        public short Short { get; set; } = 1;
        [SensitiveData]
        public ushort Ushort { get; set; } = 1;
        [SensitiveData]
        public int Int { get; set; } = 1;
        [SensitiveData]
        public uint Uint { get; set; } = 1;
        [SensitiveData]
        public long Long { get; set; } = 1;
        [SensitiveData]
        public ulong Ulong { get; set; } = 1;
        [SensitiveData]
        public float Float { get; set; } = 1;
        [SensitiveData]
        public double Double { get; set; } = 1;
        [SensitiveData]
        public decimal Decimal { get; set; } = 1;
        [SensitiveData]
        public char Char { get; set; } = '1';
        [SensitiveData]
        public DateTime DateTime { get; set; } = new DateTime().AddYears(1);
        [SensitiveData]
        public DateTimeOffset DateTimeOffset { get; set; } = new DateTimeOffset().AddYears(1);
        [SensitiveData]
        public Guid Guid { get; set; } = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    }
}
